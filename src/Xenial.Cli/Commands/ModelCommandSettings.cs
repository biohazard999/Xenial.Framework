using Spectre.Console.Cli;

using System;
using System.ComponentModel;
using System.Linq;

namespace Xenial.Cli.Commands;

public class ModelCommandSettings : BuildCommandSettings
{
    public ModelCommandSettings(string workingDirectory) : base(workingDirectory) { }

    [Description("Specifies the target framework to load the model from. Is required when the project is multi-targeted.")]
    [CommandOption("-f|--tfm")]
    public string? Tfm { get; set; }


    [Description("Specifies which namespaces to inspect. You can specify multiple by separation by semicolon.")]
    [CommandOption("-n|--namespaces")]
    public string? Namespaces { get; set; }

    [Description("Specifies which views to inspect. You can specify multiple by separation by semicolon.")]
    [CommandOption("--views")]
    public string? Views { get; set; }

    [Description("Specifies the nuget feed to restore the designer package. By default it will use the project package sources. You can specify multiple by separation by semicolon.")]
    [CommandOption("--designer-nuget-feed", IsHidden = true)]
    public string? DesignerNugetFeed { get; set; }

    [Description("Specifies the package version to restore the designer package. By default it will autodetect the correct version.")]
    [CommandOption("--designer-nuget-package-version", IsHidden = true)]
    public string? DesignerNugetPackageVersion { get; set; }

    [Description("Launches the design process with an attached debugger.")]
    [CommandOption("--designer-debug", IsHidden = true)]
    [DefaultValue(false)]
    public bool DesignerDebug { get; set; }

    [Description("Specifies if the project references should be built.")]
    [CommandOption("-r|--references")]
    [DefaultValue(true)]
    public bool BuildReferences { get; set; }

    [Description("For internal use only.")]
    [CommandOption("--launch-from-nuget", IsHidden = true)]
    [DefaultValue(true)]
    public bool LaunchFromNuget { get; set; }

    [Description("Specifies the connection timeout for the design process communitcation in milliseconds. Will be unlimited when debugger is attached.")]
    [CommandOption("--designer-connection-timeout", IsHidden = true)]
    [DefaultValue(10000)]
    public int DesignerConnectionTimeout { get; set; } = 10000;
}
