using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; //@todo: re-implement utilities used
using System.Text;
using System.Threading.Tasks;

namespace EnumeratorExtensions
{
    public interface IOrderedEnumerable<TSource> : IEnumerable<TSource>
    {
        void AppendOrderCriteria(EnumerableSorted<TSource> next);
    }

    public abstract class EnumerableSorted<TSource>
    {
        public abstract void ComputeKeys(List<TSource> source);
        public abstract int Compare(int i1, int i2);
        public abstract void AddNext(EnumerableSorted<TSource> next);
    }

    internal class EnumerableOrderBy<TSource, TKey>: IOrderedEnumerable<TSource>
    {
        //@todo: optimization - skip computekey if source didn't changed ??
        IEnumerable<TSource> source;
        EnumerableSorted<TSource, TKey> orderCriteria;
        EnumerableSorted<TSource> orderCriteriaTail;


        public EnumerableOrderBy(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, bool decending, IComparer<TKey> comparer)
        {
            if (source == null)
                throw new ArgumentNullException("'source' can't be null");

            this.source = source;
            orderCriteria = new EnumerableSorted<TSource, TKey>(keySelector, decending, comparer);
            orderCriteriaTail = orderCriteria;
        }

        public IEnumerable<TSource> GetSource()
        {
            return source;
        }

        public void AppendOrderCriteria(EnumerableSorted<TSource> next)
        {
            orderCriteriaTail.AddNext(next);
            orderCriteriaTail = next;
        }

        public IEnumerator<TSource> GetEnumerator()
        {
            return new Enumerator(this);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class Enumerator : IEnumerator<TSource>
        {
            //@todo: multi-threaded scenario
            List<TSource> sourceList;
            int[] orderMap;
            int currentIndex;

            public TSource Current  {
                get
                {
                    if (currentIndex == -1)
                        throw new InvalidOperationException();

                    return sourceList[orderMap[currentIndex]];
                }
            }
            object IEnumerator.Current => Current;

            public Enumerator(EnumerableOrderBy<TSource, TKey> orderHead)
            {
                sourceList = GetSourceAsList(orderHead.source);
                orderHead.orderCriteria.ComputeKeys(sourceList);
                orderMap = GetOrderedIndexArray(orderHead.orderCriteria);
                currentIndex = -1;
            }

            private List<TSource> GetSourceAsList(IEnumerable<TSource> source)
            {
                //@todo: re-implement as lightweight collection
                return new List<TSource>(source);
            }
            private int[] GetOrderedIndexArray(EnumerableSorted<TSource, TKey> orderCriteriaHead)
            {
                //@todo: re-implement List<> as lightweight, make Sort() a utility method
                //@todo: write own utilities for Enumerable.Range remove reliance to Linq utilities
                List<int> map = new List<int>(Enumerable.Range(0, sourceList.Count));
                map.Sort(orderCriteriaHead);
                return map.ToArray();
            }

            public bool MoveNext()
            {
                return ++currentIndex < sourceList.Count;
            }

            public void Reset()
            {
                currentIndex = -1;
            }

            public void Dispose()
            {
                //@todo: GC optimization ?
            }
        }
    }

    internal class EnumerableSorted<TSource, TKey> : EnumerableSorted<TSource>, IComparer<int>
    {
        Func<TSource, TKey> keySelector;
        bool decending;
        IComparer<TKey> comparer;
        EnumerableSorted<TSource> next;

        TKey[] keys;

        public EnumerableSorted(Func<TSource, TKey> keySelector, bool decending, IComparer<TKey> comparer)
        {
            if (keySelector == null)
                throw new ArgumentNullException("keySelector can't be null");

            this.keySelector = keySelector;
            this.decending = decending;
            this.comparer = comparer ?? Comparer<TKey>.Default;
            next = null;
        }

        public override void AddNext(EnumerableSorted<TSource> next)
        {
            this.next = next;
        }

        public override void ComputeKeys(List<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException("source can't be null");

            keys = new TKey[source.Count];
            for(int i = 0; i < source.Count; i++)
                keys[i] = keySelector(source[i]);

            if (next != null)
                next.ComputeKeys(source);
        }

        public override int Compare(int i1, int i2)
        {
            int order = comparer.Compare(keys[i1], keys[i2]);
            if(order == 0)
            {
                if(next != null)
                    return next.Compare(i1, i2);

                return i2 - i1;
            }
            return decending ? -order : order;
        }
    }
}
