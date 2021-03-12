using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; //@todo: re-implement utilities used


namespace EnumeratorExtensions
{
    public interface IOrderedEnumerable<TSource> : IEnumerable<TSource>
    {
        IEnumerator<TSource> CreateOrderedEnumerable(IEnumerableSorted<TSource> nextOrderLevel);
    }

    public interface IEnumerableSorted<TSource> : IComparer<int>
    {
        void ComputeKeys(List<TSource> source);
        void SetNext(IEnumerableSorted<TSource> next);
    }

    internal class EnumerableOrderBy<TSource, TKey>: IOrderedEnumerable<TSource>
    {
        IEnumerable<TSource> source;
        EnumerableSorted<TSource, TKey> orderRule;


        public EnumerableOrderBy(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, bool decending, IComparer<TKey> comparer)
        {
            if (source == null)
                throw new ArgumentNullException("'source' can't be null");

            this.source = source;
            orderRule = new EnumerableSorted<TSource, TKey>(keySelector, decending, comparer, this);
        }

        public IEnumerator<TSource> CreateOrderedEnumerable(IEnumerableSorted<TSource> nextSortLevel)
        {
            orderRule.SetNext(nextSortLevel);
            return new Enumerator(this);
        }

        public IEnumerator<TSource> GetEnumerator()
        {
            orderRule.SetNext(null);
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
                orderHead.orderRule.ComputeKeys(sourceList);
                orderMap = GetOrderedIndexArray(orderHead.orderRule);
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

    internal class EnumerableSorted<TSource, TKey> : IOrderedEnumerable<TSource>, IEnumerableSorted<TSource>
    {
        Func<TSource, TKey> keySelector;
        IComparer<TKey> comparer;
        bool decending;

        IOrderedEnumerable<TSource> parent;
        IEnumerableSorted<TSource> next;

        TKey[] keys;

        public EnumerableSorted(Func<TSource, TKey> keySelector, bool decending, IComparer<TKey> comparer, IOrderedEnumerable<TSource> parent)
        {
            if (keySelector == null)
                throw new ArgumentNullException("'keySelector' can't be null");

            this.keySelector = keySelector;
            this.decending = decending;
            this.comparer = comparer ?? Comparer<TKey>.Default;
            this.parent = parent;
        }

        public void SetNext(IEnumerableSorted<TSource> next)
        {
            this.next = next;
        }

        public void ComputeKeys(List<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException("'source' can't be null");

            keys = new TKey[source.Count];
            for(int i = 0; i < source.Count; i++)
                keys[i] = keySelector(source[i]);

            if(next != null)
                next.ComputeKeys(source);
        }

        public int Compare(int i1, int i2)
        {
            int order = comparer.Compare(keys[i1], keys[i2]);
            if (order == 0)
            {
                if (next != null)
                    return next.Compare(i1, i2);

                return i2 - i1;
            }
            return decending ? -order : order;
        }

        public IEnumerator<TSource> CreateOrderedEnumerable(IEnumerableSorted<TSource> next)
        {
            if(parent == null)
                throw new InvalidOperationException("");

            this.SetNext(next);
            return parent.CreateOrderedEnumerable(this);
        }

        public IEnumerator<TSource> GetEnumerator()
        {
            if (parent == null)
                throw new InvalidOperationException("");

            this.SetNext(null);
            return parent.CreateOrderedEnumerable(this);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
