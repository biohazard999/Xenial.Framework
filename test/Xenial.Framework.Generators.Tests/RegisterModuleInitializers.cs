using VerifyTests;

namespace Xenial.Framework.Generators.Tests
{

    public static class RegisterModuleInitializers
    {
#if NET5_0_OR_GREATER
        [System.Runtime.CompilerServices.ModuleInitializer]
#endif
        public static void RegisterVerifiers()
            => VerifySourceGenerators.Enable();
    }
}
