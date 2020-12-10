using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Standard.Licensing.Validation;

using Xenial;

namespace Xenial.Framework
{
    [XenialCheckLicence]
    internal class Foo
    {

    }
    //internal static class LicenseCheck
    //{
    //    internal static bool CheckLicense()
    //    {
    //        using (var licStream = typeof(LicenseCheck).Assembly.GetManifestResourceStream($"{typeof(LicenseCheck).Assembly.GetName().Name}.License.xml"))
    //        {
    //            var lic = Standard.Licensing.License.Load(licStream);
    //            var validation = lic.Validate()
    //               .ExpirationDate()
    //               .And()
    //               .Signature("MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAE\u002BfPM9MW73SRjhQZuQUAbhdc9MCbpLNXTR0IKtXMi3BCGoA\u002BqFdECmDF\u002BFOiCaIYHccVhebcH3QlYLE2JuiJ39Q==")
    //               .AssertValidLicense();
    //            return !validation.Any();
    //        }
    //    }

    //    internal static bool IsTrial()
    //        => true;
    //}
}
