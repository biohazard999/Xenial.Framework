using VerifyTests;

namespace Xenial.Framework.Generators.Tests;

public static class RegisterModuleInitializers
{
    private static readonly object locker = new object();
    private static bool wasCalled;

#if NET5_0_OR_GREATER
    [System.Runtime.CompilerServices.ModuleInitializer]
#endif
    public static void RegisterVerifiers()
    {
        if (wasCalled)
        {
            return;
        }

        lock (locker)
        {
            wasCalled = true;
            VerifySourceGenerators.Enable();
        }
    }
}
