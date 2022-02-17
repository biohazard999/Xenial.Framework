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

using Xenial.Framework.Generators.Attributes;
using Xenial.Framework.Generators.Base;
using Xenial.Framework.MsBuild;

namespace Xenial.Framework.Generators.Partial;

public record XenialCollectControllersGenerator(bool AddSource = true) : XenialPartialGenerator(AddSource)
{
    public XenialCollectControllersAttributeGenerator AttributeGenerator { get; } = new XenialCollectControllersAttributeGenerator(false);

    public override IEnumerable<XenialAttributeGenerator> DependsOnGenerators => new[] { AttributeGenerator };

    private const string fullQualifiedBaseControllerType = "DevExpress.ExpressApp.Controller";

    public override bool Accepts(TypeDeclarationSyntax typeDeclarationSyntax)
        => true; //TODO: Optimize by only looking at correct types

    //{
    //    _ = typeDeclarationSyntax ?? throw new ArgumentNullException(nameof(typeDeclarationSyntax));

    //    if (typeDeclarationSyntax.BaseList is not null)
    //    {
    //        var baseTypeSyntax = typeDeclarationSyntax.BaseList.Types.OfType<SimpleBaseTypeSyntax>();

    //        var derivesFrom = baseTypeSyntax.Select(b => b.Type).OfType<QualifiedNameSyntax>()
    //            .Any(g => g.ToString().Contains("Controller"));

    //        return derivesFrom;
    //    }

    //    return false;
    //}

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

        if (!FindAttributeFromGenerator(compilation, AttributeGenerator, out compilation))
        {
            return compilation;
        }

        var attribute = GetAttributeFromGenerator(compilation, AttributeGenerator);

        var baseTypesToCollect = new[]
        {
            compilation.GetTypeByMetadataName(fullQualifiedBaseControllerType)!
        }.Where(baseType => baseType is not null)
         .ToArray();

        var collectedControllerTypes = new List<TargetSymbol>();

        foreach (var @class in types)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (TryGetTarget(context, compilation, @class, out var targetSymbol))
            {
                if (!targetSymbol.IsAbstract && targetSymbol.HasBaseClasses(baseTypesToCollect))
                {
                    collectedControllerTypes.Add(targetSymbol);
                }
            }
        }

        foreach (var @class in types)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            if (TryGetTargetWithAttribute(context, compilation, @class, attribute, out var targetSymbol))
            {
                if (!IsTargetPartial(context, targetSymbol, attribute))
                {
                    continue;
                }

                var isGlobalNamespace = targetSymbol.Symbol.ContainingNamespace.ToString() == "<global namespace>";
                if (isGlobalNamespace)
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            GeneratorDiagnostics.ClassNeedsToBeInNamespace(
                            attribute.Name
                        ), @class.GetLocation())
                    );

                    continue;
                }

                if (!collectedControllerTypes.Any())
                {
                    continue;
                }

                var builder = CurlyIndenter.Create();

                builder.WriteLine("// <auto-generated />");
                builder.WriteLine();
                builder.WriteLine("using System;");
                builder.WriteLine("using System.Collections.Generic;");
                builder.WriteLine("using System.Runtime.CompilerServices;");
                builder.WriteLine();


                var visibility = context.GetDefaultAttributeModifier();
                using (builder.OpenBrace($"namespace {targetSymbol.Namespace}"))
                {
                    var compilerGeneratedAttribute = compilation.GetTypeByMetadataName("System.Runtime.CompilerServices.CompilerGeneratedAttribute")!;
                    if (!targetSymbol.HasAttribute(compilerGeneratedAttribute))
                    {
                        builder.WriteLine("[CompilerGenerated]");
                    }

                    //We don't need to specify any other modifier
                    //because the user can decide if he want it to be an instance type.
                    //We also don't need to specify the visibility for partial types
                    using (builder.OpenBrace($"partial {(targetSymbol.Symbol.IsRecord ? "record" : "class")} {targetSymbol.Symbol.Name}"))
                    {
                        builder.Write($"{visibility} static readonly IEnumerable<Type> ControllerTypes = ");

                        using (builder.OpenBrace("new Type[]", closeBrace: "};"))
                        {
                            foreach (var controller in collectedControllerTypes.Distinct())
                            {
                                builder.WriteLine($"typeof({controller.Symbol}),");
                            }
                        }
                    }
                }

                compilation = AddCode(
                    context,
                    compilation,
                    addedSourceFiles,
                    $"{Path.GetFileNameWithoutExtension(@class?.SyntaxTree.FilePath)}.ControllerTypes",
                    builder.ToString()
                );
            }
        }

        return compilation;
    }
}

