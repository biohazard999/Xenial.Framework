using System;

using Xenial.Framework.Model.GeneratorUpdaters;

namespace DevExpress.ExpressApp.Model.Core;

/// <summary>
/// Class ModelNodesGeneratorUpdatersExtentions.
/// </summary>
public static partial class ModelNodesGeneratorUpdaterExtentions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="updaters"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static ModelNodesGeneratorUpdaters UseXenialImages(this ModelNodesGeneratorUpdaters updaters)
    {
        _ = updaters ?? throw new ArgumentNullException(nameof(updaters));
        updaters.Add(new ModelImageSourceModelNodesGeneratorUpdater());
        return updaters;
    }
}
