using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Xenial.Framework.Tests.Layouts.TestModelApplicationFactory;
using static Xenial.Tasty;


namespace Xenial.Framework.Tests.Layouts.ColumnItems
{
    /// <summary>   The columns integration facts. </summary>
    public static class ColumnsIntegrationFacts
    {
        /// <summary>   Columns integration tests. </summary>
        public static void ColumnsIntegrationTests() => Describe("Columns integration", () =>
        {
            It("should work with fluent syntax", () =>
            {
                var listView = CreateComplexListViewWithLayout(b => new()
                {
                    b.Column(m => m.BoolProperty),
                    b.Column(m => m.IntegerProperty),
                    b.Column(m => m.NullableBoolProperty)
                });
            });
        });
    }
}
