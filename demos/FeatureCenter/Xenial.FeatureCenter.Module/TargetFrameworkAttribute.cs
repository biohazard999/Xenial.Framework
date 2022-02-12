using System;
using System.Collections.Generic;
using System.Text;

namespace Xenial.FeatureCenter.Module;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
public sealed class TargetFrameworkAttribute : Attribute
{
    public string TargetFramework { get; }

    public TargetFrameworkAttribute(string targetFramework)
        => TargetFramework = targetFramework;
}
