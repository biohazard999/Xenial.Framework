using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using DevExpress.ExpressApp;

namespace Xenial.Framework.Deeplinks;

/// <summary>
/// 
/// </summary>
public sealed class XafDeeplinkSingleInstance : DeeplinkSingleInstance
{
    /// <summary>
    /// 
    /// </summary>
    public XafApplication Application { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    public XafDeeplinkSingleInstance(XafApplication application) : this(application, application?.ApplicationName!)
    {
        _ = application ?? throw new ArgumentNullException(nameof(application));
        if (string.IsNullOrEmpty(application.ApplicationName))
        {
            throw new ArgumentException($"{nameof(application)}.{nameof(application.ApplicationName)} must not be empty");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="identifier"></param>
    public XafDeeplinkSingleInstance(XafApplication application, string identifier)
        : base(identifier)
    {
        _ = application ?? throw new ArgumentNullException(nameof(application));
        Application = application;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="arguments"></param>
    protected override void OnArgumentsReceived(string[] arguments)
    {
        base.OnArgumentsReceived(arguments);
        if (!Debugger.IsAttached)
        {
            Debugger.Launch();
        }
        Debugger.Break();
    }

    //public static void InstanceOnArgumentsReceived(object sender, ArgumentsReceivedEventArgs argumentsReceivedEventArgs)
    //{
    //    if (TaskbarApplication == null || !(TaskbarApplication.MainWindow is WinWindow))
    //        return;

    //    var window = TaskbarApplication.MainWindow as WinWindow;

    //    var arguments = argumentsReceivedEventArgs.Args;

    //    if (arguments.Length > 0)
    //    {
    //        window.Form.SafeInvoke(() =>
    //        {
    //            var sc = CreateViewShortcutFromArguments(arguments[0]);
    //            ShowViewFromShortcut(sc);
    //        });
    //    }
    //}

    //public static void ShowViewFromShortcut(ViewShortcut sc)
    //{
    //    View shortCutView = TaskbarApplication.ProcessShortcut(sc);

    //    TaskbarApplication.ShowViewStrategy.ShowView(new ShowViewParameters(shortCutView), new ShowViewSource(null, null));

    //    var winWindow = (TaskbarApplication.MainWindow as WinWindow);
    //    if (winWindow == null)
    //        return;

    //    if (winWindow.Form.WindowState == System.Windows.Forms.FormWindowState.Minimized)
    //        winWindow.Form.WindowState = System.Windows.Forms.FormWindowState.Normal;

    //    if (!winWindow.Form.TopMost)
    //    {
    //        winWindow.Form.TopMost = true;
    //        winWindow.Form.TopMost = false;
    //    }

    //    winWindow.Form.BringToFront();
    //    winWindow.Form.Focus();
    //}

    //public static ViewShortcut CreateViewShortcutFromArguments(string argument)
    //{
    //    var a2 = CleanUriProtocols(argument);

    //    a2 = CleanNavigationItemJumplistArgumentName(TaskbarApplication, a2);

    //    var sc = ViewShortcut.FromString(a2);
    //    return sc;
    //}

    //private static string CleanUriProtocols(string argument)
    //{
    //    if (argument.Length > 0)
    //    {
    //        if (TaskbarApplication != null && TaskbarApplication.Model.Options is IModelCustomProtocolOptions)
    //        {
    //            var options = TaskbarApplication.Model.Options as IModelCustomProtocolOptions;

    //            if (options.CustomProtocolOptions.EnableProtocols)
    //            {
    //                if (argument.StartsWith(options.CustomProtocolOptions.ProtocolHandler, StringComparison.InvariantCultureIgnoreCase))
    //                {
    //                    argument = argument.Substring(options.CustomProtocolOptions.ProtocolHandler.Length, argument.Length - options.CustomProtocolOptions.ProtocolHandler.Length);

    //                    if (argument.Length > 0)
    //                    {
    //                        if (argument.EndsWith("/"))
    //                        {
    //                            argument = argument.Substring(0, argument.Length - 1);
    //                        }
    //                    }

    //                    if (!argument.StartsWith("/", StringComparison.InvariantCultureIgnoreCase))
    //                    {
    //                        argument = "/" + argument;
    //                    }
    //                }
    //            }

    //        }

    //    }
    //    return argument;
    //}

    //private static string CleanNavigationItemJumplistArgumentName(XafApplication application, string argument)
    //{
    //    if (application == null)
    //        return argument;

    //    if (application.Model == null || !(application.Model.Options is IModelTaskbarOptions))
    //        return argument;

    //    var optionsTaskbar = application.Model.Options as IModelTaskbarOptions;

    //    var navigationArgument = optionsTaskbar.TaskbarJumplistOptions.NavigationItemJumplistArgumentName;

    //    if (!string.IsNullOrEmpty(navigationArgument))
    //    {
    //        if (!navigationArgument.StartsWith("/", StringComparison.InvariantCultureIgnoreCase))
    //            navigationArgument = "/" + navigationArgument;

    //        if (argument.Length > 0 && argument.StartsWith(navigationArgument))
    //            argument = argument.Substring(navigationArgument.Length, argument.Length - navigationArgument.Length);
    //    }

    //    return argument;
    //}
}
