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
    /// All strategies based on this website: (German)
    /// http://www.hib-wien.at/leute/wurban/mathematik/sudoku_strategie.pdf
    /// You can find a english version here:
    /// http://www.sadmansoftware.com/sudoku/blockcolumnrow.php
    /// </summary>
    /// <remarks>
    /// Defenitions:
    /// - Cell : Is just one tile of a Sudoku grid with just one value.
    /// - Line: A line is a horizontal or vertical line with exact 9 cells.
    /// - Column: Is a vertical line.
    /// - Row: Is a horizontal line.
    /// - Block: Is a 3 by 3 cell grid. A block cannot be a part of another block.
    /// - Section: Is a line or a block.
    /// </remarks>
    public class Solver
    {
        /// <summary>
        /// Contains all cells of one Sudoku puzzle. ( 9 by 9 )
        /// </summary>
        public Grid Grid;

        /// <summary>
        /// Constructor of Solver.
        /// </summary>
        public Solver(Grid grid)
        {
            Grid = grid;
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
            byte min = IsRow ? Grid.GetBlock(primaryBlock).Min(s => s.Row) : Grid.GetBlock(primaryBlock).Min(s => s.Column);

            List<List<Cell>> blockA = new List<List<Cell>>();
            List<List<Cell>> blockB = new List<List<Cell>>();

            for (byte i = min; i < min + 3; i++)
            {
                if (IsRow)
                {
                    blockA.Add(new List<Cell>(Grid.GetBlock(primaryBlock).Where(s => s.Row == i)));
                    blockB.Add(new List<Cell>(Grid.GetBlock(secondaryBlock).Where(s => s.Row == i)));
                }
                else
                {
                    blockA.Add(new List<Cell>(Grid.GetBlock(primaryBlock).Where(s => s.Column == i)));
                    blockB.Add(new List<Cell>(Grid.GetBlock(secondaryBlock).Where(s => s.Column == i)));
                }
                // Now we got a list who it is irrelevant if we use a column or a row.
            }

            for (byte number = 1; number < 10; number++)
            {
                try
                { // Search if just one line do not have got the number.
                    List<Cell> a = blockA.Single(s => !s.Any(e => e.ContainsPossibleNumber(number) || e.Value == number));
                    List<Cell> b = blockB.Single(s => !s.Any(e => e.ContainsPossibleNumber(number) || e.Value == number));
                    if (blockA.IndexOf(a) == blockB.IndexOf(b)) // checks if both blocks have got the same line where the number is missing.
                    {
                        for (byte i = min; i < min + 3; i++)
                        {
                            if (IsRow && i != a.ElementAt(0).Row)
                            { // delete the number in this line.
                                foreach (var item in Grid.GetRow(i).Where(s => !(s.Block == primaryBlock || s.Block == secondaryBlock)))
                                    item.RemovePossibleNumber(number);
                            }
                            if (!IsRow && i != a.ElementAt(0).Column)
                            {
                                foreach (var item in Grid.GetColumn(i).Where(s => !(s.Block == primaryBlock || s.Block == secondaryBlock)))
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

            var blockSection = Grid.GetBlock(block); // take the information of the block
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
                        foreach (var cell in Grid.GetColumn(columnRow).Where(i => i.Block != block)) // Yes? Then remove the number in the column, exept the block
                            cell.RemovePossibleNumber(number);
                    }
                    // Row
                    if (blockSection.Any(b => b.ContainsPossibleNumber(number) && b.Row == columnRow) && // Exist the number in the row?
                        !blockSection.Any(b => b.ContainsPossibleNumber(number) && b.Row != columnRow)) // and not in the others! (In the Block)
                    {
                        foreach (var cell in Grid.GetRow(columnRow).Where(i => i.Block != block)) // Yes? Then remove the possible number in the row, exept the block
                            cell.RemovePossibleNumber(number);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the cell from a specific column and row.
        /// </summary>
        /// <param name="column">The Column ( 1 - 9 )</param>
        /// <param name="row">The Row  ( 1 - 9 )</param>
        /// <returns></returns>
        public Cell GetItem(byte column, byte row)
        {
            return Grid.GetItem(column, row);
        }

        /// <summary>
        /// Execute the hidden subset strategie over the whole puzzle.
        /// </summary>
        public void HiddenSubset()
        {
            for (int section = 1; section < 10; section++) // Go through all blocks/rows/columns
            {
                HiddenSubset(Grid.GetBlock(section));
                HiddenSubset(Grid.GetRow(section));
                HiddenSubset(Grid.GetColumn(section));
            }
        }

        /// <summary>
        /// Execute the hidden subset strategie over a specific section. (Block, Column or Row)
        /// </summary>
        /// <param name="sectionCells">Block, Column or Row</param>
        public void HiddenSubset(IEnumerable<Cell> sectionCells)
        {
            if (!(sectionCells.All(i => i.Block == sectionCells.ElementAt(0).Block) 
                || sectionCells.All(i => i.Column == sectionCells.ElementAt(0).Column) || sectionCells.All(i => i.Row == sectionCells.ElementAt(0).Row)))
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
                                Grid.RemovePossibleNumber(cell, i);
                        }
                    }
                }
            }
        }



        /// <summary>
        /// Delete all numbers from the item.possibleNumbers list which are solved in the block,row and column.
        /// </summary>
        /// <param name="cell">The cell.</param>
        public void NackedSingle(Cell cell)
        {
            if (!Grid.Any(i => i == cell))
                throw new ArgumentException("The cell is not a part of the " + nameof(Solver), nameof(cell));

            foreach (var rowItem in Grid.GetRow(cell.Row, CellStatus.Solved))
                cell.RemovePossibleNumber(rowItem.Value);

            foreach (var colItem in Grid.GetColumn(cell.Column, CellStatus.Solved))
                cell.RemovePossibleNumber(colItem.Value);

            foreach (var bItem in Grid.GetBlock(cell.Block, CellStatus.Solved))
                cell.RemovePossibleNumber(bItem.Value);
        }

        /// <summary>
        /// Execute the nacked subset strategie on the specific cell.
        /// </summary>
        /// <param name="cell">The cell.</param>
        public void NackedSubset(Cell cell)
        {
            if (cell.IsSolved)
                return;
            if (!Grid.Any(i => i == cell))
                throw new ArgumentException("The cell is not a part of the " + nameof(Solver), nameof(cell));

            var bitems = Grid.GetBlock(cell.Block, CellStatus.Unsolved);
            NackedSubsetHelper(cell, bitems);

            bitems = Grid.GetRow(cell.Row, CellStatus.Unsolved);
            NackedSubsetHelper(cell, bitems);

            bitems = Grid.GetColumn(cell.Column, CellStatus.Unsolved);
            NackedSubsetHelper(cell, bitems);
        }

        /// <summary>
        /// Execute the nacked subset strategie over the whole puzzle.
        /// </summary>
        public void NackedSubset()
        {
            foreach (var item in Grid.Where(CellStatus.Unsolved))
                NackedSubset(item);
        }

        // TODO Rewrite this methods
        /// <summary>
        /// Solves the given <see cref="Grid"/>
        /// </summary>
        public void Solve()
        {
            bool count;
            Grid.ForEach(c => c.PossibleNumberChanged += 
            (s, e) =>
            {
                count = true;
            });

            do
            {
                count = false;
                SolveRotation();
                if (!Grid.Any(CellStatus.Unsolved))
                    break;
            } while (count);
        }

        /// <summary>
        /// One solve rotation. <seealso cref="Solve"/>
        /// </summary>
        public void SolveRotation()
        {
            int oldCount = 0;
            do
            {
                oldCount = Grid.Count(CellStatus.Unsolved);
                foreach (var item in Grid.Where(CellStatus.Unsolved))
                {
                    NackedSingle(item);
                    HiddenSingle(item);
                }

            } while (Grid.Count(CellStatus.Unsolved) != oldCount);

            NackedSubset();
            HiddenSubset();
            BlockLineInteraction();
            BlockBlockInteractions();
        }

        /// <summary>
        /// Checks if just one value is possible in the specific row, column or block.
        /// </summary>
        /// <param name="cell">The cell.</param>
        public void HiddenSingle(Cell cell)
        {
            if (!Grid.Any(i => i == cell))
                throw new ArgumentException("The cell is not a part of the " + nameof(Solver), nameof(cell));

            for (byte number = 1; number < 10; number++) // Run through all possible numbers
            {
                if (!cell.ContainsPossibleNumber(number))
                    continue;
                var neededCells = Grid.Where(i => i != cell && (i.Row == cell.Row || i.Column == cell.Column || i.Block == cell.Block));
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

        /// <summary>
        /// Help method for <see cref="NackedSubset(Cell)"/>
        /// </summary>
        /// <param name="cell">The <see cref="Cell"/></param>
        /// <param name="sectionCells"> The section cells/></param>
        private void NackedSubsetHelper(Cell cell, IEnumerable<Cell> sectionCells)
        {
            var possibleElements = sectionCells.Where(b => b.PossibleNumbers.Count <= cell.PossibleNumbers.Count);
            if (possibleElements.Count() != cell.PossibleNumbers.Count || possibleElements.Count() > 7 || possibleElements.Count() == sectionCells.Count())
                return;

            foreach (var element in possibleElements)
            {
                if (element == cell)
                    continue;

                int MissCount = 0; //Counts if something doesn't fit
                foreach (var number in cell.PossibleNumbers)
                {
                    if (!(element.ContainsPossibleNumber(number)))
                        MissCount++;
                    if (MissCount + element.PossibleNumbers.Count() > cell.PossibleNumbers.Count)
                        return;
                }
            }
            // All elements accomplish the conditions

            // Delete the elements from bItems. To find out where we have to delete the possible Numbers
            sectionCells = sectionCells.Where(b => !possibleElements.Contains(b));

            // Delete the possible number
            foreach (var b in sectionCells)
                foreach (var pN in cell.PossibleNumbers)
                    b.RemovePossibleNumber(pN);
        }
    }
}
