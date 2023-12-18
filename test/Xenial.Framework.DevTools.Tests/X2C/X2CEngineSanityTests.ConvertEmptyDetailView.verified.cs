using System;
using System.Linq;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.ColumnItems;
using Xenial.Framework.Layouts.Items.Base;

namespace HtmlDemoXAFApplication.Module.BusinessObjects
{
    public sealed partial class FooBarPersistentLayoutBuilder : LayoutBuilder<FooBarPersistent>
    {
        public Layout BuildLayout() => new Layout();
    }
}
