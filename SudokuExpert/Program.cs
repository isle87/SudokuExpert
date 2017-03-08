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
            s.LoadSudokuCSV("test-002.csv");
            s.ConsoleGrid();
            Console.WriteLine();
            s.SimpleSolve();
            s.ConsoleGrid();
        }

        public static int getIndex(int c, int r)
        {
            return (r - 1) * 9 + c - 1;
        }
    }
}
