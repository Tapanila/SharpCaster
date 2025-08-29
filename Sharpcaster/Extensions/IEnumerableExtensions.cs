using System;
using System.Collections.Generic;
using System.Linq;

namespace Sharpcaster.Extensions
{
    public static class IEnumerableExtensions
    {
        public static string ToString<T>(this IEnumerable<T> l, string separator)
        {
            return "[" + String.Join(separator, l.Select(i => i?.ToString() ?? "null").ToArray()) + "]";
        }
    }
}