using System;
using System.Threading.Tasks;
using static Xenial.Framework.Tests.Model.Core.NullDiffsStoreFacts;
using static Xenial.Framework.Tests.Model.GeneratorUpdaters.ModelOptionsNodesGeneratorUpdaterFacts;
using static Xenial.Tasty;

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
            });

            return await Run(args);
        }
    }
}
