using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using VerifyXunit;

using Xenial.Framework.Generators.Tests.Base;

using Xunit;

using static Xenial.Framework.Generators.Tests.TestReferenceAssemblies;

namespace Xenial.Framework.Generators.Tests;

[UsesVerify]
public class IntegrationTest
{
    [Fact]
    public async Task SanityTest()
        => await BaseGeneratorTest.RunTest(o => o with
        {
            AddSources = false
        });

    [Fact]
    public void SanityCompileTest()
        => BaseGeneratorTest.Compile(o => o with
        {
            AddSources = true,
            ReferenceAssembliesProvider = (o) => o.ReferenceAssemblies.Concat(new[]
            {
                MetadataReference.CreateFromFile(typeof(ModuleBase).Assembly.Location)
            })
        });
}
