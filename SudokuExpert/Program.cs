using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuExpert
{
    class Program
    {
        static void Main(string[] args)
        {
            Solver s = new Solver();
            for (byte i = 4; i < 10; i++)
                s.ItemGet(5, 4).DeletePossibleNumber(i);

            for (byte i = 4; i < 10; i++)
                s.ItemGet(4, 6).DeletePossibleNumber(i);

            for (byte i = 3; i < 10; i++) // Some special
                s.ItemGet(6, 6).DeletePossibleNumber(i);

            s.nackedSubsetTest();

            var testElements = s.Items.Where(i => i.Block == 5 && i != s.ItemGet(5, 4) && i != s.ItemGet(4, 6) && i != s.ItemGet(6, 6));
        }

        public static int getIndex(int c, int r)
        {
            return (r - 1) * 9 + c - 1;
        }
    }
}
