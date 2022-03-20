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

#if FULL_FRAMEWORK || NETSTANDARD
    public static bool TryPop<T>(this Stack<T> stack, out T? value)
    {
        if (stack.Count > 0)
        {
            value = stack.Pop();
            return true;
        }

        value = default;
        return false;
    }
#endif
}
