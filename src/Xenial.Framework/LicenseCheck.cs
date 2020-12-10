using System;
using System.Linq;

namespace Xenial.Framework
{
    [XenialCheckLicence]
    internal class Foo
    {
        public Foo()
        {
            var isValid = XenialLicenseCheck.IsValid;
        }
    }
}
