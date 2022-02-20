﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DevExpress.ExpressApp;

using Xenial.Framework.Deeplinks.Model;

namespace DevExpress.ExpressApp.Model;

/// <summary>
/// 
/// </summary>
public static class XenialDeeplinksModelInterfaceExtenders
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="extenders"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static ModelInterfaceExtenders UseXenialDeeplinks(this ModelInterfaceExtenders extenders)
    {
        _ = extenders ?? throw new ArgumentNullException(nameof(extenders));

        extenders.Add<IModelOptions, IModelDeeplinkProtocols>();

        return extenders;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="regularTypes"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    //TODO: move to code generator and correct namespace
    public static IEnumerable<Type> UseXenialDeeplinksRegularTypes(this IEnumerable<Type> regularTypes)
    {
        _ = regularTypes ?? throw new ArgumentNullException(nameof(regularTypes));
        return regularTypes.Concat(new[]
       
            typeof(IModelDeeplinkProtocols),
            typeof(IModelDeeplinkProtocol),
            typeof(ModelDeeplinkProtocolLogic),
        });
    }
}
