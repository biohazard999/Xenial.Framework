using System;
using System.Linq;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.ColumnItems;

namespace HtmlDemoXAFApplication.Module.BusinessObjects
{
    public sealed partial class FooBarColumnsBuilder : ColumnsBuilder<FooBar>
    {
        public Columns BuildColumns() => new Columns(new ListViewOptions
        {
        })
        {
            Column.Oid with 
            {
                Index = -1,
            },
            Column.Label,
        };
    }
}
