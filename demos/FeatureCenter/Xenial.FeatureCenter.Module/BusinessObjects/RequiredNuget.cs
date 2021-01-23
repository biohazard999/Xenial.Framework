using System;

namespace Xenial.FeatureCenter.Module.BusinessObjects
{
    public record RequiredNuget(string? ModuleName, AvailablePlatform? Platform = null)
    {
        private readonly string nugetPostFix = Platform.HasValue ? $".{Platform.Value}" : string.Empty;
        private string nugetName = string.IsNullOrEmpty(ModuleName) ? "Xenial.Framework" : $"Xenial.Framework.{ModuleName}";
        public string Nuget => $"{nugetName}{nugetPostFix}";
    }

    public enum AvailablePlatform
    {
        Win,
        Blazor
    }
}
