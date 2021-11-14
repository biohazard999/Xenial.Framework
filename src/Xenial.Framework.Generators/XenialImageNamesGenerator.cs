

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Xenial.Framework.MsBuild;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Xenial.Framework.Generators;

[Generator]
public class XenialImageNamesGenerator : ISourceGenerator
{
    private const string xenialImageNamesAttributeName = "XenialImageNamesAttribute";
    private const string xenialNamespace = "Xenial";
    private const string xenialImageNamesAttributeFullName = $"{xenialNamespace}.{xenialImageNamesAttributeName}";
    private const string generateXenialImageNamesAttributeMSBuildProperty = $"Generate{xenialImageNamesAttributeName}";

    private const string markAsXenialImageSourceMetadataAttribute = "XenialImageNames";

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

        var compilation = GenerateAttribute(context);

        var generateXenialImageNamesAttribute = compilation.GetTypeByMetadataName(xenialImageNamesAttributeFullName);

        foreach (var @class in syntaxReceiver.Classes)
        {
            if (!@class.Modifiers.Any(mod => mod.Text == Token(SyntaxKind.PartialKeyword).Text))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        GeneratorDiagnostics.ClassNeedsToBePartial(xenialImageNamesAttributeFullName),
                        @class.GetLocation()
                ));
            }
        }
    }

    private static IEnumerable<ImageInformation> GetImages(GeneratorExecutionContext context)
    {
        foreach (var additionalText in context.AdditionalFiles)
        {
            var options = context.AnalyzerConfigOptions.GetOptions(additionalText);
            if (options is not null && options.TryGetValue(markAsXenialImageSourceMetadataAttribute, out var markedAsImageSourceStr))
            {
                if (bool.TryParse(markedAsImageSourceStr, out var markedAsImageSource))
                {
                    if (markedAsImageSource)
                    {
                        var path = additionalText.Path;
                        var fileName = Path.GetFileName(path);
                        var name = Path.GetFileNameWithoutExtension(path);
                        var extension = Path.GetExtension(path);

                        yield return new ImageInformation(path, fileName, name, extension);
                    }
                }
                else
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            GeneratorDiagnostics.InvalidBooleanMsBuildProperty(
                            markAsXenialImageSourceMetadataAttribute,
                            markedAsImageSourceStr
                        ), null)
                    );
                }
            }
        }
    }


    private static Compilation GenerateAttribute(GeneratorExecutionContext context)
    {
        var compilation = context.Compilation;
        if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{generateXenialImageNamesAttributeMSBuildProperty}", out var generateXenialImageNamesAttrStr))
        {
            if (bool.TryParse(generateXenialImageNamesAttrStr, out var generateXenialImageNamesAttr))
            {
                if (!generateXenialImageNamesAttr)
                {
                    return compilation;
                }
            }
            else
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        GeneratorDiagnostics.InvalidBooleanMsBuildProperty(
                            generateXenialImageNamesAttributeMSBuildProperty,
                            generateXenialImageNamesAttrStr
                        )
                        , null
                    ));
                return compilation;
            }
        }

        var syntaxWriter = new CurlyIndenter(new System.CodeDom.Compiler.IndentedTextWriter(new StringWriter()));

        syntaxWriter.WriteLine($"using System;");
        syntaxWriter.WriteLine();

        syntaxWriter.WriteLine($"namespace {xenialNamespace}");
        syntaxWriter.OpenBrace();

        syntaxWriter.WriteLine("[AttributeUsage(AttributeTargets.Class, Inherited = false)]");
        syntaxWriter.WriteLine($"{context.GetDefaultAttributeModifier()} sealed class {xenialImageNamesAttributeName} : Attribute");
        syntaxWriter.OpenBrace();
        syntaxWriter.WriteLine($"{context.GetDefaultAttributeModifier()} {xenialImageNamesAttributeName}() {{ }}");
        syntaxWriter.CloseBrace();

        syntaxWriter.CloseBrace();

        var syntax = syntaxWriter.ToString();
        var source = SourceText.From(syntax, Encoding.UTF8);
        context.AddSource($"XenialImageNamesAttribute.{context.Compilation.AssemblyName}.g.cs", source);

        return compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(syntax, (CSharpParseOptions)context.ParseOptions, cancellationToken: context.CancellationToken));
    }
}

public record ImageInformation(string Path, string FileName, string Name, string Extension) { }
