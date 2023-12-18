﻿using System;
using System.Linq;

using DevExpress.ExpressApp.DC;

using Xenial.Framework;
using Xenial.Framework.Base;

namespace DevExpress.ExpressApp.Updating;

/// <summary>   Class ModuleUpdaterExtentions. </summary>
public static class ModuleUpdaterExtentions
{
    /// <summary>   Ensures the singletons. </summary>
    ///
    /// <exception cref="ArgumentNullException">    objectSpace. </exception>
    ///
    /// <param name="objectSpace">  The object space. </param>

    public static void EnsureSingletons(this IObjectSpace objectSpace)
    {
        _ = objectSpace ?? throw new ArgumentNullException(nameof(objectSpace));

        foreach (var typeinfo in objectSpace.TypesInfo.PersistentTypes.Where(p => p.IsAttributeDefined<SingletonAttribute>(false)))
        {
            if (objectSpace.CanInstantiate(typeinfo.Type))
            {
                var singletonObject = objectSpace.GetSingleton(typeinfo.Type);
            }
        }
    }
}
