
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    private const string xenialDebugSourceGenerators = "XenialDebugSourceGenerators";

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
        public List<TypeDeclarationSyntax> Classes { get; } = new();

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
            if (context.Node is RecordDeclarationSyntax { AttributeLists.Count: > 0 } recordDeclarationSyntax)
            {
                var classSymbol = context.SemanticModel.GetDeclaredSymbol(recordDeclarationSyntax);

                if (classSymbol is not null)
                {
                    Classes.Add(recordDeclarationSyntax);
                }
            }
        }
    }
    private static void CheckForDebugger(GeneratorExecutionContext context)
    {
        if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{xenialDebugSourceGenerators}", out var xenialDebugSourceGeneratorsAttrString))
        {
            if (bool.TryParse(xenialDebugSourceGeneratorsAttrString, out var xenialDebugSourceGeneratorsBool))
            {
                if (xenialDebugSourceGeneratorsBool)
                {
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        return;
                    }

                    System.Diagnostics.Debugger.Launch();
                }
            }
            else
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        GeneratorDiagnostics.InvalidBooleanMsBuildProperty(
                            xenialDebugSourceGenerators,
                            xenialDebugSourceGeneratorsAttrString
                        )
                        , null
                    ));
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

        CheckForDebugger(context);

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

        using (syntaxWriter.OpenBrace($"namespace {xenialNamespace}"))
        {
            syntaxWriter.WriteLine("[AttributeUsage(AttributeTargets.Class, Inherited = false)]");
            using (syntaxWriter.OpenBrace($"{context.GetDefaultAttributeModifier()} sealed class {xenialActionAttributeName} : Attribute"))
            {
                syntaxWriter.WriteLine($"{context.GetDefaultAttributeModifier()} {xenialActionAttributeName}() {{ }}");

                syntaxWriter.WriteLine($"public string Caption {{ get; set; }}");
                syntaxWriter.WriteLine($"public string ImageName {{ get; set; }}");
                syntaxWriter.WriteLine($"public string Category {{ get; set; }}");
            }
            syntaxWriter.WriteLine();

            syntaxWriter.WriteLine($"{context.GetDefaultAttributeModifier()} interface IDetailViewAction<T> {{ }}");
            syntaxWriter.WriteLine();
            syntaxWriter.WriteLine($"{context.GetDefaultAttributeModifier()} interface IListViewAction<T> {{ }}");
        }

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

            // This is normally not needed, especially if we use the semantic model to generate the code
            ////Import usings from user code
            //builder.WriteLine("//Begin-User defined using statements");
            //var root = @class.SyntaxTree.GetCompilationUnitRoot(context.CancellationToken);
            //foreach (var @using in root.Usings)
            //{
            //    builder.WriteLine(@using.ToString());
            //}
            //builder.WriteLine("//End-User defined using statements");
            //builder.WriteLine();

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
                ITypeSymbol? GetTargetType()
                {
                    var detailViewActionInterface = @classSymbol.AllInterfaces
                        .FirstOrDefault(i => i.OriginalDefinition.ToDisplayString() == "Xenial.IDetailViewAction<T>");

                    if (detailViewActionInterface is not null)
                    {
                        if (detailViewActionInterface.IsGenericType)
                        {
                            var targetType = detailViewActionInterface.TypeArguments.First();

                            return targetType;
                        }
                    }

                    return null;
                }

                List<(IMethodSymbol? ctor, IMethodSymbol invoker)> partialMethods = new();

                if (@class.HasModifier(SyntaxKind.PartialKeyword))
                {
                    builder.WriteLine("[CompilerGenerated]");
                    using (builder.OpenBrace($"partial {(@classSymbol.IsRecord ? "record" : "class")} {@classSymbol.Name}"))
                    {
                        var methods = @class.Members.OfType<MethodDeclarationSyntax>();

                        if (!methods.Any(method => method.Identifier.Text == "Execute"))
                        {
                            var targetType = GetTargetType();
                            if (targetType is not null)
                            {
                                builder.WriteLine($"partial void Execute({targetType.ToDisplayString()} targetObject);");
                            }
                            else
                            {
                                //TODO: Warn to implement one or more of the target interfaces
                                builder.WriteLine("partial void Execute(object targetObject);");
                            }
                        }
                        else
                        {
                            var method = methods.FirstOrDefault(method => method.Identifier.Text == "Execute");
                            if (method is not null)
                            {
                                var methodSymbol = semanticModel.GetDeclaredSymbol(method, context.CancellationToken);


                                var modifiers = new SyntaxTokenList(
                                    method.Modifiers.Where(t => !t.IsKind(SyntaxKind.AsyncKeyword))
                                );

                                var returnTypeSymbol = semanticModel.GetTypeInfo(method.ReturnType, context.CancellationToken);

                                if (methodSymbol is not null && returnTypeSymbol.Type is not null)
                                {
                                    var possibleCtors = classSymbol.InstanceConstructors
                                            //record copy constructor are implicitly declared
                                            .Where(ctor => !ctor.IsImplicitlyDeclared)
                                            .GroupBy(ctor => ctor.Parameters.Length)
                                            .Select(g => (lenght: g.Key, ctors: g.ToArray()))
                                            .OrderByDescending(g => g.lenght)
                                            .Select(g => g.ctors)
                                            .FirstOrDefault();

                                    //TODO: error on conflicting ctor count
                                    var possibleCtor = possibleCtors?.FirstOrDefault();

                                    partialMethods.Add((possibleCtor, methodSymbol));

                                    var targetType = GetTargetType();
                                    if (targetType is not null)
                                    {
                                        var parameterString = string.Join(", ", methodSymbol.Parameters.Select(p => $"{p} {p.Name}"));

                                        builder.WriteLine($"{modifiers} {returnTypeSymbol.Type.ToDisplayString()} Execute({parameterString});");
                                    }
                                    else
                                    {
                                        builder.WriteLine($"{modifiers} {returnTypeSymbol.Type.ToDisplayString()} Execute(object myTarget);");
                                    }
                                }
                            }
                        }
                    }
                }

                builder.WriteLine();

                var actionId = $"{@classSymbol.ContainingNamespace}.{@classSymbol.Name}SimpleAction";
                var actionName = $"{@classSymbol.Name}SimpleAction";
                var controllerName = $"{@classSymbol.Name}Controller";

                builder.WriteLine("[CompilerGenerated]");
                using (builder.OpenBrace($"public partial class {controllerName} : DevExpress.ExpressApp.ViewController"))
                {
                    builder.WriteLine($"public DevExpress.ExpressApp.Actions.SimpleAction {actionName} {{ get; private set; }}");

                    builder.WriteLine();

                    using (builder.OpenBrace($"public {controllerName}()"))
                    {
                        builder.WriteLine("this.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;");

                        var targetType = GetTargetType();
                        if (targetType is not null)
                        {
                            builder.WriteLine($"this.TargetObjectType = typeof({targetType.ToDisplayString()});");
                        }


                        var category = attribute.GetAttributeValue("Category", "Edit") ?? "Edit";
                        //TODO: Action Category
                        builder.WriteLine($"this.{actionName} = new DevExpress.ExpressApp.Actions.SimpleAction(this, \"{actionId}\", \"{category}\");");
                        builder.WriteLine($"this.{actionName}.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;");

                        var caption = attribute.GetAttributeValue("Caption", string.Empty);
                        if (!string.IsNullOrEmpty(caption))
                        {
                            builder.WriteLine($"this.{actionName}.Caption = \"{caption}\";");
                        }

                        var imageName = attribute.GetAttributeValue("ImageName", string.Empty);
                        if (!string.IsNullOrEmpty(imageName))
                        {
                            builder.WriteLine($"this.{actionName}.ImageName = \"{imageName}\";");
                        }
                    }

                    builder.WriteLine();
                    builder.WriteLine("partial void OnActivatedCore();");
                    builder.WriteLine();
                    builder.WriteLine("partial void OnDeactivatedCore();");
                    builder.WriteLine();

                    using (builder.OpenBrace("protected override void OnActivated()"))
                    {
                        builder.WriteLine("base.OnActivated();");

                        builder.WriteLine($"this.{actionName}.Execute -= {actionName}Execute;");
                        builder.WriteLine($"this.{actionName}.Execute += {actionName}Execute;");

                        builder.WriteLine("this.OnActivatedCore();");
                    }
                    builder.WriteLine();

                    using (builder.OpenBrace("protected override void OnDeactivated()"))
                    {
                        builder.WriteLine($"this.{actionName}.Execute -= {actionName}Execute;");

                        builder.WriteLine("this.OnDeactivatedCore();");
                        builder.WriteLine("base.OnDeactivated();");
                    }
                    builder.WriteLine();

                    using (builder.OpenBrace($"private void {actionName}Execute(object sender, DevExpress.ExpressApp.Actions.SimpleActionExecuteEventArgs e)"))
                    {
                        var targetType = GetTargetType();
                        if (targetType is not null)
                        {
                            using (builder.OpenBrace($"if(e.CurrentObject is {targetType.ToDisplayString()})"))
                            {
                                builder.WriteLine($"{targetType.ToDisplayString()} currentObject = ({targetType.ToDisplayString()})e.CurrentObject;");

                                var typeMap = new Dictionary<string, string>()
                                {
                                    ["DevExpress.ExpressApp.XafApplication"] = "this.Application",
                                    ["DevExpress.ExpressApp.IObjectSpace"] = "this.ObjectSpace"
                                };

                                if (partialMethods.Count > 0)
                                {
                                    var (ctor, invoker) = partialMethods.First();
                                    if (ctor is null)
                                    {
                                        builder.WriteLine($"{classSymbol.ToDisplayString()} action = new {classSymbol.ToDisplayString()}();");
                                    }
                                    else
                                    {
                                        builder.Write($"{classSymbol.ToDisplayString()} action = new {classSymbol.ToDisplayString()}(");

                                        var parameters = new List<string>();
                                        foreach (var parameter in ctor.Parameters)
                                        {
                                            if (typeMap.TryGetValue(parameter.ToString(), out var resovledValue))
                                            {
                                                parameters.Add(resovledValue);
                                            }
                                        }

                                        builder.Write(string.Join(", ", parameters));

                                        builder.WriteLine(");");
                                    }

                                    builder.WriteLine();
                                    builder.Write($"action.Execute(currentObject");

                                    foreach (var parameter in invoker.Parameters)
                                    {
                                        if (typeMap.TryGetValue(parameter.ToString(), out var resovledValue))
                                        {
                                            builder.Write($", {resovledValue}");
                                        }
                                    }

                                    builder.WriteLine(");");
                                }
                                else
                                {
                                    //builder.WriteLine($"action.Execute(currentObject);");
                                }
                            }
                            //TODO: type match failed for whatever reason
                            //using (builder.OpenBrace("else"))
                            //{

                            //}
                        }
                    }
                }
            }

            compilation = AddGeneratedCode(context, compilation, @class, builder);
        }
    }

    private static Compilation AddGeneratedCode(
        GeneratorExecutionContext context,
        Compilation compilation,
        TypeDeclarationSyntax @class,
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

        context.AddSource($"{fileName}.{@class.Identifier}.g.cs", source);

        return compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(syntax, (CSharpParseOptions)context.ParseOptions, cancellationToken: context.CancellationToken));
    }

    private static AttributeData GetXenialActionAttribute(INamedTypeSymbol symbol, INamedTypeSymbol generateXenialActionAttribute)
        => symbol.GetAttribute(generateXenialActionAttribute);

    private static (SemanticModel? semanticModel, INamedTypeSymbol? @classSymbol, bool isAttributeDeclared) TryGetTargetType(
        GeneratorExecutionContext context,
        Compilation compilation,
        TypeDeclarationSyntax @class,
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

        //if (isAttributeDeclared && !@class.HasModifier(SyntaxKind.PartialKeyword))
        //{
        //    context.ReportDiagnostic(
        //        Diagnostic.Create(
        //            GeneratorDiagnostics.ClassNeedsToBePartial(xenialActionAttributeFullName),
        //            @class.GetLocation()
        //    ));

        //    return (semanticModel, symbol, false);
        //}

        return (semanticModel, symbol, isAttributeDeclared);
    }

}


