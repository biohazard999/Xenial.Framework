using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using Xenial.Framework.Generators.Internal;
using Xenial.Framework.Generators.XAF.Utils;
using Xenial.Framework.MsBuild;

namespace Xenial.Framework.Generators.XAF;

internal record XenialAutoMappedAttributeGenerator(bool AddSource = true) : IXenialSourceGenerator
{
    internal static (Compilation, INamedTypeSymbol) FindXenialAutoMappedAttribute(Compilation compilation)
    {
        var attribute = compilation.GetTypeByMetadataName("Xenial.XenialAutoMappedAttribute");

        return (compilation, attribute!);
    }

    public bool Accepts(TypeDeclarationSyntax typeDeclarationSyntax) => false;

    public Compilation Execute(GeneratorExecutionContext context, Compilation compilation, IList<TypeDeclarationSyntax> types, IList<string> addedSourceFiles)
    {
        (compilation, var attribute) = FindXenialAutoMappedAttribute(compilation);
        if (attribute is not null)
        {
            return compilation;
        }

        (compilation, _) = GenerateXenialAutoMappedAttribute(context, compilation, addedSourceFiles);
        return compilation;
    }

    private static Compilation AddGeneratedCode(
        GeneratorExecutionContext context,
        Compilation compilation,
        TypeDeclarationSyntax @class,
        CurlyIndenter builder,
        IList<string> addedSourceFiles,
        string? hintName = null,
        bool emitFile = true
    )
    {
        var syntax = builder.ToString();
        var source = SourceText.From(syntax, Encoding.UTF8);

        var fileName = Path.GetFileNameWithoutExtension(@class.SyntaxTree.FilePath);

        if (fileName is "")
        {
            fileName = Guid.NewGuid().ToString();
        }

        hintName = string.IsNullOrEmpty(hintName) ? $"{fileName}.{@class.Identifier}.g.cs" : $"{@class.Identifier}.{hintName}.g.cs";

        AddFileToContext(context, addedSourceFiles, emitFile, hintName, source);

        var syntaxTree = CSharpSyntaxTree.ParseText(syntax, (CSharpParseOptions)context.ParseOptions, cancellationToken: context.CancellationToken);

        return compilation.AddSyntaxTrees(syntaxTree);
    }

    private static void AddFileToContext(
        GeneratorExecutionContext context,
        IList<string> addedSourceFiles,
        bool emitFile,
        string hintName,
        SourceText source
    )
    {
        if (emitFile)
        {
            if (!addedSourceFiles.Contains(hintName))
            {
                addedSourceFiles.Add(hintName);
                var indexOf = addedSourceFiles.IndexOf(hintName);
                var contextHintName = $"{ShortNameHelper.Encode(indexOf)}.g.cs";
                context.AddSource(contextHintName, source);
            }
        }
    }

    private (Compilation, INamedTypeSymbol) GenerateXenialAutoMappedAttribute(GeneratorExecutionContext context, Compilation compilation, IList<string> addedSourceFiles)
    {
        var (source, syntaxTree) = GenerateXenialAutoMappedAttribute(
            (CSharpParseOptions)context.ParseOptions,
            cancellationToken: context.CancellationToken
        );

        if (AddSource)
        {
            var fileName = $"XenialAutoMappedAttribute.g.cs";
            addedSourceFiles.Add(fileName);
            context.AddSource(fileName, source);
        }

        compilation = compilation.AddSyntaxTrees(syntaxTree);

        var attribute = compilation.GetTypeByMetadataName("Xenial.XenialAutoMappedAttribute");

        return (compilation, attribute!);
    }

    public static (SourceText source, SyntaxTree syntaxTree) GenerateXenialAutoMappedAttribute(
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
            syntaxWriter.WriteLine("[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]");

            using (syntaxWriter.OpenBrace($"{visibility} sealed class XenialAutoMappedAttribute : Attribute"))
            {
                syntaxWriter.WriteLine();
                using (syntaxWriter.OpenBrace($"{visibility} XenialAutoMappedAttribute()"))
                {
                }
            }
        }

        var syntax = syntaxWriter.ToString();
        var source = SourceText.From(syntax, Encoding.UTF8);
        var syntaxTree = CSharpSyntaxTree.ParseText(syntax, parseOptions, cancellationToken: cancellationToken);
        return (source, syntaxTree);
    }
}
