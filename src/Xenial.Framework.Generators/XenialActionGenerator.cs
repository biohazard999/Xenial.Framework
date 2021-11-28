
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using Xenial.Framework.MsBuild;

namespace Xenial.Framework.Generators;

[Generator]
public class XenialActionGenerator : ISourceGenerator
{
    private const string xenialActionAttributeName = "XenialActionAttribute";
    private const string xenialNamespace = "Xenial";
    private const string xenialActionAttributeFullName = $"{xenialNamespace}.{xenialActionAttributeName}";
    private const string generateXenialActionAttributeMSBuildProperty = $"Generate{xenialActionAttributeName}";

    public void Initialize(GeneratorInitializationContext context)
        => context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());

    /// <summary>
    /// Receives all the classes that have the Xenial.XenialImageNamesAttribute set.
    /// </summary>
    internal class SyntaxReceiver : ISyntaxContextReceiver
    {
        public List<ClassDeclarationSyntax> Classes { get; } = new();

        /// <summary>
        /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
        /// </summary>
        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is ClassDeclarationSyntax { AttributeLists.Count: > 0 } classDeclarationSyntax)
            {
                var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax);

                if (classSymbol is not null)
                {
                    Classes.Add(classDeclarationSyntax);
                }
            }
        }
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is not SyntaxReceiver syntaxReceiver)
        {
            return;
        }

        context.CancellationToken.ThrowIfCancellationRequested();

        var compilation = context.Compilation;
        if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{generateXenialActionAttributeMSBuildProperty}", out var generateXenialActionAttrStr))
        {
            if (bool.TryParse(generateXenialActionAttrStr, out var generateXenialActionAttr))
            {
                if (!generateXenialActionAttr)
                {
                    return;
                }
            }
            else
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        GeneratorDiagnostics.InvalidBooleanMsBuildProperty(
                            generateXenialActionAttributeMSBuildProperty,
                            generateXenialActionAttrStr
                        )
                        , null
                    ));
                return;
            }
        }

        var syntaxWriter = new CurlyIndenter(new System.CodeDom.Compiler.IndentedTextWriter(new StringWriter()));

        syntaxWriter.WriteLine($"using System;");
        syntaxWriter.WriteLine();

        syntaxWriter.WriteLine($"namespace {xenialNamespace}");
        syntaxWriter.OpenBrace();

        syntaxWriter.WriteLine("[AttributeUsage(AttributeTargets.Class, Inherited = false)]");
        syntaxWriter.WriteLine($"{context.GetDefaultAttributeModifier()} sealed class {xenialActionAttributeName} : Attribute");
        syntaxWriter.OpenBrace();
        syntaxWriter.WriteLine($"{context.GetDefaultAttributeModifier()} {xenialActionAttributeName}() {{ }}");
        syntaxWriter.CloseBrace();

        syntaxWriter.WriteLine($"{context.GetDefaultAttributeModifier()} interface IDetailViewAction<T> {{ }}");
        syntaxWriter.WriteLine($"{context.GetDefaultAttributeModifier()} interface IListViewAction<T> {{ }}");

        syntaxWriter.CloseBrace();

        var syntax = syntaxWriter.ToString();
        var source = SourceText.From(syntax, Encoding.UTF8);
        context.AddSource($"{xenialActionAttributeName}.g.cs", source);

        compilation = compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(syntax, (CSharpParseOptions)context.ParseOptions, cancellationToken: context.CancellationToken));

        var generateXenialActionAttribute = compilation.GetTypeByMetadataName(xenialActionAttributeFullName);

        if (generateXenialActionAttribute is null)
        {
            //TODO: Warning Diagnostics for either setting the right MSBuild properties or referencing `Xenial.Framework.CompilerServices`
            return;
        }

        foreach (var @class in syntaxReceiver.Classes)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var (semanticModel, @classSymbol, isAttributeDeclared) = TryGetTargetType(context, compilation, @class, generateXenialActionAttribute);
            if (!isAttributeDeclared || semanticModel is null || @classSymbol is null)
            {
                continue;
            }

            var @attribute = GetXenialActionAttribute(@classSymbol, generateXenialActionAttribute);

            var builder = CurlyIndenter.Create();

            builder.WriteLine("// <auto-generated />");
            builder.WriteLine();
            builder.WriteLine("using System;");
            builder.WriteLine("using System.IO;");
            builder.WriteLine("using System.Runtime.CompilerServices;");
            builder.WriteLine();

            var isGlobalNamespace = classSymbol.ContainingNamespace.ToString() == "<global namespace>";
            if (isGlobalNamespace)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        GeneratorDiagnostics.ClassNeedsToBeInNamespace(
                        xenialActionAttributeName
                    ), @class.GetLocation())
                );

                return;
            }

            using (builder.OpenBrace($"namespace {@classSymbol.ContainingNamespace}"))
            {
                builder.WriteLine("[CompilerGenerated]");
                using (builder.OpenBrace($"partial class {@classSymbol.Name}"))
                {

                }

                builder.WriteLine();

                var actionId = $"{@classSymbol.ContainingNamespace}.{@classSymbol.Name}SimpleAction";
                var actionName = $"{@classSymbol.Name}SimpleAction";
                var controllerName = $"{@classSymbol.Name}Controller";

                builder.WriteLine("[CompilerGenerated]");
                using (builder.OpenBrace($"public partial class {controllerName} : DevExpress.ExpressApp.ViewController"))
                {
                    builder.WriteLine($"public DevExpress.ExpressApp.Actions.SimpleAction {actionName} {{ get; private set; }}");
                    using (builder.OpenBrace($"public {controllerName}()"))
                    {
                        //TODO: Action Category
                        builder.WriteLine($"this.{actionName} = new DevExpress.ExpressApp.Actions.SimpleAction(this, \"{actionId}\", \"Edit\");");
                    }
                }
            }

            compilation = AddGeneratedCode(context, compilation, @class, builder);
        }
    }

    private static Compilation AddGeneratedCode(
        GeneratorExecutionContext context,
        Compilation compilation,
        ClassDeclarationSyntax @class,
        CurlyIndenter builder
    )
    {
        var syntax = builder.ToString();
        var source = SourceText.From(syntax, Encoding.UTF8);

        var fileName = Path.GetFileNameWithoutExtension(@class.SyntaxTree.FilePath);

        if (fileName is "")
        {
            fileName = Guid.NewGuid().ToString();
        }

        context.AddSource($"{fileName}.g.cs", source);

        return compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(syntax, (CSharpParseOptions)context.ParseOptions, cancellationToken: context.CancellationToken));
    }

    private static AttributeData GetXenialActionAttribute(INamedTypeSymbol symbol, INamedTypeSymbol generateXenialActionAttribute)
        => symbol.GetAttribute(generateXenialActionAttribute);

    private static (SemanticModel? semanticModel, INamedTypeSymbol? @classSymbol, bool isAttributeDeclared) TryGetTargetType(
        GeneratorExecutionContext context,
        Compilation compilation,
        ClassDeclarationSyntax @class,
        INamedTypeSymbol generateXenialImageNamesAttribute
    )
    {
        var semanticModel = compilation.GetSemanticModel(@class.SyntaxTree);
        if (semanticModel is null)
        {
            return (semanticModel, null, false);
        }

        var symbol = semanticModel.GetDeclaredSymbol(@class, context.CancellationToken);

        if (symbol is null)
        {
            return (semanticModel, null, false);
        }

        var isAttributeDeclared = symbol.IsAttributeDeclared(generateXenialImageNamesAttribute);

        if (isAttributeDeclared && !@class.HasModifier(SyntaxKind.PartialKeyword))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    GeneratorDiagnostics.ClassNeedsToBePartial(xenialActionAttributeFullName),
                    @class.GetLocation()
            ));

            return (semanticModel, symbol, false);
        }

        return (semanticModel, symbol, isAttributeDeclared);
    }
}


