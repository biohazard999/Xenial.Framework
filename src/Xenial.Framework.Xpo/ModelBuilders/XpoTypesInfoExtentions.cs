﻿using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;

using Xenial.Framework.ModelBuilders;

namespace DevExpress.ExpressApp.DC
{
    /// <summary>   Class XpoModelBuilderExtentions. </summary>
    public static partial class XpoTypesInfoExtentions
    {
        /// <summary>   Removes the xpo views from application model. </summary>
        ///
        /// <param name="typesInfo">    The types information. </param>
        ///
        /// <returns>   ITypesInfo. </returns>

        public static ITypesInfo RemoveXpoViewsFromApplicationModel(this ITypesInfo typesInfo)
        {
            typesInfo
                .CreateModelBuilder<RemoveViewsModelBuilder<PersistentBase>>()
                .Build();

            typesInfo
                .CreateModelBuilder<RemoveViewsModelBuilder<XPBaseObject>>()
                .Build();

            return typesInfo;
        }
    }
}
