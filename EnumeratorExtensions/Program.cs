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
            List<string> list = new List<string>(new string[] { "Six666", "O", "Tw", "Thr", "Five5", "Seven77", "Four", "22"});

            foreach(string s in list.OrderByDescending(x => x[0]).ThenByDecending(x => x.Length))
            {
                System.Console.WriteLine(s);
            }

            System.Console.ReadLine();
        }
    }
}
