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
            XenialLicense.Register();

            Console.WriteLine("Hello World");
            _ = true;
        }
    }

    public class FeatureCenterWindowsFromsApplication : WinApplication
    {

    }
}
