using System;
using System.Linq;

namespace Xenial.Framework.Generators.Tests;

public class XenialExpandMemberTests : AttributeGeneratorBaseTests<XenialExpandMemberAttributeGenerator>
{
    protected override XenialExpandMemberAttributeGenerator CreateTargetGenerator()
        => new XenialExpandMemberAttributeGenerator(false);
}
