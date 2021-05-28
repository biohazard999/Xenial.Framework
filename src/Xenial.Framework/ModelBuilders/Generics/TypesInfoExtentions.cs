using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp.SystemModule;

using Xenial.Framework.ModelBuilders;

namespace DevExpress.ExpressApp.DC
{
    /// <summary>   Class XpoModelBuilderExtentions. </summary>
    public static partial class TypesInfoExtentions
    {
        /// <summary>   Removes the xpo views from application model. </summary>
        ///
        /// <param name="typesInfo">        The types information. </param>
        /// <param name="removeDashboards"> (Optional) if set to <c>true</c> [remove dashboards]. </param>
        ///
        /// <returns>   ITypesInfo. </returns>

        public static ITypesInfo RemoveXafViewsFromApplicationModel(this ITypesInfo typesInfo, bool removeDashboards = true)
        {
            typesInfo
                .CreateModelBuilder<RemoveViewsModelBuilder<NonPersistentBaseObject>>()
                .Build();

            typesInfo
                .CreateModelBuilder<RemoveViewsModelBuilder<NonPersistentObjectImpl>>()
                .Build();

            typesInfo
                .CreateModelBuilder<RemoveViewsModelBuilder<NonPersistentEntityObject>>()
                .Build();

            typesInfo
                .CreateModelBuilder<RemoveViewsModelBuilder<NonPersistentBaseObject>>()
                .Build();

            typesInfo
                .CreateModelBuilder<RemoveViewsModelBuilder<NonPersistentLiteObject>>()
                .Build();

            #region SystemModule

            typesInfo
                .CreateModelBuilder<RemoveViewsModelBuilder<SystemModule.ProcessDataLockingInfoDialogObject>>()
                .Build();

            if (removeDashboards)
            {
                ModelBuilder.Create<ViewDashboardOrganizationItem>(typesInfo)
                    .GenerateNoListViews()
                    .Build();

                ModelBuilder.Create<DashboardCreationInfo>(typesInfo)
                    .GenerateNoListViews()
                    .Build();

                ModelBuilder.Create<DashboardViewItemDescriptor>(typesInfo)
                    .GenerateNoDetailView()
                    .GenerateNoListView()
                    .Build();

                ModelBuilder.Create<DashboardName>(typesInfo)
                   .GenerateNoDetailView()
                   .GenerateNoLookupListView()
                   .Build();

                ModelBuilder.Create<DashboardOrganizationItem>(typesInfo)
                  .GenerateNoDetailView()
                  .GenerateNoLookupListView()
                  .Build();
            }

            #endregion

            return typesInfo;
        }
    }
}
