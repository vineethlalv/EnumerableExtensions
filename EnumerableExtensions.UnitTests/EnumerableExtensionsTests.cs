using System;
using System.Collections.Generic;
using EnumeratorExtensions;
using NUnit.Framework;


namespace EnumerableExtensions.UnitTests
{
    public static class TestDataProvider
    {

        static List<string> strList;
        public static List<string> StrList
        {
            get
            {
                if (strList == null)
                    strList = new List<string>(new string[] { "blindworm", "spheroid", "haimish", "foible", "avant-garde", "pleonasm", "sweven", "proceleusmatic",
                                                              "grubstake", "confabulate", "perdure", "zeugma", "frumious", "polyglot", "darg", "rueful", "lunisolar",
                                                              "oeillade", "uitwaaien", "encased" });

                return strList;
            }
        }
    }



    [TestFixture]
    public class EnumerableExtensionsTests
    {
        [Test]
        public void Select_ReturnEnumerable_CompareEqual()
        {
            char[] seq1 = System.Linq.Enumerable.ToArray(TestDataProvider.StrList.Select(x => x[0]));
            char[] seq2 = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Select(TestDataProvider.StrList, x => x[0]));

            bool equal = seq1.Length == seq2.Length;
            if(equal)
            {
                for(int i = 0; i < seq1.Length; i++)
                {
                    if(seq1[i] != seq2[i])
                    {
                        equal = false;
                        break;
                    }
                }
            }


            Assert.True(equal, "Select differs from LINQ Select");
        }
    }
}
