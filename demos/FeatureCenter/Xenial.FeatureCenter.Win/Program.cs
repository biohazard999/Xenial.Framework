using System;
using System.Windows.Forms;

using DevExpress.ExpressApp.Win;

namespace Xenial.FeatureCenter.Win
{
    internal static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            XenialLicense.Register();
            sw.Stop();
            var elapsed = $"{sw.Elapsed}";

            MessageBox.Show(elapsed);

            _ = true;
        }
    }

    public class FeatureCenterWindowsFromsApplication : WinApplication
    {

    }
}
