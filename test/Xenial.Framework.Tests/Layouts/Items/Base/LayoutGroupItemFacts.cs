using System.Linq;

using Shouldly;

using Xenial.Framework.Layouts.Items;

using static Xenial.Tasty;

namespace Xenial.Framework.Tests.Layouts.Items
{
    public static partial class LayoutGroupItemFacts
    {
        public static void LayoutGroupItemChildrenTests() => Describe(nameof(LayoutGroupItem), () =>
        {
            It("When adding a child, parent is set", () =>
            {
                var parentNode = new LayoutGroupItem();
                var childNode = new LayoutGroupItem();
                parentNode.Add(childNode);

                childNode.ShouldSatisfyAllConditions(
                    () => childNode.Parent.ShouldNotBeNull(),
                    () => childNode.Parent.ShouldBe(parentNode)
                );
            });

            It("When adding a child, it is in collection", () =>
            {
                var parentNode = new LayoutGroupItem();
                var childNode = new LayoutGroupItem();
                parentNode.Add(childNode);

                parentNode.ShouldContain(childNode);
            });

            It("When setting the parent, it is in collection", () =>
            {
                var parentNode = new LayoutGroupItem();
                var childNode = new LayoutGroupItem();
                childNode.Parent = parentNode;

                parentNode.ShouldContain(childNode);
            });

            It("When unsetting the parent, it is removed from collection", () =>
            {
                var parentNode = new LayoutGroupItem();
                var childNode = new LayoutGroupItem();
                childNode.Parent = parentNode;
                childNode.Parent = null;

                parentNode.ShouldNotContain(childNode);
            });

            It("When removing a child, it is removed from collection", () =>
            {
                var parentNode = new LayoutGroupItem();
                var childNode = new LayoutGroupItem();
                parentNode.Add(childNode);
                parentNode.Remove(childNode);

                parentNode.ShouldNotContain(childNode);
            });

            It("When removing a child, it's parent is unset", () =>
            {
                var parentNode = new LayoutGroupItem();
                var childNode = new LayoutGroupItem();
                parentNode.Add(childNode);
                parentNode.Remove(childNode);

                childNode.Parent.ShouldBeNull();
            });

            It("When setting a new parent, tree structure is correct", () =>
            {
                var rootNode = new LayoutGroupItem();
                var childNode1 = new LayoutGroupItem();
                var childNode2 = new LayoutGroupItem();
                rootNode.Add(childNode1);
                rootNode.Add(childNode2);
                childNode2.Parent = childNode1;

                rootNode.ShouldSatisfyAllConditions(
                    () => rootNode.Parent.ShouldBeNull(),
                    () => childNode1.Parent.ShouldBe(rootNode),
                    () => childNode2.Parent.ShouldBe(childNode1),
                    () => rootNode.ShouldContain(childNode1),
                    () => childNode1.ShouldContain(childNode2),
                    () => childNode2.ShouldBeEmpty()
                );
            });

            It("settings parent twice has no effect", () =>
            {
                var rootNode = new LayoutGroupItem();
                var childNode = new LayoutGroupItem();
                rootNode.Add(childNode);

                childNode.ShouldSatisfyAllConditions(
                    () => childNode.Parent.ShouldBe(rootNode)
                );
            });

            It("clear unsets the parents", () =>
            {
                var rootNode = new LayoutGroupItem();
                var childNode1 = new LayoutGroupItem();
                var childNode2 = new LayoutGroupItem();
                var childNode3 = new LayoutGroupItem();
                var childNode4 = new LayoutGroupItem();
                rootNode.Add(childNode1);
                rootNode.Add(childNode2);
                rootNode.Add(childNode3);
                rootNode.Add(childNode4);

                rootNode.Clear();

                rootNode.ShouldSatisfyAllConditions(
                    () => rootNode.ShouldBeEmpty(),
                    () => childNode1.Parent.ShouldBeNull(),
                    () => childNode2.Parent.ShouldBeNull(),
                    () => childNode3.Parent.ShouldBeNull(),
                    () => childNode4.Parent.ShouldBeNull()
                );
            });

            It("when using with expression sets parent", () =>
            {
                var rootNode = new LayoutGroupItem();
                var childNode1 = new LayoutGroupItem();
                var childNode2 = new LayoutGroupItem();

                rootNode = rootNode with
                {
                    Children = new()
                    {
                        childNode1,
                        childNode2,
                        new LayoutGroupItem()
                    }
                };

                rootNode.ShouldSatisfyAllConditions(
                    () => childNode1.Parent.ShouldBe(rootNode),
                    () => childNode2.Parent.ShouldBe(rootNode),
                    () => rootNode.Count().ShouldBe(3)
                );
            });

            It("when using short with expression sets parent", () =>
            {
                var rootNode = new LayoutGroupItem();
                var childNode1 = new LayoutGroupItem();
                var childNode2 = new LayoutGroupItem();

                rootNode = rootNode with
                {
                    Children = new(
                        childNode1,
                        childNode2,
                        new LayoutGroupItem()
                    )
                };

                rootNode.ShouldSatisfyAllConditions(
                    () => childNode1.Parent.ShouldBe(rootNode),
                    () => childNode2.Parent.ShouldBe(rootNode),
                    () => rootNode.Count().ShouldBe(3)
                );
            });

            It("when using short initializer expression sets parent", () =>
            {
                var childNode1 = new LayoutGroupItem();
                var childNode2 = new LayoutGroupItem();

                var rootNode = new LayoutGroupItem
                {
                    childNode1,
                    childNode2,
                    new LayoutGroupItem()
                };

                rootNode.ShouldSatisfyAllConditions(
                    () => childNode1.Parent.ShouldBe(rootNode),
                    () => childNode2.Parent.ShouldBe(rootNode),
                    () => rootNode.Count().ShouldBe(3)
                );
            });

            It("when using long initializer expression sets parent", () =>
            {
                var childNode1 = new LayoutGroupItem();
                var childNode2 = new LayoutGroupItem();

                var rootNode = new LayoutGroupItem
                {
                    Children = new()
                    {
                        childNode1,
                        childNode2,
                        new LayoutGroupItem()
                    }
                };

                rootNode.ShouldSatisfyAllConditions(
                    () => childNode1.Parent.ShouldBe(rootNode),
                    () => childNode2.Parent.ShouldBe(rootNode),
                    () => rootNode.Count().ShouldBe(3)
                );
            });

            It("when using copy initializer expression sets parent", () =>
            {
                var childNode1 = new LayoutGroupItem();
                var childNode2 = new LayoutGroupItem();

                var nodes = new[] { childNode1, childNode2 };

                var rootNode = new LayoutGroupItem
                {
                    Children = new(nodes)
                    {
                        new LayoutGroupItem()
                    }
                };

                rootNode.ShouldSatisfyAllConditions(
                    () => childNode1.Parent.ShouldBe(rootNode),
                    () => childNode2.Parent.ShouldBe(rootNode),
                    () => rootNode.Count().ShouldBe(3)
                );
            });

            It("when using short copy initializer expression sets parent", () =>
            {
                var childNode1 = new LayoutGroupItem();
                var childNode2 = new LayoutGroupItem();

                var rootNode = new LayoutGroupItem
                {
                    Children = new(new[] { childNode1, childNode2, new LayoutGroupItem() })
                };

                rootNode.ShouldSatisfyAllConditions(
                    () => childNode1.Parent.ShouldBe(rootNode),
                    () => childNode2.Parent.ShouldBe(rootNode),
                    () => rootNode.Count().ShouldBe(3)
                );
            });

            It("when using ultra short copy initializer expression sets parent", () =>
            {
                var childNode1 = new LayoutGroupItem();
                var childNode2 = new LayoutGroupItem();

                var rootNode = new LayoutGroupItem
                {
                    Children = new(
                        childNode1,
                        childNode2,
                        new LayoutGroupItem()
                    )
                };

                rootNode.ShouldSatisfyAllConditions(
                    () => childNode1.Parent.ShouldBe(rootNode),
                    () => childNode2.Parent.ShouldBe(rootNode),
                    () => rootNode.Count().ShouldBe(3)
                );
            });
        });
    }
}
