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
            Grid g = new Grid();
            g.GetItem(5, 1).Value = 1;
            g.GetItem(5, 2).Value = 2;
            g.GetItem(3, 4).Value = 3;
            g.GetItem(4, 4).Value = 4;
            g.GetItem(4, 5).Value = 5;
            g.GetItem(4, 6).Value = 6;
            g.GetItem(5, 7).Value = 7;
            g.GetItem(7, 4).Value = 8;
            Solver s = new Solver(g);
            s.NackedSingle(g.GetItem(5, 4));
        }

        public static int GetIndex(int c, int r)
        {
            return (r - 1) * 9 + c - 1;
        }
    }
}
