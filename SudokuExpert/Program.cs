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
                s.GetItem(5, 4).RemovePossibleNumber(i);

            for (byte i = 4; i < 10; i++)
                s.GetItem(4, 6).RemovePossibleNumber(i);

            for (byte i = 3; i < 10; i++) // Some special
                s.GetItem(6, 6).RemovePossibleNumber(i);

            /**
            * - - -| - - -| - - -| 
            * - - -| - - -| - - -|
            * - - -| - - -| - - -|
            * --------------------
            * - - -| - X -| - - -|
            * - - -| X - -| - - -|
            * - - -| - - X| - - -|
            * --------------------
            * - - -| - - -| - - -|
            * - - -| - - -| - - -|
            * - - -| - - -| - - -|
            * */

            s.NacketSubset();

            var testElements = s.Cells.Where(i => i.Block == 5 && i != s.GetItem(5, 4) && i != s.GetItem(4, 6) && i != s.GetItem(6, 6));
        }

        public static int GetIndex(int c, int r)
        {
            return (r - 1) * 9 + c - 1;
        }
    }
}
