using System;
using System.Linq;

using Xenial.Framework.Generators.Attributes;

namespace Xenial.Framework.Generators.Tests.Attributes;

public class XenialCollectControllersAttributeTests : AttributeGeneratorBaseTests<XenialCollectControllersAttributeGenerator>
{
    protected override XenialCollectControllersAttributeGenerator CreateTargetGenerator()
        => new();
}
