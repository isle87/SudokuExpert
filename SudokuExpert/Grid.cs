using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuExpert
{
    /// <summary>
    /// 
    /// </summary>
    public enum CellStatus
    {
        Solved,
        Unsolved,
        All
    }

     
    public class Grid
    {
        public Grid()
        {
            // Create all cells ( 9 by 9 )
            Cells = new List<Cell>();
            for (int i = 0; i <= 80; i++)
                Cells.Add(new Cell(0, (byte)i));
        }

        public List<Cell> Cells { get; set; }

        public bool Any(Func<Cell, bool> func)
        {
            return Cells.Any(func);
        }

        public bool Any(CellStatus cellStatus = CellStatus.All)
        {
            return Cells.Any(GetCellStatusFunction(cellStatus));
        }

        public void ForEach(Action<Cell> action)
        {
            foreach (var item in Cells)
                action(item);
        }

        public int Count(CellStatus cellStatus = CellStatus.All)
        {
            return Where(cellStatus).Count();
        }

        public IEnumerable<Cell> GetBlock(int blockID, CellStatus cellStatus = CellStatus.All)
        {
            IsBetween1and9(blockID);
            Func<Cell, bool> func = GetCellStatusFunction(cellStatus);
            return Cells.Where(c => c.Block == blockID).Where(func);
        }

        public IEnumerable<Cell> GetColumn(int columnID, CellStatus cellStatus = CellStatus.All)
        {
            IsBetween1and9(columnID);
            Func<Cell, bool> func = GetCellStatusFunction(cellStatus);
            return Cells.Where(c => c.Column == columnID).Where(func);
        }

        public Cell GetItem(int columnID, int rowID)
        {
            IsBetween1and9(columnID);
            IsBetween1and9(rowID);
            return Cells.ElementAt((rowID - 1) * 9 + columnID - 1);
        }

        public Cell GetItem(int index)
        {
            if (index > 80 || index < 0) // 9 * 9 -1
                throw new ArgumentOutOfRangeException("index");

            int column = (byte)((index % 9) + 1);
            byte row = (byte)(index / 9 + 1);
            return GetItem(column, row);
        }

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

        public void RemovePossibleNumber(Cell cell, int number)
        {
            IsBetween1and9(number);
            Cells.First(c => c == cell).RemovePossibleNumber((byte)number);
        }

        public IEnumerable<Cell> Where(CellStatus cellStatus = CellStatus.All)
        {
            Func<Cell, bool> func = GetCellStatusFunction(cellStatus);
            return Cells.Where(func);
        }

        public IEnumerable<Cell> Where(Func<Cell, bool> predicate)
        {
            return Cells.Where(predicate);
        }

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

        private static void IsBetween1and9(int id)
        {
            if (id == 0 || id > 9)
                throw new ArgumentException("The " + nameof(id) + " have to be between 1 and 9", nameof(id));
        }
    }
}
