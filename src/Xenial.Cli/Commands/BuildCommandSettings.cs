using Microsoft.Build.Framework;

using Spectre.Console;
using Spectre.Console.Cli;

using System;
using System.ComponentModel;
using System.Linq;

namespace Xenial.Cli.Commands;

public class BuildCommandSettings : BaseCommandSettings
{
    public BuildCommandSettings(string? workingDirectory) : base(workingDirectory) { }

    [Description("Project (csproj) or Solution (sln) file")]
    [CommandArgument(0, "[project]")]
    public string ProjectOrSolution { get; set; } = "";

    [Description("Doesn't execute an implicit restore during build.")]
    [CommandOption("--no-restore")]
    public bool NoRestore { get; set; }

    [Description("Doesn't execute an implicit build.")]
    [CommandOption("--no-build")]
    public bool NoBuild { get; set; }

    [Description("Specifies the msbuild log verbosity during build.")]
    [CommandOption("--build-verbosity")]
    public LoggerVerbosity? MsBuildVerbosity { get; set; }

    [Description("Specifies if msbuild should log to command line.")]
    [CommandOption("--log-build")]
    [DefaultValue(false)]
    public bool LogMSBuildCommandLine { get; set; }

    [Description("Specifies the target framework monikers. Use * for all, separate by semicolon for multiple values")]
    [CommandOption("--tfms")]
    [DefaultValue("*")]
    public string Tfms { get; set; } = "*";

    [Description("Specifies if error behavior should be strict. When set to false, it continues even if build errors are present")]
    [CommandOption("-s|--strict")]
    [DefaultValue(true)]
    public bool Strict { get; set; }

    public override ValidationResult Validate()
    {
        var result = base.Validate();

        if (string.IsNullOrEmpty(ProjectOrSolution))
        {
            var files = Directory.EnumerateFiles(WorkingDirectory, "*.csproj").Concat(Directory.EnumerateFiles(WorkingDirectory, "*.sln")).ToList();

            var slns = files.Select(f => (ext: Path.GetExtension(f), f)).Where((f) => ".sln".Equals(f.ext, StringComparison.OrdinalIgnoreCase)).ToList();
            if (slns.Count > 1)
            {
                return ValidationResult.Error($"There are mutiple sln files in the directory '{WorkingDirectory}' please specify one explicitly.");
            }
            var (_, sln) = slns.FirstOrDefault();
            if (!string.IsNullOrEmpty(sln))
            {
                ProjectOrSolution = sln;
            }

            var csprojs = files.Select(f => (ext: Path.GetExtension(f), f)).Where((f) => ".csproj".Equals(f.ext, StringComparison.OrdinalIgnoreCase)).ToList();
            if (csprojs.Count > 1)
            {
                return ValidationResult.Error($"There are mutiple csproj files in the directory '{WorkingDirectory}' please specify one explicitly.");
            }
            var (_, csproj) = csprojs.FirstOrDefault();
            if (!string.IsNullOrEmpty(csproj))
            {
                ProjectOrSolution = csproj;
            }
        }

        if (!File.Exists(ProjectOrSolution))
        {
            return ValidationResult.Error($"The project file '{ProjectOrSolution}' does not exist.");
        }

        return result;
    }
}
