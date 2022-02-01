using System;
using System.Linq;

using Xenial.Framework.Generators.Attributes;

namespace Xenial.Framework.Generators.Tests;

public class XenialExpandMemberTests : AttributeGeneratorBaseTests<XenialExpandMemberAttributeGenerator>
{
    protected override XenialExpandMemberAttributeGenerator CreateTargetGenerator()
        => new XenialExpandMemberAttributeGenerator(false);
}
