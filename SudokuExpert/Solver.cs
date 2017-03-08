using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuExpert
{
    /// <summary>
    /// <see cref="http://www.hib-wien.at/leute/wurban/mathematik/sudoku_strategie.pdf"/>
    /// </summary>
    public class Solver
    {
        public Solver()
        {
            create();
        }

        public List<SudokuItem> Items = new List<SudokuItem>();
        private IEnumerable<SudokuItem> unsolvedItems = new List<SudokuItem>();

        /// <summary>
        /// Create all nedded (81) cells of the Sudoku field
        /// </summary>
        private void create()
        {
            for (int i = 0; i <= 80; i++)
                Items.Add(new SudokuItem(0, (byte)i));
        }

        /// <summary>
        /// Gets the cell from a specific column and row.
        /// </summary>
        /// <param name="Column"></param>
        /// <param name="Row"></param>
        /// <returns></returns>
        public SudokuItem GetItem(byte Column, byte Row)
        {
            return Items.ElementAt((Row - 1) * 9 + Column - 1);
        }

        /// <summary>
        /// Delete all numbers from the item.possibleNumbers list which are solved in the block,row and column.
        /// </summary>
        /// <param name="item"></param>
        private void nackedSingle(SudokuItem item)
        {
            var rowItems = Items.Where(i => i.Row == item.Row && i.Value != 0);
            foreach (var rowItem in rowItems)
                item.DeletePossibleNumber(rowItem.Value);

            var columnItems = Items.Where(i => i.Column == item.Column && i.Value != 0);
            foreach (var colItem in columnItems)
                item.DeletePossibleNumber(colItem.Value);

            var blockItems = Items.Where(i => i.Block == item.Block && i.Value != 0);
            foreach (var bItem in blockItems)
                item.DeletePossibleNumber(bItem.Value);
        }

        /// <summary>
        /// Checks if just one value is possible in the specific row, column or block.
        /// </summary>
        /// <param name="item"></param>
        private void hiddenSingle(SudokuItem item)
        {
            for(byte number= 1; number <10; number++) // Run through all possible numbers
            {
                if (!item.ContainsPossibleNumber(number))
                    continue;
                var neededCells = Items.Where(i => i != item && (i.Row == item.Row || i.Column == item.Column || i.Block == item.Block));
                if (item.IsSolved)
                    return;

                if (!neededCells.Any(i => i.Row == item.Row && !i.IsSolved && i.ContainsPossibleNumber(number)))
                {
                    item.Value = number;
                }
                if (!neededCells.Any(i => i.Column == item.Column && !i.IsSolved && i.ContainsPossibleNumber(number)))
                {
                    item.Value = number;
                }
                if (!neededCells.Any(i => i.Block == item.Block && !i.IsSolved && i.ContainsPossibleNumber(number)))
                {
                    item.Value = number;
                }
            }
        }

        private void nackedSubset(SudokuItem item)
        {
            if (item.IsSolved)
                return;

            var bitems = unsolvedItems.Where(b => b.Block == item.Block && b.Value == 0);
            nackedSubsetHelper(item, bitems);

            bitems = unsolvedItems.Where(b => b.Row == item.Row && b.Value == 0);
            nackedSubsetHelper(item, bitems);

            bitems = unsolvedItems.Where(b => b.Column == item.Column && b.Value == 0);
            nackedSubsetHelper(item, bitems);
        }

        private void nackedSubsetHelper(SudokuItem item, IEnumerable<SudokuItem> sectionItems)
        {
            var possibleElements = sectionItems.Where(b => b.PossibleNumbers.Count <= item.PossibleNumbers.Count);
            if (possibleElements.Count() != item.PossibleNumbers.Count || possibleElements.Count() > 7 || possibleElements.Count() == sectionItems.Count())
                return;

            foreach (var element in possibleElements)
            {
                if (element == item)
                    continue;

                int MissCount = 0; //Counts if something doesn't fit
                foreach (var number in item.PossibleNumbers)
                {
                    if (!(element.ContainsPossibleNumber(number)))
                        MissCount++;
                    if (MissCount + element.PossibleNumbers.Count() > item.PossibleNumbers.Count)
                        return;
                }
            }
            // All elements accomplish the conditions

            // Delete the elements from bItems. To find out where we have to delete the possible Numbers
            sectionItems = sectionItems.Where(b => !possibleElements.Contains(b));

            // Delete the possible number
            foreach (var b in sectionItems)
                foreach (var pN in item.PossibleNumbers)
                    b.DeletePossibleNumber(pN);
        }

        /// <summary>
        /// Loads a File with a sudoku puzzle.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="seperator"></param>
        public void LoadSudokuCSV(string filename, char seperator = ';')
        {
            if (!filename.EndsWith(".csv"))
                throw new Exception("This is not a CSV file!");

            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                StreamReader r = new StreamReader(fs);
                int i = -1;                                 
                while (r.Peek() != -1)
                {
                    string line = r.ReadLine();
                    for (int j = 0; j < line.Length; j++)
                    {
                        if (line[j] == seperator)
                        {
                            if (j == line.Length - 1)
                                i++;
                            else if (j + 1 < line.Length && line[j + 1] == seperator)
                                i += j == 0 ? 2 : 1;
                            else if (j == 0)
                                i++;
                            continue;
                        }
                        else
                            i++;

                        Items[i].Value = Convert.ToByte(char.GetNumericValue(line[j]));
                    }
                }
            }
        }

        /// <summary>
        /// Draw the sudoku puzzle on the console.
        /// </summary>
        public void ConsoleGrid()
        {
            for (int i = 0; i < Items.Count(); i++)
            {
                Console.Write(" {0} ", Items[i].Value == 0? "-" : Items[i].Value.ToString());
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
        /// <param name="text"></param>
        private void ConsoleHighlight(string text)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.Write(text);
            Console.ResetColor();
        }

        // TODO: Test this!
        private void hiddenSubset(IEnumerable<SudokuItem> sectionItems)
        {
            for (byte number = 1; number < 9; number++) // Go through all numbers
            {
                var contains = sectionItems.Where(i => i.ContainsPossibleNumber(number)); // save all cells who contains the number
                var containsNot = sectionItems.Where(i => !i.ContainsPossibleNumber(number)); // save all cells who contains not the number
                List<byte> pairList = new List<byte>(); // create a list to save the number who are a pair
                pairList.Add(number);

                for (byte pairNumber = number; pairNumber <= 9; pairNumber++) // go through the remaining number, to find a pair
                {
                    if (contains.All(c => c.ContainsPossibleNumber(pairNumber)) && !containsNot.Any(cn => cn.ContainsPossibleNumber(pairNumber)))
                        pairList.Add(pairNumber); 
                }

                if (pairList.Count == contains.Count()) // If the count of pairs matches the count of cellsm then we had to do the final stuff
                {
                    
                    foreach (var cell in contains) // Go through all cells (Who contains the pairs) and delete all numbers who are not equal to the pairs.
                    {
                        for (byte i = 1; i <= 9; i++)
                        {
                            if (!pairList.Exists(pl => pl == i))
                                cell.DeletePossibleNumber(i);
                        }
                    }
                }
            }
        }


        #region TestRegion
#if DEBUG
        public void SimpleSolve()
        {
            while (true)
            {
                SimpleSolveRotation();
                if (!unsolvedItems.Any())
                    break;
            }
        }

        public void SimpleSolveRotation()
        {
            int oldCount = 0;
            unsolvedItems = Items.Where(i => i.Value == 0);
            do
            {
                oldCount = unsolvedItems.Count();
                foreach (var item in unsolvedItems)
                {
                    nackedSingle(item);
                    hiddenSingle(item);
                }
                unsolvedItems = unsolvedItems.Where(i => i.Value == 0);
            } while (unsolvedItems.Count() != oldCount);

            foreach (var item in unsolvedItems)
                nackedSubset(item);
            ConsoleGrid();
        }

        public void NackedSingle(SudokuItem item)
        {
            nackedSingle(item);
        }

        public void nackedSubsetTest()
        {
            unsolvedItems = Items.Where(i => i.Value == 0);

            foreach (var item in unsolvedItems)
                nackedSubset(item);
        }

        public void hiddenSubsetTest(IEnumerable<SudokuItem> sectionItem) => hiddenSubset(sectionItem);
#endif
        #endregion
    }
}
