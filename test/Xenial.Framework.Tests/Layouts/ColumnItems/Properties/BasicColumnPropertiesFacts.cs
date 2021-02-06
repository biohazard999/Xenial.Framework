using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bogus;

using DevExpress.ExpressApp.Model;

using Xenial.Framework.Layouts.ColumnItems;

using static Xenial.Tasty;
using static Xenial.Framework.Tests.Layouts.TestModelApplicationFactory;
using Xenial.Framework.Tests.Assertions;
using DevExpress.Data;

namespace Xenial.Framework.Tests.Layouts.ColumnItems.Properties
{
    public static class BasicColumnPropertiesFacts
    {
        public static void ColumnPropertiesTests() => Describe(nameof(Column), () =>
        {
            var faker = new Faker();
            FDescribe("Properties", () =>
            {
                It($"{nameof(IModelColumn)}", () =>
                {
                    var width = faker.Random.Number(100);
                    var sortOrder = faker.Random.Enum<ColumnSortOrder>();
                    var groupInterval = faker.Random.Enum<GroupInterval>();
                    var sortIndex = faker.Random.Number(100);
                    var groupIndex = faker.Random.Number(100);

                    var listView = CreateListViewWithColumns(b => new()
                    {
                        b.Column(m => m.StringProperty) with
                        {
                            Width = width,
                            SortOrder = sortOrder,
                            SortIndex = sortIndex,
                            GroupIndex = groupIndex,
                            GroupInterval = groupInterval
                        },
                    });

                    listView.AssertColumnProperties<IModelColumn, IModelColumn>((e) => new()
                    {
                        [e.Property(m => m.Width)] = width,
                        [e.Property(m => m.SortOrder)] = sortOrder,
                        [e.Property(m => m.SortIndex)] = sortIndex,
                        [e.Property(m => m.GroupIndex)] = groupIndex,
                        [e.Property(m => m.GroupInterval)] = groupInterval
                    });
                });
            });
        });
    }
}
