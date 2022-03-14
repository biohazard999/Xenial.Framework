using System;
using System.Linq;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.ColumnItems;

namespace HtmlDemoXAFApplication.Module.BusinessObjects
{
    public sealed partial class FooBarPersistentLayoutBuilder : LayoutBuilder<FooBarPersistent>
    {
        public Layout BuildLayout() => new Layout(new DetailViewOptions
        {
        })
        {
            Editor.Label
        };
    }
}
