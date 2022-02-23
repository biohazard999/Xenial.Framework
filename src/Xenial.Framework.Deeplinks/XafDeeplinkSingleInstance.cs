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
}
