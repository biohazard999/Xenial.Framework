using System;

using Shouldly;

using Xenial.Framework.Utils;

using static Xenial.Tasty;

namespace Xenial.Framework.Tests.Utils
{
    public static class ResourceUtilFacts
    {
        public static void ResourceExtentionsTests() => Describe(nameof(ResourceUtil), () =>
        {
            var resourceNames = new[]
            {
                "Utils.TestResources.TestEmbeddedResource.txt",
                "Utils\\TestResources\\TestEmbeddedResource.txt",
                "Utils/TestResources/TestEmbeddedResource.txt"
            };

            It($"Guards against null Type", () =>
            {
                Should.Throw<ArgumentNullException>(() => ResourceUtil.GetResourceStream(null!, null!));
            });

            It($"Guards against null path", () =>
            {
                Should.Throw<ArgumentNullException>(() => ResourceUtil.GetResourceStream(typeof(ResourceUtilFacts), null!));
            });

            It($"Guards against invalid path", () =>
            {
                Should.Throw<ArgumentNullException>(() => ResourceUtil.GetResourceStream(typeof(ResourceUtilFacts), string.Empty));
            });

            foreach (var resourceName in resourceNames)
            {
                It($"existing resource stream is not null with type: path: '{resourceName}'", () =>
                {
                    using var resourceStream = ResourceUtil.GetResourceStream(typeof(ResourceUtilFacts), resourceName);
                    resourceStream.ShouldNotBeNull();
                });

                It($"existing resource string is not null with type: path: '{resourceName}'", () =>
                {
                    var resourceString = ResourceUtil.GetResourceString(typeof(ResourceUtilFacts), resourceName);
                    resourceString.ShouldBe("42");
                });
            }

            It($"Throws with not found resource", () =>
            {
                var resourceName = "Resource.Is.Not.Valid.xml";
                var resourceType = typeof(ResourceUtilFacts);
                var name = resourceType.Assembly.GetName().Name;
                var resourcePath = $"{name}.{resourceName}";

                var execption = Should.Throw<ResourceNotFoundException>(() => ResourceUtil.GetResourceStream(resourceType, resourceName));

                execption.ShouldSatisfyAllConditions(
                    () => execption.ResourcePath.ShouldBe(resourcePath),
                    () => execption.Assembly.ShouldBe(resourceType.Assembly),
                    () => execption.ResourceName.ShouldBe(resourceName)
                );
            });
        });
    }
}
