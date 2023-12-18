using System;
using System.Collections.Generic;

namespace Xenial.Framework.Utils.Slugger;

/// <summary>   Used to configure the a <see cref="Slugifier"/> instance. </summary>
public class SlugifierConfig
{
    /// <summary>   Initializes a new instance of the <see cref="SlugifierConfig"/> class. </summary>
    public SlugifierConfig() => StringReplacements = new Dictionary<string, string>
        {
            { " ", "-" }
        };

    /// <summary>   Gets the string replacements. </summary>
    ///
    /// <value> The string replacements. </value>

    public IDictionary<string, string> StringReplacements { get; }

    /// <summary>   Gets or sets a value indicating whether [force lower case]. </summary>
    ///
    /// <value> <c>true</c> if [force lower case]; otherwise, <c>false</c>. </value>

    public bool ForceLowerCase { get; set; } = true;

    /// <summary>   Gets or sets a value indicating whether [collapse white space]. </summary>
    ///
    /// <value> <c>true</c> if [collapse white space]; otherwise, <c>false</c>. </value>

    public bool CollapseWhiteSpace { get; set; } = true;

    /// <summary>   Gets or sets the denied characters regex. </summary>
    ///
    /// <value> The denied characters regex. </value>

    public string DeniedCharactersRegex { get; set; } = @"[^a-zA-Z0-9\-\._]";

    /// <summary>   Gets or sets a value indicating whether [collapse dashes]. </summary>
    ///
    /// <value> <c>true</c> if [collapse dashes]; otherwise, <c>false</c>. </value>

    public bool CollapseDashes { get; set; } = true;

    /// <summary>   Gets or sets a value indicating whether [trim whitespace]. </summary>
    ///
    /// <value> <c>true</c> if [trim whitespace]; otherwise, <c>false</c>. </value>

    public bool TrimWhitespace { get; set; } = true;
}

