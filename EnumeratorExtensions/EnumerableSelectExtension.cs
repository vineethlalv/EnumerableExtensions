using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnumeratorExtensions
{
    internal class EnumerableSelect<TSource, TResult> : IEnumerable<TResult>
    {
        IEnumerable<TSource> source;
        Func<int, TSource, TResult> selector;

        public EnumerableSelect(IEnumerable<TSource> source, Func<int, TSource, TResult> selector)
        {
            if (source == null)
                throw new ArgumentNullException("'source' can't be null");
            if (selector == null)
                throw new ArgumentNullException("'selector' can't be null");

            this.source = source;
            this.selector = selector;
        }

        public IEnumerator<TResult> GetEnumerator()
        {
            return new Enumerator(this);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class Enumerator : IEnumerator<TResult>
        {
            //@todo: review operation in multi-threaded scenario
            EnumerableSelect<TSource, TResult> parent;
            IEnumerator<TSource> enumerator;            
            int index;

            public TResult Current
            {
                get
                {
                    if (index == -1)
                        throw new InvalidOperationException();

                    return parent.selector(index, enumerator.Current);
                }
            }
            object IEnumerator.Current => Current;


            public Enumerator(EnumerableSelect<TSource, TResult> parent)
            {
                this.parent = parent;
                enumerator = parent.source.GetEnumerator();
                index = -1;
            }

            public void Dispose()
            {
                enumerator.Dispose();
                //@todo: GC optimization
            }

            public bool MoveNext()
            {
                bool ret = enumerator.MoveNext();
                index = ret ? index + 1 : -1;

                return ret;
            }

            public void Reset()
            {
                enumerator.Reset();
                index = -1;
            }
        }
    }

    internal class EnumerableSelectMany<TSource, TResult> : IEnumerable<TResult>
    {
        IEnumerable<TSource> source;
        Func<int, TSource, IEnumerable<TResult>> selector;

        public EnumerableSelectMany(IEnumerable<TSource> source, Func<int, TSource, IEnumerable<TResult>> selector)
        {
            if (source == null)
                throw new ArgumentNullException("'source' can't be null");
            if (selector == null)
                throw new ArgumentNullException("'selector' can't be null");

            this.source = source;
            this.selector = selector;
        }

        public IEnumerator<TResult> GetEnumerator()
        {
            return new Enumerator(this);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class Enumerator : IEnumerator<TResult>
        {
            //@todo: multi-threaded scenario
            EnumerableSelectMany<TSource, TResult> parent;
            IEnumerator<TSource> enumerator;
            IEnumerator<TResult> subEnumerator;
            int index;

            public TResult Current
            {
                get
                {
                    if (index == -1)
                        throw new InvalidOperationException();

                    return subEnumerator.Current;
                }
            }
            object IEnumerator.Current => Current;


            public Enumerator(EnumerableSelectMany<TSource, TResult> parent)
            {
                this.parent = parent;
                enumerator = parent.source.GetEnumerator();
                index = -1;

                subEnumerator = Enumerable.Empty<TResult>().GetEnumerator(); //@todo: re-implement LINQ utility class
            }

            public void Dispose()
            {
                enumerator.Dispose();
                subEnumerator.Dispose();
                //@todo: GC optimization
            }

            public bool MoveNext()
            {
                ++index;
                while (!subEnumerator.MoveNext())
                {
                    if (!enumerator.MoveNext())
                        return false;

                    subEnumerator = parent.selector(index, enumerator.Current).GetEnumerator();
                }
                return true;
            }

            public void Reset()
            {
                enumerator.Reset();
                index = -1;
            }
        }
    }

    internal class EnumerableZip<TSource1, TSource2, TResult> : IEnumerable<TResult>
    {
        IEnumerable<TSource1> source1;
        IEnumerable<TSource2> source2;
        Func<TSource1, TSource2, TResult> selector;

        public EnumerableZip(IEnumerable<TSource1> source1, IEnumerable<TSource2> source2, Func<TSource1, TSource2, TResult> selector)
        {
            if (source1 == null)
                throw new ArgumentNullException("'source1' can't be null");
            if (source2 == null)
                throw new ArgumentNullException("'source2' can't be null");
            if (selector == null)
                throw new ArgumentNullException("'selector' can't be null");

            this.source1 = source1;
            this.source2 = source2;
            this.selector = selector;
        }

        public IEnumerator<TResult> GetEnumerator()
        {
            return new Enumerator(this);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class Enumerator : IEnumerator<TResult>
        {
            //@todo: multi-threaded scenario
            EnumerableZip<TSource1, TSource2, TResult> parent;
            IEnumerator<TSource1> enumerator1;
            IEnumerator<TSource2> enumerator2;
            bool valid;

            public Enumerator(EnumerableZip<TSource1, TSource2, TResult> parent)
            {
                this.parent = parent;
                this.enumerator1 = parent.source1.GetEnumerator();
                this.enumerator2 = parent.source2.GetEnumerator();
                valid = false;
            }

            public TResult Current
            {
                get
                {
                    if (!valid)
                        throw new InvalidOperationException();

                    return parent.selector(enumerator1.Current, enumerator2.Current);
                }
            }
            object IEnumerator.Current => Current;

            public void Dispose()
            {
                enumerator1.Dispose();
                enumerator2.Dispose();
                //@todo GC optimization
            }

            public bool MoveNext()
            {
                return valid = enumerator1.MoveNext() && enumerator2.MoveNext();
            }

            public void Reset()
            {
                enumerator1.Reset();
                enumerator2.Reset();
                valid = true;
            }
        }
    }

    internal class EnumerableWhere<TSource> : IEnumerable<TSource>
    {
        IEnumerable<TSource> source;
        Func<TSource, bool> predicate; 

        public EnumerableWhere(IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            this.source = source;
            this.predicate = predicate;
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
            EnumerableWhere<TSource> parent;
            IEnumerator<TSource> enumerator;
            TSource current;
            bool valid;

            public Enumerator(EnumerableWhere<TSource> parent)
            {
                this.parent = parent;
                this.enumerator = parent.source.GetEnumerator();
                valid = false;
            }

            public TSource Current
            {
                get
                {
                    if (!valid)
                        throw new InvalidOperationException();

                    return current;
                }
            }
            object IEnumerator.Current => Current;

            public void Dispose()
            {
                enumerator.Dispose();
                //@todo: GC optimization
            }

            public bool MoveNext()
            {
                valid = false;
                while (enumerator.MoveNext())
                {
                    if (valid = parent.predicate(current = enumerator.Current))
                        break;
                }

                return valid;
            }

            public void Reset()
            {
                enumerator.Reset();
                valid = false;
            }
        }
    }
}
