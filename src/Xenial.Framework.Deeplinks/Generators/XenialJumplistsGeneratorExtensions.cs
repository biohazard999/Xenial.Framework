using System;

using Xenial.Framework.Deeplinks.Generators;

namespace DevExpress.ExpressApp.Model.Core;

/// <summary>
///
/// </summary>
public static class XenialJumplistsGeneratorExtensions
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="updaters"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static ModelNodesGeneratorUpdaters UseXenialJumplists(
        this ModelNodesGeneratorUpdaters updaters!!,
        ModelJumplistOptions options!!)
    {
        //Add the protocols before the options so we can have a default protocol
        updaters.Add(new ModelJumplistOptionsGeneratorUpdaters(options));
        updaters.Add(new ModelJumplistCustomCategoriesGeneratorUpdater(options));
        updaters.Add(new ModelJumplistTasksCategoryGeneratorUpdater(options));

        return updaters;
    }
}
