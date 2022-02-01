using System;
using System.Linq;

using Xenial.Framework.Generators.Attributes;

namespace Xenial.Framework.Generators.Tests.Attributes;

public class XenialImagesNamesAttributeTests : AttributeGeneratorBaseTests<XenialImageNamesAttributeGenerator>
{
    protected override XenialImageNamesAttributeGenerator CreateTargetGenerator()
        => new XenialImageNamesAttributeGenerator(false);
}
