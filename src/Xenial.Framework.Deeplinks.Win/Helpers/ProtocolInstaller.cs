
using DevExpress.Persistent.Base;

using Microsoft.Win32;

using System;
using System.Diagnostics;
using System.Linq;

using System.Security;

namespace Xenial.Framework.Deeplinks.Win.Helpers;

/// <summary>
/// 
/// </summary>
/// <param name="Executable"></param>
/// <param name="Name"></param>
/// <param name="Description"></param>
public record Protocol(string Executable, string Name, string Description);

/// <summary>
/// 
/// </summary>
public static class ProtocolInstaller
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly Guid ErrorReasonSecurity = new Guid("00D18896-7EB3-4E57-B50D-D1E9C1AF89E3");

    /// <summary>
    /// 
    /// </summary>
    /// <param name="protocol"></param>
    /// <returns></returns>
    public static bool UnRegisterProtocol(Protocol protocol)
    {
        ProtocolGuard(protocol);
        try
        {
            var key = Registry.CurrentUser.OpenSubKey(@"Software\Classes", true);

            if (key is null)
            {
                return false;
            }

            key.DeleteSubKeyTree(protocol.Name, false);

            return true;
        }
        catch (SecurityException ex)
        {
            Tracing.LogError(ErrorReasonSecurity, ex);
            return false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="protocol"></param>
    /// <returns></returns>
    public static bool RegisterProtocol(Protocol protocol)
    {
        ProtocolGuard(protocol);

        try
        {
            var key = Registry.CurrentUser.OpenSubKey(@"Software\Classes", true);

            if (key is null)
            {
                return false;
            }

            RegistryKey protocolKey;
            using (protocolKey = key.OpenSubKey(protocol.Name, true))
            {
                if (protocolKey is null)
                {
                    protocolKey = key.CreateSubKey(protocol.Name);
                }

                protocolKey.SetValue("URL Protocol", "");
                protocolKey.SetValue("", $"URL:{protocol.Description}");

                RegistryKey defaultIcon;
                using (defaultIcon = protocolKey.OpenSubKey("DefaultIcon", true))
                {
                    if (defaultIcon is null)
                    {
                        defaultIcon = protocolKey.CreateSubKey("DefaultIcon");
                    }

                    defaultIcon.SetValue("", $"\"{protocol.Executable}\",0");

                    defaultIcon.Close();
                }

                RegistryKey shell;
                using (shell = protocolKey.OpenSubKey("shell", true))
                {
                    if (shell is null)
                    {
                        shell = protocolKey.CreateSubKey("shell");
                    }

                    RegistryKey open;

                    using (open = shell.OpenSubKey("open", true))
                    {
                        if (open is null)
                        {
                            open = shell.CreateSubKey("open");
                        }

                        RegistryKey command;
                        using (command = open.OpenSubKey("command", true))
                        {
                            if (command == null)
                            {
                                command = open.CreateSubKey("command");
                            }

                            command.SetValue("", $"\"{protocol.Executable}\" \"%l\"");

                            command.Close();
                        }
                        open.Close();
                    }
                    shell.Close();
                }

                protocolKey.Close();
            }

            return true;
        }
        catch (SecurityException ex)
        {
            Tracing.LogError(ErrorReasonSecurity, ex);
            return false;
        }
    }

    private static void ProtocolGuard(Protocol protocol)
    {
        _ = protocol ?? throw new ArgumentNullException(nameof(protocol));

        if (string.IsNullOrEmpty(protocol.Executable))
        {
            throw new ArgumentException($"{nameof(protocol.Executable)} must not be empty");
        }

        if (string.IsNullOrEmpty(protocol.Name))
        {
            throw new ArgumentException($"{nameof(protocol.Name)} must not be empty");
        }
    }
}

