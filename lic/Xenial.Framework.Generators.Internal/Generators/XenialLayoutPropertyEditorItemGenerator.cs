using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using Xenial.Framework.MsBuild;

namespace Xenial.Framework.Generators.Internal.Generators;

internal class XenialLayoutPropertyEditorItemGenerator : XenialBaseGenerator, IXenialSourceGenerator
{
    public XenialLayoutPropertyEditorItemGenerator(IDictionary<string, string>? constantsToInject) : base(constantsToInject) { }

    public Compilation Execute(GeneratorExecutionContext context, Compilation compilation, IList<TypeDeclarationSyntax> types)
    {
        compilation = GenerateXenialLayoutBuilderAttribute(context, compilation);

        return compilation;
    }

    private static Compilation GenerateXenialLayoutBuilderAttribute(GeneratorExecutionContext context, Compilation compilation)
    {
        var (source, syntaxTree) = GenerateXenialLayoutBuilderAttribute(
            (CSharpParseOptions)context.ParseOptions,
            cancellationToken: context.CancellationToken
        );

        context.AddSource($"XenialLayoutPropertyEditorItemAttribute.g.cs", source);

        return compilation.AddSyntaxTrees(syntaxTree);
    }

    public static (SourceText source, SyntaxTree syntaxTree) GenerateXenialLayoutBuilderAttribute(
        CSharpParseOptions? parseOptions = null,
        string visibility = "internal",
        CancellationToken cancellationToken = default)
    {
        parseOptions = parseOptions ?? CSharpParseOptions.Default;

        var syntaxWriter = CurlyIndenter.Create();

        syntaxWriter.WriteLine($"using System;");
        syntaxWriter.WriteLine($"using System.ComponentModel;");
        syntaxWriter.WriteLine();

        using (syntaxWriter.OpenBrace($"namespace Xenial"))
        {
            syntaxWriter.WriteLine("[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]");

            using (syntaxWriter.OpenBrace($"{visibility} sealed class XenialLayoutPropertyEditorItemAttribute : Attribute"))
            {
                syntaxWriter.WriteLine($"public Type PropertyType {{ get; private set; }}");
                syntaxWriter.WriteLine();
                using (syntaxWriter.OpenBrace($"{visibility} XenialLayoutPropertyEditorItemAttribute(Type propertyType)"))
                {
                    syntaxWriter.WriteLine($"this.PropertyType = propertyType;");
                }
            }
        }

        var syntax = syntaxWriter.ToString();
        var source = SourceText.From(syntax, Encoding.UTF8);
        var syntaxTree = CSharpSyntaxTree.ParseText(syntax, parseOptions, cancellationToken: cancellationToken);
        return (source, syntaxTree);
    }
}
