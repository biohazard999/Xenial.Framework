﻿using System;
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

internal record XenialModelNodeMappingGenerator(bool AddSources = true) : IXenialSourceGenerator
{
    public bool Accepts(TypeDeclarationSyntax typeDeclarationSyntax) => false;

    public Compilation Execute(GeneratorExecutionContext context, Compilation compilation, IList<TypeDeclarationSyntax> types, IList<string> addedSourceFiles)
    {
        (compilation, var autoMappedAttribute) = XenialAutoMappedAttributeGenerator.FindXenialAutoMappedAttribute(compilation);

        (compilation, var modelOptionsAttribute) = GenerateXenialModelOptionsAttribute(context, compilation, addedSourceFiles);
        (compilation, var modelOptionsMapperAttribute) = GenerateXenialModelOptionsMapperAttribute(context, compilation, addedSourceFiles);

        (compilation, var optionTypes) = GenerateXenialModelOptionsCode(context, compilation, types, modelOptionsAttribute, autoMappedAttribute, addedSourceFiles);
        (compilation, var mappedTypes) = GenerateXenialModelOptionsMapperCode(context, compilation, types, optionTypes, modelOptionsMapperAttribute, autoMappedAttribute, addedSourceFiles);
        compilation = GenerateXenialModelOptionsMapCode(context, compilation, mappedTypes, addedSourceFiles);

        return compilation;
    }

    private (Compilation, List<(INamedTypeSymbol classSymbol, INamedTypeSymbol targetType)>) GenerateXenialModelOptionsCode(GeneratorExecutionContext context, Compilation compilation, IList<TypeDeclarationSyntax> types, INamedTypeSymbol modelOptionsAttribute, INamedTypeSymbol autoMappedAttribute, IList<string> addedSourceFiles)
    {
        var optionTypes = new List<(INamedTypeSymbol classSymbol, INamedTypeSymbol targetType)>();

        foreach (var @class in types)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var (semanticModel, @classSymbol) = TryGetTargetType(context, compilation, @class);
            if (semanticModel is null || @classSymbol is null)
            {
                continue;
            }

            foreach (var attribute in classSymbol.GetAttributes(modelOptionsAttribute))
            {
                var interfaceTypeSymbol = (INamedTypeSymbol)attribute.ConstructorArguments[0].Value!;
                var ignoredMembers = attribute.GetAttributeValues<string>("IgnoredMembers");

                var properties = GetPropertySymbols(interfaceTypeSymbol, ignoredMembers);

                static IEnumerable<string> GetExistingPropertyNames(INamedTypeSymbol classSymbol)
                {
                    var properties = classSymbol.GetMembers()
                        .OfType<IPropertySymbol>()
                        .Select(m => m.Name);

                    if (classSymbol.BaseType is not null)
                    {
                        properties = properties.Concat(GetExistingPropertyNames(classSymbol.BaseType));
                    }
                    return properties;
                }

                var existingPropertyNames = GetExistingPropertyNames(classSymbol).ToArray();
                properties = properties
                    .Where(p => !existingPropertyNames.Contains(p.Name))
                    .ToArray();

                var builder = CurlyIndenter.Create();

                builder.WriteLine("// <auto-generated />");
                builder.WriteLine();
                builder.WriteLine("using System;");
                builder.WriteLine("using System.Runtime.CompilerServices;");
                builder.WriteLine();
                builder.WriteLine("#nullable disable");
                builder.WriteLine();

                using (builder.OpenBrace($"namespace {@classSymbol.ContainingNamespace}"))
                {
                    var genericArguments = @classSymbol.IsGenericType
                        ? $"<{string.Join(", ", classSymbol.TypeArguments)}>"
                        : "";

                    using (builder.OpenBrace($"partial {(@classSymbol.IsRecord ? "record" : "class")} {@classSymbol.Name}{genericArguments}"))
                    {
                        foreach (var property in properties)
                        {
                            var privateField = property.Name.FirstCharToLowerCase();

                            builder.WriteLine($"private {property.Type} {privateField};");
                            builder.WriteLine($"internal System.Boolean Was{property.Name}Set {{ get; set; }}");

                            builder.WriteLine($"/// <summary>");
                            builder.WriteLine($"/// {GetDescription(compilation, property)}");
                            builder.WriteLine($"/// </summary>");
                            builder.WriteLine($"[{autoMappedAttribute}]");
                            using (builder.OpenBrace($"public {property.Type} {property.Name}"))
                            {
                                using (builder.OpenBrace("get"))
                                {
                                    builder.WriteLine($"return this.{privateField};");
                                }
                                using (builder.OpenBrace("set"))
                                {
                                    builder.WriteLine($"this.Was{property.Name}Set = true;");
                                    builder.WriteLine($"this.{privateField} = value;");
                                }
                            }
                            builder.WriteLine();
                        }
                    }
                }

                optionTypes.Add((classSymbol, interfaceTypeSymbol));

                var hintName = @classSymbol.IsGenericType
                    ? $"{@classSymbol.Name}.{interfaceTypeSymbol}.{string.Join(".", classSymbol.TypeArguments)}"
                    : $"{@classSymbol.Name}.{interfaceTypeSymbol}";

                compilation = AddGeneratedCode(context, compilation, @class, builder, addedSourceFiles, hintName, emitFile: AddSources);
            }
        }

        return (compilation, optionTypes);
    }

    private (Compilation compilation, Dictionary<(TypeDeclarationSyntax, INamedTypeSymbol), List<(INamedTypeSymbol fromType, INamedTypeSymbol toType)>>) GenerateXenialModelOptionsMapperCode(GeneratorExecutionContext context, Compilation compilation, IList<TypeDeclarationSyntax> types, List<(INamedTypeSymbol classSymbol, INamedTypeSymbol targetType)> optionTypes, INamedTypeSymbol modelOptionsMapperAttribute, INamedTypeSymbol autoMappedAttribute, IList<string> addedSourceFiles)
    {
        var mappedItems = new Dictionary<(TypeDeclarationSyntax, INamedTypeSymbol), List<(INamedTypeSymbol editorType, INamedTypeSymbol modelMemberType)>>();

        foreach (var @class in types)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var (semanticModel, @classSymbol) = TryGetTargetType(context, compilation, @class);
            if (semanticModel is null || @classSymbol is null)
            {
                continue;
            }

            foreach (var attribute in classSymbol.GetAttributes(modelOptionsMapperAttribute))
            {
                var fromTypeSymbol = (INamedTypeSymbol)attribute.ConstructorArguments[0].Value!;

                var fromTypeSymbols = optionTypes
                    .Where(m =>
                        m.classSymbol.ToString() == fromTypeSymbol.ToString()
                );

                foreach (var (_, targetInterface) in fromTypeSymbols)
                {
                    var builder = CurlyIndenter.Create();

                    builder.WriteLine("// <auto-generated />");
                    builder.WriteLine();
                    builder.WriteLine("using System;");
                    builder.WriteLine("using System.Runtime.CompilerServices;");
                    builder.WriteLine();
                    builder.WriteLine("#nullable disable");
                    builder.WriteLine();

                    var (from, to) = (fromTypeSymbol, targetInterface);

                    using (builder.OpenBrace($"namespace {@classSymbol.ContainingNamespace}"))
                    {
                        var genericArguments = @classSymbol.IsGenericType
                            ? $"<{string.Join(", ", classSymbol.TypeArguments)}>"
                            : "";

                        var properties = GetAutoMappedPropertySymbols(fromTypeSymbol, autoMappedAttribute);
                        var targetTypeProperties = GetPropertySymbols(targetInterface, Array.Empty<string>())
                            .Select(m => m.Name)
                            .ToArray();

                        var propertiesToGenerate = properties.Where(m => targetTypeProperties.Contains(m.Name))
                            .ToArray();

                        using (builder.OpenBrace($"partial {(@classSymbol.IsRecord ? "record" : "class")} {@classSymbol.Name}{genericArguments}"))
                        {
                            using (builder.OpenBrace($"private void MapNode({from} from, {to} to)"))
                            {
                                foreach (var property in propertiesToGenerate)
                                {
                                    using (builder.OpenBrace($"if(from.Was{property.Name}Set)"))
                                    {
                                        builder.WriteLine($"to.{property.Name} = from.{property.Name};");
                                    }
                                    builder.WriteLine();
                                }
                                builder.WriteLine($"this.MapNodeCore(from, to);");
                            }
                            builder.WriteLine();
                            builder.WriteLine($"partial void MapNodeCore({from} from, {to} to);");
                        }
                    }

                    if (!mappedItems.ContainsKey((@class, @classSymbol)))
                    {
                        mappedItems[(@class, @classSymbol)] = new();
                    }

                    mappedItems[(@class, @classSymbol)].Add((from, to));

                    var hintName = @classSymbol.IsGenericType
                        ? $"{@classSymbol.Name}.{fromTypeSymbol}.{targetInterface}.{string.Join(".", classSymbol.TypeArguments)}"
                        : $"{@classSymbol.Name}.{fromTypeSymbol}.{targetInterface}";

                    compilation = AddGeneratedCode(context, compilation, @class, builder, addedSourceFiles, hintName, emitFile: AddSources);
                }
            }
        }

        return (compilation, mappedItems);
    }

    private Compilation GenerateXenialModelOptionsMapCode(GeneratorExecutionContext context, Compilation compilation, Dictionary<(TypeDeclarationSyntax, INamedTypeSymbol), List<(INamedTypeSymbol from, INamedTypeSymbol to)>> mappedClasses, IList<string> addedSourceFiles)
    {
        foreach (var (key, members) in mappedClasses)
        {
            var (@class, @classSymbol) = key;
            var builder = CurlyIndenter.Create();

            builder.WriteLine("// <auto-generated />");
            builder.WriteLine();
            builder.WriteLine("using System;");
            builder.WriteLine("using System.Runtime.CompilerServices;");
            builder.WriteLine();

            using (builder.OpenBrace($"namespace {@classSymbol.ContainingNamespace}"))
            {
                using (builder.OpenBrace($"partial {(@classSymbol.IsRecord ? "record" : "class")} {@classSymbol.Name}"))
                {
                    foreach (var members2 in members.GroupBy(m => m.from.ToString()))
                    {
                        var methodSigniture = $"internal void Map({members2.Key} from, DevExpress.ExpressApp.Model.IModelNode to)";
                        using (builder.OpenBrace(methodSigniture))
                        {
                            foreach (var (_, to) in members2)
                            {
                                using (builder.OpenBrace($"if(to is {to})"))
                                {
                                    builder.WriteLine($"{to} toCasted = ({to})to;");
                                    builder.WriteLine($"this.MapNode(from, toCasted);");
                                }

                                builder.WriteLine();
                            }
                            builder.WriteLine("this.MapNodeCore(from, to);");
                        }
                        builder.WriteLine();
                        builder.WriteLine($"partial void MapNodeCore({members2.Key} from, DevExpress.ExpressApp.Model.IModelNode to);");
                        builder.WriteLine();
                    }
                }
            }

            var hintName = $"{@classSymbol.Name}.BaseMapper";

            compilation = AddGeneratedCode(context, compilation, @class, builder, addedSourceFiles, hintName, emitFile: AddSources);
        }

        return compilation;
    }


    private static IPropertySymbol[] GetAutoMappedPropertySymbols(INamedTypeSymbol @classSymbol, INamedTypeSymbol autoMappedAttribute)
    {
        var propertyInfos = new List<IPropertySymbol>();
        var considered = new List<INamedTypeSymbol>();
        var queue = new Queue<INamedTypeSymbol>();
        considered.Add(@classSymbol);
        queue.Enqueue(@classSymbol);

        var baseType = @classSymbol.BaseType;

        while (baseType is not null)
        {
            if (!considered.Contains(baseType))
            {
                considered.Add(baseType);
                queue.Enqueue(baseType);
            }
            baseType = baseType.BaseType;
        }

        while (queue.Count > 0)
        {
            var subType = queue.Dequeue();

            var typeProperties = subType.GetMembers().OfType<IPropertySymbol>();

            var newPropertyInfos = typeProperties
                .Where(x => !propertyInfos.Contains(x));

            propertyInfos.InsertRange(0, newPropertyInfos);
        }

        return WithAutoMappedAttribute(
            WithSetterAndGetter(
                DistinctByName(propertyInfos.ToArray()
            )), autoMappedAttribute
        ).ToArray();
    }

    private static IPropertySymbol[] WithAutoMappedAttribute(IPropertySymbol[] propertySymbols, INamedTypeSymbol autoMappedAttribute)
        => propertySymbols.Where(m => m.HasAttribute(autoMappedAttribute)).ToArray();


    private static string GetDescription(Compilation compilation, IPropertySymbol propertySymbol)
    {
        var descriptionAttribute = compilation.GetTypeByMetadataName(typeof(DescriptionAttribute).FullName);
        if (descriptionAttribute is null)
        {
            return "";
        }

        var attribute = propertySymbol.FindAttribute(descriptionAttribute);
        if (attribute is null)
        {
            return "";
        }

        if (attribute.ConstructorArguments.Length > 0)
        {
            return attribute.ConstructorArguments[0].Value?.ToString() ?? "";
        }

        return "";
    }

    private static IPropertySymbol[] GetPropertySymbols(INamedTypeSymbol @interfaceSymbol, IEnumerable<string> ignoredMembers)
    {
        var propertyInfos = new List<IPropertySymbol>();
        var considered = new List<INamedTypeSymbol>();
        var queue = new Queue<INamedTypeSymbol>();
        considered.Add(@interfaceSymbol);
        queue.Enqueue(@interfaceSymbol);

        var baseType = @interfaceSymbol.BaseType;

        while (baseType is not null)
        {
            if (!considered.Contains(baseType))
            {
                considered.Add(baseType);
                queue.Enqueue(baseType);
            }
            baseType = baseType.BaseType;
        }

        propertyInfos.AddRange(interfaceSymbol.AllInterfaces.SelectMany(i => GetPropertySymbols(i, ignoredMembers)));

        while (queue.Count > 0)
        {
            var subType = queue.Dequeue();

            var typeProperties = subType.GetMembers().OfType<IPropertySymbol>();

            var newPropertyInfos = typeProperties
                .Where(x => !propertyInfos.Contains(x));

            propertyInfos.InsertRange(0, newPropertyInfos);
        }

        return WithoutIgnoredMembers(
            WithSetterAndGetter(
                DistinctByName(propertyInfos.ToArray()
            )), ignoredMembers.ToArray()
        );
    }

    private static IPropertySymbol[] WithoutIgnoredMembers(IPropertySymbol[] propertySymbols, string[] ignoredMembers)
        => propertySymbols
            .Where(m => !ignoredMembers.Contains(m.Name))
            .ToArray();

    private static IPropertySymbol[] WithSetterAndGetter(IPropertySymbol[] propertySymbols)
        => propertySymbols
            .Where(m => m.GetMethod is not null && m.SetMethod is not null && !m.IsIndexer)
            .ToArray();

    private static IPropertySymbol[] DistinctByName(IPropertySymbol[] propertySymbols)
        => propertySymbols
            .GroupBy(car => car.Name)
            .Select(g => g.First())
            .ToArray();

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

    private (Compilation, INamedTypeSymbol) GenerateXenialModelOptionsAttribute(GeneratorExecutionContext context, Compilation compilation, IList<string> addedSourceFiles)
    {
        var (source, syntaxTree) = GenerateXenialModelOptionsAttribute(
            (CSharpParseOptions)context.ParseOptions,
            cancellationToken: context.CancellationToken
        );

        if (AddSources)
        {
            var fileName = $"XenialModelOptionsAttribute.g.cs";
            addedSourceFiles.Add(fileName);
            context.AddSource(fileName, source);
        }

        compilation = compilation.AddSyntaxTrees(syntaxTree);

        var attribute = compilation.GetTypeByMetadataName("Xenial.XenialModelOptionsAttribute");

        return (compilation, attribute!);
    }

    public static (SourceText source, SyntaxTree syntaxTree) GenerateXenialModelOptionsAttribute(
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

            using (syntaxWriter.OpenBrace($"{visibility} sealed class XenialModelOptionsAttribute : Attribute"))
            {
                syntaxWriter.WriteLine($"public Type InterfaceType {{ get; private set; }}");
                syntaxWriter.WriteLine($"public string[] IgnoredMembers {{ get; set; }} = Array.Empty<string>();");
                syntaxWriter.WriteLine();
                using (syntaxWriter.OpenBrace($"{visibility} XenialModelOptionsAttribute(Type interfaceType)"))
                {
                    syntaxWriter.WriteLine($"this.InterfaceType = interfaceType;");
                }
            }
        }

        var syntax = syntaxWriter.ToString();
        var source = SourceText.From(syntax, Encoding.UTF8);
        var syntaxTree = CSharpSyntaxTree.ParseText(syntax, parseOptions, cancellationToken: cancellationToken);
        return (source, syntaxTree);
    }


    private (Compilation, INamedTypeSymbol) GenerateXenialModelOptionsMapperAttribute(GeneratorExecutionContext context, Compilation compilation, IList<string> addedSourceFiles)
    {
        var (source, syntaxTree) = GenerateXenialModelOptionsMapperAttribute(
            (CSharpParseOptions)context.ParseOptions,
            cancellationToken: context.CancellationToken
        );

        if (AddSources)
        {
            var fileName = $"XenialModelOptionsMapperAttribute.g.cs";
            addedSourceFiles.Add(fileName);
            context.AddSource(fileName, source);
        }

        compilation = compilation.AddSyntaxTrees(syntaxTree);

        var attribute = compilation.GetTypeByMetadataName("Xenial.XenialModelOptionsMapperAttribute");

        return (compilation, attribute!);
    }

    public static (SourceText source, SyntaxTree syntaxTree) GenerateXenialModelOptionsMapperAttribute(
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

            using (syntaxWriter.OpenBrace($"{visibility} sealed class XenialModelOptionsMapperAttribute : Attribute"))
            {
                syntaxWriter.WriteLine($"public Type OptionsType {{ get; private set; }}");
                syntaxWriter.WriteLine();
                using (syntaxWriter.OpenBrace($"{visibility} XenialModelOptionsMapperAttribute(Type optionsType)"))
                {
                    syntaxWriter.WriteLine($"this.OptionsType = optionsType;");
                }
            }
        }

        var syntax = syntaxWriter.ToString();
        var source = SourceText.From(syntax, Encoding.UTF8);
        var syntaxTree = CSharpSyntaxTree.ParseText(syntax, parseOptions, cancellationToken: cancellationToken);
        return (source, syntaxTree);
    }
}
