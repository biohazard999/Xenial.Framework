using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;

namespace Xenial.Framework.Deeplinks;

/// <summary>
/// 
/// </summary>
public sealed class XenialDeeplinksModule : XenialModuleBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected override IEnumerable<Type> GetDeclaredControllerTypes()
        => base.GetDeclaredControllerTypes()
            .UseXenialDeeplinksControllerTypes();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected override IEnumerable<Type> GetRegularTypes()
        => base.GetRegularTypes()
            .UseXenialDeeplinksRegularTypes();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="extenders"></param>
    public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
    {
        base.ExtendModelInterfaces(extenders);

        extenders
            .UseXenialDeeplinks()
            .UseXenialJumplists();
    }
}
