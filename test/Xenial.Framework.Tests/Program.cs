using System;
using System.Threading.Tasks;

using Xenial.Framework.Tests.Model.Core;
using Xenial.Framework.Tests.Model.GeneratorUpdaters;
using Xenial.Framework.Tests.ModelBuilders;
using Xenial.Utils.Tests;

using static Xenial.Tasty;

namespace Xenial.Framework.Tests
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            Describe(nameof(Xenial), () =>
            {
                NullDiffsStoreFacts.NullDiffsStoreTests();
                ModelOptionsNodesGeneratorUpdaterFacts.ModelOptionsNodesGeneratorUpdaterTests();
                ModuleTypeListExtentionsFacts.ModuleTypeListExtentionsTests();
                ExpressionHelperFacts.ExpressionHelperTests();

                ModelBuilderFacts.ModelBuilderTests();
                ModelBuilderExtensionFacts.ModelBuilderExtensionTests();
                PropertyBuilderExtensionsFacts.PropertyBuilderExtensionsTests();
            });

            return await Run(args);
        }
    }
}
