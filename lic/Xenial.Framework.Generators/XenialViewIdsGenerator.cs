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

public class XenialViewIdsGenerator : IXenialSourceGenerator
{
    private const string xenialViewIdsAttributeName = "XenialViewIdsAttribute";
    private const string xenialNamespace = "Xenial";
    private const string xenialImageNamesAttributeFullName = $"{xenialNamespace}.{xenialViewIdsAttributeName}";
    public const string GenerateXenialViewIdsAttributeMSBuildProperty = $"Generate{xenialViewIdsAttributeName}";

    private const string fullQualifiedDomainComponentAttribute = "DevExpress.ExpressApp.DC.DomainComponentAttribute";
    private const string fullQualifiedXpoPersistentAttribute = "DevExpress.Xpo.PersistentAttribute";
    private const string fullQualifiedXpoNonPersistentAttribute = "DevExpress.Xpo.NonPersistentAttribute";

    private const string fullQualifiedGenerateNoDetailViewAttribute = "Xenial.Framework.Base.GenerateNoDetailViewAttribute";
    private const string fullQualifiedGenerateNoListViewAttribute = "Xenial.Framework.Base.GenerateNoListViewAttribute";
    private const string fullQualifiedGenerateNoLookupListViewAttribute = "Xenial.Framework.Base.GenerateNoLookupListViewAttribute";
    private const string fullQualifiedGenerateNoNestedListViewAttribute = "Xenial.Framework.Base.GenerateNoNestedListViewAttribute";

    private const string fullQualifiedDeclareDetailViewAttribute = "Xenial.Framework.Base.DeclareDetailViewAttribute";
    private const string fullQualifiedDeclareListViewAttribute = "Xenial.Framework.Base.DeclareListViewAttribute";
    private const string fullQualifiedDeclareDashboardViewAttribute = "Xenial.Framework.Base.DeclareDashboardViewAttribute";

    public bool Accepts(TypeDeclarationSyntax typeDeclarationSyntax) => false;

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

        var generateXenialImageNamesAttribute = compilation.GetTypeByMetadataName(xenialImageNamesAttributeFullName);

        if (generateXenialImageNamesAttribute is null)
        {
            //TODO: Warning Diagnostics for either setting the right MSBuild properties or referencing `Xenial.Framework.CompilerServices`
            return compilation;
        }

        var collectedAttributes = new[]
        {
            compilation.GetTypeByMetadataName(fullQualifiedDomainComponentAttribute),
            compilation.GetTypeByMetadataName(fullQualifiedXpoPersistentAttribute),
            compilation.GetTypeByMetadataName(fullQualifiedXpoNonPersistentAttribute)
        };

        var collectedViewIds = new List<string>();

        foreach (var collectedAttribute in collectedAttributes)
        {
            if (collectedAttribute is not null)
            {
                static (SemanticModel? semanticModel, INamedTypeSymbol? @classSymbol, bool isAttributeDeclared) IsAttributeDeclared(
                       GeneratorExecutionContext context,
                       Compilation compilation,
                       TypeDeclarationSyntax @class,
                       INamedTypeSymbol attributeSymbol
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

                    var isAttributeDeclared = symbol.IsAttributeDeclared(attributeSymbol);

                    return (semanticModel, symbol, isAttributeDeclared);
                }


                foreach (var @class in types)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    var (_, @classSymbol, isAttributeDeclared) = IsAttributeDeclared(context, compilation, @class, collectedAttribute);
                    if (isAttributeDeclared && @classSymbol is not null)
                    {
                        if (!classSymbol.IsAttributeDeclared(fullQualifiedGenerateNoDetailViewAttribute))
                        {
                            collectedViewIds.Add($"{@classSymbol.Name}_DetailView");
                        }
                        if (!classSymbol.IsAttributeDeclared(fullQualifiedGenerateNoListViewAttribute))
                        {
                            collectedViewIds.Add($"{@classSymbol.Name}_ListView");
                        }
                        if (!classSymbol.IsAttributeDeclared(fullQualifiedGenerateNoLookupListViewAttribute))
                        {
                            collectedViewIds.Add($"{@classSymbol.Name}_LookupListView");
                        }

                        static void AddViewByAttribute(
                            INamedTypeSymbol classSymbol,
                            List<string> collectedViewIds,
                            string attributeName
                        )
                        {
                            if (classSymbol.IsAttributeDeclared(attributeName))
                            {
                                foreach (var attribute in classSymbol.GetAttributes(attributeName))
                                {
                                    var argument = attribute.ConstructorArguments.FirstOrDefault();
                                    if (argument.Value is not null && argument.Value is string viewId)
                                    {
                                        collectedViewIds.Add(viewId);
                                    }
                                }
                            }
                        }

                        AddViewByAttribute(classSymbol, collectedViewIds, fullQualifiedDeclareDetailViewAttribute);
                        AddViewByAttribute(classSymbol, collectedViewIds, fullQualifiedDeclareListViewAttribute);
                        AddViewByAttribute(classSymbol, collectedViewIds, fullQualifiedDeclareDashboardViewAttribute);

                        foreach (var property in classSymbol
                            .GetMembers()
                            .OfType<IPropertySymbol>()
                            .Where(p =>
                                p.GetMethod is not null
                                && p.GetMethod.GetResultantVisibility() == SymbolVisibility.Public
                                && p.GetMethod.ReturnType.AllInterfaces.Any(m => m.ToDisplayString() == "System.Collections.ICollection")
                                && p.GetMethod.ReturnType is INamedTypeSymbol collectionType
                            ))
                        {
                            var viewId = $"{@classSymbol.Name}_{property.Name}_ListView";
                            collectedViewIds.Add(viewId);
                            if (classSymbol.IsAttributeDeclared(fullQualifiedGenerateNoNestedListViewAttribute))
                            {
                                foreach (var nestedListViewAttribute in classSymbol.GetAttributes(fullQualifiedGenerateNoNestedListViewAttribute))
                                {
                                    var argument = nestedListViewAttribute.ConstructorArguments.FirstOrDefault();
                                    if (argument.Value is not null && argument.Value is string propertyName)
                                    {
                                        if (propertyName == property.Name)
                                        {
                                            collectedViewIds.Remove(viewId);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        foreach (var @class in types)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var (semanticModel, @classSymbol, isAttributeDeclared) = TryGetTargetType(context, compilation, @class, generateXenialImageNamesAttribute);
            if (!isAttributeDeclared || semanticModel is null || @classSymbol is null)
            {
                continue;
            }

            var @attribute = GetXenialViewIdsAttribute(@classSymbol, generateXenialImageNamesAttribute);

            var builder = CurlyIndenter.Create();

            builder.WriteLine("// <auto-generated />");
            builder.WriteLine();
            builder.WriteLine("using System;");
            builder.WriteLine("using System.Runtime.CompilerServices;");
            builder.WriteLine();

            var isGlobalNamespace = classSymbol.ContainingNamespace.ToString() == "<global namespace>";
            if (isGlobalNamespace)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        GeneratorDiagnostics.ClassNeedsToBeInNamespace(
                        xenialViewIdsAttributeName
                    ), @class.GetLocation())
                );

                return compilation;
            }

            var visibility = context.GetDefaultAttributeModifier();
            using (builder.OpenBrace($"namespace {@classSymbol.ContainingNamespace}"))
            {
                builder.WriteLine("[CompilerGenerated]");
                //We don't need to specify any other modifier
                //because the user can decide if he want it to be an instance type.
                //We also don't need to specify the visibility for partial types
                using (builder.OpenBrace($"partial {(@classSymbol.IsRecord ? "record" : "class")} {@classSymbol.Name}"))
                {
                    foreach (var viewId in collectedViewIds.Distinct())
                    {
                        builder.WriteLine($"{visibility} const string {viewId} = \"{viewId}\";");
                    }
                }
            }

            compilation = AddGeneratedCode(context, compilation, @class, builder, addedSourceFiles);
        }

        return compilation;
    }

    private static Compilation AddGeneratedCode(
        GeneratorExecutionContext context,
        Compilation compilation,
        TypeDeclarationSyntax @class,
        CurlyIndenter builder,
        IList<string> addedSourceFiles
    )
    {
        var syntax = builder.ToString();
        var source = SourceText.From(syntax, Encoding.UTF8);

        var fileName = Path.GetFileNameWithoutExtension(@class.SyntaxTree.FilePath);

        if (fileName is "")
        {
            fileName = Guid.NewGuid().ToString();
        }

        var hintName = $"{fileName}.g.cs";
        if (!addedSourceFiles.Contains(hintName))
        {
            context.AddSource(hintName, source);
            return compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(syntax, (CSharpParseOptions)context.ParseOptions, cancellationToken: context.CancellationToken));
        }

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
                    GeneratorDiagnostics.ClassNeedsToBePartialWhenUsingAttribute(xenialImageNamesAttributeFullName),
                    @class.GetLocation()
            ));

            return (semanticModel, symbol, false);
        }

        return (semanticModel, symbol, isAttributeDeclared);
    }

    private static AttributeData GetXenialViewIdsAttribute(INamedTypeSymbol symbol, INamedTypeSymbol generateXenialViewIdsAttribubute)
        => symbol.GetAttribute(generateXenialViewIdsAttribubute);

    private static Compilation GenerateAttribute(GeneratorExecutionContext context, Compilation compilation)
    {
        if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{GenerateXenialViewIdsAttributeMSBuildProperty}", out var generateXenialViewIdsAttrStr))
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
                            GenerateXenialViewIdsAttributeMSBuildProperty,
                            generateXenialViewIdsAttrStr
                        )
                        , null
                    ));
                return compilation;
            }
        }

        var (source, syntaxTree) = GenerateXenialViewIdsAttribute(
            (CSharpParseOptions)context.ParseOptions,
            context.GetDefaultAttributeModifier(),
            context.CancellationToken
        );

        context.AddSource($"{xenialViewIdsAttributeName}.g.cs", source);

        return compilation.AddSyntaxTrees(syntaxTree);
    }

    public static (SourceText source, SyntaxTree syntaxTree) GenerateXenialViewIdsAttribute(
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

            using (syntaxWriter.OpenBrace($"{visibility} sealed class {xenialViewIdsAttributeName} : Attribute"))
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
        }

        var syntax = syntaxWriter.ToString();
        var source = SourceText.From(syntax, Encoding.UTF8);
        var syntaxTree = CSharpSyntaxTree.ParseText(syntax, parseOptions, cancellationToken: cancellationToken);
        return (source, syntaxTree);
    }
}

