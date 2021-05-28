
using System;
using System.Linq;

using DevExpress.ExpressApp.SystemModule;

namespace Xenial.Framework.Badges.Win.Adapters
{
    internal interface IAdornerAdapter : IDisposable
    {
        /// <summary>   Disables this object. </summary>
        void Disable();

        /// <summary>   Enables the given show navigation item controller. </summary>
        ///
        /// <param name="showNavigationItemController"> The show navigation item controller. </param>

        void Enable(ShowNavigationItemController showNavigationItemController);
    }
}
