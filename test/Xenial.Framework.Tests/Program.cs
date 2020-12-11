using System;
using System.Threading.Tasks;
using static Xenial.Framework.Tests.Model.Core.NullDiffsStoreFacts;
using static Xenial.Framework.Tests.Model.GeneratorUpdaters.ModelOptionsNodesGeneratorUpdaterFacts;
using static Xenial.Framework.Tests.ModuleTypeListExtentionsFacts;
using static Xenial.Tasty;
using static Xenial.Utils.Tests.ExpressionHelperFacts;

namespace Xenial.Framework.Tests
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            Describe(nameof(Xenial), () =>
            {
                NullDiffsStoreTests();
                ModelOptionsNodesGeneratorUpdaterTests();
                ModuleTypeListExtentionsTests();
                ExpressionHelperTests();
            });

            return await Run(args);
        }
    }
}
