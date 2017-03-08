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
        public IEnumerable<SudokuItem> UnsolvedItems = new List<SudokuItem>();

        private void create()
        {
            for (int i = 0; i <= 80; i++)
            {
                Items.Add(new SudokuItem(0, (byte)i));
            }
        }

        public SudokuItem ItemGet(byte Column, byte Row)
        {
            return Items.ElementAt((Row - 1) * 9 + Column - 1);
        }


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

        private void hiddenSingle(SudokuItem item)
        {
            for(byte number= 1; number <10; number++) // Run through all possible numbers
            {
                if (!item.ContainsPossibleNumber(number))
                    continue;
                var neededCells = Items.Where(i => i != item && (i.Row == item.Row || i.Column == item.Column || i.Block == item.Block));
                if (item.IsSolved)
                    return;
                if (neededCells.Any(i => i.Row == item.Row && i.Value == number)) // Check if some value is equal to the searched value(number)
                {
                    item.DeletePossibleNumber(number);
                    continue;
                }
                if (neededCells.Any(i => i.Column == item.Column && i.Value == number)) // Check if some value is equal to the searched value(number)
                {
                    item.DeletePossibleNumber(number);
                    continue;
                }
                if (neededCells.Any(i => i.Block == item.Block && i.Value == number)) // Check if some value is equal to the searched value(number)
                {
                    item.DeletePossibleNumber(number);
                    continue;
                }

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

            var bitems = UnsolvedItems.Where(b => b.Block == item.Block && b.Value == 0);
            nackedSubsetHelper(item, bitems);

            bitems = UnsolvedItems.Where(b => b.Row == item.Row && b.Value == 0);
            nackedSubsetHelper(item, bitems);

            bitems = UnsolvedItems.Where(b => b.Column == item.Column && b.Value == 0);
            nackedSubsetHelper(item, bitems);
        }

        private void nackedSubsetHelper(SudokuItem item, IEnumerable<SudokuItem> bItems)
        {
            var possibleElements = bItems.Where(b => b.PossibleNumbers.Count <= item.PossibleNumbers.Count);
            if (possibleElements.Count() != item.PossibleNumbers.Count || possibleElements.Count() > 7 || possibleElements.Count() == bItems.Count())
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
            bItems = bItems.Where(b => !possibleElements.Contains(b));

            // Delete the possible number
            foreach (var b in bItems)
                foreach (var pN in item.PossibleNumbers)
                    b.DeletePossibleNumber(pN);
        }

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

        private void ConsoleHighlight(string text)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.Write(text);
            Console.ResetColor();
        }

        // TODO: HiddenSubset

        #region TestRegion
#if DEBUG
        public void SimpleSolve()
        {
            while (true)
            {
                SimpleSolveRotation();
                if (!UnsolvedItems.Any())
                    break;
            }
        }

        public void SimpleSolveRotation()
        {
            int oldCount = 0;
            UnsolvedItems = Items.Where(i => i.Value == 0);
            do
            {
                oldCount = UnsolvedItems.Count();
                foreach (var item in UnsolvedItems)
                    nackedSingle(item);
                UnsolvedItems = UnsolvedItems.Where(i => i.Value == 0);
            } while (UnsolvedItems.Count() != oldCount);

            foreach (var item in UnsolvedItems)
                hiddenSingle(item);
            UnsolvedItems = UnsolvedItems.Where(i => i.Value == 0);
            if (oldCount != UnsolvedItems.Count())
                return;

            foreach (var item in UnsolvedItems)
                nackedSubset(item);
            UnsolvedItems = Items.Where(i => i.Value == 0);
            ConsoleGrid();
        }

        public void NackedSingle(SudokuItem item)
        {
            nackedSingle(item);
        }

        public void nackedSubsetTest()
        {
            UnsolvedItems = Items.Where(i => i.Value == 0);

            foreach (var item in UnsolvedItems)
                nackedSubset(item);
        }
#endif
        #endregion
    }
}
