using System;
using System.Threading.Tasks;

using Xenial.Framework.Tests.Model.Core;
using Xenial.Framework.Tests.Model.GeneratorUpdaters;
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
            });

            return await Run(args);
        }
    }
}
