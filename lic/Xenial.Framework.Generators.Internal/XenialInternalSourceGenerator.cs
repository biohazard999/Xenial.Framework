using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Xenial.Framework.Generators.Internal.Generators;

namespace Xenial.Framework.Generators.Internal
{
    [Generator]
    public class XenialInternalSourceGenerator : ISourceGenerator
    {
        public IList<IXenialSourceGenerator> Generators { get; }
        public IDictionary<string, string> ConstantsToInject { get; }

        public XenialInternalSourceGenerator()
        {
            ConstantsToInject = new Dictionary<string, string>();
            Generators = new List<IXenialSourceGenerator>
            {
                new XenialTypeForwardGenerator(ConstantsToInject),
                new XenialCopyEnumerationsGenerator(ConstantsToInject),
                new XenialInjectTypeForwardedTypesGenerator(ConstantsToInject)
            };
        }

        public void Initialize(GeneratorInitializationContext context) { }

        public void Execute(GeneratorExecutionContext context)
        {
            var compilation = context.Compilation;
            foreach (var generator in Generators)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                compilation = generator.Execute(context, compilation, Array.Empty<TypeDeclarationSyntax>());
            }
        }
    }
}
