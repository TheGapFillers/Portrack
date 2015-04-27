using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheGapFillers.Tools
{
    public static class Tools
    {
        public static IEnumerable<T> GetDescendants<T, R>(this IEnumerable<T> source, Func<T, R> recursion) where R : IEnumerable<T>
        {
            return source.SelectMany(x => (recursion(x) != null && recursion(x).Any()) ? recursion(x).GetDescendants(recursion) : null)
                         .Where(x => x != null);
        }

        public static IEnumerable<T> GetLeaves<T, R>(this IEnumerable<T> sources, Func<T, R> recursion) where R : IEnumerable<T>
        {
            foreach (T source in sources)
            {
                if (recursion(source) != null && recursion(source).Any())
                {
                    recursion(source).GetLeaves(recursion);
                }
                else
                {
                    yield return source;
                }
            }
        }
    }
}
