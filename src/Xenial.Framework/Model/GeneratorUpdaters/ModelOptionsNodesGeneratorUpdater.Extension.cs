﻿using System;

using Xenial.Framework.Model.GeneratorUpdaters;

namespace DevExpress.ExpressApp.Model.Core;

/// <summary>
/// Class ModelNodesGeneratorUpdatersExtentions.
/// </summary>
public static partial class ModelNodesGeneratorUpdaterExtentions
{
    /// <summary>   Uses the application options. </summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null. </exception>
    ///
    /// <param name="updaters"> The updaters. </param>
    /// <param name="options">  The options. </param>
    ///
    /// <returns>   ModelNodesGeneratorUpdaters. </returns>

    public static ModelNodesGeneratorUpdaters UseApplicationOptions(this ModelNodesGeneratorUpdaters updaters, ApplicationOptions options)
    {
        _ = updaters ?? throw new ArgumentNullException(nameof(updaters));
        _ = options ?? throw new ArgumentNullException(nameof(options));
        updaters.Add(new ModelOptionsNodesGeneratorUpdater(options));
        return updaters;
    }

    /// <summary>   Uses the application options. </summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null. </exception>
    ///
    /// <param name="updaters"> The updaters. </param>
    /// <param name="options">  The options. </param>
    ///
    /// <returns>   ModelNodesGeneratorUpdaters. </returns>

    public static ModelNodesGeneratorUpdaters UseApplicationOptions(this ModelNodesGeneratorUpdaters updaters, Func<ApplicationOptions, ApplicationOptions> options)
    {
        _ = options ?? throw new ArgumentNullException(nameof(options));
        return updaters.UseApplicationOptions(options(new()));
    }
}
