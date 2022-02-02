using System;
using System.Linq;

using Xenial.Framework.Generators.Attributes;

namespace Xenial.Framework.Generators.Tests.Attributes;

public class SystemIsExternalInitAttributeTests : AttributeGeneratorBaseTests<SystemIsExternalInitAttributeGenerator>
{
    protected override SystemIsExternalInitAttributeGenerator CreateTargetGenerator()
        => new SystemIsExternalInitAttributeGenerator(false);
}
