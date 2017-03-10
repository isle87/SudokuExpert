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
            s.GetItem(1, 7).Value = 1;
            s.GetItem(2, 7).Value = 2;
            s.GetItem(3, 6).Value = 9;
            s.GetItem(7, 7).Value = 3;
            s.GetItem(8, 7).Value = 4;
            s.GetItem(9, 4).Value = 9;

            /**
             * - - -| - - -| - - -| 
             * - - -| - - -| - - -|
             * - - -| - - -| - - -|
             * --------------------
             * - - -| - - -| - - 9|
             * - - -| - - -| - - -|
             * - - 9| - - -| - - -|
             * --------------------
             * 1 2 -| - - -| 3 4 -|
             * - - -| T T T| - - -|
             * - - -| T T T| - - -|
             * */
            s.Items.ForEach(i => s.NackedSingleTest(i));
            s.BlockBlockInteractionTest();
            s.ConsoleGrid();
        }

        public static int GetIndex(int c, int r)
        {
            return (r - 1) * 9 + c - 1;
        }
    }
}
