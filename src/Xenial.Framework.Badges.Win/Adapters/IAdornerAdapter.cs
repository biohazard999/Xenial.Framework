
using System;
using System.Linq;

using DevExpress.ExpressApp.SystemModule;

namespace Xenial.Framework.Badges.Win.Adapters
{
    internal interface IAdornerAdapter : IDisposable
    {
        void Disable();
        void Enable(ShowNavigationItemController showNavigationItemController);
    }
}
