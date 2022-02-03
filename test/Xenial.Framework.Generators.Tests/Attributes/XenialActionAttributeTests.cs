using System;
using System.Linq;

using Xenial.Framework.Generators.Attributes;

namespace Xenial.Framework.Generators.Tests.Attributes;

public class XenialActionAttributeTests : AttributeGeneratorBaseTests<XenialActionAttributeGenerator>
{
    protected override XenialActionAttributeGenerator CreateTargetGenerator()
        => new();
}
