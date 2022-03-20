using System;
using System.Linq;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.ColumnItems;

namespace HtmlDemoXAFApplication.Module.BusinessObjects
{
    public sealed partial class FooBarPersistentColumnsBuilder : ColumnsBuilder<FooBarPersistent>
    {
        public Columns BuildDetailViewColumns() => new Columns();
    }
}
