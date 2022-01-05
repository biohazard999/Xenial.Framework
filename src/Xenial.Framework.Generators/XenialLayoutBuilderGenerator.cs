﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using Xenial.Framework.Generators.Internal;
using Xenial.Framework.MsBuild;

namespace Xenial.Framework.Generators;

public class XenialLayoutBuilderGenerator : IXenialSourceGenerator
{
    private const string layoutBuilderBaseType = "Xenial.Framework.Layouts.LayoutBuilder<TModelClass>";

    public bool Accepts(TypeDeclarationSyntax typeDeclarationSyntax)
    {
        _ = typeDeclarationSyntax ?? throw new ArgumentNullException(nameof(typeDeclarationSyntax));

        if (typeDeclarationSyntax.BaseList is not null)
        {
            var baseTypeSyntax = typeDeclarationSyntax.BaseList.Types.OfType<SimpleBaseTypeSyntax>();

            var derivesFromLayoutBuilder = baseTypeSyntax.Select(b => b.Type).OfType<GenericNameSyntax>()
                .Any(g => g.Identifier.ToFullString().Contains("LayoutBuilder"));

            return derivesFromLayoutBuilder;
        }

        return false;
    }

    public Compilation Execute(
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

        var xenialExpandMemberAttribute = compilation.GetTypeByMetadataName(XenialExpandMemberAttributeGenerator.XenialExpandMemberAttributeFullName);

        if (xenialExpandMemberAttribute is null)
        {
            //TODO: Warning Diagnostics for either setting the right MSBuild properties or referencing `Xenial.Framework.CompilerServices`
            return compilation;
        }

        foreach (var @class in types)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var (semanticModel, @classSymbol) = TryGetTargetType(context, compilation, @class);
            if (semanticModel is null || @classSymbol is null)
            {
                continue;
            }

            if (
                classSymbol.BaseType is null
                || !classSymbol.BaseType.IsGenericType
                || classSymbol.BaseType.OriginalDefinition.ToDisplayString() != layoutBuilderBaseType
            )
            {
                continue;
            }

            INamedTypeSymbol? targetType = null;
            if (
                classSymbol.BaseType.IsGenericType
                && classSymbol.BaseType.OriginalDefinition.ToDisplayString() == layoutBuilderBaseType
            )
            {
                targetType = classSymbol.BaseType.TypeArguments.OfType<INamedTypeSymbol>().FirstOrDefault();

                if (targetType is not null && @class.HasModifier(SyntaxKind.AbstractKeyword))
                {
                    continue;
                }

                if (targetType is not null && !@class.HasModifier(SyntaxKind.PartialKeyword))
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            GeneratorDiagnostics.ClassShouldBePartialWhenDerivingFrom(layoutBuilderBaseType),
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

            var builder = CurlyIndenter.Create();

            builder.WriteLine("// <auto-generated />");
            builder.WriteLine();
            builder.WriteLine("using System;");
            builder.WriteLine("using System.Runtime.CompilerServices;");
            builder.WriteLine();
            builder.WriteLine("using Xenial.Framework.Layouts;");
            builder.WriteLine("using Xenial.Framework.Layouts.Items;");
            builder.WriteLine("using Xenial.Framework.Layouts.Items.Base;");
            builder.WriteLine("using Xenial.Framework.Layouts.Items.LeafNodes;");
            builder.WriteLine();

            var isGlobalNamespace = classSymbol.ContainingNamespace.ToString() == "<global namespace>";
            if (isGlobalNamespace)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        GeneratorDiagnostics.ClassShouldBeInNamespaceWhenDerivingFrom(
                        layoutBuilderBaseType
                    ), @class.GetLocation())
                );

                return compilation;
            }

            var visibility = context.GetDefaultAttributeModifier();
            using (builder.OpenBrace($"namespace {@classSymbol.ContainingNamespace}"))
            {
                builder.WriteLine("[CompilerGenerated]");
                ////We don't need to specify any other modifier
                ////because the user can decide if he want it to be an instance type.
                ////We also don't need to specify the visibility for partial types
                using (builder.OpenBrace($"partial {(@classSymbol.IsRecord ? "record" : "class")} {@classSymbol.Name}"))
                {
                    //using (builder.OpenBrace("private struct PropertyIdentifier"))
                    //{
                    //    builder.WriteLine("private string propertyName;");
                    //    builder.WriteLine("public string PropertyName { get { return this.propertyName; } }");
                    //    builder.WriteLine();

                    //    using (builder.OpenBrace("private PropertyIdentifier(string propertyName)"))
                    //    {
                    //        builder.WriteLine("this.propertyName = propertyName;");
                    //    }
                    //    builder.WriteLine();

                    //    using (builder.OpenBrace("public static implicit operator string(PropertyIdentifier identifier)"))
                    //    {
                    //        builder.WriteLine("return identifier.PropertyName;");
                    //    }
                    //    builder.WriteLine();

                    //    using (builder.OpenBrace("public static PropertyIdentifier Create(string propertyName)"))
                    //    {
                    //        builder.WriteLine("return new PropertyIdentifier(propertyName);");
                    //    }
                    //}

                    if (GetAllProperties(targetType).Any())
                    {
                        builder.WriteLine();
                        var properties = GetAllProperties(targetType).ToList();

                        using (builder.OpenBrace("private partial struct Constants"))
                        {
                            WritePropertyConstants(builder, properties, Enumerable.Empty<string>());
                        }
                        builder.WriteLine();

                        //using (builder.OpenBrace("private partial struct Property"))
                        //{
                        //    WritePropertyConstants(builder, properties, Enumerable.Empty<string>());
                        //}
                        //builder.WriteLine();

                        using (builder.OpenBrace("private partial struct Editor"))
                        {
                            WritePropertyLayoutItems(builder, properties, Enumerable.Empty<string>());
                        }
                    }
                }
            }

            compilation = AddGeneratedCode(context, compilation, @class, builder, addedSourceFiles, emitFile: false);

            var exandedFields = new List<string>();

            compilation = AddExpandedFields(
                context,
                compilation,
                @class,
                xenialExpandMemberAttribute,
                classSymbol,
                targetType,
                builder,
                exandedFields,
                addedSourceFiles
            );

            compilation = AddGeneratedCode(context, compilation, @class, builder, addedSourceFiles);
        }

        return compilation;
    }

    internal static IEnumerable<IPropertySymbol> GetAllProperties(INamedTypeSymbol targetType)
    {
        var members = targetType.GetMembers().OfType<IPropertySymbol>();

        static bool ShouldYieldMember(IPropertySymbol member)
            => !member.IsAbstract
                && !member.IsIndexer
                && !member.IsStatic
                && member.GetResultantVisibility() == SymbolVisibility.Public;

        foreach (var member in members)
        {
            if (ShouldYieldMember(member))
            {
                yield return member;
            }
        }

        if (targetType.BaseType is not null)
        {
            foreach (var baseMember in GetAllProperties(targetType.BaseType))
            {
                if (ShouldYieldMember(baseMember))
                {
                    yield return baseMember;
                }
            }
        }
    }

    private static Compilation AddExpandedFields(
        GeneratorExecutionContext context,
        Compilation compilation,
        TypeDeclarationSyntax @class,
        INamedTypeSymbol xenialExpandMemberAttribute,
        INamedTypeSymbol? classSymbol,
        INamedTypeSymbol? targetType,
        CurlyIndenter builder,
        List<string> expandedFields,
        IList<string> addedSourceFiles,
        string? parentPrefix = null
    )
    {
        if (targetType is null || classSymbol is null)
        {
            return compilation;
        }

        targetType = compilation.GetTypeByMetadataName(targetType.ToDisplayString());
        classSymbol = compilation.GetTypeByMetadataName(classSymbol.ToDisplayString());

        if (targetType is null || classSymbol is null)
        {
            return compilation;
        }

        var expandMemberAttributes = classSymbol.GetAttributes(xenialExpandMemberAttribute).ToList();

        var expandMembers = expandMemberAttributes
            .Where(a => a.ConstructorArguments.Length > 0)
            .Select(a => a.ConstructorArguments[0].Value!.ToString())
            .Where(expandMember => !expandedFields.Contains(expandMember))
            .ToList();

        if (expandMembers.Count <= 0)
        {
            return compilation;
        }

        foreach (var expandMember in expandMembers.Distinct())
        {
            var expandMemberParts = expandMember.Split('.');

            static (IEnumerable<string>, IEnumerable<IPropertySymbol>) FindMemberTrain(INamedTypeSymbol targetType, string[] expandMemberParts)
            {
                var typeToExpandMembers = targetType;
                foreach (var expandMemberPart in expandMemberParts)
                {
                    var foundPart = GetAllProperties(typeToExpandMembers).FirstOrDefault(m => m.Name == expandMemberPart);
                    if (foundPart is not null && foundPart.Type is INamedTypeSymbol namedType)
                    {
                        typeToExpandMembers = namedType;
                    }
                }

                return (
                    expandMemberParts.ToArray(),
                    GetAllProperties(typeToExpandMembers).ToArray()
                );
            }

            var (parents, properties) = FindMemberTrain(targetType, expandMemberParts);

            if (parents.Any() && properties.Any())
            {
                using (builder.OpenBrace($"namespace {@classSymbol.ContainingNamespace}"))
                {
                    using (builder.OpenBrace($"partial {(@classSymbol.IsRecord ? "record" : "class")} {@classSymbol.Name}"))
                    {
                        using (builder.OpenBrace("private partial struct Constants"))
                        {
                            using (WriteParentClassTrain(builder, parents))
                            {
                                WritePropertyConstants(builder, properties, parents);
                            }
                        }
                        builder.WriteLine();

                        //using (builder.OpenBrace("private partial struct Property"))
                        //{
                        //    using (WriteParentClassTrain(builder, parents))
                        //    {
                        //        WritePropertyIdentitfiers(builder, properties, parents);
                        //    }
                        //}
                        //builder.WriteLine();

                        using (builder.OpenBrace("private partial struct Editor"))
                        {
                            using (WriteParentClassTrain(builder, parents))
                            {
                                WritePropertyLayoutItems(builder, properties, parents);
                            }
                        }
                    }
                }
            }
        }

        compilation = AddGeneratedCode(
            context,
            compilation,
            @class,
            builder,
            addedSourceFiles,
            emitFile: false
        );

        return AddExpandedFields(context, compilation, @class, xenialExpandMemberAttribute, classSymbol, targetType, builder, expandedFields, addedSourceFiles);
    }

    private static string ToPropertyTrain(IEnumerable<string> prefix, string name)
        => string.Join(".", prefix.Concat(new[] { name }));

    private static void WritePropertyConstants(
        CurlyIndenter builder,
        IEnumerable<IPropertySymbol> properties,
        IEnumerable<string> prefix
    )
    {
        foreach (var property in properties)
        {
            var value = ToPropertyTrain(prefix, property.Name);
            builder.WriteLine($"public const string {property.Name} = \"{value}\";");
        }
    }

    private static void WritePropertyIdentitfiers(
        CurlyIndenter builder,
        IEnumerable<IPropertySymbol> properties,
        IEnumerable<string> prefix
    )
    {
        foreach (var property in properties)
        {
            var value = ToPropertyTrain(prefix, property.Name);
            builder.WriteLine($"public static PropertyIdentifier {property.Name} {{ get {{ return PropertyIdentifier.Create(\"{value}\"); }} }}");
        }
    }

    private static void WritePropertyLayoutItems(
        CurlyIndenter builder,
        IEnumerable<IPropertySymbol> properties,
        IEnumerable<string> prefix
    )
    {
        const string booleanEditor = "BooleanLayoutPropertyEditorItem";
        const string stringEditor = "StringLayoutPropertyEditorItem";
        const string numberEditor = "NumberLayoutPropertyEditorItem";
        const string defaultEditor = "LayoutPropertyEditorItem";

        static string GetLayoutPropertyEditorItemType(IPropertySymbol property)
            => property.Type.SpecialType switch
            {
                SpecialType.System_Boolean => booleanEditor,
                SpecialType.System_String => stringEditor,
                SpecialType.System_Int16 => numberEditor,
                SpecialType.System_Int32 => numberEditor,
                SpecialType.System_Int64 => numberEditor,
                SpecialType.System_Double => numberEditor,
                SpecialType.System_Single => numberEditor,
                SpecialType.System_Decimal => numberEditor,
                _ => defaultEditor
            };

        foreach (var property in properties)
        {
            var editor = GetLayoutPropertyEditorItemType(property);
            var value = ToPropertyTrain(prefix, property.Name);
            builder.WriteLine($"public static {editor} {property.Name} {{ get {{ return {editor}.Create(\"{value}\"); }} }}");
        }
    }

    private static IDisposable WriteParentClassTrain(CurlyIndenter builder, IEnumerable<string> parentTrain)
    {
        //We need to materialize the immediate, because otherwise it's called when disposed, which defeats the purpose
        var disposables = parentTrain
            .Select(parentName => builder.OpenBrace($"public partial struct _{parentName}"))
            .ToList();

        return new AggregateDisposable(disposables);
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

    private static (SemanticModel? semanticModel, INamedTypeSymbol? @classSymbol) TryGetTargetType(
        GeneratorExecutionContext context,
        Compilation compilation,
        TypeDeclarationSyntax @class
    )
    {
        var semanticModel = compilation.GetSemanticModel(@class.SyntaxTree);
        if (semanticModel is null)
        {
            return (semanticModel, null);
        }

        var symbol = semanticModel.GetDeclaredSymbol(@class, context.CancellationToken);

        if (symbol is null)
        {
            return (semanticModel, null);
        }

        return (semanticModel, symbol);
    }

    private record AggregateDisposable(IList<IDisposable> Disposables) : IDisposable
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "It collects and throws an AggregateException")]
        void IDisposable.Dispose()
        {
            List<Exception> exceptions = new();
            foreach (var disposable in Disposables)
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }
            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }
        }
    }
}

