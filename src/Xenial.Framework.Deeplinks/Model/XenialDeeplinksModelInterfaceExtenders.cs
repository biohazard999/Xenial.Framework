using System;
using System.Collections.Generic;
using System.Linq;

using Xenial.Framework.Deeplinks.Generators;
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

        extenders.Add<IModelOptions, IModelOptionsDeeplinkProtocols>();

        return extenders;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="extenders"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static ModelInterfaceExtenders UseXenialJumplists(this ModelInterfaceExtenders extenders)
    {
        _ = extenders ?? throw new ArgumentNullException(nameof(extenders));

        extenders.Add<IModelOptions, IModelOptionsJumplists>();

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
        {
            typeof(IModelOptionsDeeplinkProtocols),
            typeof(IModelDeeplinkProtocols),
            typeof(IModelDeeplinkProtocol),

            typeof(ModelDeeplinksProtocolLogic),
            typeof(ModelDeeplinkProtocolLogic),
            typeof(ModelDeeplinkProtocolsGenerator),

            //JumpLists
            typeof(IModelOptionsJumplists),
            typeof(IModelJumplistTaskCategory),
            typeof(IModelJumplists),
            typeof(IModelJumplistCustomCategory),
            typeof(IModelJumplistCustomCategories),

            //Jumplist Items
            typeof(IModelJumplistItem),
            typeof(IModelJumplistItemBase),

            typeof(IModelJumplistItemAction),
            typeof(ModelJumplistItemActionDomainLogic),

            typeof(IModelJumplistItemLaunch),

            typeof(IModelJumplistItemNavigationItem),
            typeof(ModelJumplistItemNavigationItemDomainLogic),

            typeof(IModelJumplistItemSeperator),

            typeof(IModelJumplistItemProtocol),
            typeof(ModelJumplistItemProtocolDomainLogic),

            typeof(IModelJumplistItemView),
            typeof(ModelJumplistItemViewDomainLogic),
        });
    }
}
