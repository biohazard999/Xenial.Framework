using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp.Model;

using Xenial.Framework.Layouts.Items.Base;

namespace Xenial.Framework.Model.GeneratorUpdaters.Layouts
{
    /// <summary>   Interface ILeafNodeBuilder. </summary>
    public interface IModelLayoutItemLeafNodeBuilder
    {
        /// <summary>   Builds the leaf. </summary>
        ///
        /// <param name="detailView">       The detail view. </param>
        /// <param name="parentNode">       The parent node. </param>
        /// <param name="layoutItemLeaf">   The layout item leaf. </param>
        ///
        /// <returns>   IModelLayoutItem. </returns>

        IModelLayoutItem BuildLeaf(IModelDetailView detailView, IModelNode parentNode, LayoutItemLeaf layoutItemLeaf);
    }
}
