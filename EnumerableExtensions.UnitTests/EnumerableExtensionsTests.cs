using System;
using System.Collections;
using System.Collections.Generic;
using EnumeratorExtensions;
using NUnit.Framework;


namespace EnumerableExtensions.UnitTests
{
    public static class TestDataProvider
    {
        public class TestType : IComparer<TestType>, IEqualityComparer<TestType>
        {
            public string str;
            public int i;

            public int Compare(TestType x, TestType y)
            {
                if (x == null)
                {
                    if (y == null) return 0;
                    else return -1;
                }
                else if (y == null && x != null) return 1;                


                int order = Comparer<string>.Default.Compare(x.str, y.str);
                if(order == 0)
                {
                    return x.i - y.i;
                }

                return order;
            }

            public bool Equals(TestType x, TestType y)
            {
                return 0 == Compare(x, y);
            }

            public int GetHashCode(TestType obj)
            {
                if(obj == null) return 0;

                int hash = 0;
                if (obj.str != null)
                    hash += obj.str.GetHashCode();
                hash += obj.i;

                return hash;
            }
        }

        static List<string> strList;
        public static IEnumerable<string> StrList
        {
            get
            {
                if (strList == null)
                    strList = new List<string>(new string[] { "blindworm", "spheroid", "haimish", "foible", "avant-garde", "foldbl", "pleonasm", "sweven", "proceleusmatic",
                                                              "grubstake", "confabulate", "perdure", "zeugma", "frumious", "polyglot", "darg", "rueful", "lunisolar",
                                                              "oeillade", "uitwaaien", "encased" });

                return System.Linq.Enumerable.AsEnumerable(strList);
            }
        }

        static List<int> intList;
        public static IEnumerable<int> IntList
        {
            get
            {
                if (intList == null)
                    intList = new List<int>(new int[] { 1, 2, 5, 2, 5, 3, 4, 6, 7, 9, 8, 11, 10, 13, 12, 15, 14, 18, 16, 17 });

                return System.Linq.Enumerable.AsEnumerable(intList);
            }
        }


        public static bool ValidateIenumeratorInvalidCurrentStates(IEnumerator enumerator)
        {
            object o;

            try
            {
                o = enumerator.Current;
                return false;
            }
            catch { }

            while (enumerator.MoveNext()) ;
            try
            {
                o = enumerator.Current;
                return false;
            }
            catch { }

            return true;
        }

        public static bool CompareSequence<T>(IEnumerable<T> seq1, IEnumerable<T> seq2, IEqualityComparer<T> eqComparer)
        {
            T[] a1 = System.Linq.Enumerable.ToArray(seq1);
            T[] a2 = System.Linq.Enumerable.ToArray(seq2);

            bool ret = false;
            if (a1.Length == a2.Length)
            {
                ret = true;
                for (int i = 0; i < a1.Length; i++)
                {
                    System.Console.WriteLine(a1[i].ToString() + "  "+ a2[i].ToString());
                    if (!eqComparer.Equals(a1[i], a2[i]))
                    {
                        ret = false;
                        break;
                    }
                }
            }

            return ret;
        }
    }



    [TestFixture]
    public class EnumerableExtensionsTests
    {
        [Test]
        public void Select_InvalidCurrent_Exception()
        {
            IEnumerator seq = TestDataProvider.StrList.Select(x => x[0]).GetEnumerator();

            Assert.True(TestDataProvider.ValidateIenumeratorInvalidCurrentStates(seq));
        }

        [Test]
        public void SelectMany_InvalidCurrent_Exception()
        {
            IEnumerator seq = TestDataProvider.StrList.SelectMany(x => x).GetEnumerator();

            Assert.True(TestDataProvider.ValidateIenumeratorInvalidCurrentStates(seq));
        }

        [Test]
        public void Zip_InvalidCurrent_Exception()
        {
            IEnumerator seq = TestDataProvider.StrList.Zip(TestDataProvider.IntList, (s, i) => i).GetEnumerator();

            Assert.True(TestDataProvider.ValidateIenumeratorInvalidCurrentStates(seq));
        }

        [Test]
        public void Where_InvalidCurrent_Exception()
        {
            IEnumerator seq = TestDataProvider.IntList.Where(x => x > 3).GetEnumerator();

            Assert.True(TestDataProvider.ValidateIenumeratorInvalidCurrentStates(seq));
        }

        [Test]
        public void OrderBy_InvalidCurrent_Exception()
        {
            IEnumerator seq = TestDataProvider.IntList.OrderBy(x => x).GetEnumerator();

            Assert.True(TestDataProvider.ValidateIenumeratorInvalidCurrentStates(seq));
        }

        [Test]
        public void OrderByThenBy_InvalidCurrent_Exception()
        {
            IEnumerator seq = TestDataProvider.StrList.OrderBy(x => x[0]).ThenBy(x => x.Length).GetEnumerator();

            Assert.True(TestDataProvider.ValidateIenumeratorInvalidCurrentStates(seq));
        }
    }
}
