using Shouldly;

using Xenial.Framework.Layouts.Items.Base;

using static Xenial.Tasty;

namespace Xenial.Framework.Tests.Layouts.Items.Base
{
    public static class GeneralLayoutFacts
    {
        public static void GeneralLayoutTests() => Describe(nameof(LayoutItemNode), () =>
        {
            It("When adding a child, parent is set", () =>
            {
                var parentNode = new LayoutItem();
                var childNode = new LayoutItem();
                parentNode.Add(childNode);

                childNode.ShouldSatisfyAllConditions(
                    () => childNode.Parent.ShouldNotBeNull(),
                    () => childNode.Parent.ShouldBe(parentNode)
                );
            });

            It("When adding a child, it is in collection", () =>
            {
                var parentNode = new LayoutItem();
                var childNode = new LayoutItem();
                parentNode.Add(childNode);

                parentNode.ShouldContain(childNode);
            });

            It("When setting the parent, it is in collection", () =>
            {
                var parentNode = new LayoutItem();
                var childNode = new LayoutItem();
                childNode.Parent = parentNode;

                parentNode.ShouldContain(childNode);
            });

            It("When unsetting the parent, it is removed from collection", () =>
            {
                var parentNode = new LayoutItem();
                var childNode = new LayoutItem();
                childNode.Parent = parentNode;
                childNode.Parent = null;

                parentNode.ShouldNotContain(childNode);
            });

            It("When removing a child, it is removed from collection", () =>
            {
                var parentNode = new LayoutItem();
                var childNode = new LayoutItem();
                parentNode.Add(childNode);
                parentNode.Remove(childNode);

                parentNode.ShouldNotContain(childNode);
            });

            It("When removing a child, it's parent is unset", () =>
            {
                var parentNode = new LayoutItem();
                var childNode = new LayoutItem();
                parentNode.Add(childNode);
                parentNode.Remove(childNode);

                childNode.Parent.ShouldBeNull();
            });

            It("When setting a new parent, tree structure is correct", () =>
            {
                var rootNode = new LayoutItem();
                var childNode1 = new LayoutItem();
                var childNode2 = new LayoutItem();
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
                var rootNode = new LayoutItem();
                var childNode = new LayoutItem();
                rootNode.Add(childNode);

                childNode.ShouldSatisfyAllConditions(
                    () => childNode.Parent.ShouldBe(rootNode)
                );
            });

            It("clear unsets the parents", () =>
            {
                var rootNode = new LayoutItem();
                var childNode1 = new LayoutItem();
                var childNode2 = new LayoutItem();
                var childNode3 = new LayoutItem();
                var childNode4 = new LayoutItem();
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
        });
    }
}
