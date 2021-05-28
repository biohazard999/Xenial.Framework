﻿namespace Xenial.Framework.Utils.Slugger
{
    /// <summary>   Interface ISlugify. </summary>
    public interface ISlugify
    {
        /// <summary>   Generates a slug from the provided <paramref name="inputString"/> </summary>
        ///
        /// <param name="inputString">  The string to slugify. </param>
        ///
        /// <returns>   A slugified version of <paramref name="inputString"/> </returns>

        string GenerateSlug(string inputString);
    }
}
