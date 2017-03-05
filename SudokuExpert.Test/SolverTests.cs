using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SudokuExpert;
using System.Linq;

namespace SudokuExpert.Test
{
    [TestClass]
    public class SolverTests
    {
        [TestMethod]
        public void NakedSingle_ValidTest()
        {
            Solver s = new Solver();
            s.ItemGet(5, 1).Value = 1;
            s.ItemGet(5, 2).Value = 2;
            s.ItemGet(3, 4).Value = 3;
            s.ItemGet(4, 4).Value = 4;
            s.ItemGet(4, 5).Value = 5;
            s.ItemGet(4, 6).Value = 6;
            s.ItemGet(5, 7).Value = 7;
            s.ItemGet(7, 4).Value = 8;
            s.NackedSingle(s.ItemGet(5, 4));
            Assert.AreEqual(9, s.ItemGet(5, 4).Value);
        }
        public int getIndex(int c, int r)
        {
            return (r - 1) * 9 + c - 1;
        }

        [TestMethod]
        public void HiddenSingle_BlockValidTest_IncludeNackedTest()
        {
            Solver s = new Solver();
            s.ItemGet(4, 3).Value = 1;
            s.ItemGet(5, 5).Value = 2;
            s.ItemGet(5, 6).Value = 3;
            s.ItemGet(6, 8).Value = 1;
            s.SolveRotation();
            Assert.AreEqual(1, s.ItemGet(5, 4).Value);
            int[] check = new int[] { getIndex(4, 3), getIndex(5, 4), getIndex(5, 5), getIndex(5, 6), getIndex(6, 8) };
            for (int i = 0; i < s.Items.Count; i++)
            {
                if (check.Contains(i))
                    continue;
                else
                    Assert.AreEqual(0, s.Items[i].Value, s.Items[i].ToString());
            }
        }

        [TestMethod]
        public void HiddenSingle_RowValidTest_IncludeNackedTest()
        {
            Solver s = new Solver();
            s.ItemGet(4, 3).Value = 1;
            s.ItemGet(7, 2).Value = 1;
            s.ItemGet(1, 1).Value = 3;
            s.ItemGet(2, 1).Value = 2;
            s.SolveRotation();
            Assert.AreEqual(1, s.ItemGet(3, 1).Value);
            int[] check = new int[] { getIndex(4, 3), getIndex(7, 2), getIndex(1, 1), getIndex(2, 1), getIndex(3, 1) };
            for (int i = 0; i < s.Items.Count; i++)
            {
                if (check.Contains(i))
                    continue;
                else
                    Assert.AreEqual(0, s.Items[i].Value, s.Items[i].ToString());
            }
        }

        [TestMethod]
        public void NackedSubset_BlockValidTest()
        {
            Solver s = new Solver();
            for (byte i = 4; i < 10; i++)
                s.ItemGet(5, 4).DeletePossibleNumber(i);

            for (byte i = 4; i < 10; i++)
                s.ItemGet(4, 6).DeletePossibleNumber(i);

            for (byte i = 3; i < 10; i++) // Some special
                s.ItemGet(6, 6).DeletePossibleNumber(i);

            s.nackedSubsetTest();

            var testElements = s.Items.Where(i => i.Block == 5 && i != s.ItemGet(5,4) && i != s.ItemGet(4,6) && i != s.ItemGet(6,6));
            foreach (var element in testElements)
            {
                if (element.GotPossibleNumber(1) || element.GotPossibleNumber(2) || element.GotPossibleNumber(3))
                    Assert.Fail("The Test Failed" + element.ToString());
            }
        }
    }
}