using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VerifyXunit;

using Xenial.Framework.Generators.Tests.Base;

using Xunit;

namespace Xenial.Framework.Generators.Tests;

[UsesVerify]
public class IntegrationTest
{
    //TODO: enable sanity check
    [Fact(Skip = "Once we refactor all code, enable")]
    public async Task SanityTest()
        => await GeneratorTest.RunTest(o => o with
        {
            AddSources = false,
            Compile = true
        });
}
