using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuExpert.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Grid g = new Grid();
            GridConsole gc = new GridConsole(g);
            g.LoadCSV("test-005.csv");
            gc.Draw();
            Solver s = new Solver(g);
            s.Solve();
            gc.Draw();
            
        }
    }
}
