using System;
using System.Collections.Generic;
using System.Linq;

namespace TheGapFillers.Tools
{
    public static class Tools
    {
        public static IEnumerable<T> GetDescendants<T, TR>(this IEnumerable<T> source, Func<T, TR> recursion) where TR : IEnumerable<T>
        {
            return source.SelectMany(x => (recursion(x) != null && recursion(x).Any()) ? recursion(x).GetDescendants(recursion) : null)
                         .Where(x => x != null);
        }

        public static IEnumerable<T> GetLeaves<T, TR>(this IEnumerable<T> sources, Func<T, TR> recursion) 
            where TR : IEnumerable<T>
        {
            foreach (T source in sources)
            {
                if (recursion(source) != null && recursion(source).Any())
                {
                    foreach (var leaf in recursion(source).GetLeaves(recursion))
                    {
                        yield return leaf;
                    }
                }
                else
                {
                    yield return source;
                }
            }
        }
    }
}
