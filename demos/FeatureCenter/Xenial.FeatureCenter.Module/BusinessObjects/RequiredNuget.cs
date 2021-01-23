using System;

namespace Xenial.FeatureCenter.Module.BusinessObjects
{
    public record RequiredNuget(string ModuleName, AvailablePlatform? Platform = null)
    {
        private readonly string nugetPostFix = Platform.HasValue ? $".{Platform.Value}" : string.Empty;
        public string Nuget => $"Xenial.Framework.{ModuleName}{nugetPostFix}";
    }

    public enum AvailablePlatform
    {
        Win,
        Blazor
    }
}
