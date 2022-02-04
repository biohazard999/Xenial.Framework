using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.ExpressApp;

using Microsoft.CodeAnalysis;

using Xenial.Framework.Generators.Attributes;

namespace Xenial.Framework.Generators.Tests.Attributes;

public class XenialActionAttributeTests : AttributeGeneratorBaseTests<XenialActionAttributeGenerator>
{
    protected override IEnumerable<PortableExecutableReference> AdditionalAssemblies() => new[]
    {
        MetadataReference.CreateFromFile(typeof(ModuleBase).Assembly.Location)
    };

    protected override XenialActionAttributeGenerator CreateTargetGenerator()
        => new();
}
