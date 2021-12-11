using System;

using Xenial.Framework.Model.GeneratorUpdaters;

namespace DevExpress.ExpressApp.Model.Core;

/// <summary>
/// Class ModelNodesGeneratorUpdatersExtentions.
/// </summary>
public static partial class ModelNodesGeneratorUpdaterExtentions
{
    /// <summary>   Uses the app options. </summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null. </exception>
    ///
    /// <param name="updaters"> The updaters. </param>
    /// <param name="options">  The options. </param>
    ///
    /// <returns>   ModelNodesGeneratorUpdaters. </returns>

    public static ModelNodesGeneratorUpdaters UseAppOptions(this ModelNodesGeneratorUpdaters updaters, AppOptions options)
    {
        _ = updaters ?? throw new ArgumentNullException(nameof(updaters));
        _ = options ?? throw new ArgumentNullException(nameof(options));
        updaters.Add(new ModelAppOptionsNodesGeneratorUpdater(options));
        return updaters;
    }

    /// <summary>   Uses the app options. </summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null. </exception>
    ///
    /// <param name="updaters"> The updaters. </param>
    /// <param name="options">  The options. </param>
    ///
    /// <returns>   ModelNodesGeneratorUpdaters. </returns>

    public static ModelNodesGeneratorUpdaters UseAppOptions(this ModelNodesGeneratorUpdaters updaters, Func<AppOptions, AppOptions> options)
    {
        _ = options ?? throw new ArgumentNullException(nameof(options));
        return updaters.UseAppOptions(options(new()));
    }
}
