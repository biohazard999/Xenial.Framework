using System;

namespace Xenial;

[AttributeUsage(AttributeTargets.Class)]
internal sealed class XenialCheckLicenseAttribute : Attribute
{
    public XenialCheckLicenseAttribute() { }
}
