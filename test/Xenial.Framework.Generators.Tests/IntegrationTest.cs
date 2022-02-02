using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VerifyXunit;

using Xenial.Framework.Generators.Tests.Attributes;

using Xunit;

namespace Xenial.Framework.Generators.Tests;

[UsesVerify]
public class IntegrationTest
{
    [Fact]
    public async Task SanityTest()
        => await GeneratorTest.RunTest(o => o with
        {
            AddSources = false,
            Compile = true
        });
}
