﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using System.Threading;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using Xenial.Framework.Generators;
using Xenial.Framework.Generators.Internal;
using Xenial.Framework.Generators.XAF.Utils;
using Xenial.Framework.MsBuild;

namespace Xenial.Framework.Generators.XAF;

internal record XenialLayoutPropertyEditorItemGenerator(bool AddSources = true) : IXenialSourceGenerator
{
    public bool Accepts(TypeDeclarationSyntax typeDeclarationSyntax) => false;

    public Compilation Execute(GeneratorExecutionContext context, Compilation compilation, IList<TypeDeclarationSyntax> types, IList<string> addedSourceFiles)
    {
        (compilation, var autoMappedAttribute) = XenialAutoMappedAttributeGenerator.FindXenialAutoMappedAttribute(compilation);
        compilation = GenerateLayoutEditorItemCode(context, compilation, types, autoMappedAttribute, addedSourceFiles);
        (compilation, var mappedItems) = GenerateLayoutEditorItemMapperCode(context, compilation, types, autoMappedAttribute, addedSourceFiles);
        compilation = GenerateLayoutEditorItemMapperMapCode(context, compilation, mappedItems, addedSourceFiles);
        return compilation;
    }

    private Compilation GenerateLayoutEditorItemCode(GeneratorExecutionContext context, Compilation compilation, IList<TypeDeclarationSyntax> types, INamedTypeSymbol autoMappedAttribute, IList<string> addedSourceFiles)
    {
        (compilation, var attributeSymbol) = GenerateXenialLayoutPropertyEditorItemAttribute(context, compilation, addedSourceFiles);

        foreach (var @class in types)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            var (semanticModel, @classSymbol, isAttributeDeclared) = TryGetTargetType(context, compilation, @class, attributeSymbol);
            if (!isAttributeDeclared || semanticModel is null || @classSymbol is null)
            {
                continue;
            }

            var attribute = @classSymbol.GetAttribute(attributeSymbol);

            var targetTypeString = (ITypeSymbol)attribute.ConstructorArguments[0].Value!;
            var modelTypeString = attribute.ConstructorArguments[1].Value?.ToString();

            var modelType = typeof(XafApplication).Assembly.GetType(modelTypeString, true, false);

            var targetTypeProperties = DistinctByName(GetPropertySymbols(classSymbol));
            var properties = GetPublicProperties(modelType);

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
                builder.WriteLine("[CompilerGenerated]");

                var genericArguments = @classSymbol.IsGenericType
                    ? $"<{string.Join(", ", classSymbol.TypeArguments)}>"
                    : "";

                using (builder.OpenBrace($"partial {(@classSymbol.IsRecord ? "record" : "class")} {@classSymbol.Name}{genericArguments}"))
                {
                    foreach (var property in properties)
                    {
                        var exists = targetTypeProperties.Any(t => t.Name == property.Name);
                        if (exists)
                        {
                            continue;
                        }

                        var shouldBeAdded = ShouldBeAdded(targetTypeString, property);

                        if (shouldBeAdded)
                        {
                            var privateField = property.Name.FirstCharToLowerCase();

                            builder.WriteLine($"private {property.PropertyType.FullName} {privateField};");
                            builder.WriteLine($"internal System.Boolean Was{property.Name}Set {{ get; set; }}");

                            builder.WriteLine($"/// <summary>");
                            builder.WriteLine($"/// {GetDescription(property)}");
                            builder.WriteLine($"/// </summary>");
                            builder.WriteLine($"[{autoMappedAttribute}]");
                            using (builder.OpenBrace($"public {property.PropertyType.FullName} {property.Name}"))
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
            }

            var hintName = @classSymbol.IsGenericType
                ? $"{@classSymbol.Name}.{string.Join(".", classSymbol.TypeArguments)}"
                : null;

            compilation = AddGeneratedCode(context, compilation, @class, builder, addedSourceFiles, hintName, emitFile: AddSources);
        }

        return compilation;
    }

    private (Compilation compilation, Dictionary<(TypeDeclarationSyntax, INamedTypeSymbol), List<(INamedTypeSymbol editorType, INamedTypeSymbol modelMemberType)>>) GenerateLayoutEditorItemMapperCode(GeneratorExecutionContext context, Compilation compilation, IList<TypeDeclarationSyntax> types, INamedTypeSymbol autoMappedAttribute, IList<string> addedSourceFiles)
    {
        (compilation, var attributeSymbol) = GenerateXenialLayoutPropertyEditorItemMapperAttribute(context, compilation, addedSourceFiles);

        var overloads = new List<(INamedTypeSymbol editor, ITypeSymbol target)>();
        var mappedItems = new Dictionary<(TypeDeclarationSyntax, INamedTypeSymbol), List<(INamedTypeSymbol editorType, INamedTypeSymbol modelMemberType)>>();

        foreach (var @class in types)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            var (semanticModel, @classSymbol, isAttributeDeclared) = TryGetTargetType(context, compilation, @class, attributeSymbol);
            if (!isAttributeDeclared || semanticModel is null || @classSymbol is null)
            {
                continue;
            }

            foreach (var attribute in @classSymbol.GetAttributes(attributeSymbol))
            {
                var editorTypeSymbol = (ITypeSymbol)attribute.ConstructorArguments[0].Value!;
                var targetTypeSymbol = (ITypeSymbol)attribute.ConstructorArguments[1].Value!;
                var modelTypeString = attribute.ConstructorArguments[1].Value?.ToString();

                var modelType = typeof(XafApplication).Assembly.GetType(modelTypeString, true, false);

                if (editorTypeSymbol is INamedTypeSymbol editorNamedTypeSymbol
                    && targetTypeSymbol is INamedTypeSymbol targetNamedTypeSymbol)
                {
                    var isMappedOverload = overloads.Any(m =>
                        m.editor.ToString() == editorNamedTypeSymbol.ToString()
                        && m.target.ToString() == targetTypeSymbol.ToString()
                    );

                    if (isMappedOverload)
                    {
                        continue;
                    }

                    var mappingProperties = WithAutoMappedAttributes(autoMappedAttribute, DistinctByName(GetPropertySymbols(editorNamedTypeSymbol)));

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
                            var methodSigniture = $"private void Map({editorNamedTypeSymbol} from, {targetTypeSymbol} to)";
                            using (builder.OpenBrace(methodSigniture))
                            {
                                foreach (var property in mappingProperties)
                                {
                                    using (builder.OpenBrace($"if(from.Was{property.Name}Set)"))
                                    {
                                        builder.WriteLine($"to.{property.Name} = from.{property.Name};");
                                    }
                                }
                                builder.WriteLine();
                                builder.WriteLine("this.MapCore(from, to);");
                            }
                            builder.WriteLine();
                            builder.WriteLine($"partial void MapCore({editorNamedTypeSymbol} from,{targetTypeSymbol} to);");
                        }
                    }

                    if (!mappedItems.ContainsKey((@class, @classSymbol)))
                    {
                        mappedItems[(@class, @classSymbol)] = new();
                    }

                    mappedItems[(@class, @classSymbol)].Add((editorNamedTypeSymbol, targetNamedTypeSymbol));

                    var hintName = $"{@classSymbol.Name}.{editorNamedTypeSymbol.Name}.{targetTypeSymbol.Name}";

                    compilation = AddGeneratedCode(context, compilation, @class, builder, addedSourceFiles, hintName, emitFile: AddSources);

                    overloads.Add((editorNamedTypeSymbol, targetTypeSymbol));
                }
            }
        }

        return (compilation, mappedItems);
    }

    private Compilation GenerateLayoutEditorItemMapperMapCode(GeneratorExecutionContext context, Compilation compilation, Dictionary<(TypeDeclarationSyntax, INamedTypeSymbol), List<(INamedTypeSymbol editorType, INamedTypeSymbol modelMemberType)>> mappedClasses, IList<string> addedSourceFiles)
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
                    var methodSigniture = $"internal void Map(Xenial.Framework.Layouts.Items.Base.LayoutItemNode from, DevExpress.ExpressApp.Model.IModelNode to)";
                    using (builder.OpenBrace(methodSigniture))
                    {
                        foreach (var (editorNamedTypeSymbol, targetNamedTypeSymbol) in members)
                        {
                            using (builder.OpenBrace($"if(from is {editorNamedTypeSymbol})"))
                            {
                                builder.WriteLine($"{editorNamedTypeSymbol} fromCasted = ({editorNamedTypeSymbol})from;");
                                using (builder.OpenBrace($"if(to is {targetNamedTypeSymbol})"))
                                {
                                    builder.WriteLine($"{targetNamedTypeSymbol} toCasted = ({targetNamedTypeSymbol})to;");
                                    builder.WriteLine($"this.Map(fromCasted, toCasted);");
                                }
                            }
                            builder.WriteLine();
                        }
                        builder.WriteLine("this.MapCore(from, to);");
                    }
                    builder.WriteLine();
                    builder.WriteLine("partial void MapCore(Xenial.Framework.Layouts.Items.Base.LayoutItemNode from, DevExpress.ExpressApp.Model.IModelNode to);");
                }
            }

            var hintName = $"{@classSymbol.Name}.BaseMapper";

            compilation = AddGeneratedCode(context, compilation, @class, builder, addedSourceFiles, hintName, emitFile: AddSources);
        }

        return compilation;
    }

    private static IEnumerable<IPropertySymbol> WithAutoMappedAttributes(INamedTypeSymbol autoMappedAttribute, IPropertySymbol[] propertySymbols)
    {
        foreach (var propertySymbol in propertySymbols)
        {
            if (propertySymbol.HasAttribute(autoMappedAttribute))
            {
                yield return propertySymbol;
            }
        }
    }

    private static string? GetDescription(PropertyInfo propertyInfo)
    {
        var attr = propertyInfo.GetCustomAttribute<DescriptionAttribute>();
        if (attr is not null)
        {
            return attr.Description;
        }
        return null;
    }

    private static bool ShouldBeAdded(ITypeSymbol targetTypeSymbol, PropertyInfo property)
    {
        var specialTypes = new[]
        {
            nameof(IModelPropertyEditor.ModelMember),
            nameof(IModelPropertyEditor.PropertyName),
            nameof(IModelPropertyEditor.View),
            nameof(IModelPropertyEditor.Root),
            nameof(IModelPropertyEditor.NodeCount),
            nameof(IModelPropertyEditor.Application),
            nameof(IModelPropertyEditor.PropertyEditorType),

            nameof(IModelPropertyEditor.MaxLength),

            nameof(IModelPropertyEditor.DataSourceProperty),
            nameof(IModelPropertyEditor.DataSourceCriteriaProperty),

            nameof(IModelPropertyEditor.ImageSizeMode),
            nameof(IModelPropertyEditor.ImageEditorCustomHeight),
            nameof(IModelPropertyEditor.ImageEditorMode),
            nameof(IModelPropertyEditor.ImageEditorFixedWidth),
            nameof(IModelPropertyEditor.ImageEditorFixedHeight),

            nameof(IModelPropertyEditor.LookupEditorMode),
        };

        if (specialTypes.Contains(property.Name))
        {
            return false;
        }

        if (targetTypeSymbol.SpecialType == SpecialType.System_Boolean)
        {
            var booleanSpecialTypes = new[]
            {
                nameof(IModelPropertyEditor.EditMask),
                nameof(IModelPropertyEditor.EditMaskType),
                nameof(IModelPropertyEditor.IsPassword),
                nameof(IModelPropertyEditor.DisplayFormat),
                nameof(IModelPropertyEditor.LookupProperty),
                nameof(IModelPropertyEditor.DataSourcePropertyIsNullMode),
                nameof(IModelPropertyEditor.DataSourcePropertyIsNullCriteria),
                nameof(IModelPropertyEditor.DataSourceCriteria)
            };

            if (booleanSpecialTypes.Contains(property.Name))
            {
                return false;
            }
        }

        if (targetTypeSymbol.SpecialType == SpecialType.System_Int16
            || targetTypeSymbol.SpecialType == SpecialType.System_Int32
            || targetTypeSymbol.SpecialType == SpecialType.System_Int64
        )
        {
            var intSpecialTypes = new[]
            {
                nameof(IModelPropertyEditor.IsPassword),
                nameof(IModelPropertyEditor.LookupProperty),
                nameof(IModelPropertyEditor.DataSourcePropertyIsNullMode),
                nameof(IModelPropertyEditor.DataSourcePropertyIsNullCriteria),
                nameof(IModelPropertyEditor.DataSourceCriteria)
            };

            if (intSpecialTypes.Contains(property.Name))
            {
                return false;
            }
        }

        if (targetTypeSymbol.SpecialType == SpecialType.System_String)
        {
            var intSpecialTypes = new[]
            {
                nameof(IModelPropertyEditor.LookupProperty),
                nameof(IModelPropertyEditor.DataSourcePropertyIsNullMode),
                nameof(IModelPropertyEditor.DataSourcePropertyIsNullCriteria),
                nameof(IModelPropertyEditor.DataSourceCriteria)
            };

            if (intSpecialTypes.Contains(property.Name))
            {
                return false;
            }
        }

        foreach (var browsableAttribute in property.GetCustomAttributes<BrowsableAttribute>())
        {
            if (!browsableAttribute.Browsable)
            {
                return false;
            }
        }

        foreach (var browsableCalulatorType in property.GetCustomAttributes<ModelBrowsableAttribute>().Select(m => m.VisibilityCalculatorType))
        {
            if (browsableCalulatorType is null)
            {
                continue;
            }

            if (browsableCalulatorType == typeof(BooleanPropertyOnlyCalculator))
            {
                if (targetTypeSymbol.SpecialType != SpecialType.System_Boolean)
                {
                    return false;
                }
            }
            if (browsableCalulatorType == typeof(StringPropertyOnlyCalculator))
            {
                if (targetTypeSymbol.SpecialType != SpecialType.System_String)
                {
                    return false;
                }
            }

            if (browsableCalulatorType == typeof(ImagePropertyOnlyCalculator))
            {
                //TODO: byte[]
                if (targetTypeSymbol.SpecialType != SpecialType.System_Byte)
                {
                    return false;
                }
            }

            if (browsableCalulatorType == typeof(DateTimePropertyOnlyCalculator))
            {
                if (targetTypeSymbol.SpecialType != SpecialType.System_DateTime)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static IPropertySymbol[] DistinctByName(IPropertySymbol[] propertySymbols)
        => propertySymbols
            .GroupBy(car => car.Name)
            .Select(g => g.First())
            .ToArray();

    private static IPropertySymbol[] GetPropertySymbols(INamedTypeSymbol @classSymbol)
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

        return propertyInfos.ToArray();
    }

    private static PropertyInfo[] GetPublicProperties(Type type)
    {
        if (type.IsInterface)
        {
            var propertyInfos = new List<PropertyInfo>();

            var considered = new List<Type>();
            var queue = new Queue<Type>();
            considered.Add(type);
            queue.Enqueue(type);
            while (queue.Count > 0)
            {
                var subType = queue.Dequeue();
                foreach (var subInterface in subType.GetInterfaces())
                {
                    if (considered.Contains(subInterface)) { continue; }

                    considered.Add(subInterface);
                    queue.Enqueue(subInterface);
                }

                var typeProperties = subType.GetProperties(
                    BindingFlags.FlattenHierarchy
                    | BindingFlags.Public
                    | BindingFlags.Instance);

                var newPropertyInfos = typeProperties
                    .Where(x => !propertyInfos.Contains(x));

                propertyInfos.InsertRange(0, newPropertyInfos);
            }

            return propertyInfos.ToArray();
        }

        return type.GetProperties(BindingFlags.FlattenHierarchy
            | BindingFlags.Public | BindingFlags.Instance);
    }

    private static (SemanticModel? semanticModel, INamedTypeSymbol? @classSymbol, bool isAttributeDeclared) TryGetTargetType(
        GeneratorExecutionContext context,
        Compilation compilation,
        TypeDeclarationSyntax @class,
        INamedTypeSymbol attribute
    )
    {
        var semanticModel = compilation.GetSemanticModel(@class.SyntaxTree);
        if (semanticModel is null)
        {
            return (semanticModel, null, false);
        }

        var symbol = semanticModel.GetDeclaredSymbol(@class, context.CancellationToken);

        if (symbol is null)
        {
            return (semanticModel, null, false);
        }

        var isAttributeDeclared = symbol.IsAttributeDeclared(attribute);

        return (semanticModel, symbol, isAttributeDeclared);
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

    private (Compilation, INamedTypeSymbol) GenerateXenialLayoutPropertyEditorItemAttribute(GeneratorExecutionContext context, Compilation compilation, IList<string> addedSourceFiles)
    {
        var (source, syntaxTree) = GenerateXenialLayoutPropertyEditorItemAttribute(
            (CSharpParseOptions)context.ParseOptions,
            cancellationToken: context.CancellationToken
        );

        if (AddSources)
        {
            var fileName = $"XenialLayoutPropertyEditorItemAttribute.g.cs";
            addedSourceFiles.Add(fileName);
            context.AddSource(fileName, source);
        }

        compilation = compilation.AddSyntaxTrees(syntaxTree);

        var attribute = compilation.GetTypeByMetadataName("Xenial.XenialLayoutPropertyEditorItemAttribute");

        return (compilation, attribute!);
    }

    private (Compilation, INamedTypeSymbol) GenerateXenialLayoutPropertyEditorItemMapperAttribute(GeneratorExecutionContext context, Compilation compilation, IList<string> addedSourceFiles)
    {
        var (source, syntaxTree) = GenerateXenialLayoutPropertyEditorItemMapperAttribute(
            (CSharpParseOptions)context.ParseOptions,
            cancellationToken: context.CancellationToken
        );

        if (AddSources)
        {
            var fileName = $"XenialLayoutPropertyEditorItemMapperAttribute.g.cs";
            addedSourceFiles.Add(fileName);
            context.AddSource(fileName, source);
        }

        compilation = compilation.AddSyntaxTrees(syntaxTree);

        var attribute = compilation.GetTypeByMetadataName("Xenial.XenialLayoutPropertyEditorItemMapperAttribute");

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

    public static (SourceText source, SyntaxTree syntaxTree) GenerateXenialLayoutPropertyEditorItemAttribute(
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
                syntaxWriter.WriteLine($"public Type InterfaceType {{ get; private set; }}");
                syntaxWriter.WriteLine();
                using (syntaxWriter.OpenBrace($"{visibility} XenialLayoutPropertyEditorItemAttribute(Type propertyType, Type interfaceType)"))
                {
                    syntaxWriter.WriteLine($"this.PropertyType = propertyType;");
                    syntaxWriter.WriteLine($"this.InterfaceType = interfaceType;");
                }
            }
        }

        var syntax = syntaxWriter.ToString();
        var source = SourceText.From(syntax, Encoding.UTF8);
        var syntaxTree = CSharpSyntaxTree.ParseText(syntax, parseOptions, cancellationToken: cancellationToken);
        return (source, syntaxTree);
    }

    public static (SourceText source, SyntaxTree syntaxTree) GenerateXenialLayoutPropertyEditorItemMapperAttribute(
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

            using (syntaxWriter.OpenBrace($"{visibility} sealed class XenialLayoutPropertyEditorItemMapperAttribute : Attribute"))
            {
                syntaxWriter.WriteLine($"public Type EditorItemType {{ get; private set; }}");
                syntaxWriter.WriteLine($"public Type InterfaceType {{ get; private set; }}");
                syntaxWriter.WriteLine();
                using (syntaxWriter.OpenBrace($"{visibility} XenialLayoutPropertyEditorItemMapperAttribute(Type editorItemType, Type interfaceType)"))
                {
                    syntaxWriter.WriteLine($"this.EditorItemType = editorItemType;");
                    syntaxWriter.WriteLine($"this.InterfaceType = interfaceType;");
                }
            }
        }

        var syntax = syntaxWriter.ToString();
        var source = SourceText.From(syntax, Encoding.UTF8);
        var syntaxTree = CSharpSyntaxTree.ParseText(syntax, parseOptions, cancellationToken: cancellationToken);
        return (source, syntaxTree);
    }
}
