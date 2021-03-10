using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EnumeratorExtensions
{
    class Program
    {
        static List<string> list = new List<string>(new string[] { "Six666", "O", "Tw", "Thr", "Five5", "Seven77", "Four", "22", null });

        static void Main(string[] args)
        {
            OrderByTest();
            System.Console.WriteLine(string.Empty);

            SelectTest();
            System.Console.WriteLine(string.Empty);

            SelectIndexTest();
            System.Console.WriteLine(string.Empty);

            SelectManyTest();
            System.Console.WriteLine(string.Empty);

            ZipTest();
            System.Console.WriteLine(string.Empty);

            WhereTest();
            System.Console.WriteLine(string.Empty);


            System.Console.ReadLine();
        }

        static void OrderByTest()
        {
            foreach (string s in list.OrderByDescending(x => x).ThenByDecending(x => String.IsNullOrEmpty(x) ? 0 : x.Length))
            {
                System.Console.WriteLine(s);
            }
        }

        static void SelectTest()
        {
            foreach (int len in list.Select(str => String.IsNullOrEmpty(str) ? 0 : str.Length))
            {
                System.Console.WriteLine(len);
            }
        }

        static void SelectIndexTest()
        {
            foreach (string len in list.Select((i, str) => (String.IsNullOrEmpty(str) ? "0" : str.Length.ToString()) + i.ToString()))
            {
                System.Console.WriteLine(len);
            }
        }

        static void SelectManyTest()
        {
            foreach (char c in list.SelectMany((int i, string str) => (String.IsNullOrEmpty(str) ? System.Linq.Enumerable.Empty<char>() : str.ToCharArray())))
            {
                System.Console.WriteLine(c);
            }
        }

        static void ZipTest()
        {
            List<string> zipList = new List<string>(new string[] { "+1", "+2", "+3", "+4", "+5" });
            foreach (string s in list.Zip(zipList, (a, b) => ((a ?? "") + (b ?? ""))))
            {
                System.Console.WriteLine(s);
            }
        }

        static void WhereTest()
        {
            foreach (string s in list.Where(x => x != null ? x.Length > 3 : false))
            {
                System.Console.WriteLine(s);
            }
        }
    }
}
