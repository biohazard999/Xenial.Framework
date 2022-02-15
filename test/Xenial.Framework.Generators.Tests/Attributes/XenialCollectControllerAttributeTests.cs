using System;
using System.Linq;

using Xenial.Framework.Generators.Attributes;

namespace Xenial.Framework.Generators.Tests.Attributes;

public class XenialCollectControllerAttributeTests : AttributeGeneratorBaseTests<XenialCollectControllerAttributeGenerator>
{
    protected override XenialCollectControllerAttributeGenerator CreateTargetGenerator()
        => new();
}
