using System;

using Xenial.Framework.Model.GeneratorUpdaters;

namespace Xenial.Framework.Win.Model.GeneratorUpdaters;

/// <summary>
/// Class NavigationWinOptions. Implements the
/// <see cref="Xenial.Framework.Model.GeneratorUpdaters.NavigationOptions" />
/// </summary>
///
/// <seealso cref="Xenial.Framework.Model.GeneratorUpdaters.ApplicationOptions"/>

public record NavigationWinOptions : NavigationOptions
{
    /// <summary>   Gets or sets the navigation caption. </summary>
    ///
    /// <value> The navigation caption. </value>

    public string? NavigationCaption { get; set; }
}
