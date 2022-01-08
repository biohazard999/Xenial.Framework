using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xenial.Framework.Generators.XAF.Utils;

/// <summary>
/// ShortURL: Bijective conversion between natural numbers(IDs) and short strings
///
/// ShortURL.Encode() takes an ID and turns it into a short string
/// ShortURL.Decode() takes a short string and turns it into an ID
///
/// Features:
/// + large alphabet(51 chars) and thus very short resulting strings
/// + proof against offensive words(removed 'a', 'e', 'i', 'o' and 'u')
/// + unambiguous(removed 'I', 'l', '1', 'O' and '0')
/// 
/// Example output:
/// 123456789 == pgK8p
/// </summary>
public static class ShortNameHelper
{
    private const string alphabet = "123456789abcdefghijklmnopqrstuvwxyz-_";
    private static readonly int @base = alphabet.Length;

    /// <summary>
    /// Encodes the specified number.
    /// </summary>
    /// <param name="num">The number.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">The number to shorten must be > 0. Given number '{num}'.</exception>
    public static string Encode(int num)
    {
        if (num <= 0)
        {
            throw new ArgumentException($"The number to shorten must be > 0. Given number '{num}'.");
        }

        var sb = new StringBuilder();
        while (num > 0)
        {
            sb.Insert(0, alphabet.ElementAt(num % @base));
            num = num / @base;
        }
        return sb.ToString();
    }

    /// <summary>
    /// Decodes the specified string.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">The string to decode must not be null, empty or whitespace. Given value '{str}'</exception>
    public static int Decode(string str)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            throw new ArgumentException($"The string to decode must not be null, empty or whitespace. Given value '{str}'");
        }

        var num = 0;
        for (var i = 0; i < str.Length; i++)
        {
            num = num * @base + alphabet.IndexOf(str.ElementAt(i));
        }
        return num;
    }
}
