using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnumeratorExtensions
{
    internal class SelectEnumerable<TSource, TResult> : IEnumerable<TResult>
    {
        IEnumerable<TSource> source;
        Func<TSource, TResult> selector;
        Func<int, TSource, TResult> indexedSelector;

        public SelectEnumerable(IEnumerable<TSource> source, Func<TSource, TResult> selector, Func<int, TSource, TResult> indexedSelector)
        {
            if (source == null)
                throw new ArgumentNullException("source can't be null");
            if (selector == null && indexedSelector == null)
                throw new ArgumentNullException("selector can't be null");

            this.source = source;
            this.selector = selector;
            this.indexedSelector = indexedSelector ?? DummyIndexedSelector;
        }

        private TResult DummyIndexedSelector(int i, TSource s)
        {
            return selector(s);
        }

        public IEnumerator<TResult> GetEnumerator()
        {
            return new SelectorEnumerator(source, indexedSelector);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class SelectorEnumerator : IEnumerator<TResult>
        {
            IEnumerator<TSource> sourceEnumerator;
            Func<int, TSource, TResult> selector;
            int index = -1;

            public TResult Current
            {
                get
                {
                    return selector(index, sourceEnumerator.Current);
                }
            }
            object IEnumerator.Current => Current;


            public SelectorEnumerator(IEnumerable<TSource> source, Func<int, TSource, TResult> selector)
            {
                sourceEnumerator = source.GetEnumerator();
                this.selector = selector;
            }

            public void Dispose()
            {
                sourceEnumerator.Dispose();
            }

            public bool MoveNext()
            {
                ++index;
                return sourceEnumerator.MoveNext();
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }
    }

    internal class SelectManyEnumerable<TSource, TResult> : IEnumerable<TResult>
    {
        IEnumerable<TSource> source;
        Func<TSource, IEnumerable<TResult>> selector;
        Func<int, TSource, IEnumerable<TResult>> indexedSelector;

        public SelectManyEnumerable(IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector, Func<int, TSource, IEnumerable<TResult>> indexedSelector)
        {
            if (source == null)
                throw new ArgumentNullException("source can't be null");
            if (selector == null && indexedSelector == null)
                throw new ArgumentNullException("selector can't be null");

            this.source = source;
            this.selector = selector;
            this.indexedSelector = indexedSelector ?? DummyIndexedSelector;
        }

        private IEnumerable<TResult> DummyIndexedSelector(int i, TSource s)
        {
            return selector(s);
        }

        public IEnumerator<TResult> GetEnumerator()
        {
            return new SelectManyEnumerator(source, indexedSelector);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class SelectManyEnumerator : IEnumerator<TResult>
        {
            IEnumerator<TSource> sourceEnumerator;
            IEnumerator<TResult> resultSubEnumerator;
            Func<int, TSource, IEnumerable<TResult>> selector;
            int index = -1;

            public TResult Current
            {
                get
                {
                    if (index == -1)
                        throw new InvalidOperationException();

                    return resultSubEnumerator.Current;
                }
            }
            object IEnumerator.Current => Current;


            public SelectManyEnumerator(IEnumerable<TSource> source, Func<int, TSource, IEnumerable<TResult>> selector)
            {
                sourceEnumerator = source.GetEnumerator();
                this.selector = selector;

                resultSubEnumerator = Enumerable.Empty<TResult>().GetEnumerator();
            }

            public void Dispose()
            {
                sourceEnumerator.Dispose();
            }

            public bool MoveNext()
            {
                ++index;
                while (!resultSubEnumerator.MoveNext())
                {
                    if (!sourceEnumerator.MoveNext())
                        return false;

                    resultSubEnumerator = selector(index, sourceEnumerator.Current).GetEnumerator();
                }
                return true;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }
    }
}
