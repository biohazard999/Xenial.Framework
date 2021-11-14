

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

using System;
using System.IO;
using System.Text;

using Xenial.Framework.MsBuild;

namespace Xenial.Framework.Generators
{
    [Generator]
    public class XenialImageNamesGenerator : ISourceGenerator
    {
        private const string xenialImageNamesAttributeName = "XenialImageNamesAttribute";
        private const string xenialNamespace = "Xenial";
        private const string xenialImageNamesAttributeFullName = $"{xenialNamespace}.{xenialImageNamesAttributeName}";
        private const string generateXenialImageNamesAttributeMSBuildProperty = $"Generate{xenialImageNamesAttributeName}";

        public void Execute(GeneratorExecutionContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var compilation = context.Compilation;
            if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{generateXenialImageNamesAttributeMSBuildProperty}", out var generateXenialImageNamesAttrStr))
            {
                if (bool.TryParse(generateXenialImageNamesAttrStr, out var generateXenialImageNamesAttr))
                {
                    if (!generateXenialImageNamesAttr)
                    {
                        return;
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
                    return;
                }
            }

            var syntaxWriter = new CurlyIndenter(new System.CodeDom.Compiler.IndentedTextWriter(new StringWriter()));

            syntaxWriter.WriteLine($"using System;");
            syntaxWriter.WriteLine();

            syntaxWriter.WriteLine($"namespace {xenialNamespace}");
            syntaxWriter.OpenBrace();

            syntaxWriter.WriteLine("[AttributeUsage(AttributeTargets.Class, Inherited = false)]");
            syntaxWriter.WriteLine($"internal sealed class {xenialImageNamesAttributeName} : Attribute");
            syntaxWriter.OpenBrace();
            syntaxWriter.WriteLine($"public {xenialImageNamesAttributeName}() {{ }}");
            syntaxWriter.CloseBrace();

            syntaxWriter.CloseBrace();

            var syntax = syntaxWriter.ToString();
            var source = SourceText.From(syntax, Encoding.UTF8);
            context.AddSource($"XenialImageNamesAttribute.{context.Compilation.AssemblyName}.g.cs", source);

            compilation = compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(syntax, (CSharpParseOptions)context.ParseOptions, cancellationToken: context.CancellationToken));

            var generateXenialImageNamesAttribute = compilation.GetTypeByMetadataName(xenialImageNamesAttributeFullName);

            //            var source = @"using System;
            //public static class HelloWorld
            //{
            //    public static void SayHello()
            //    {
            //        Console.WriteLine(""Hello from generated code!"");
            //    }
            //}";
            //            context.AddSource("helloWorldGenerator", SourceText.From(source, Encoding.UTF8));

            //            var descriptor = new DiagnosticDescriptor(
            //                id: "theId",
            //                title: "the title",
            //                messageFormat: "the message from {0}",
            //                category: "the category",
            //                DiagnosticSeverity.Info,
            //                isEnabledByDefault: true);

            //            var location = Location.Create(
            //                "theFile",
            //                new TextSpan(1, 2),
            //                new LinePositionSpan(
            //                    new LinePosition(1, 2),
            //                    new LinePosition(3, 4)));
            //            var diagnostic = Diagnostic.Create(descriptor, location, "hello world generator");
            //            context.ReportDiagnostic(diagnostic);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}
