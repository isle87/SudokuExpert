using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuExpert
{
    /// <summary>
    /// A class to draw a <see cref="Grid"/> on the console.
    /// </summary>
    public class GridConsole
    {
        /// <summary>
        /// The <see cref="Grid"/>
        /// </summary>
        public Grid Grid { get; set; }

        /// <summary>
        /// Creates a <see cref="GridConsole"/>
        /// </summary>
        /// <param name="grid">The <see cref="Grid"/></param>
        public GridConsole(Grid grid)
        {
            Grid = grid;
        }

        /// <summary>
        /// Draw the Sudoku puzzle on the console.
        /// </summary>
        public void Draw()
        { //TODO should be out sourced
            for (int i = 0; i < Grid.Count(); i++)
            {
                Console.Write(" {0} ", Grid.Cells[i].Value == 0 ? "-" : Grid.Cells[i].Value.ToString());
                if ((i + 1) % 3 == 0)
                    Console.Write("|");
                if ((i + 1) % 9 == 0)
                {
                    Console.WriteLine();
                }
                if ((i + 1) % 27 == 0)
                    Console.WriteLine(new string('-', 30));
            }
        }

        /// <summary>
        /// Higlights a specific text.
        /// </summary>
        /// <param name="text">The text.</param>
        private void TextHighlight(string text)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.Write(text);
            Console.ResetColor();
        }
    }
}
