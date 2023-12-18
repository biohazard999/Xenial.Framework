using System;
using System.Linq;

using Xenial.Framework.Generators.Attributes;

namespace Xenial.Framework.Generators.Tests.Attributes;

public class XenialCollectExportedTypesAttributeTests : AttributeGeneratorBaseTests<XenialCollectExportedTypesAttributeGenerator>
{
    protected override XenialCollectExportedTypesAttributeGenerator CreateTargetGenerator()
        => new();
}
