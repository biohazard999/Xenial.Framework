using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

using DevExpress.ExpressApp.Win;
using DevExpress.Persistent.Base;

using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace Xenial.Framework.WebView.Win.Helpers
{
    /// <summary>   Class EdgeRuntimeInstaller. </summary>
    public static class WebView2RuntimeInstaller
    {
        /// <summary>
        /// (Immutable)
        /// Gets the web view2 runtime installer URL.
        /// </summary>
        ///
        /// ### <returns>   The web view2 runtime installer URL. </returns>

        public static readonly string WebView2RuntimeInstallerUrl = "https://go.microsoft.com/fwlink/p/?LinkId=2124703";

        private static readonly object locker = new object();

        private static Task? installerTask;

        private static async Task<bool> DownloadAndInstallWebView2Runtime()
        {
            if (installerTask is null)
            {
                lock (locker)
                {
                    installerTask = Task.Run(async () =>
                    {
                        if (WinApplication.Messaging.Show(
                            $@"The Microsoft Edge WebView2 Runtime was not found on this computer.

Do you want to download and install it now?
",
                            "WebView2 Runtime is missing",
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                            == System.Windows.Forms.DialogResult.OK
                        )
                        {
                            using var httpClient = new HttpClient();
                            var installerTempPath = Path.GetTempFileName();
                            var installerPath = $"{installerTempPath}.exe";
                            File.Move(installerTempPath, installerPath);
                            using (var installerStream = File.OpenWrite(installerPath))
                            {
                                await httpClient.DownloadFileAsync(WebView2RuntimeInstallerUrl, installerStream).ConfigureAwait(false);
                            }

                            var proc = Process.Start(installerPath, "/install");
                            proc.WaitForExit();
                        }
                    });
                }

            }

            await installerTask.ConfigureAwait(false);

            return true;
        }

        /// <summary>   ensure core web view2 and install as an asynchronous operation. </summary>
        ///
        /// <exception cref="ArgumentNullException">        Thrown when one or more required arguments
        ///                                                 are null. </exception>
        /// <exception cref="ArgumentOutOfRangeException">  Thrown when one or more arguments are outside
        ///                                                 the required range. </exception>
        ///
        /// <param name="control">  The control. </param>
        ///
        /// <returns>   A Task. </returns>

        public static async Task EnsureCoreWebView2AndInstallAsync(this WebView2 control)
        {
            _ = control ?? throw new ArgumentNullException(nameof(control));
            try
            {
                await control.EnsureCoreWebView2Async().ConfigureAwait(false);
            }
            catch (EdgeNotFoundException ex)
            {
                Tracing.LogError(new Guid("369655EA-E64B-45C6-8481-6098F7D96183"), ex);
                if (await DownloadAndInstallWebView2Runtime().ConfigureAwait(false))
                {
                    static void SetPrivateFieldValue<T>(object obj, string propName, T val)
                    {
                        _ = obj ?? throw new ArgumentNullException(nameof(obj));
                        var t = obj.GetType();
                        FieldInfo? fi = null;
                        while (fi == null && t != null)
                        {
                            fi = t.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                            t = t.BaseType;
                        }
                        _ = fi ?? throw new ArgumentOutOfRangeException(nameof(propName), $"Field {propName} was not found in Type {obj.GetType().FullName}");
                        fi.SetValue(obj, val);
                    }

                    SetPrivateFieldValue<Task?>(control, "_initTask", null);
                    try
                    {
                        await control.EnsureCoreWebView2Async().ConfigureAwait(false);
                    }
                    catch (COMException ex2) { HandleCOMException(control, ex2); }
#pragma warning disable CA1031 // Do not catch general exception types
                    catch (Exception ex2) { HandleGenericException(control, ex2); }
#pragma warning restore CA1031 // Do not catch general exception types
                }
            }
            catch (COMException ex) { HandleCOMException(control, ex); }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex) { HandleGenericException(control, ex); }
#pragma warning restore CA1031 // Do not catch general exception types

            static void HandleCOMException(WebView2 ctrl, COMException ex)
            {
                Tracing.LogError(new Guid("FF39957F-C7E7-4498-B6B5-79317D53EAB7"), ex);
                if (!ctrl.IsDisposed)
                {
                    WinApplication.Messaging.ShowException(ex.ToString());
                }
            }

            static void HandleGenericException(WebView2 ctrl, Exception ex)
            {
                Tracing.LogError(new Guid("0985D675-A93B-48B7-AC88-E3622DD86DAC"), ex);
                if (!ctrl.IsDisposed)
                {
                    WinApplication.Messaging.ShowException(ex.ToString());
                }
            }
        }
    }
}
