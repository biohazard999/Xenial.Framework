#pragma warning disable CA1812
#pragma warning disable CA1050
#pragma warning disable xUnit1013

using System;
using System.Threading.Tasks;

using Shouldly;

using Xenial.Framework.Tests;
using Xenial.Framework.Tests.Layouts.ColumnItems;
using Xenial.Framework.Tests.Layouts.ColumnItems.Properties;
using Xenial.Framework.Tests.Layouts.Items;
using Xenial.Framework.Tests.Model.Core;
using Xenial.Framework.Tests.Model.GeneratorUpdaters;
using Xenial.Framework.Tests.ModelBuilders;
using Xenial.Framework.Tests.Utils;
using Xenial.Framework.Tests.Utils.Slugger;
using Xenial.Utils.Tests;

using Xunit;

using static Xenial.Tasty;

XUnitAdapter.Tests();

return await Run(args);

//Waiting for NRE to resolve `https://github.com/xenial-io/Tasty/issues/129`
public class XUnitAdapter
{
    public static void Tests() => Describe(nameof(Xenial), () =>
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
        ResourceUtilFacts.ResourceExtentionsTests();

        Describe("Layouts", () =>
        {
            GeneralLayoutFacts.GeneralLayoutTests();

            BasicLayoutFacts.BasicLayoutTests();
            LayoutPropertyEditorItemFacts.LayoutPropertyEditorItemTests();
            LayoutEmptySpaceItemFacts.LayoutEmptySpaceItemTests();

            LayoutGroupItemFacts.LayoutGroupItemTests();
            LayoutGroupItemFacts.LayoutGroupItemChildrenTests();
            LayoutTabGroupItemFacts.LayoutTabGroupItemTests();
            LayoutTabbedGroupItemFacts.LayoutTabbedGroupItemTests();
            LayoutTabbedGroupItemFacts.LayoutTabbedGroupItemChildTests();

            TreeBuilderFacts.TreeBuilderTests();

            LayoutIntegrationFacts.LayoutIntegrationTests();
        });

        Describe("Columns", () =>
        {
            BasicColumnsFacts.BasicColumnsTests();
            ColumnsIntegrationFacts.ColumnsIntegrationTests();
            BasicColumnPropertiesFacts.ColumnPropertiesTests();
        });
    });

    [Fact]
    public async Task TastyXUnitAdapterTests()
    {
        Tests();
        (await TastyDefaultScope.Run(typeof(XUnitAdapter).Assembly)).ShouldBe(0);
    }
}
