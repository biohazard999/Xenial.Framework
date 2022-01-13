
using System.Collections.Generic;

using Basic.Reference.Assemblies;

using Microsoft.CodeAnalysis;

namespace Xenial.Framework.Generators.Tests;

public static class TestReferenceAssemblies
{

    public static readonly IEnumerable<PortableExecutableReference> DefaultReferenceAssemblies =
#if NET6_0_OR_GREATER
        ReferenceAssemblies.Net60
#elif NET5_0_OR_GREATER
        ReferenceAssemblies.Net50
#elif NETCOREAPP3_1_OR_GREATER
        ReferenceAssemblies.NetCoreApp31
#elif FULL_FRAMEWORK
        ReferenceAssemblies.Net461
#else
        ReferenceAssemblies.NetStandard20
#endif
        ;
}

