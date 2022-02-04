﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using Xenial.Framework.Generators.Base;
using Xenial.Framework.MsBuild;

namespace Xenial.Framework.Generators.Partial;

public record XenialModelBuilderGenerator(bool AddSource = true) : XenialPartialGenerator(AddSource)
{
    private const string modelBuilderBaseType = "Xenial.Framework.ModelBuilders.ModelBuilder<TClassType>";

    public override bool Accepts(TypeDeclarationSyntax typeDeclarationSyntax)
    {
        _ = typeDeclarationSyntax ?? throw new ArgumentNullException(nameof(typeDeclarationSyntax));

        if (typeDeclarationSyntax.BaseList is not null)
        {
            var baseTypeSyntax = typeDeclarationSyntax.BaseList.Types.OfType<SimpleBaseTypeSyntax>();

            var derivesFromLayoutBuilder = baseTypeSyntax.Select(b => b.Type).OfType<GenericNameSyntax>()
                .Any(g => g.Identifier.ToFullString().Contains("ModelBuilder"));

            return derivesFromLayoutBuilder;
        }

        return false;
    }

    public override Compilation Execute(
        GeneratorExecutionContext context,
        Compilation compilation,
        IList<TypeDeclarationSyntax> types,
        IList<string> addedSourceFiles
    )
    {
        _ = compilation ?? throw new ArgumentNullException(nameof(compilation));
        _ = types ?? throw new ArgumentNullException(nameof(types));
        _ = addedSourceFiles ?? throw new ArgumentNullException(nameof(addedSourceFiles));

        context.CancellationToken.ThrowIfCancellationRequested();

        foreach (var @class in types)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (TryGetTarget(context, compilation, @class, out var target))
            {
                if (
                    target.Symbol.BaseType is null
                    || !target.Symbol.BaseType.IsGenericType
                    || target.Symbol.BaseType.OriginalDefinition.ToDisplayString() != modelBuilderBaseType
                )
                {
                    continue;
                }

                INamedTypeSymbol? targetType = null;
                if (
                    target.Symbol.BaseType.IsGenericType
                    && target.Symbol.BaseType.OriginalDefinition.ToDisplayString() == modelBuilderBaseType
                )
                {
                    targetType = target.Symbol.BaseType.TypeArguments.OfType<INamedTypeSymbol>().FirstOrDefault();

                    if (targetType is not null && @class.HasModifier(SyntaxKind.AbstractKeyword))
                    {
                        continue;
                    }

                    if (targetType is not null && !@class.HasModifier(SyntaxKind.PartialKeyword))
                    {
                        context.ReportDiagnostic(
                            Diagnostic.Create(
                                GeneratorDiagnostics.ClassShouldBePartialWhenDerivingFrom(modelBuilderBaseType),
                                @class.GetLocation()
                            ));
                        continue;
                    }
                }

                if (@class.HasModifier(SyntaxKind.AbstractKeyword) || !@class.HasModifier(SyntaxKind.PartialKeyword))
                {
                    continue;
                }

                if (targetType is null)
                {
                    continue;
                }

                //var @attribute = GetXenialLayoutBuilderAttribute(@target.Symbol, generateXenialLayoutBuilderAttribute);

                var builder = CurlyIndenter.Create();

                builder.WriteLine("// <auto-generated />");
                builder.WriteLine();
                builder.WriteLine("using System;");
                builder.WriteLine("using System.Runtime.CompilerServices;");
                builder.WriteLine();
                builder.WriteLine("using DevExpress.ExpressApp.DC;");
                builder.WriteLine();
                builder.WriteLine("using Xenial.Framework.ModelBuilders;");
                builder.WriteLine();

                var isGlobalNamespace = target.Symbol.ContainingNamespace.ToString() == "<global namespace>";
                if (isGlobalNamespace)
                {
                    if (targetType is not null && @class.HasModifier(SyntaxKind.AbstractKeyword))
                    {
                        continue;
                    }

                    if (targetType is not null && !@class.HasModifier(SyntaxKind.PartialKeyword))
                    {
                        context.ReportDiagnostic(
                            Diagnostic.Create(
                                GeneratorDiagnostics.ClassShouldBePartialWhenDerivingFrom(modelBuilderBaseType),
                                @class.GetLocation()
                            ));
                        continue;
                    }
                }

                var visibility = context.GetDefaultAttributeModifier();
                using (builder.OpenBrace($"namespace {@target.Symbol.ContainingNamespace}"))
                {
                    builder.WriteLine("[CompilerGenerated]");
                    ////We don't need to specify any other modifier
                    ////because the user can decide if he want it to be an instance type.
                    ////We also don't need to specify the visibility for partial types
                    using (builder.OpenBrace($"partial {(@target.Symbol.IsRecord ? "record" : "class")} {@target.Symbol.Name}"))
                    {
                        if (targetType.GetMembers().OfType<IPropertySymbol>().Any())
                        {
                            var properties = targetType.GetMembers().OfType<IPropertySymbol>().ToList();

                            WritePropertyBuilderAccessor(builder, properties);
                        }
                    }
                }

                compilation = AddGeneratedCode(context, compilation, @class, builder, addedSourceFiles, emitFile: true);
            }
        }

        return compilation;
    }

    private static void WritePropertyBuilderAccessor(
        CurlyIndenter builder,
        IEnumerable<IPropertySymbol> properties
    )
    {
        foreach (var property in properties)
        {
            var value = $"For<{property.Type.ToDisplayString()}>(\"{property.Name}\")";
            builder.WriteLine($"private IPropertyBuilder<{property.Type.ToDisplayString()}, {property.ContainingType.ToDisplayString()}> {property.Name} => {value};");
        }
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

        if (emitFile)
        {
            if (!addedSourceFiles.Contains(hintName))
            {
                addedSourceFiles.Add(hintName);
                context.AddSource(hintName, source);
            }
        }

        var syntaxTree = CSharpSyntaxTree.ParseText(syntax, (CSharpParseOptions)context.ParseOptions, cancellationToken: context.CancellationToken);

        return compilation.AddSyntaxTrees(syntaxTree);
    }
}

