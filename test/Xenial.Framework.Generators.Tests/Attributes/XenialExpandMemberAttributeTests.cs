using System;
using System.Linq;

using Xenial.Framework.Generators.Attributes;

namespace Xenial.Framework.Generators.Tests.Attributes;

public class XenialExpandMemberAttributeTests : AttributeGeneratorBaseTests<XenialExpandMemberAttributeGenerator>
{
    protected override XenialExpandMemberAttributeGenerator CreateTargetGenerator()
        => new();
}
