using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp;

using Xenial.Framework.Deeplinks.Model;

namespace DevExpress.ExpressApp.Model;

internal static class XenialDeeplinksModelInterfaceExtenders
{
    public static ModelInterfaceExtenders UseXenialDeeplinks(this ModelInterfaceExtenders extenders)
    {
        _ = extenders ?? throw new ArgumentNullException(nameof(extenders));

        extenders.Add<IModelOptions, IModelDeeplinkProtocols>();

        return extenders;
    }
}
