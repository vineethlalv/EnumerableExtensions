using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EnumeratorExtensions
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> list = new List<string>(new string[] { "Six666", "O", "Tw", "Thr", "Five5", "Seven77", "Four", "22", null });

            foreach(string s in list.OrderByDescending(x => x).ThenByDecending(x => String.IsNullOrEmpty(x) ? 0 : x.Length))
            {
                System.Console.WriteLine(s);
            }
            System.Console.WriteLine(string.Empty);

            foreach (int len in list.Select(str => String.IsNullOrEmpty(str) ? 0 : str.Length ))
            {
                System.Console.WriteLine(len);
            }
            System.Console.WriteLine(string.Empty);

            foreach (string len in list.Select((i, str) => (String.IsNullOrEmpty(str) ? "0" : str.Length.ToString()) + i.ToString()))
            {
                System.Console.WriteLine(len);
            }
            System.Console.WriteLine(string.Empty);

            foreach (char c in list.SelectMany((int i, string str) => (String.IsNullOrEmpty(str) ? System.Linq.Enumerable.Empty<char>() : str.ToCharArray())))
            {
                System.Console.WriteLine(c);
            }
            System.Console.WriteLine(string.Empty);

            List<string> zipList = new List<string>(new string[] { "+1", "+2", "+3", "+4", "+5"});
            foreach (string s in list.Zip(zipList, (a, b) => ((a ?? "") + (b ?? ""))))
            {
                System.Console.WriteLine(s);
            }
            System.Console.WriteLine(string.Empty);


            System.Console.ReadLine();
        }
    }
}
