using System;

namespace Xenial.Framework.Utils.Slugger;

/// <summary>   Class DefaultSlugifier. </summary>
public static class DefaultSlugifier
{
    private static SlugifierConfig defaultConfig = new();
    private static Slugifier @default = new(defaultConfig);

    /// <summary>   Gets or sets the default configuration. </summary>
    ///
    /// <exception cref="ArgumentNullException">    defaultConfig. </exception>
    ///
    /// <value> The default configuration. </value>

    public static SlugifierConfig DefaultConfig
    {
        get => defaultConfig;
        set
        {
            defaultConfig = value ?? throw new ArgumentNullException(nameof(DefaultConfig));
            Default = new Slugifier(value);
        }
    }

    /// <summary>   Gets or sets the default <see cref="Slugifier"/>. </summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
    ///                                             null. </exception>
    ///
    /// <value> The default. </value>

    public static Slugifier Default
    {
        get => @default;
        set => @default = value ?? throw new ArgumentNullException(nameof(Default));
    }

    /// <summary>   Slugifies the specified string. </summary>
    ///
    /// <param name="input">    The string. </param>
    ///
    /// <returns>   System.String. </returns>

    public static string Slugify(this string input)
        => Default.GenerateSlug(input);
}

