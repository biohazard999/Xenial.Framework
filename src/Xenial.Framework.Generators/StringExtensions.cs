using System;
using System.Collections.Generic;
using System.Globalization;

namespace Xenial.Framework.Generators;

public static class StringExtensions
{
    public static string FirstCharToLowerCase(this string str)
    {
        if (string.IsNullOrEmpty(str) || char.IsLower(str[0]))
        {
            return str;
        }

        return char.ToLower(str[0], CultureInfo.CurrentUICulture) + str.Substring(1);
    }

    public static void Deconstruct<TK, TV>(this KeyValuePair<TK, TV> keyValuePair, out TK key, out TV value)
    {
        key = keyValuePair.Key;
        value = keyValuePair.Value;
    }
}
