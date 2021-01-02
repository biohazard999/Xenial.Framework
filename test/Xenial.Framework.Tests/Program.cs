using System;
using System.Threading.Tasks;

using Xenial.Framework.Tests.Layouts.Items;
using Xenial.Framework.Tests.Layouts.Items.Base;
using Xenial.Framework.Tests.Model.Core;
using Xenial.Framework.Tests.Model.GeneratorUpdaters;
using Xenial.Framework.Tests.ModelBuilders;
using Xenial.Framework.Tests.Utils.Slugger;
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
                PropertyBuilderFacts.PropertyBuilderTests();
                PropertyBuilderExtensionsFacts.PropertyBuilderExtensionsTests();

                BuilderManagerFacts.BuilderManagerTests();
                XafBuilderManagerFacts.XafBuilderManagerTests();

                SlugerFacts.SluggerTests();

                FDescribe("Layouts", () =>
                {
                    GeneralLayoutFacts.GeneralLayoutTests();

                    BasicLayoutFacts.BasicLayoutTests();
                    LayoutPropertyEditorItemFacts.LayoutPropertyEditorItemTests();
                    LayoutEmptySpaceItemFacts.LayoutEmptySpaceItemTests();

                    LayoutGroupItemFacts.LayoutGroupItemTests();
                    LayoutTabGroupItemFacts.LayoutTabGroupItemTests();
                });
            });

            return await Run(args);
        }
    }
}
