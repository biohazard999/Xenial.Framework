using System;
using System.Linq;

using Xenial.Framework.Generators.Attributes;

namespace Xenial.Framework.Generators.Tests;

public class XenialImagesNamesTests : AttributeGeneratorBaseTests<XenialImageNamesAttributeGenerator>
{
    protected override XenialImageNamesAttributeGenerator CreateTargetGenerator()
        => new XenialImageNamesAttributeGenerator(false);
}
