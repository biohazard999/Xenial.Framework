using System;
using System.Collections.Generic;
using System.Text;

namespace System.Linq;

internal static class LinqExtensions
{
    public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source)
    {
        T previous = default!;
        var first = true;
        foreach (var element in source)
        {
            if (!first)
            {
                yield return previous;
            }
            previous = element;
            first = false;
        }
    }
}
