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
        /// <summary>
        /// Contains all cells of one Sudoku puzzle. ( 9 by 9 )
        /// </summary>
        public List<SudokuCell> Cells = new List<SudokuCell>();

        private IEnumerable<SudokuCell> unsolvedItems = new List<SudokuCell>();

        /// <summary>
        /// Constructor of Solver.
        /// </summary>
        public Solver()
        {
            // Create all cells ( 9 by 9 )
            for (int i = 0; i <= 80; i++)
                Cells.Add(new SudokuCell(0, (byte)i));
        }

        /// <summary>
        /// Execute the Block-Block-Interaction strategie over the whole puzzle.
        /// </summary>
        /// <remarks>http://www.mathrec.org/sudoku/block.html</remarks>
        public void BlockBlockInteractions()
        {
            for (byte index = 1; index < 9; index++)
            {
                for (byte block = 1; block < 7; block++)
                {
                    if (block > 3) block = 6;
                    if (index + block > 9) continue;
                    BlockBlockInteractions(index, (byte)(index + block));
                }
            }
        }

        /// <summary>
        /// Execute the Block-Block-Interaction strategie over the two given blocks.
        /// </summary>
        /// <remarks>http://www.mathrec.org/sudoku/block.html</remarks>
        /// <param name="primaryBlock">The first Block number ( 1-9 )</param>
        /// <param name="secondaryBlock">The second Block number ( 1-9 )</param>
        public void BlockBlockInteractions(byte primaryBlock, byte secondaryBlock)
        {
            if (primaryBlock > 9 || primaryBlock == 0)
                throw new ArgumentOutOfRangeException(nameof(primaryBlock));
            if (secondaryBlock > 9 || secondaryBlock == 0)
                throw new ArgumentOutOfRangeException(nameof(secondaryBlock));

            bool IsRow = (secondaryBlock - primaryBlock) < 3;
            byte min = IsRow ? Cells.Where(s => s.Block == primaryBlock).Min(s => s.Row) : Cells.Where(s => s.Block == primaryBlock).Min(s => s.Column);

            List<List<SudokuCell>> blockA = new List<List<SudokuCell>>();
            List<List<SudokuCell>> blockB = new List<List<SudokuCell>>();

            for (byte i = min; i < min + 3; i++)
            {
                if (IsRow)
                {
                    blockA.Add(new List<SudokuCell>(Cells.Where(s => s.Row == i && s.Block == primaryBlock)));
                    blockB.Add(new List<SudokuCell>(Cells.Where(s => s.Row == i && s.Block == secondaryBlock)));
                }
                else
                {
                    blockA.Add(new List<SudokuCell>(Cells.Where(s => s.Column == i && s.Block == primaryBlock)));
                    blockB.Add(new List<SudokuCell>(Cells.Where(s => s.Column == i && s.Block == secondaryBlock)));
                }
                // Now we got a list who it is irrelevant if we use a column or a row.
            }

            for (byte number = 1; number < 10; number++)
            {
                try
                {
                    List<SudokuCell> a = blockA.Single(s => !s.Any(e => e.ContainsPossibleNumber(number) || e.Value == number));
                    List<SudokuCell> b = blockB.Single(s => !s.Any(e => e.ContainsPossibleNumber(number) || e.Value == number));
                    if (blockA.IndexOf(a) == blockB.IndexOf(b))
                    {
                        for (byte i = min; i < min + 3; i++)
                        {
                            if (IsRow && i != a.ElementAt(0).Row)
                            {
                                foreach (var item in Cells.Where(s => s.Row == i && !(s.Block == primaryBlock || s.Block == secondaryBlock)))
                                    item.RemovePossibleNumber(number);
                            }
                            if (!IsRow && i != a.ElementAt(0).Column)
                            {
                                foreach (var item in Cells.Where(s => s.Column == i && !(s.Block == primaryBlock || s.Block == secondaryBlock)))
                                    item.RemovePossibleNumber(number);
                            }
                        }
                    }
                }
                catch (InvalidOperationException)
                {
                    //No single found
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        /// <summary>
        /// Execute the Block-Line-Interaction strategie over the whole puzzle.
        /// </summary>
        public void BlockLineInteraction()
        {
            for (int block = 1; block < 10; block++) // Go through all blocks
            {
                BlockLineInteraction(block);
            }
        }

        /// <summary>
        /// Execute the Block-Line-Interaction strategie over the specific block.
        /// </summary>
        /// <param name="block">Block number ( 1 - 9 )</param>
        public void BlockLineInteraction(int block)
        {
            if (block > 9 || block == 0)
                throw new ArgumentOutOfRangeException(nameof(block));

            var blockSection = Cells.Where(i => i.Block == block); // take the information of the block
            for (int columnRow = 1; columnRow < 10; columnRow++) // Go throug all columns and rows
            {
                if (!blockSection.Any(b => b.Column == columnRow || b.Row == columnRow)) // check if the column/row is not a part of the block
                    continue;
                for (byte number = 1; number < 10; number++) // go through all possible numbers
                {
                    // Column
                    if (blockSection.Any(b => b.ContainsPossibleNumber(number) && b.Column == columnRow) && // Exist the number in the column?
                        !blockSection.Any(b => b.ContainsPossibleNumber(number) && b.Column != columnRow)) // and not in the others! (In the Block)
                    {
                        foreach (var cell in Cells.Where(i => i.Column == columnRow && i.Block != block)) // Yes? Then remove the number in the column, exept the block
                            cell.RemovePossibleNumber(number);
                    }
                    // Row
                    if (blockSection.Any(b => b.ContainsPossibleNumber(number) && b.Row == columnRow) && // Exist the number in the row?
                        !blockSection.Any(b => b.ContainsPossibleNumber(number) && b.Row != columnRow)) // and not in the others! (In the Block)
                    {
                        foreach (var cell in Cells.Where(i => i.Row == columnRow && i.Block != block)) // Yes? Then remove the possible number in the row, exept the block
                            cell.RemovePossibleNumber(number);
                    }
                }
            }
        }

        /// <summary>
        /// Draw the Sudoku puzzle on the console.
        /// </summary>
        public void ConsoleGrid()
        {
            for (int i = 0; i < Cells.Count(); i++)
            {
                Console.Write(" {0} ", Cells[i].Value == 0 ? "-" : Cells[i].Value.ToString());
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
        /// Gets the cell from a specific column and row.
        /// </summary>
        /// <param name="column">The Column ( 1 - 9 )</param>
        /// <param name="row">The Row  ( 1 - 9 )</param>
        /// <returns></returns>
        public SudokuCell GetItem(byte column, byte row)
        {
            if (column == 0 || column > 9) throw new ArgumentOutOfRangeException(nameof(column));
            if (row == 0 || column > 9) throw new ArgumentOutOfRangeException(nameof(row));
            return Cells.ElementAt((row - 1) * 9 + column - 1);
        }

        /// <summary>
        /// Execute the hidden subset strategie over the whole puzzle.
        /// </summary>
        public void HiddenSubset()
        {
            for (int block = 1; block < 10; block++) // Go through all blocks/rows/columns
            {
                HiddenSubset(Cells.Where(i => i.Block == block));
                HiddenSubset(Cells.Where(i => i.Row == block));
                HiddenSubset(Cells.Where(i => i.Column == block));
            }
        }

        /// <summary>
        /// Execute the hidden subset strategie over a specific section. (Block, Column or Row)
        /// </summary>
        /// <param name="sectionCells">Block, Column or Row</param>
        public void HiddenSubset(IEnumerable<SudokuCell> sectionCells)
        {
            if (!(sectionCells.All(i => i.Block == sectionCells.ElementAt(0).Block) || sectionCells.All(i => i.Column == sectionCells.ElementAt(0).Column) || sectionCells.All(i => i.Row == sectionCells.ElementAt(0).Row)))
                throw new ArgumentException("The " + nameof(sectionCells) + " are not in the right section", nameof(sectionCells));

            for (byte number = 1; number < 9; number++) // Go through all numbers
            {
                var contains = sectionCells.Where(i => i.ContainsPossibleNumber(number)); // save all cells who contains the number
                var containsNot = sectionCells.Where(i => !i.ContainsPossibleNumber(number)); // save all cells who contains not the number
                List<byte> pairList = new List<byte> { number }; // create a list to save the number who are a pair

                for (byte pairNumber = (byte)(number + 1); pairNumber <= 9; pairNumber++) // go through the remaining number, to find a pair
                {
                    if (contains.All(c => c.ContainsPossibleNumber(pairNumber)) && !containsNot.Any(cn => cn.ContainsPossibleNumber(pairNumber)))
                        pairList.Add(pairNumber);
                }

                // If the count of pairs matches the count of cells then we had to do the final stuff
                if (pairList.Count == contains.Count() && pairList.Count < 8 && pairList.Count > 1)
                {
                    foreach (var cell in contains) // Go through all cells (Who contains the pairs) and delete all numbers who are NOT equal to the pairs.
                    {
                        for (byte i = 1; i <= 9; i++)
                        {
                            if (!pairList.Exists(pl => pl == i))
                                cell.RemovePossibleNumber(i);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads a File with a sudoku puzzle.
        /// </summary>
        /// <param name="filename">The filename</param>
        /// <param name="seperator">The data separator</param>
        public void LoadSudokuCSV(string filename, char seperator = ';')
        { // TODO: Exeptions
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

                        Cells[i].Value = Convert.ToByte(char.GetNumericValue(line[j]));
                    }
                }
            }
        }

        /// <summary>
        /// Delete all numbers from the item.possibleNumbers list which are solved in the block,row and column.
        /// </summary>
        /// <param name="cell">The cell.</param>
        public void NackedSingle(SudokuCell cell)
        {
            if (!Cells.Any(i => i == cell))
                throw new ArgumentException("The cell is not a part of the " + nameof(Solver), nameof(cell));

            var rowItems = Cells.Where(i => i.Row == cell.Row && i.Value != 0);
            foreach (var rowItem in rowItems)
                cell.RemovePossibleNumber(rowItem.Value);

            var columnItems = Cells.Where(i => i.Column == cell.Column && i.Value != 0);
            foreach (var colItem in columnItems)
                cell.RemovePossibleNumber(colItem.Value);

            var blockItems = Cells.Where(i => i.Block == cell.Block && i.Value != 0);
            foreach (var bItem in blockItems)
                cell.RemovePossibleNumber(bItem.Value);
        }

        /// <summary>
        /// Execute the nacked subset strategie on the specific cell.
        /// </summary>
        /// <param name="cell">The cell.</param>
        public void NackedSubset(SudokuCell cell)
        {
            if (cell.IsSolved)
                return;
            if (!Cells.Any(i => i == cell))
                throw new ArgumentException("The cell is not a part of the " + nameof(Solver), nameof(cell));
            SearchForUnsolvedItems();
            var bitems = unsolvedItems.Where(b => b.Block == cell.Block && b.Value == 0);
            NackedSubsetHelper(cell, bitems);

            bitems = unsolvedItems.Where(b => b.Row == cell.Row && b.Value == 0);
            NackedSubsetHelper(cell, bitems);

            bitems = unsolvedItems.Where(b => b.Column == cell.Column && b.Value == 0);
            NackedSubsetHelper(cell, bitems);
        }

        /// <summary>
        /// Execute the nacked subset strategie over the whole puzzle.
        /// </summary>
        public void NacketSubset()
        {
            SearchForUnsolvedItems();
            foreach (var item in unsolvedItems)
                NackedSubset(item);
        }

        /// <summary>
        /// Search for unsolved cells. This method is useful to cast before some methods like <see cref="NackedSubset(SudokuCell)"/>
        /// </summary>
        public void SearchForUnsolvedItems()
        {
            unsolvedItems = Cells.Where(i => !i.IsSolved);
        }

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
            unsolvedItems = Cells.Where(i => i.Value == 0);
            do
            {
                oldCount = unsolvedItems.Count();
                foreach (var item in unsolvedItems)
                {
                    NackedSingle(item);
                    HiddenSingle(item);
                }

                unsolvedItems = unsolvedItems.Where(i => i.Value == 0);
            } while (unsolvedItems.Count() != oldCount);

            foreach (var item in unsolvedItems)
                NackedSubset(item);
            ConsoleGrid();
        }

        /// <summary>
        /// Higlights a specific text.
        /// </summary>
        /// <param name="text">The text.</param>
        private void ConsoleHighlight(string text)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.Write(text);
            Console.ResetColor();
        }

        /// <summary>
        /// Checks if just one value is possible in the specific row, column or block.
        /// </summary>
        /// <param name="cell">The cell.</param>
        public void HiddenSingle(SudokuCell cell)
        {
            if (!Cells.Any(i => i == cell))
                throw new ArgumentException("The cell is not a part of the " + nameof(Solver), nameof(cell));

            for (byte number = 1; number < 10; number++) // Run through all possible numbers
            {
                if (!cell.ContainsPossibleNumber(number))
                    continue;
                var neededCells = Cells.Where(i => i != cell && (i.Row == cell.Row || i.Column == cell.Column || i.Block == cell.Block));
                if (cell.IsSolved)
                    return;

                if (!neededCells.Any(i => i.Row == cell.Row && !i.IsSolved && i.ContainsPossibleNumber(number)))
                    cell.Value = number;
                if (!neededCells.Any(i => i.Column == cell.Column && !i.IsSolved && i.ContainsPossibleNumber(number)))
                    cell.Value = number;
                if (!neededCells.Any(i => i.Block == cell.Block && !i.IsSolved && i.ContainsPossibleNumber(number)))
                    cell.Value = number;
            }
        }
        private void NackedSubsetHelper(SudokuCell item, IEnumerable<SudokuCell> sectionItems)
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
                    b.RemovePossibleNumber(pN);
        }
    }
}
