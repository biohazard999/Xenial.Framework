
using System;
using System.IO;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

    public void Execute(GeneratorExecutionContext context)
    {
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

        syntaxWriter.CloseBrace();

        var syntax = syntaxWriter.ToString();
        var source = SourceText.From(syntax, Encoding.UTF8);
        context.AddSource($"{xenialActionAttributeName}.g.cs", source);

        compilation = compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(syntax, (CSharpParseOptions)context.ParseOptions, cancellationToken: context.CancellationToken));

        var generateXenialActionAttribute = compilation.GetTypeByMetadataName(xenialActionAttributeFullName);

    }

    public void Initialize(GeneratorInitializationContext context)
    {
    }
}


