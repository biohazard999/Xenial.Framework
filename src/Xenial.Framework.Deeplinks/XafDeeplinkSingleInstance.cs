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
    public XafDeeplinkSingleInstance(XafApplication application) : base(application.ApplicationName)
    {
    }
}
