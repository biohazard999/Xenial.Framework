using System;
using System.Linq;

using VerifyXunit;

namespace Xenial.Framework.Generators.Tests;

[UsesVerify]
public class ActionsGeneratorTests : BaseGeneratorTests<XenialActionGenerator>
{
    protected override string GeneratorEmitProperty => XenialActionGenerator.GenerateXenialActionAttributeMSBuildProperty;
}
