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
    {
        _ = typeDeclarationSyntax ?? throw new ArgumentNullException(nameof(typeDeclarationSyntax));

        if (typeDeclarationSyntax.BaseList is not null)
        {
            var baseTypeSyntax = typeDeclarationSyntax.BaseList.Types.OfType<SimpleBaseTypeSyntax>();

            var derivesFrom = baseTypeSyntax.Select(b => b.Type).OfType<QualifiedNameSyntax>()
                .Any(g => g.ToString().Contains("Controller"));

            return derivesFrom;
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

        if (!FindAttributeFromGenerator(compilation, AttributeGenerator, out compilation))
        {
            return compilation;
        }

        var attribute = GetAttributeFromGenerator(compilation, AttributeGenerator);

        var baseTypesToCollect = new[]
        {
            compilation.GetTypeByMetadataName(fullQualifiedBaseControllerType)!
        }
        .Where(baseType => baseType is not null)
        .ToArray();

        var collectedControllerTypes = new List<TargetSymbol>();

        foreach (var @class in types)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (TryGetTarget(context, compilation, @class, out var targetSymbol))
            {
                if (targetSymbol.HasBaseClasses(baseTypesToCollect))
                {
                    collectedControllerTypes.Add(targetSymbol);
                }
            }
        }

        return compilation;
    }
}

