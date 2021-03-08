using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnumeratorExtensions
{
    public interface IOrderedEnumerable<TSource> : IEnumerable<TSource>
    {
        void AppendSortLevel(EnumerableSorted<TSource> next);
    }

    public abstract class EnumerableSorted<TSource>
    {
        public abstract void ComputeKeys(List<TSource> source);
        public abstract int Compare(int i1, int i2);
        public abstract void AddNext(EnumerableSorted<TSource> next);
    }

    internal class OrderedEnumerable<TSource, TKey>: IOrderedEnumerable<TSource>
    {
        IEnumerable<TSource> source;
        EnumerableSorted<TSource, TKey> sortParameters;
        EnumerableSorted<TSource> sortParametersTail;


        public OrderedEnumerable(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, bool decending, IComparer<TKey> comparer)
        {
            if (source == null)
                throw new ArgumentNullException("source can't be null");

            this.source = source;
            sortParameters = new EnumerableSorted<TSource, TKey>(keySelector, decending, comparer);
            sortParametersTail = sortParameters;
        }

        public IEnumerable<TSource> GetSource()
        {
            return source;
        }

        public void AppendSortLevel(EnumerableSorted<TSource> next)
        {
            sortParametersTail.AddNext(next);
            sortParametersTail = next;
        }

        public IEnumerator<TSource> GetEnumerator()
        {
            List<TSource> sourceList = new List<TSource>(source);
            sortParameters.ComputeKeys(sourceList);

            int[] orderMap = ComputeOrderMap(sourceList);

            return new OrderEnumerator(sourceList, orderMap);
        }

        int[] ComputeOrderMap(List<TSource> elements)
        {
            List<int> map = new List<int>(Enumerable.Range(0, elements.Count));
            map.Sort(sortParameters);
            return map.ToArray();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        class OrderEnumerator : IEnumerator<TSource>
        {
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

            public OrderEnumerator(List<TSource> list, int[] map)
            {
                if (list == null)
                    throw new ArgumentNullException("list can't be null");
                if (map == null)
                    throw new ArgumentNullException("ordermap can't be null");
                if (list.Count != map.Count())
                    throw new ArgumentException("non matching sequence lengths");


                currentIndex = -1;
                sourceList = list;
                orderMap = map;
            }
            public bool MoveNext()
            {
                return ++currentIndex < sourceList.Count;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
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
