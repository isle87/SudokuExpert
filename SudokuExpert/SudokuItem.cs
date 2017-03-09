using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuExpert
{
    public class SudokuItem
    {
        private byte _Column;

        private byte _Row;

        private byte _Value;

        public SudokuItem()
        {
            SetUpPossibleNumbers();
        }

        public SudokuItem(byte value, int index) : this()
        {
            Value = value;
            SetColumnAndRow(index);
        }

        public SudokuItem(byte value, byte column, byte row) : this()
        {
            Value = value;
            Row = row;
            Column = column;
        }

        public event EventHandler SudokuItemSolved;

        /// <summary>
        /// Returns the block id. A block is a 3 by 3 grid.
        /// </summary>
        public byte Block
        {
            get
            {
                // Identifing the Column
                byte ho = (byte)(Column < 4 ? 1 : Column < 7 ? 2 : 3);
                // Identifing the Row
                return ho += (byte)(Row < 4 ? 0 : Row < 7 ? 3 : 6);
            }
        }

        /// <summary>
        /// Returns the column. A column is a vertical line.
        /// </summary>
        public byte Column
        {
            get { return _Column; }
            set { _Column = CheckOneToNine(value); }
        }

        /// <summary>
        /// Returns the Index of a cell.
        /// </summary>
        /// <example>
        /// The cell with the column 1 and the row 1 returns the first index of 0.
        /// The cell with the column 2 and the row 1 returns the the index of 1.
        /// The cell with the column 9 and the row 9 returns the last index of 80.
        /// </example>
        public int Index
        {
            get
            {
                return (Row - 1) * 9 + Column - 1;
            }
        }

        /// <summary>
        /// Returns if the cell is solved.
        /// </summary>
        public bool IsSolved
        {
            get
            {
                return Value != 0;
            }
        }

        /// <summary>
        /// Returns all possible Numbers of the cell.
        /// </summary>
        public List<byte> PossibleNumbers { get; protected set; }

        /// <summary>
        /// Returns the row. A row is a horizontal line.
        /// </summary>
        public byte Row
        {
            get { return _Row; }
            set { _Row = CheckOneToNine(value); }
        }

        /// <summary>
        /// Sets or returns the solved value.
        /// </summary>
        public byte Value
        {
            get { return _Value; }
            set
            {
                _Value = CheckOneToNine(value, true);
                OnSudokuItemSolved();
            }
        }

        /// <summary>
        /// Sets the column and row with the given index.
        /// </summary>
        /// <param name="index"></param>
        public void SetColumnAndRow(int index)
        {
            if (index > 80 || index < 0) // 9 * 9 -1
                throw new ArgumentOutOfRangeException("index");

            Column = (byte)((index % 9) + 1);
            Row = (byte)(index / 9 + 1);
        }

        /// <summary>
        /// Return true if the <paramref name="number"/> is a part of <see cref="PossibleNumbers"/> 
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        public bool ContainsPossibleNumber(byte number)
        {
            return PossibleNumbers.Exists(x => x == number);
        }

        protected virtual void OnSudokuItemSolved()
        {
            if (Value != 0)
            {
                SudokuItemSolved?.Invoke(this, EventArgs.Empty);
                PossibleNumbers.Clear();
            }
        }

        private byte CheckOneToNine(byte value, bool zero = false)
        {
            byte x = zero ? (byte)0 : (byte)1;
            if (value >= x && value <= 9)
                return value;
            else
                throw new ArgumentOutOfRangeException("value", "have to be between " + (zero ? "0" : "1") + " - 9");
        }

        private void SetUpPossibleNumbers()
        {
            PossibleNumbers = new List<byte>();
            if (Value != 0)
                return;
            for (byte i = 1; i < 10; i++)
            {
                PossibleNumbers.Add(i);
            }
        }

        public void RemovePossibleNumber(byte number)
        {
            if (IsSolved)
                return;
            if (PossibleNumbers.Remove(number))
                CheckPossibleNumbers();
        }

        private void CheckPossibleNumbers()
        {
            if(PossibleNumbers.Count == 1)
            {
                Value = PossibleNumbers[0];
                PossibleNumbers.Clear();
            }
        }

        public override string ToString()
        {
            return "C: " + Column.ToString() + "| R: " + Row.ToString() + " | V: " + Value.ToString();
        }
    }
}
