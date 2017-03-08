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
            setUpPossibleNumbers();
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

        public byte Column
        {
            get { return _Column; }
            set { _Column = checkOneToNine(value); }
        }

        public int Index
        {
            get
            {
                return (Row - 1) * 9 + Column - 1;
            }
        }

        public bool IsSolved
        {
            get
            {
                return Value != 0;
            }
        }

        public List<byte> PossibleNumbers { get; protected set; }

        public byte Row
        {
            get { return _Row; }
            set { _Row = checkOneToNine(value); }
        }

        public byte Value
        {
            get { return _Value; }
            set
            {
                _Value = checkOneToNine(value, true);
                onSudokuItemSolved();
            }
        }
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

        protected virtual void onSudokuItemSolved()
        {
            if (Value != 0)
            {
                SudokuItemSolved?.Invoke(this, EventArgs.Empty);
                PossibleNumbers.Clear();
            }
        }

        private byte checkOneToNine(byte value, bool zero = false)
        {
            byte x = zero ? (byte)0 : (byte)1;
            if (value >= x && value <= 9)
                return value;
            else
                throw new ArgumentOutOfRangeException("value", "have to be between " + (zero ? "0" : "1") + " - 9");
        }

        private void setUpPossibleNumbers()
        {
            PossibleNumbers = new List<byte>();
            if (Value != 0)
                return;
            for (byte i = 1; i < 10; i++)
            {
                PossibleNumbers.Add(i);
            }
        }

        public void DeletePossibleNumber(byte number)
        {
            if (IsSolved)
                return;
            if (PossibleNumbers.Remove(number))
                checkPossibleNumbers();
        }

        private void checkPossibleNumbers()
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
