﻿using System;
using System.Collections.Generic;
using System.Globalization;
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

public class XenialXpoBuilderGenerator : IXenialSourceGenerator
{
    private const string xenialXpoBuilderAttributeName = "XenialXpoBuilderAttribute";
    private const string xenialNamespace = "Xenial";
    private const string xenialXpoBuilderAttributeFullName = $"{xenialNamespace}.{xenialXpoBuilderAttributeName}";
    public const string GenerateXenialXpoBuilderAttributeMSBuildProperty = $"Generate{xenialXpoBuilderAttributeName}";

    private const string fullQualifiedXpoPersistentAttribute = "DevExpress.Xpo.PersistentAttribute";
    private const string fullQualifiedXpoNonPersistentAttribute = "DevExpress.Xpo.NonPersistentAttribute";

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

        var generateXenialXpoBuilderAttribute = compilation.GetTypeByMetadataName(xenialXpoBuilderAttributeFullName);

        if (generateXenialXpoBuilderAttribute is null)
        {
            //TODO: Warning Diagnostics for either setting the right MSBuild properties or referencing `Xenial.Framework.CompilerServices`
            return compilation;
        }

        var builders = new Dictionary<INamedTypeSymbol, string>(SymbolEqualityComparer.Default);

        foreach (var @class in types)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var (semanticModel, @classSymbol, isAttributeDeclared) = TryGetTargetType(context, compilation, @class, generateXenialXpoBuilderAttribute);
            if (!isAttributeDeclared || semanticModel is null || @classSymbol is null)
            {
                continue;
            }

            var @attribute = GetXenialAttribute(@classSymbol, generateXenialXpoBuilderAttribute);

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
                        xenialXpoBuilderAttributeName
                    ), @class.GetLocation())
                );

                return compilation;
            }

            compilation = BuildBuilderType(
                context,
                compilation,
                builder,
                addedSourceFiles,
                builders,
                @class,
                classSymbol
            );
        }

        return compilation;
    }

    private static Compilation BuildBuilderType(
        GeneratorExecutionContext context,
        Compilation compilation,
        CurlyIndenter builder,
        IList<string> addedSourceFiles,
        Dictionary<INamedTypeSymbol, string> builders,
        TypeDeclarationSyntax @class,
        INamedTypeSymbol classSymbol
    )
    {
        var visibility = context.GetDefaultAttributeModifier();
        using (builder.OpenBrace($"namespace {@classSymbol.ContainingNamespace}"))
        {
            static bool IsXpoClass(GeneratorExecutionContext context, Compilation compilation, TypeDeclarationSyntax syntax, INamedTypeSymbol classSymbol)
            {
                var attributes = new[]
                {
                    fullQualifiedXpoPersistentAttribute,
                    fullQualifiedXpoNonPersistentAttribute
                }.Select(attr => compilation.GetTypeByMetadataName(attr))
                 .Where(attr => attr is not null)
                 .ToArray();

                var isAttributeDeclared = attributes.Any(
                    attr => attr is not null
                    && classSymbol.IsAttributeDeclared(attr)
                );

                if (isAttributeDeclared)
                {
                    return true;
                }

                if (classSymbol.AllInterfaces.Any(x => x.ToDisplayString() == "DevExpress.Xpo.Helpers.ISessionProvider"))
                {
                    return true;
                }

                return false;
            }

            var isXpoClass = IsXpoClass(context, compilation, @class, @classSymbol);

            var builderClassName = $"{@classSymbol.Name}Builder";

            builder.WriteLine("[CompilerGenerated]");
            builder.WriteLine($"{visibility} partial class {builderClassName} : {builderClassName}<{@classSymbol.ToDisplayString()}, {builderClassName}> {{ }}");

            builder.WriteLine();

            builder.WriteLine("[CompilerGenerated]");
            //We don't need to specify any other modifier
            //because the user can decide if he want it to be an instance type.
            //We also don't need to specify the visibility for partial types
            builder.WriteLine($"{visibility} partial abstract class {builderClassName}<TClass, TBuilder>");
            builder.Indent();
            builder.WriteLine($"where TClass : {@classSymbol.ToDisplayString()}");
            builder.WriteLine($"where TBuilder : {builderClassName}<TClass, TBuilder>");
            builder.UnIndent();
            using (builder.OpenBrace())
            {
                using (builder.OpenBrace("protected TBuilder This"))
                using (builder.OpenBrace("get"))
                {
                    builder.WriteLine("return (TBuilder)this;");
                }
                builder.WriteLine();
                using (builder.OpenBrace("protected virtual TClass CreateTarget()"))
                {
                    builder.WriteLine($"return (TClass)new {@classSymbol.ToDisplayString()}();");
                }
                var mappedMembers = new List<(
                    SpecialType specialType,
                    ITypeSymbol type,
                    string name,
                    string wasCalledName
                )>();

                foreach (var memberName in @classSymbol.MemberNames)
                {
                    var members = classSymbol.GetMembers(memberName).OfType<IPropertySymbol>();
                    foreach (var member in members)
                    {
                        if (member.SetMethod is not null && member.GetMethod is not null)
                        {
                            var specialType = member.Type.SpecialType;
                            var typeName = member.Type.ToDisplayString();
                            var name = member.Name;
                            var parameterName = name.FirstCharToLowerCase();
                            var wasCalledName = $"Was{name}Called";

                            builder.WriteLine();
                            builder.WriteLine($"protected {typeName} {name} {{ get; set; }}");
                            builder.WriteLine($"protected bool {wasCalledName} {{ get; private set; }}");
                            builder.WriteLine();

                            using (builder.OpenBrace($"public TBuilder With{name}({typeName} {parameterName})"))
                            {
                                builder.WriteLine($"this.{name} = {parameterName};");
                                builder.WriteLine($"this.{wasCalledName} = true;");
                                builder.WriteLine("return This;");
                            }
                            mappedMembers.Add((specialType, member.Type, name, wasCalledName));
                        }
                    }
                }

                builder.WriteLine();

                using (builder.OpenBrace("public virtual TClass Build()"))
                {
                    builder.WriteLine($"TClass target = this.CreateTarget();");

                    foreach (var mappedMember in mappedMembers)
                    {
                        builder.WriteLine();
                        using (builder.OpenBrace($"if(this.{mappedMember.wasCalledName})"))
                        {
                            builder.WriteLine($"target.{mappedMember.name} = this.{mappedMember.name};");
                        }
                    }

                    builder.WriteLine();
                    builder.WriteLine("return target;");
                }

            }
            var builderFullName = $"{@classSymbol.ContainingNamespace}.{builderClassName}";
            builders.Add(@classSymbol, builderFullName);
        }

        compilation = AddGeneratedCode(context, compilation, @class, builder, addedSourceFiles);
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

        var hintName = $"{fileName}.{@class.Identifier.Text}.Builder.g.cs";
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

        return (semanticModel, symbol, isAttributeDeclared);
    }

    private static AttributeData GetXenialAttribute(INamedTypeSymbol symbol, INamedTypeSymbol generateXenialViewIdsAttribubute)
        => symbol.GetAttribute(generateXenialViewIdsAttribubute);

    private static Compilation GenerateAttribute(GeneratorExecutionContext context, Compilation compilation)
    {
        if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{GenerateXenialXpoBuilderAttributeMSBuildProperty}", out var generateXenialViewIdsAttrStr))
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
                            GenerateXenialXpoBuilderAttributeMSBuildProperty,
                            generateXenialViewIdsAttrStr
                        )
                        , null
                    ));
                return compilation;
            }
        }

        var (source, syntaxTree) = GenerateXenialXpoBuilderAttribute(
            (CSharpParseOptions)context.ParseOptions,
            context.GetDefaultAttributeModifier(),
            context.CancellationToken
        );

        context.AddSource($"{xenialXpoBuilderAttributeName}.g.cs", source);

        return compilation.AddSyntaxTrees(syntaxTree);
    }

    public static (SourceText source, SyntaxTree syntaxTree) GenerateXenialXpoBuilderAttribute(
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

            //using (syntaxWriter.OpenBrace($"{visibility} sealed class {xenialXpoBuilderAttributeName} : Attribute"))
            //{
            //    syntaxWriter.WriteLine($"public Type TargetType {{ get; private set; }}");
            //    syntaxWriter.WriteLine();

            //    using (syntaxWriter.OpenBrace($"{visibility} {xenialXpoBuilderAttributeName}(Type targetType)"))
            //    {
            //        syntaxWriter.WriteLine("this.TargetType = targetType;");
            //    }
            //}
            using (syntaxWriter.OpenBrace($"{visibility} sealed class {xenialXpoBuilderAttributeName} : Attribute"))
            {
                //syntaxWriter.WriteLine($"public Type TargetType {{ get; private set; }}");
                //syntaxWriter.WriteLine();

                //using (syntaxWriter.OpenBrace($"{visibility} {xenialXpoBuilderAttributeName}(Type targetType)"))
                //{
                //    syntaxWriter.WriteLine("this.TargetType = targetType;");
                //}
            }
        }

        var syntax = syntaxWriter.ToString();
        var source = SourceText.From(syntax, Encoding.UTF8);
        var syntaxTree = CSharpSyntaxTree.ParseText(syntax, parseOptions, cancellationToken: cancellationToken);
        return (source, syntaxTree);
    }
}

public static class StringExtensions
{
    public static string FirstCharToLowerCase(this string str)
    {
        if (string.IsNullOrEmpty(str) || char.IsLower(str[0]))
        {
            return str;
        }

        return char.ToLower(str[0], CultureInfo.CurrentUICulture) + str.Substring(1);
    }
}
