using System.Runtime.CompilerServices;

using VerifyTests;

namespace Xenial.Framework.Generators.Tests
{

    public static class RegisterModuleInitializers
    {
#if NET5_0_OR_GREATER
        [ModuleInitializer]
#endif
        public static void RegisterVerifiers()
            => VerifySourceGenerators.Enable();
    }
}
