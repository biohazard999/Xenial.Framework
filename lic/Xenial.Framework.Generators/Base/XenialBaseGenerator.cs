
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using System;
using System.Collections.Generic;
using System.Text;

using Xenial.Framework.Generators.Internal;

namespace Xenial.Framework.Generators.Base;

public abstract record XenialBaseGenerator(bool AddSource = true) : IXenialSourceGenerator
{
    protected const string XenialNamespace = "Xenial";

    public virtual string AttributeNamespace => XenialNamespace;

    public abstract bool Accepts(TypeDeclarationSyntax typeDeclarationSyntax);

    public abstract Compilation Execute(GeneratorExecutionContext context, Compilation compilation, IList<TypeDeclarationSyntax> types, IList<string> addedSourceFiles);

    protected Compilation AddCode(
        GeneratorExecutionContext context,
        Compilation compilation,
        IList<string> addedSourceFiles,
        string fileNameWithoutExtension,
        string syntax
    )
    {
        _ = compilation ?? throw new ArgumentNullException(nameof(compilation));
        _ = addedSourceFiles ?? throw new ArgumentNullException(nameof(addedSourceFiles));
        _ = fileNameWithoutExtension ?? throw new ArgumentNullException(nameof(fileNameWithoutExtension));

        var fileName = $"{fileNameWithoutExtension}.g.cs";

        var source = SourceText.From(syntax, Encoding.UTF8);

        if (AddSource)
        {
            if (!addedSourceFiles.Contains(fileName))
            {
                addedSourceFiles.Add(fileName);

                static void AddShortFile(
                    GeneratorExecutionContext context,
                    IList<string> addedSourceFiles,
                    string fileNameWithoutExtension,
                    string fileName,
                    SourceText source
                )
                {
                    var indexOf = addedSourceFiles.IndexOf(fileName);

                    var first10Chars = fileNameWithoutExtension.Length < 10
                        ? fileNameWithoutExtension
                        : fileNameWithoutExtension.Substring(0, 10);

                    var contextHintName = $"{first10Chars}.{Utils.ShortNameHelper.Encode(indexOf)}.g.cs";
                    context.AddSource(contextHintName, source);
                }

                static void AddFile(
                    GeneratorExecutionContext context,
                    string fileName,
                    SourceText source
                )
                {
                    context.AddSource(fileName, source);
                }

                if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.EmitCompilerGeneratedFiles", out var generateXenialAttrStr) && bool.TryParse(generateXenialAttrStr, out var emitCompilerGeneratedFiles))
                {
                    if (emitCompilerGeneratedFiles)
                    {
                        AddShortFile(context, addedSourceFiles, fileNameWithoutExtension, fileName, source);
                    }
                    else
                    {
                        AddFile(context, fileName, source);
                    }
                }
                else
                {
                    AddFile(context, fileName, source);
                }
            }
        }

        var syntaxTree = CSharpSyntaxTree.ParseText(syntax, (CSharpParseOptions)context.ParseOptions, cancellationToken: context.CancellationToken);

        return compilation.AddSyntaxTrees(syntaxTree);
    }

    protected static bool IsInGlobalNamespace(
        GeneratorExecutionContext context,
        Compilation compilation,
        INamedTypeSymbol classSymbol,
        string attributeName,
        Location location,
        out Compilation comp
    )
    {
        comp = compilation;
        var isGlobalNamespace = classSymbol?.ContainingNamespace.ToString() == "<global namespace>";
        if (isGlobalNamespace)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    GeneratorDiagnostics.ClassNeedsToBeInNamespace(
                    attributeName
                ), location)
            );

            return true;
        }

        return false;
    }
}
