using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public void OrderBy_ThenByResultSeq_EqualToLinq()
        {
            bool seqEqual = TestDataProvider.CompareSequence<string>(TestDataProvider.StrList.OrderBy(x => x[0]).ThenBy(x => x.Length),
                                                                     System.Linq.Enumerable.ThenBy(System.Linq.Enumerable.OrderBy(TestDataProvider.StrList, x => x[0]), x => x.Length),
                                                                     EqualityComparer<string>.Default);

            Assert.True(seqEqual);
        }
    }
}
