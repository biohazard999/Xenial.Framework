using System;

using Xenial.Framework.Model.GeneratorUpdaters;

namespace DevExpress.ExpressApp.Model.Core;

public static partial class ModelNodesGeneratorUpdatersExtentions
{
    /// <summary>   Uses the no views generator updater. </summary>
    ///
    /// <exception cref="ArgumentNullException">    updaters. </exception>
    ///
    /// <param name="updaters"> The updaters. </param>
    ///
    /// <returns>   ModelNodesGeneratorUpdaters. </returns>

    public static ModelNodesGeneratorUpdaters UseNoViewsGeneratorUpdater(this ModelNodesGeneratorUpdaters updaters)
    {
        _ = updaters ?? throw new ArgumentNullException(nameof(updaters));

        updaters.Add(new ModelViewsGenerateNoViewsUpdater());

        return updaters;
    }
}
