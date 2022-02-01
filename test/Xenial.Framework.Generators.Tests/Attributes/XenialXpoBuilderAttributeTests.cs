using System;
using System.Linq;

using Xenial.Framework.Generators.Attributes;

namespace Xenial.Framework.Generators.Tests.Attributes;

public class XenialXpoBuilderAttributeTests : AttributeGeneratorBaseTests<XenialXpoBuilderAttributeGenerator>
{
    protected override XenialXpoBuilderAttributeGenerator CreateTargetGenerator()
        => new XenialXpoBuilderAttributeGenerator(false);
}
