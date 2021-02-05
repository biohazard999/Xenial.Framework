using System;

using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.ModelBuilders;
using Xenial.Framework.Tests.Assertions;
using Xenial.Framework.Tests.Layouts.Items;

namespace Xenial.Framework.Tests.Layouts
{
    [DomainComponent]
    public sealed class LayoutPropertyEditorItemBusinessObject
    {
        public string? StringProperty { get; set; }
    }

    internal static partial class TestModelApplicationFactory
    {
        internal static IModelDetailView? CreateDetailViewWithLayout(Func<LayoutBuilder<LayoutPropertyEditorItemBusinessObject>, Layout> layoutFunctor)
        {
            var model = CreateApplication(new(new[]
            {
                typeof(LayoutPropertyEditorItemBusinessObject)
            },
            typesInfo =>
            {
                ModelBuilder.Create<LayoutPropertyEditorItemBusinessObject>(typesInfo)
                    .RemoveAttribute(typeof(DetailViewLayoutBuilderAttribute))
                    .WithDetailViewLayout(layoutFunctor)
                .Build();
            }));

            var detailView = model.FindDetailView<LayoutPropertyEditorItemBusinessObject>();
            return detailView;
        }

        internal static IModelDetailView? CreateComplexDetailViewWithLayout(Func<LayoutBuilder<SimpleBusinessObject>, Layout> layoutFunctor)
        {
            var model = CreateApplication(new(new[]
            {
                typeof(SimpleBusinessObject)
            },
            typesInfo =>
            {
                ModelBuilder.Create<SimpleBusinessObject>(typesInfo)
                    .RemoveAttribute(typeof(DetailViewLayoutBuilderAttribute))
                    .WithDetailViewLayout(layoutFunctor)
                .Build();
            }));

            var detailView = model.FindDetailView<SimpleBusinessObject>();
            return detailView;
        }

        internal static IModelListView? CreateComplexListViewWithLayout(Func<ColumnsBuilder<SimpleBusinessObject>, Columns> columnsFunctor)
        {
            var model = CreateApplication(new(new[]
            {
                typeof(SimpleBusinessObject)
            },
            typesInfo =>
            {
                ModelBuilder.Create<SimpleBusinessObject>(typesInfo)
                    .RemoveAttribute(typeof(ListViewColumnsBuilderAttribute))
                    .WithListViewColumns(columnsFunctor)
                .Build();
            }));

            var listView = model.FindListView<SimpleBusinessObject>();
            return listView;
        }
    }
}
