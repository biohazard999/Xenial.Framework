using System;

using Xenial.Framework.Win.Model.GeneratorUpdaters;

namespace DevExpress.ExpressApp.Model.Core;

/// <summary>
/// Class ModelNodesGeneratorUpdatersExtentions.
/// </summary>
public static partial class ModelNodesGeneratorUpdatersExtentions
{
    /// <summary>   Uses the application win options. </summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null. </exception>
    ///
    /// <param name="updaters"> The updaters. </param>
    /// <param name="options">  The options. </param>
    ///
    /// <returns>   ModelNodesGeneratorUpdaters. </returns>

    public static ModelNodesGeneratorUpdaters UseApplicationWinOptions(this ModelNodesGeneratorUpdaters updaters, ApplicationWinOptions options)
    {
        _ = updaters ?? throw new ArgumentNullException(nameof(updaters));
        _ = options ?? throw new ArgumentNullException(nameof(options));

        updaters.Add(new ModelOptionsWinNodesGeneratorUpdater(options));
        return updaters;
    }

    /// <summary>   Uses the application win options. </summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null. </exception>
    ///
    /// <param name="updaters"> The updaters. </param>
    /// <param name="options">  The options. </param>
    ///
    /// <returns>   ModelNodesGeneratorUpdaters. </returns>

    public static ModelNodesGeneratorUpdaters UseApplicationWinOptions(this ModelNodesGeneratorUpdaters updaters, Func<ApplicationWinOptions, ApplicationWinOptions> options)
    {
        _ = options ?? throw new ArgumentNullException(nameof(options));
        return updaters.UseApplicationWinOptions(options(new()));
    }
}
