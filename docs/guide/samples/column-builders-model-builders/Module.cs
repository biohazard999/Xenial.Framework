using System;
using System.Collections.Generic;

using DevExpress.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Xpo;

using Xenial.Framework.ModelBuilders;
using Xenial.Framework.Layouts;

using MainDemo.Module.BusinessObjects;

namespace MainDemo.Module
{
    public sealed partial class MyApplicationModule : ModuleBase
    {
        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters)
        {
            base.AddGeneratorUpdaters(updaters);

            updaters.UseDetailViewLayoutBuilders();
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo)
        {
            base.CustomizeTypesInfo(typesInfo);

            ModelBuilder.Create<Person>(typesInfo)
                .WithListViewColumns(b => new Columns(new ListViewOptions
                {
                    Caption = "All Persons",
                    IsGroupPanelVisible = true,
                    IsFooterVisible = true,
                    ShowAutoFilterRow = true,
                    ShowFindPanel = true,
                    AutoExpandAllGroups = true
                })
                {
                    b.Column(m => m.Address1.City, "Address", c =>
                    {
                        c.Index = -1;
                        c.GroupIndex = 0;
                        c.SortOrder = ColumnSortOrder.Ascending;
                    }),
                    b.Column(m => m.FirstName, 70, c => c.SortOrder = ColumnSortOrder.Ascending),
                    b.Column(m => m.LastName, 70),
                    b.Column(m => m.Phone, 30),
                    b.Column(m => m.Email, 30)
                })
            .Build();
        }
    }
}
