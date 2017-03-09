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
            var block = s.Items.Where(b => b.Block == 1);
            foreach (var item in block)
            {
                if (item == s.GetItem(2, 1) || item == s.GetItem(3, 3))
                    continue;
                item.RemovePossibleNumber(1);
                item.RemovePossibleNumber(4);
                if (new Random().Next(0,1) == 1)
                    item.RemovePossibleNumber((byte)(new Random().Next(5, 9)));
                if (new Random().Next(0, 2) == 1)
                    item.RemovePossibleNumber((byte)(new Random().Next(5, 9)));
            }

            s.HiddenSubsetTest();
        }

        public static int GetIndex(int c, int r)
        {
            return (r - 1) * 9 + c - 1;
        }
    }
}
