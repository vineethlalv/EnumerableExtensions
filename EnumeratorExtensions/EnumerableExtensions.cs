using System;
using System.Collections.Generic;

namespace EnumeratorExtensions
{
    public static class EnumerableExtensions
    {
        public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return OrderBy<TSource, TKey>(source, keySelector, null);
        }
        public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            return new OrderedEnumerable<TSource, TKey>(source, keySelector, false, comparer);
        }

        public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return OrderByDescending<TSource, TKey>(source, keySelector, null); ;
        }
        public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            return new OrderedEnumerable<TSource, TKey>(source, keySelector, true, comparer);
        }

        public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return ThenBy(source, keySelector, null);
        }
        public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            if (source == null)
                throw new ArgumentNullException("source can't be null");

            source.AppendSortLevel(new EnumerableSorted<TSource, TKey>(keySelector, false, comparer));
            return source;
        }

        public static IOrderedEnumerable<TSource> ThenByDecending<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return ThenByDecending(source, keySelector, null);
        }
        public static IOrderedEnumerable<TSource> ThenByDecending<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            if (source == null)
                throw new ArgumentNullException("source can't be null");

            source.AppendSortLevel(new EnumerableSorted<TSource, TKey>(keySelector, true, comparer));
            return source;
        }
    }
}