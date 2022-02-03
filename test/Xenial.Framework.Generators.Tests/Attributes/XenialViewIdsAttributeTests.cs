using System;
using System.Linq;

using Xenial.Framework.Generators.Attributes;

namespace Xenial.Framework.Generators.Tests.Attributes;

public class XenialViewIdsAttributeTests : AttributeGeneratorBaseTests<XenialViewIdsAttributeGenerator>
{
    protected override XenialViewIdsAttributeGenerator CreateTargetGenerator()
        => new();
}
