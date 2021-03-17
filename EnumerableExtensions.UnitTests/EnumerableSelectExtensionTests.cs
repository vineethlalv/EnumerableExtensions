using System;
using System.Collections;
using System.Collections.Generic;
using EnumeratorExtensions;
using NUnit.Framework;


namespace EnumerableExtensions.UnitTests
{
    [TestFixture]
    class EnumerableSelectExtensionTests
    {
        [Test]
        public void Select_ResultSeq_EqualToLinq()
        {
            bool seqEqual = TestDataProvider.CompareSequence<int>(TestDataProvider.IntList.Select(x => x * 10),
                                                                  System.Linq.Enumerable.Select(TestDataProvider.IntList, x => x * 10),
                                                                  EqualityComparer<int>.Default);

            Assert.True(seqEqual);
        }

        [Test]
        public void SelectMany_ResultSeq_EqualToLinq()
        {
            bool seqEqual = TestDataProvider.CompareSequence<char>(TestDataProvider.StrList.SelectMany(x => x),
                                                                   System.Linq.Enumerable.SelectMany(TestDataProvider.StrList, x => x),
                                                                   EqualityComparer<char>.Default);

            Assert.True(seqEqual);
        }

        [Test]
        public void Where_ResultSeq_EqualToLinq()
        {
            bool seqEqual = TestDataProvider.CompareSequence<int>(TestDataProvider.IntList.Where(x => x > 3),
                                                                   System.Linq.Enumerable.Where(TestDataProvider.IntList, x => x > 3),
                                                                   EqualityComparer<int>.Default);

            Assert.True(seqEqual);
        }

        [Test]
        public void Zip_ResultSeq_EqualToLinq()
        {
            bool seqEqual = TestDataProvider.CompareSequence<string>(TestDataProvider.IntList.Zip(TestDataProvider.StrList, (i, s) => i.ToString() + s),
                                                                     System.Linq.Enumerable.Zip(TestDataProvider.IntList, TestDataProvider.StrList, (i, s) => i.ToString() + s),
                                                                     EqualityComparer<string>.Default);

            Assert.True(seqEqual);
        }
    }
}
