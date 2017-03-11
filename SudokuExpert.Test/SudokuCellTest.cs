using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SudokuExpert;

namespace SudokuExpert.Test
{
    [TestClass]
    public class SudokuCellTest
    {
        [TestMethod]
        public void Block_WithValidAmount_GetBlock()
        {
            SudokuCell si = new SudokuCell();
            si.Column = 4;
            si.Row = 7;
            byte expected = 8;
            Assert.AreEqual(expected, si.Block);
            si.Column = 1;
            si.Row = 1;
            expected = 1;
            Assert.AreEqual(expected, si.Block);
            si.Column = 9;
            si.Row = 9;
            expected = 9;
            Assert.AreEqual(expected, si.Block);
            si.Column = 4;
            si.Row = 6;
            expected = 5;
            Assert.AreEqual(expected, si.Block);
        }

        [TestMethod] [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Value_WithInvalidAmount()
        {
            SudokuCell si = new SudokuCell();
            si.Value = 16;
        }

        [TestMethod] [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Row_WithInvalidAmount()
        {
            SudokuCell si = new SudokuCell();
            si.Row = 16;
        }

        [TestMethod]
        public void SetColumnAndRow_ValidNumbers_IsExpected()
        {
            int index = 80;
            byte eC = 9;
            byte eR = 9;
            SudokuCell si = new SudokuCell();
            si.SetColumnAndRow(index);
            Assert.AreEqual(eC, si.Column);
            Assert.AreEqual(eR, si.Row);
            index = 0;
            eC = 1;
            eR = 1;
            si.SetColumnAndRow(index);
            Assert.AreEqual(eC, si.Column);
            Assert.AreEqual(eR, si.Row);
            index = 40;
            eC = 5;
            eR = 5;
            si.SetColumnAndRow(index);
            Assert.AreEqual(eC, si.Column);
            Assert.AreEqual(eR, si.Row);
        }

        [TestMethod]
        public void Index_ValidNumbers_CheckFormel()
        {
            SudokuCell si = new SudokuCell();
            byte c = 1;
            byte r = 1;
            byte e = 0;
            si.Column = c;
            si.Row = r;
            Assert.AreEqual(e, si.Index);
            c = 5;
            r = 4;
            e = 31;
            si.Column = c;
            si.Row = r;
            Assert.AreEqual(e, si.Index);
            c = 9;
            r = 9;
            e = 80;
            si.Column = c;
            si.Row = r;
            Assert.AreEqual(e, si.Index);

        }
    }
}
