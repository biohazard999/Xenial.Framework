using System;

using Xenial.Framework.Model.GeneratorUpdaters;

namespace DevExpress.ExpressApp.Model.Core;

public static partial class ModelNodesGeneratorUpdatersExtentions
{
    /// <summary>   Uses the singleton navigation items. </summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null. </exception>
    ///
    /// <param name="modelNodesGeneratorUpdaters">  The model nodes generator updaters. </param>
    ///
    /// <returns>   ModelNodesGeneratorUpdaters. </returns>

    public static ModelNodesGeneratorUpdaters UseSingletonNavigationItems(this ModelNodesGeneratorUpdaters modelNodesGeneratorUpdaters)
    {
        _ = modelNodesGeneratorUpdaters ?? throw new ArgumentNullException(nameof(modelNodesGeneratorUpdaters));
        modelNodesGeneratorUpdaters.Add(new SingletonNavigationItemNodesUpdater());
        return modelNodesGeneratorUpdaters;
    }
}

