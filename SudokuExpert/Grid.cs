using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuExpert
{
    /// <summary>
    /// Contains the status of a cell.
    /// </summary>
    public enum CellStatus
    {
        /// <summary>
        /// The cell value is solved.
        /// </summary>
        Solved,
        /// <summary>
        /// The cell value is not solved.
        /// </summary>
        Unsolved,
        /// <summary>
        /// Doesn't matter about the cell status.
        /// </summary>
        All
    }

     /// <summary>
     /// A Sudoku grid with 9 by 9 Sudoku cells.
     /// </summary>
    public class Grid
    {
        /// <summary>
        /// Initialize a new Sudoku grid.
        /// </summary>
        public Grid()
        {
            // Create all cells ( 9 by 9 )
            Cells = new List<Cell>();
            for (int i = 0; i <= 80; i++)
                Cells.Add(new Cell(0, (byte)i));
        }

        /// <summary>
        /// Get or set the cells.
        /// </summary>
        public List<Cell> Cells { get; set; }

        /// <summary>
        /// Determines if any elemement met the condition of a sequence.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public bool Any(Func<Cell, bool> predicate)
        {
            return Cells.Any(predicate);
        }

        /// <summary>
        /// Determines whether any element statisfies the given CellStatus.
        /// </summary>
        /// <param name="cellStatus">The <see cref="CellStatus"/></param>
        /// <returns></returns>
        public bool Any(CellStatus cellStatus = CellStatus.Solved)
        {
            return Cells.Any(GetCellStatusFunction(cellStatus));
        }

        /// <summary>
        /// Performes the specified action on each element on the <see cref="Grid"/>
        /// </summary>
        /// <param name="action">The <see cref="Action{Cell}"/> delegate to perform on each element on the <see cref="Grid"/></param>
        public void ForEach(Action<Cell> action)
        {
            foreach (var item in Cells)
                action(item);
        }

        /// <summary>
        /// Returns the count of elements in the <see cref="Grid"/>
        /// </summary>
        /// <param name="cellStatus">The cell status</param>
        /// <returns></returns>
        public int Count(CellStatus cellStatus = CellStatus.All)
        {
            return Where(cellStatus).Count();
        }

        /// <summary>
        /// Gets the cells from the specified block as <see cref="IEnumerable{Cell}"/>
        /// </summary>
        /// <param name="blockID">The block identification number.</param>
        /// <param name="cellStatus">The cell status</param>
        /// <returns>Returns a <see cref="IEnumerable{Cell}"/> of the specified block.</returns>
        public IEnumerable<Cell> GetBlock(int blockID, CellStatus cellStatus = CellStatus.All)
        {
            IsBetween1and9(blockID);
            Func<Cell, bool> func = GetCellStatusFunction(cellStatus);
            return Cells.Where(c => c.Block == blockID).Where(func);
        }

        /// <summary>
        /// Gets the cells from the specified column as <see cref="IEnumerable{Cell}"/>
        /// </summary>
        /// <param name="columnID">The column identification number.</param>
        /// <param name="cellStatus"></param>
        /// <returns></returns>
        public IEnumerable<Cell> GetColumn(int columnID, CellStatus cellStatus = CellStatus.All)
        {
            IsBetween1and9(columnID);
            Func<Cell, bool> func = GetCellStatusFunction(cellStatus);
            return Cells.Where(c => c.Column == columnID).Where(func);
        }

        /// <summary>
        /// Gets the <see cref="Cell"/> in the specified column and row.
        /// </summary>
        /// <param name="columnID">The column id. <seealso cref="Cell.Column"/></param>
        /// <param name="rowID">The row id. <seeaslo cref="Cell.Row"/></param>
        /// <returns></returns>
        public Cell GetItem(int columnID, int rowID)
        {
            IsBetween1and9(columnID);
            IsBetween1and9(rowID);
            return Cells.ElementAt((rowID - 1) * 9 + columnID - 1);
        }

        /// <summary>
        /// Gets the <see cref="Cell"/> on the specified Index.
        /// </summary>
        /// <param name="index">The Index. <seealso cref="Cell.Index"/></param>
        /// <returns></returns>
        public Cell GetItem(int index)
        {
            if (index > 80 || index < 0) // 9 * 9 -1
                throw new ArgumentOutOfRangeException("index");

            int column = (byte)((index % 9) + 1);
            byte row = (byte)(index / 9 + 1);
            return GetItem(column, row);
        }

        /// <summary>
        /// Gets the Index with the specified column and row.
        /// </summary>
        /// <param name="columnID">The column id.</param>
        /// <param name="rowID">The row id.</param>
        /// <returns></returns>
        public int GetIndex(int columnID, int rowID)
        {
            return (rowID - 1) * 9 + columnID - 1;
        }

        /// <summary>
        /// Gets the cells from the specified row as <see cref="IEnumerable{Cell}"/>
        /// </summary>
        /// <param name="rowID">The row id. <seealso cref="Cell.Row"/></param>
        /// <param name="cellStatus">The cell status. <seealso cref="CellStatus"/></param>
        /// <returns></returns>
        public IEnumerable<Cell> GetRow(int rowID, CellStatus cellStatus = CellStatus.All)
        {
            IsBetween1and9(rowID);
            Func<Cell, bool> func = GetCellStatusFunction(cellStatus);
            return Cells.Where(c => c.Row == rowID).Where(func);
        }

        /// <summary>
        /// Loads a File with a sudoku puzzle.
        /// </summary>
        /// <param name="filename">The filename</param>
        /// <param name="seperator">The data separator</param>
        public void LoadCSV(string filename, char seperator = ';')
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
        /// Removes the specified number in the specified cell from <see cref="Cell.PossibleNumbers"/>.
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="number"></param>
        public void RemovePossibleNumber(Cell cell, int number)
        {
            IsBetween1and9(number);
            Cells.First(c => c == cell).RemovePossibleNumber((byte)number);
        }

        /// <summary>
        /// Filters a sequence of values based on a <see cref="CellStatus"/>.
        /// </summary>
        /// <param name="cellStatus">The cell status. <seealso cref="CellStatus"/></param>
        /// <returns>Returns a filtered <see cref="IEnumerable{Cell}"/></returns>
        public IEnumerable<Cell> Where(CellStatus cellStatus = CellStatus.All)
        {
            Func<Cell, bool> func = GetCellStatusFunction(cellStatus);
            return Cells.Where(func);
        }

        /// <summary>
        /// Filters a sequence of values based on the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public IEnumerable<Cell> Where(Func<Cell, bool> predicate)
        {
            return Cells.Where(predicate);
        }

        /// <summary>
        /// Returns a predicate based on the specified <see cref="CellStatus"/>
        /// </summary>
        /// <param name="cellStatus">The cell status. <seealso cref="CellStatus"/></param>
        /// <returns>Returns a predicate based on the specified <see cref="CellStatus"/></returns>
        private static Func<Cell, bool> GetCellStatusFunction(CellStatus cellStatus)
        {
            Func<Cell, bool> func;
            switch (cellStatus)
            {
                case CellStatus.Solved:
                    func = c => c.IsSolved;
                    break;
                case CellStatus.Unsolved:
                    func = c => !c.IsSolved;
                    break;
                case CellStatus.All:
                default:
                    func = s => true;
                    break;
            }
            return func;
        }

        /// <summary>
        /// Checks if the specified number is between one and nine.
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="ArgumentException">Throws a <see cref="ArgumentException"/> if the <paramref name="id"/> is not between one and nine</exception>
        private static void IsBetween1and9(int id)
        {
            if (id == 0 || id > 9)
                throw new ArgumentException("The " + nameof(id) + " have to be between 1 and 9", nameof(id));
        }
    }
}
