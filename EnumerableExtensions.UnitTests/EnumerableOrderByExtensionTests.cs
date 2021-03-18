using System;
using System.Collections.Generic;
using EnumeratorExtensions;
using NUnit.Framework;

namespace EnumerableExtensions.UnitTests
{
    [TestFixture]
    public class EnumerableOrderByExtensionTests
    {
        [Test]
        public void OrderBy_ResultSeq_EqualToLinq()
        {
            bool seqEqual = TestDataProvider.CompareSequence<int>(TestDataProvider.IntList.OrderBy(x => x),
                                                                  System.Linq.Enumerable.OrderBy(TestDataProvider.IntList, x => x),
                                                                  EqualityComparer<int>.Default);

            Assert.True(seqEqual);
        }

        [Test]
        public void OrderBy_DescResultSeq_EqualToLinq()
        {
            bool seqEqual = TestDataProvider.CompareSequence<int>(TestDataProvider.IntList.OrderByDescending(x => x),
                                                                  System.Linq.Enumerable.OrderByDescending(TestDataProvider.IntList, x => x),
                                                                  EqualityComparer<int>.Default);

            Assert.True(seqEqual);
        }

        [Test]
        public void OrderBy_ThenByResultSeq_EqualToLinq()
        {
            bool seqEqual = TestDataProvider.CompareSequence<string>(TestDataProvider.StrList.OrderBy(x => x[0]).ThenBy(x => x.Length),
                                                                     System.Linq.Enumerable.ThenBy(System.Linq.Enumerable.OrderBy(TestDataProvider.StrList, x => x[0]), x => x.Length),
                                                                     EqualityComparer<string>.Default);

            Assert.True(seqEqual);
        }

        [Test]
        public void OrderBy_DescThenByResultSeq_EqualToLinq()
        {
            bool seqEqual = TestDataProvider.CompareSequence<string>(TestDataProvider.StrList.OrderBy(x => x[0]).ThenByDescending(x => x.Length),
                                                                     System.Linq.Enumerable.ThenByDescending(System.Linq.Enumerable.OrderBy(TestDataProvider.StrList, x => x[0]), x => x.Length),
                                                                     EqualityComparer<string>.Default);

            Assert.True(seqEqual);
        }

        [Test]
        public void OrderBy_MultiResultSeq_EqualToLinq()
        {
            bool seqEqual = TestDataProvider.CompareSequence<TestDataProvider.TestType>(TestDataProvider.StrList.Zip(TestDataProvider.IntList, (s, n) => new TestDataProvider.TestType() { str = s, i = n })
                                                                                                                .OrderBy(x => x.str[0])
                                                                                                                .ThenByDescending(x => x.str.Length)
                                                                                                                .ThenBy(x => x.i),
                                                                                        System.Linq.Enumerable.ThenBy(System.Linq.Enumerable.ThenByDescending(System.Linq.Enumerable.OrderBy(System.Linq.Enumerable.Zip(TestDataProvider.StrList, TestDataProvider.IntList, (s, n) => new TestDataProvider.TestType() { str = s, i = n }),
                                                                                                                                                                                             x => x.str[0]),
                                                                                                                                                              x => x.str.Length),
                                                                                                                      x => x.i),
                                                                                        new TestDataProvider.TestType());

            Assert.True(seqEqual);
        }
    }
}
