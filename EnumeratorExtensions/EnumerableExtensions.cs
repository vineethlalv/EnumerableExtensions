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


        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            Func<TSource, TResult> selectorCopy = selector;
            Func<int, TSource, TResult> selectorWithIndex = (int i, TSource s) => { return selectorCopy(s); } ; //@todo: research implications

            return new EnumerableSelect<TSource, TResult>(source, selectorWithIndex);
        }
        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<int, TSource, TResult> selector)
        {
            return new EnumerableSelect<TSource, TResult>(source, selector);
        }

        public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            Func<TSource, IEnumerable<TResult>> selectorCopy = selector;
            Func<int, TSource, IEnumerable<TResult>> selectorWithIndex = (int i, TSource s) => { return selectorCopy(s); }; //@todo: research implications

            return new EnumerableSelectMany<TSource, TResult>(source, selectorWithIndex);
        }
        public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<int, TSource, IEnumerable<TResult>> selector)
        {
            return new EnumerableSelectMany<TSource, TResult>(source, selector);
        }

        public static IEnumerable<TResult> Zip<TSource1, TSource2, TResult>(this IEnumerable<TSource1> source, IEnumerable<TSource2> source2, Func<TSource1, TSource2, TResult> selector)
        {
            return new EnumerableZip<TSource1, TSource2, TResult>(source, source2, selector);
        }

        public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return new EnumerableWhere<TSource>(source, predicate);
        }
    }
}