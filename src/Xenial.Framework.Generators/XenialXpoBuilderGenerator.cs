
using System;
using System.IO;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

using Xenial.Framework.MsBuild;

namespace Xenial.Framework.Generators;

[Generator]
public class XenialXpoBuilderGenerator : ISourceGenerator
{
    private const string xenialXpoBuilderAttributeName = "XenialXpoBuilderAttribute";
    private const string xenialNamespace = "Xenial";
    private const string xenialXpoBuilderAttributeFullName = $"{xenialNamespace}.{xenialXpoBuilderAttributeName}";
    private const string generateXenialXpoBuilderAttributeMSBuildProperty = $"Generate{xenialXpoBuilderAttributeName}";

    public void Execute(GeneratorExecutionContext context)
    {
        context.CancellationToken.ThrowIfCancellationRequested();

        var compilation = context.Compilation;
        if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{generateXenialXpoBuilderAttributeMSBuildProperty}", out var generateXenialXpoBuilderAttrStr))
        {
            if (bool.TryParse(generateXenialXpoBuilderAttrStr, out var generateXenialXpoBuilderAttr))
            {
                if (!generateXenialXpoBuilderAttr)
                {
                    return;
                }
            }
            else
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        GeneratorDiagnostics.InvalidBooleanMsBuildProperty(
                            generateXenialXpoBuilderAttributeMSBuildProperty,
                            generateXenialXpoBuilderAttrStr
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
        syntaxWriter.WriteLine($"{context.GetDefaultAttributeModifier()} sealed class {xenialXpoBuilderAttributeName} : Attribute");
        syntaxWriter.OpenBrace();
        syntaxWriter.WriteLine($"{context.GetDefaultAttributeModifier()} {xenialXpoBuilderAttributeName}() {{ }}");
        syntaxWriter.CloseBrace();

        syntaxWriter.CloseBrace();

        var syntax = syntaxWriter.ToString();
        var source = SourceText.From(syntax, Encoding.UTF8);
        context.AddSource($"{xenialXpoBuilderAttributeName}.g.cs", source);

        compilation = compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(syntax, (CSharpParseOptions)context.ParseOptions, cancellationToken: context.CancellationToken));

        var generateXenialXpoBuilderAttribute = compilation.GetTypeByMetadataName(xenialXpoBuilderAttributeFullName);
    }

    public void Initialize(GeneratorInitializationContext context)
    {
    }
}


//public class Class1
//{
//    public abstract class Class1Builder<TClass, TBuilder>
//        where TClass : Class1
//        where TBuilder : Class1Builder<TClass, TBuilder>
//    {

//    }
//}

//public class Class2 : Class1
//{
//    public class Class2Builder : Class2Builder<Class2, Class2Builder> { }
//    public abstract partial class Class2Builder<TClass, TBuilder> : Class1Builder<TClass, TBuilder>
//        where TClass : Class2
//        where TBuilder : Class2Builder<TClass, TBuilder>
//    {

//    }

//    public partial class Class2Builder<TClass, TBuilder>
//    {

//    }
//}
