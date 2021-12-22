using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using Xenial.Framework.Generators.Internal;
using Xenial.Framework.MsBuild;

namespace Xenial.Framework.Generators;

public class XenialLayoutBuilderGenerator : IXenialSourceGenerator
{
    private const string xenialLayoutBuilderAttributeName = "XenialLayoutBuilderAttribute";
    private const string xenialExpandMemberAttributeName = "XenialExpandMemberAttribute";
    private const string xenialNamespace = "Xenial";
    private const string xenialLayoutBuilderAttributeFullName = $"{xenialNamespace}.{xenialLayoutBuilderAttributeName}";
    private const string xenialExpandMemberAttributeFullName = $"{xenialNamespace}.{xenialExpandMemberAttributeName}";
    public const string GenerateXenialLayoutBuilderAttributeMSBuildProperty = $"Generate{xenialLayoutBuilderAttributeName}";

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

        compilation = GenerateAttribute(context, compilation);

        var generateXenialLayoutBuilderAttribute = compilation.GetTypeByMetadataName(xenialLayoutBuilderAttributeFullName);
        var xenialExpandMemberAttribute = compilation.GetTypeByMetadataName(xenialExpandMemberAttributeFullName);

        if (generateXenialLayoutBuilderAttribute is null || xenialExpandMemberAttribute is null)
        {
            //TODO: Warning Diagnostics for either setting the right MSBuild properties or referencing `Xenial.Framework.CompilerServices`
            return compilation;
        }

        foreach (var @class in types)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var (semanticModel, @classSymbol, isAttributeDeclared) = TryGetTargetType(context, compilation, @class, generateXenialLayoutBuilderAttribute);
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

            //var @attribute = GetXenialLayoutBuilderAttribute(@classSymbol, generateXenialLayoutBuilderAttribute);

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
                        GeneratorDiagnostics.ClassNeedsToBeInNamespace(
                        xenialLayoutBuilderAttributeName
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
                    using (builder.OpenBrace("private struct PropertyIdentifier"))
                    {
                        builder.WriteLine("private string propertyName;");
                        builder.WriteLine("public string PropertyName { get { return this.propertyName; } }");
                        builder.WriteLine();

                        using (builder.OpenBrace("private PropertyIdentifier(string propertyName)"))
                        {
                            builder.WriteLine("this.propertyName = propertyName;");
                        }
                        builder.WriteLine();

                        using (builder.OpenBrace("public static implicit operator string(PropertyIdentifier identifier)"))
                        {
                            builder.WriteLine("return identifier.PropertyName;");
                        }
                        builder.WriteLine();

                        using (builder.OpenBrace("public static PropertyIdentifier Create(string propertyName)"))
                        {
                            builder.WriteLine("return new PropertyIdentifier(propertyName);");
                        }
                    }

                    if (targetType.GetMembers().OfType<IPropertySymbol>().Any())
                    {
                        builder.WriteLine();
                        var properties = targetType.GetMembers().OfType<IPropertySymbol>().ToList();

                        using (builder.OpenBrace("private partial struct Constants"))
                        {
                            foreach (var property in properties)
                            {
                                builder.WriteLine($"public const string {property.Name} = \"{property.Name}\";");
                            }
                        }
                        builder.WriteLine();

                        using (builder.OpenBrace("private partial struct Property"))
                        {
                            foreach (var property in properties)
                            {
                                builder.WriteLine($"public static PropertyIdentifier {property.Name} {{ get {{ return PropertyIdentifier.Create(\"{property.Name}\"); }} }}");
                                builder.WriteLine();
                            }
                        }
                        builder.WriteLine();

                        using (builder.OpenBrace("private partial struct Editor"))
                        {
                            foreach (var property in properties)
                            {
                                builder.WriteLine($"public static LayoutPropertyEditorItem {property.Name} {{ get {{ return LayoutPropertyEditorItem.Create(\"{property.Name}\"); }} }}");
                                builder.WriteLine();
                            }
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
                    var foundPart = typeToExpandMembers.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(m => m.Name == expandMemberPart);
                    if (foundPart is not null && foundPart.Type is INamedTypeSymbol namedType)
                    {
                        typeToExpandMembers = namedType;
                    }
                }

                return (
                    expandMemberParts.ToArray(),
                    typeToExpandMembers.GetMembers().OfType<IPropertySymbol>().ToArray()
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

                        using (builder.OpenBrace("private partial struct Property"))
                        {
                            using (WriteParentClassTrain(builder, parents))
                            {
                                WritePropertyIdentitfiers(builder, properties, parents);
                            }
                        }
                        builder.WriteLine();

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
        foreach (var property in properties)
        {
            var value = ToPropertyTrain(prefix, property.Name);
            builder.WriteLine($"public static LayoutPropertyEditorItem {property.Name} {{ get {{ return LayoutPropertyEditorItem.Create(\"{value}\"); }} }}");
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

        //context.ReportDiagnostic(
        //    Diagnostic.Create(
        //        GeneratorDiagnostics.ConflictingClasses(
        //            xenialActionAttributeName,
        //            @class.ToString()
        //        ), @class.GetLocation()
        //    ));

        return compilation;
    }

    private static (SemanticModel? semanticModel, INamedTypeSymbol? @classSymbol, bool isAttributeDeclared) TryGetTargetType(
        GeneratorExecutionContext context,
        Compilation compilation,
        TypeDeclarationSyntax @class,
        INamedTypeSymbol generateXenialImageNamesAttribute
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

        var isAttributeDeclared = symbol.IsAttributeDeclared(generateXenialImageNamesAttribute);

        if (isAttributeDeclared && !@class.HasModifier(SyntaxKind.PartialKeyword))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    GeneratorDiagnostics.ClassNeedsToBePartialWhenUsingAttribute(xenialLayoutBuilderAttributeFullName),
                    @class.GetLocation()
            ));

            return (semanticModel, symbol, false);
        }

        return (semanticModel, symbol, isAttributeDeclared);
    }

    private static Compilation GenerateAttribute(GeneratorExecutionContext context, Compilation compilation)
    {
        if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{GenerateXenialLayoutBuilderAttributeMSBuildProperty}", out var generateXenialViewIdsAttrStr))
        {
            if (bool.TryParse(generateXenialViewIdsAttrStr, out var generateXenialViewIdsAttr))
            {
                if (!generateXenialViewIdsAttr)
                {
                    return compilation;
                }
            }
            else
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        GeneratorDiagnostics.InvalidBooleanMsBuildProperty(
                            GenerateXenialLayoutBuilderAttributeMSBuildProperty,
                            generateXenialViewIdsAttrStr
                        )
                        , null
                    ));
                return compilation;
            }
        }

        var (source, syntaxTree) = GenerateXenialLayoutBuilderAttribute(
            (CSharpParseOptions)context.ParseOptions,
            context.GetDefaultAttributeModifier(),
            context.CancellationToken
        );

        context.AddSource($"{xenialLayoutBuilderAttributeName}.g.cs", source);

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

        using (syntaxWriter.OpenBrace($"namespace {xenialNamespace}"))
        {
            syntaxWriter.WriteLine("[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]");

            using (syntaxWriter.OpenBrace($"{visibility} sealed class {xenialLayoutBuilderAttributeName} : Attribute"))
            {
                //syntaxWriter.WriteLine($"{visibility} {xenialViewIdsAttributeName}() {{ }}");

                //syntaxWriter.WriteLine();

                ////Properties need to be public in order to be used
                //syntaxWriter.WriteLine($"public bool {AttributeNames.Sizes} {{ get; set; }}");
                //syntaxWriter.WriteLine($"public bool {AttributeNames.SmartComments} {{ get; set; }}");
                //syntaxWriter.WriteLine($"public bool {AttributeNames.ResourceAccessors} {{ get; set; }}");

                //syntaxWriter.WriteLine("[EditorBrowsable(EditorBrowsableState.Never)]");
                //syntaxWriter.WriteLine($"public string {AttributeNames.DefaultImageSize} {{ get; set; }} = \"{AttributeNames.DefaultImageSizeValue}\";");
            }

            syntaxWriter.WriteLine();

            syntaxWriter.WriteLine("[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]");

            using (syntaxWriter.OpenBrace($"{visibility} sealed class XenialExpandMemberAttribute : Attribute"))
            {
                syntaxWriter.WriteLine($"public string ExpandMember {{ get; private set; }}");
                syntaxWriter.WriteLine();
                using (syntaxWriter.OpenBrace($"{visibility} XenialExpandMemberAttribute(string expandMember)"))
                {
                    syntaxWriter.WriteLine($"this.ExpandMember = expandMember;");
                }
            }
        }

        var syntax = syntaxWriter.ToString();
        var source = SourceText.From(syntax, Encoding.UTF8);
        var syntaxTree = CSharpSyntaxTree.ParseText(syntax, parseOptions, cancellationToken: cancellationToken);
        return (source, syntaxTree);
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

