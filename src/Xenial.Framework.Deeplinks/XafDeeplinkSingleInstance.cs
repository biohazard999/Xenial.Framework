using System;
using System.Collections.Generic;
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
    /// <param name="application"></param>
    public XafDeeplinkSingleInstance(XafApplication application) : base(application?.ApplicationName!)
    {
        _ = application ?? throw new ArgumentNullException(nameof(application));
        if (string.IsNullOrEmpty(application.ApplicationName))
        {
            throw new ArgumentException($"{nameof(application)}.{nameof(application.ApplicationName)} must not be empty");
        }
    }
}
