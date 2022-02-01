﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using GlobExpressions;

using Xenial.Delicious.Beer.Json;
using Xenial.Delicious.Beer.Recipes;

using static Bullseye.Targets;
using static SimpleExec.Command;
using static Xenial.Delicious.Beer.Recipes.IISRecipe;

namespace Xenial.Build
{
    internal static partial class Program
    {
        internal static async Task Main(string[] args)
        {
            var versionTask = new Lazy<Task<string>>(async () => (await ReadToolAsync(() => ReadAsync("dotnet", "minver -v e -t v", noEcho: true))).Trim());
            var branchTask = new Lazy<Task<string>>(async () => (await ReadToolAsync(() => ReadAsync("git", "branch --show-current", noEcho: true))).Trim());

            var version = await versionTask.Value;
            var branch = await branchTask.Value;

            //On Github actions, for whatever reason, there is no branch name
            //TODO: use GithubActions Environment variable name
            if (string.IsNullOrEmpty(branch))
            {
                branch = "main";
            }

            var artifactsDirectory = Path.GetFullPath("./artifacts");

            const string PleaseSet = "PLEASE SET BEFORE USE";
            var PublicKey = "MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAE3VFauRJrFzuZveL+J/naEs+CrNLBrc/sSDihdkUTo3Np/o4IoM8fxR6kYHIdH/7LXfXltFRREkv2ceTN8gyZuw==";

            static string logOptions(string target)
                => $"/maxcpucount /nologo /verbosity:minimal /bl:./artifacts/logs/xenial.framework.{target}.binlog";

            const string Configuration = "Release";
            const string ConfigurationDebug = "Debug";

            var sln = RuntimeInformation
                .IsOSPlatform(OSPlatform.Windows)
                ? "Xenial.Framework.sln"
                : "Xenial.Framework.CrossPlatform.sln";

            var tagName = (await ReadAsync("git", "tag --points-at")).Trim();
            var isTagged = !string.IsNullOrWhiteSpace(tagName);

            Console.WriteLine($"Is platform windows? {RuntimeInformation.IsOSPlatform(OSPlatform.Windows)}");
            Console.WriteLine($"Platform: {System.Environment.OSVersion.Platform}");
            Console.WriteLine($"SLN: {sln}");
            Console.WriteLine($"IsTagged: {isTagged}");
            if (isTagged)
            {
                Console.WriteLine($"TagName: {tagName}");
            }

            var featureCenterBlazorDir = "./demos/FeatureCenter/Xenial.FeatureCenter.Blazor.Server";
            var featureCenterBlazor = Path.Combine(featureCenterBlazorDir, "Xenial.FeatureCenter.Blazor.Server.csproj");

            string GetProperties(string configuration = null) => string.Join(" ", new Dictionary<string, string>
            {
                ["Configuration"] = configuration ?? Configuration,
                ["XenialPublicKey"] = PublicKey,
                ["XenialLicGenVersion"] = $"{version}",
                ["RepositoryBranch"] = $"{branch}",
                ["XenialDebug"] = false.ToString()
            }.Select(p => $"/P:{p.Key}=\"{p.Value}\""));

            Target("ensure-tools", () => EnsureTools());

            Target("clean", DependsOn("ensure-tools"),
                () => RunAsync("dotnet", $"rimraf . -i **/bin/**/*.* -i **/obj/**/*.* -i artifacts/**/*.* -e node_modules/**/*.* -e build/**/*.* -e artifacts/**/.gitkeep -q")
            );

            Target("pack.lic", DependsOn("ensure-tools"),
                () => RunAsync("dotnet", $"pack ./lic/Xenial.Framework.Licensing.sln  -c {Configuration} {logOptions("pack.lic")} {GetProperties()}")
            );

            Target("lint", DependsOn("pack.lic", "ensure-tools"),
                //TODO: Linting is currently failing
                () => RunAsync("dotnet", "--version")
                //() => RunAsync("dotnet", $"format {sln} --exclude ext --check --verbosity diagnostic")
            );

            Target("restore", DependsOn("pack.lic", "lint"),
                () => RunAsync("dotnet", $"restore {sln} {logOptions("restore")} {GetProperties()}")
            );

            Target("format", DependsOn("ensure-tools"),
                () => RunAsync("dotnet", $"format {sln} --exclude ext")
            );

            Target("build", DependsOn("restore"),
                () => RunAsync("dotnet", $"build {sln} --no-restore -c {Configuration} {logOptions("build")} {GetProperties()}")
            );

            Target("build:debug", DependsOn("restore"),
                () => RunAsync("dotnet", $"build {sln} --no-restore -c {ConfigurationDebug} {logOptions("build.debug")} {GetProperties(ConfigurationDebug)}")
            );

            Target("test:base", DependsOn("build"), async () =>
            {
                var (fullFramework, netcore, net5, _, _) = FindTfms();

                var tfms = RuntimeInformation
                            .IsOSPlatform(OSPlatform.Windows)
                            ? new[] { fullFramework, netcore, net5 }
                            : new[] { netcore, net5 };

                var tests = tfms
                    .Select(tfm => RunAsync("dotnet", $"run --project test/Xenial.Framework.Tests/Xenial.Framework.Tests.csproj --no-build --no-restore --framework {tfm} -c {Configuration} {GetProperties()}"))
                    .ToArray();

                await Task.WhenAll(tests);
            });

            Target("test:win", DependsOn("build"), async () =>
            {
                var (fullFramework, _, _, winVersion, _) = FindTfms();

                var tfms = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                            ? new[] { fullFramework, winVersion }
                            : Array.Empty<string>();

                var tests = tfms
                    .Select(tfm => RunAsync("dotnet", $"run --project test/Xenial.Framework.Win.Tests/Xenial.Framework.Win.Tests.csproj --no-build --no-restore --framework {tfm} -c {Configuration} {GetProperties()}"))
                    .ToArray();

                await Task.WhenAll(tests);
            });

            Target("test:xunit", DependsOn("build"),
                () => RunAsync("dotnet", $"test {sln} --no-restore --no-build -c {Configuration} {logOptions("build")} {GetProperties()}")
            );

            Target("test", DependsOn("test:base", "test:win", "test:xunit"));

            Target("lic", DependsOn("test"),
                async () =>
                {
                    if (isTagged)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"This is a tagged commit {tagName}, generating licenses");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("This is not a tagged commit, skip license generation");
                        Console.ResetColor();
                        return;
                    }

                    var files = Directory.EnumerateFiles(@"src", "*.csproj", SearchOption.AllDirectories).Select(file => new
                    {
                        ProjectName = $"src/{Path.GetFileNameWithoutExtension(file)}/{Path.GetFileName(file)}",
                        ThirdPartyName = $"src/{Path.GetFileNameWithoutExtension(file)}/THIRD-PARTY-NOTICES.TXT"
                    });

                    // Filter files that are cross platform
                    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("No need to pack on other platforms than windows");
                        Console.ResetColor();
                        return;
                    }

                    var tasks = files.Select(proj => RunAsync("dotnet", $"thirdlicense --project {proj.ProjectName} --output {proj.ThirdPartyName}"));

                    await Task.WhenAll(tasks);
                }
            );

            Target("pack:nuget", DependsOn("lic"),
                () => RunAsync("dotnet", $"pack {sln} --no-restore --no-build -c {Configuration} {logOptions("pack.nuget")} {GetProperties()}")
            );

            Target("pack:zip", DependsOn("pack:nuget"),
                async () =>
                {
                    if (isTagged)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"This is a tagged commit {tagName}, creating zip packages");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("This is not a tagged commit, skip zip packages");
                        Console.ResetColor();
                        return;
                    }

                    var (fullFramework, _, net5, _, netstandardVersion) = FindTfms();

                    foreach (var tfm in new[] { fullFramework, net5, netstandardVersion })
                    {
                        var basePath = Path.GetFullPath("./src");
                        var targetDirectory = Path.Combine(artifactsDirectory, "bin", tfm);

                        if (!Directory.Exists(targetDirectory))
                        {
                            Directory.CreateDirectory(targetDirectory);
                        }

                        foreach (var file in Glob.Files(basePath, $"**/bin/Release/{tfm}/Xenial.*.{{dll,xml}}").Where(f => !f.Contains("Xenial.Framework.Lab")))
                        {
                            var sourceFileName = Path.Combine(basePath, file);
                            var targetFileName = Path.Combine(targetDirectory, Path.GetFileName(file));
                            Console.WriteLine($"{sourceFileName} -> {targetFileName}");
                            File.Copy(sourceFileName, targetFileName, true);
                        }
                        var zipFileName = $"{targetDirectory}.zip";

                        if (File.Exists(zipFileName))
                        {
                            File.Delete(zipFileName);
                        }

                        ZipFile.CreateFromDirectory(targetDirectory, zipFileName);

                        if (Directory.Exists(targetDirectory))
                        {
                            Directory.Delete(targetDirectory, true);
                        }
                    }
                }
            );

            Target("pack", DependsOn("pack:nuget", "pack:zip"));

            Target("publish:Xenial.FeatureCenter.Win", DependsOn("pack"), async () =>
            {
                await RunAsync("dotnet", "--version");

                if (isTagged)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"This is a tagged commit {tagName}, installing dotnet zip install");
                    Console.ResetColor();
                    await RunAsync("dotnet", "zip install");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("This is not a tagged commit, skip dotnet zip install");
                    Console.ResetColor();
                }


                foreach (var (tfm, rid) in new[] { ("net462", ""), ("net6.0-windows", "win-x64"), ("net6.0-windows", "win-x86") })
                {
                    //TODO: remove /p:ErrorOnDuplicatePublishOutputFiles=false
                    //TODO: and investigate https://docs.microsoft.com/en-us/dotnet/core/compatibility/sdk/6.0/duplicate-files-in-output

                    var r2r = isTagged ? "/p:PublishReadyToRun=true" : "";

                    if (isTagged)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"This is a tagged commit {tagName}, will increase performance by using R2R");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("This is not a tagged commit, skip R2R");
                        Console.ResetColor();
                    }

                    var ridP = string.IsNullOrEmpty(rid) ? "" : $"/p:RuntimeIdentifier={rid}";
                    var suffix = string.IsNullOrEmpty(rid) ? "" : $".{rid.Substring("win-".Length)}";
                    var package = string.IsNullOrEmpty(tfm) ? "" : tfm.Split('-')[0];

                    await RunAsync("dotnet", $"publish demos/FeatureCenter/Xenial.FeatureCenter.Win/Xenial.FeatureCenter.Win.csproj --framework {tfm} {ridP} {r2r} /p:ErrorOnDuplicatePublishOutputFiles=false {logOptions($"publish:Xenial.FeatureCenter.Win.{tfm}{suffix}")} {GetProperties()} /p:PackageVersion={version} /p:XenialDemoPackageVersion={version} /p:XenialDebug=false");


                    if (isTagged)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"This is a tagged commit {tagName}, zipping up Xenial.FeatureCenter.Win");
                        Console.ResetColor();

                        await RunAsync("dotnet", $"msbuild demos/FeatureCenter/Xenial.FeatureCenter.Win/Xenial.FeatureCenter.Win.csproj /t:Restore;Build;Publish;CreateZip {logOptions($"zip:Xenial.FeatureCenter.Win.{tfm}{suffix}")} {GetProperties()} /p:ErrorOnDuplicatePublishOutputFiles=false /p:TargetFramework={tfm} {r2r} {ridP} /p:PackageVersion={version} /p:XenialDemoPackageVersion={version} /p:XenialDebug=false /p:PackageName=Xenial.FeatureCenter.Win.v{version}.{package}{suffix} /p:PackageDir={artifactsDirectory}");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("This is not a tagged commit, skip zipping up Xenial.FeatureCenter.Win");
                        Console.ResetColor();
                    }
                }
            });

            BuildAndDeployIISProject(new IISDeployOptions("Xenial.FeatureCenter.Blazor.Server", "framework.featurecenter.xenial.io")
            {
                DotnetCore = true,
                PathToCsproj = featureCenterBlazor,
                AssemblyProperties = $"/property:XenialDebug=false /property:XenialDemoPackageVersion={version}",
                PrepareTask = async () =>
                {
                    var settingsPath = Path.Combine(featureCenterBlazorDir, "appsettings.json");

                    var serverSettings = await File.ReadAllTextAsync(settingsPath);

                    serverSettings = serverSettings
                        .AddOrUpdateJsonValue(
                            "ConnectionStrings:DefaultConnection",
                            Environment.GetEnvironmentVariable("XENIAL_FEATURECENTER_DEFAULTCONNECTIONSTRING") ?? PleaseSet
                        )
                    ;

                    await File.WriteAllTextAsync(settingsPath, serverSettings);
                }
            }, "framework.featurecenter.xenial.io");

            Target("demos", DependsOn("pack", "publish:framework.featurecenter.xenial.io", "publish:Xenial.FeatureCenter.Win"));

            Target("docs:prepare",
                () => RunAsync("npm", windowsName: "cmd", workingDirectory: "docs", args: "ci", windowsArgs: "/c npm ci")
            );

            Target("docs", DependsOn("docs:prepare"),
                () => RunAsync("npm", windowsName: "cmd", workingDirectory: "docs", args: "run build", windowsArgs: "/c npm run build")
            );

            Target("docs:serve", DependsOn("docs:prepare"),
                () => RunAsync("npm", windowsName: "cmd", workingDirectory: "docs", args: "start", windowsArgs: "/c npm start")
            );

            Target("deploy.nuget", DependsOn("ensure-tools"), async () =>
            {
                var files = Directory.EnumerateFiles("artifacts/nuget", "*.nupkg");

                foreach (var file in files)
                {
                    await RunAsync("dotnet", $"nuget push {file} --skip-duplicate -s https://api.nuget.org/v3/index.json -k {Environment.GetEnvironmentVariable("NUGET_AUTH_TOKEN")}",
                        noEcho: true
                    );
                }
            });

            Target("release",
                () => Release()
            );

            Target("LOC", () =>
            {
                var locLic = CountNumberOfLinesInCSFilesOfDirectory("./lic");
                var locSrc = CountNumberOfLinesInCSFilesOfDirectory("./src");

                Console.WriteLine($"LOC lic: {locLic}");
                Console.WriteLine($"LOC src: {locSrc}");

                int CountNumberOfLinesInCSFilesOfDirectory(string dirPath)
                {
                    var csFiles = new DirectoryInfo(dirPath.Trim()).GetFiles("*.cs", SearchOption.AllDirectories);

                    var totalNumberOfLines = 0;
                    Parallel.ForEach(csFiles, async fo =>
                    {
                        Interlocked.Add(ref totalNumberOfLines, await CountNumberOfLine(fo));
                    });
                    return totalNumberOfLines;
                }

                async Task<int> CountNumberOfLine(FileInfo fo)
                {
                    var count = 0;
                    var inComment = 0;
                    using (var sr = fo.OpenText())
                    {
                        string line;
                        while ((line = await sr.ReadLineAsync()) != null)
                        {
                            if (IsRealCode(line.Trim(), ref inComment))
                            {
                                count++;
                            }
                        }
                    }
                    return count;
                }

                bool IsRealCode(string trimmed, ref int inComment)
                {
                    if (trimmed.StartsWith("/*") && trimmed.EndsWith("*/"))
                    {
                        return false;
                    }
                    else if (trimmed.StartsWith("/*"))
                    {
                        inComment++;
                        return false;
                    }
                    else if (trimmed.EndsWith("*/"))
                    {
                        inComment--;
                        return false;
                    }

                    return
                           inComment == 0
                        && !trimmed.StartsWith("//")
                        && (trimmed.StartsWith("if")
                            || trimmed.StartsWith("else if")
                            || trimmed.StartsWith("using (")
                            || trimmed.StartsWith("else  if")
                            || trimmed.Contains(';')
                            || trimmed.StartsWith("public") //method signature
                            || trimmed.StartsWith("private") //method signature
                            || trimmed.StartsWith("protected") //method signature
                            );
                }
            });

            Target("ci", DependsOn("publish:Xenial.FeatureCenter.Win", "publish:framework.featurecenter.xenial.io"));
            Target("ci:full", DependsOn("ci", "docs"));

            Target("local", DependsOn("build:debug"));

            Target("default", DependsOn("test"));

            await RunTargetsAndExitAsync(args);
        }
    }
}
