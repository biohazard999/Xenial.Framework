using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.Build.Framework;

using Newtonsoft.Json;

#nullable disable

namespace Xenial.Framework.MsBuild
{
    public class XenialLicenseValidation : Microsoft.Build.Utilities.Task, ICancelableTask
    {
        private const string prefix = "Xenial:";

        private bool cancelled;
        public void Cancel() => cancelled = true;

        [Output]
        public string GeneratedLicenseFile { get; set; }

        public override bool Execute()
        {
#if DEBUG
            // In Visual Studio or Visual Studio Code, you can add a breakpoint to this file.
            // Then, run MSBuild and use the "Attach to Process" feature to attach to the process
            // ID that this prints to the console.

            // Obviously, remove this when you're finished debugging as it will wait indefinitely
            // for the debugger to attach.
            // System.Console.WriteLine("PID = " + System.Diagnostics.Process.GetCurrentProcess().Id);
            // while (!System.Diagnostics.Debugger.IsAttached && !cancelled)
            // {
            // }
#endif
            if (cancelled)
            {
                return false;
            }

            var xenialLicense = Environment.GetEnvironmentVariable("XENIAL_LICENSE");
            if (string.IsNullOrEmpty(xenialLicense))
            {
                var profileDirectory = GetProfileDirectory();
                var licPath = Path.Combine(profileDirectory, "License.xml");
                if (File.Exists(licPath))
                {
                    xenialLicense = File.ReadAllText(licPath);
                }
            }

            var xenialPublicKeys = Environment.GetEnvironmentVariable("XENIAL_LICENSE_PUBLIC_KEYS");
            if (string.IsNullOrEmpty(xenialPublicKeys))
            {
                var profileDirectory = GetProfileDirectory();
                var xenialPublicKeysPath = Path.Combine(profileDirectory, "License.PublicKeys.json");
                if (File.Exists(xenialPublicKeysPath))
                {
                    xenialPublicKeys = File.ReadAllText(xenialPublicKeysPath);
                }
            }

            var isTrial = true;

            if (string.IsNullOrEmpty(xenialLicense) || string.IsNullOrEmpty(xenialPublicKeys))
            {
                Log.LogWarning("Could not find Xenial.License or Xenial.PublicKey");
                Log.LogWarning("Fall back to trial mode");
            }
            else
            {
                var license = Standard.Licensing.License.Load(xenialLicense);
                var publicKey = JsonConvert.DeserializeObject<Dictionary<string, string>>(xenialPublicKeys)["Xenial"];
                var isSignitureValid = license.VerifySignature(publicKey);
                if (!isSignitureValid)
                {
                    Log.LogError($"{prefix} Xenial.Signiture is invalid");
                    return false;
                }

                isTrial = license.Type == Standard.Licensing.LicenseType.Trial;
            }

            if (isTrial)
            {
                Log.LogWarning($"{prefix} will build in trial mode");
            }

            GeneratedLicenseFile = xenialLicense;

            return true;

            static string GetProfileDirectory()
                => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".xenial");
        }
    }
}
