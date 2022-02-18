using System;
using System.Linq;
using System.Collections.Generic;

namespace EnumerableExtensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.Shuffle(new Random().Next);
        }
        public static IEnumerable<T> Shuffle<T, TResult>(this IEnumerable<T> source, Func<TResult> rng)
        {
            return source.OrderBy(a => rng());
        }
        public static IEnumerable<T> Shuffle<T, TResult>(this IEnumerable<T> source, Func<T, TResult> rng)
        {
            return source.OrderBy(a => rng(a));
        }
    }
}