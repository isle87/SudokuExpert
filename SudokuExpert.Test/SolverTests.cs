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
            s.GetItem(5, 1).Value = 1;
            s.GetItem(5, 2).Value = 2;
            s.GetItem(3, 4).Value = 3;
            s.GetItem(4, 4).Value = 4;
            s.GetItem(4, 5).Value = 5;
            s.GetItem(4, 6).Value = 6;
            s.GetItem(5, 7).Value = 7;
            s.GetItem(7, 4).Value = 8;
            s.NackedSingleTest(s.GetItem(5, 4));
            Assert.AreEqual(9, s.GetItem(5, 4).Value);
        }

        public int GetIndex(int c, int r)
        {
            return (r - 1) * 9 + c - 1;
        }

        [TestMethod]
        public void HiddenSingle_BlockValidTest_IncludeNackedTest()
        {
            Solver s = new Solver();
            s.GetItem(4, 3).Value = 1;
            s.GetItem(5, 5).Value = 2;
            s.GetItem(5, 6).Value = 3;
            s.GetItem(6, 8).Value = 1;
            s.SimpleSolveRotation();
            Assert.AreEqual(1, s.GetItem(5, 4).Value);
            int[] check = new int[] { GetIndex(4, 3), GetIndex(5, 4), GetIndex(5, 5), GetIndex(5, 6), GetIndex(6, 8) };
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
            s.GetItem(4, 3).Value = 1;
            s.GetItem(7, 2).Value = 1;
            s.GetItem(1, 1).Value = 3;
            s.GetItem(2, 1).Value = 2;
            s.SimpleSolveRotation();
            Assert.AreEqual(1, s.GetItem(3, 1).Value);
            int[] check = new int[] { GetIndex(4, 3), GetIndex(7, 2), GetIndex(1, 1), GetIndex(2, 1), GetIndex(3, 1) };
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
                s.GetItem(5, 4).RemovePossibleNumber(i);

            for (byte i = 4; i < 10; i++)
                s.GetItem(4, 6).RemovePossibleNumber(i);

            for (byte i = 3; i < 10; i++) // Some special
                s.GetItem(6, 6).RemovePossibleNumber(i);

            s.NackedSubsetTest();

            var testElements = s.Items.Where(i => i.Block == 5 && i != s.GetItem(5, 4) && i != s.GetItem(4, 6) && i != s.GetItem(6, 6));
            foreach (var element in testElements)
            {
                if (element.ContainsPossibleNumber(1) || element.ContainsPossibleNumber(2) || element.ContainsPossibleNumber(3))
                    Assert.Fail("The Test Failed" + element.ToString());
            }
        }

        [TestMethod]
        public void LoadSudokuCSV_ValidValues()
        {
            Solver s = new Solver();
            s.LoadSudokuCSV("test-001.csv");

        }

        [TestMethod]
        public void SimpleSolveRotation_ValidValues_EasyCase_Tagesspiegel()
        {
            // Tagesspiegel.de is the test source. It seems to be to easy!
            Solver s = new Solver();
            s.LoadSudokuCSV("test-001.csv");
            s.SimpleSolve();
            FindDoubles(s);
        }

        [TestMethod]
        public void SimpleSolveRotation_ValidValues_MediumCase_Tagesspiegel()
        {
            Solver s = new Solver();
            s.LoadSudokuCSV("test-002.csv");
            s.SimpleSolve();
            FindDoubles(s);
        }

        [TestMethod]
        public void SimpleSolveRotation_ValidValues_HardCase_Tagesspiegel()
        {
            Solver s = new Solver();
            s.LoadSudokuCSV("test-003.csv");
            s.SimpleSolve();
            FindDoubles(s);
        }

        [TestMethod]
        public void SimpleSolveRotation_ValidValues_VeryHardCase_Tagesspiegel()
        {
            // Tagesspiegel.de is the test source. It seems to be to easy! The programm just need nacked and hidden single.
            Solver s = new Solver();
            s.LoadSudokuCSV("test-004.csv");
            s.SimpleSolve();
            FindDoubles(s);
        }

        private static void FindDoubles(Solver s)
        {
            for (int i = 1; i < 10; i++)
            {
                if (s.Items.Exists(si => si.Value == 0))
                    Assert.Fail("Not Solved");
                // Find doubles in Row
                var row = s.Items.Where(si => si.Row == i);
                for (int c = 1; c < 10; c++)
                {
                    var shouldCountOne = row.Where(sco => sco.Value == c);
                    if (shouldCountOne.Count() > 1)
                        Assert.Fail("Found Double" + shouldCountOne.ToString());
                }

                // Find doubles in Column
                var column = s.Items.Where(si => si.Row == i);
                for (int c = 1; c < 10; c++)
                {
                    var shouldCountOne = column.Where(sco => sco.Value == c);
                    if (shouldCountOne.Count() > 1)
                        Assert.Fail("Found Double" + shouldCountOne.ToString());
                }

                // Find doubles in Block
                var block = s.Items.Where(si => si.Row == i);
                for (int c = 1; c < 10; c++)
                {
                    var shouldCountOne = block.Where(sco => sco.Value == c);
                    if (shouldCountOne.Count() > 1)
                        Assert.Fail("Found Double" + shouldCountOne.ToString());
                }
            }
        }

        [TestMethod]
        public void HiddenSubset_ValidValues_SimpleBlockTest()
        {
            Solver s = new Solver();
            var block = s.Items.Where(b => b.Block == 1);
            foreach (var item in block)
            {
                if (item == s.GetItem(2, 1) || item == s.GetItem(3, 3))
                    continue;
                item.RemovePossibleNumber(1);
                item.RemovePossibleNumber(4);
                if (item.Index % 2 == 0)
                    item.RemovePossibleNumber(2);
                else
                    item.RemovePossibleNumber(8);
                if (item.Index % 3 == 0)
                    item.RemovePossibleNumber(9);
            }

            s.HiddenSubsetTest();
            Assert.AreEqual(2, s.GetItem(2, 1).PossibleNumbers.Count, 2);
            Assert.IsTrue(s.GetItem(2, 1).PossibleNumbers.Contains(1));
            Assert.IsTrue(s.GetItem(2, 1).PossibleNumbers.Contains(4));
            Assert.AreEqual(2,s.GetItem(3, 3).PossibleNumbers.Count);
            Assert.IsTrue(s.GetItem(3, 3).PossibleNumbers.Contains(1));
            Assert.IsTrue(s.GetItem(3, 3).PossibleNumbers.Contains(4));

        }

        [TestMethod]
        public void BlockLineInteraction_ValidValues()
        {
            Solver s = new Solver();
            s.GetItem(1, 1).Value = 1;
            s.GetItem(3, 1).Value = 2;
            s.GetItem(1, 3).Value = 3;
            s.GetItem(3, 3).Value = 4;
            s.GetItem(4, 2).Value = 9;
            for (byte i = 1; i < 4; i++)
                s.GetItem(i, 2).RemovePossibleNumber(9);
            s.BlockLineInteractionTest();

            if (s.Items.Any(i => i.Column == 2 && i.Block != 1 && i.ContainsPossibleNumber(9)))
                Assert.Fail();
        }

        [TestMethod]
        public void BlockBlockInteraction_ValidValues_Vertical()
        {
            Solver s = new Solver();
            s.GetItem(3, 1).Value = 1;
            s.GetItem(3, 2).Value = 2;
            s.GetItem(4, 3).Value = 9;
            s.GetItem(3, 7).Value = 3;
            s.GetItem(3, 8).Value = 4;
            s.GetItem(4, 9).Value = 9;

            /**
             * - - 1| - - -| - - -| 
             * - - 2| - - -| - - -|
             * - - -| 9 - -| - - -|
             * --------------------
             * T T -| - - -| - - -|
             * T T -| - - -| - - -|
             * T T -| - - -| - - -|
             * --------------------
             * - - 3| - - -| - - -|
             * - - 4| - - -| - - -|
             * - - -| 9 - -| - - -|
             * */
            s.Items.ForEach(i => s.NackedSingleTest(i));
            s.BlockBlockInteractionTest();
            Assert.IsFalse(s.Items.Where(i => i.Block == 4 && i.Column < 3).Any(i => i.ContainsPossibleNumber(9)));
        }

        [TestMethod]
        public void BlockBlockInteraction_ValidValues_Horizontal()
        {
            Solver s = new Solver();
            s.GetItem(1, 7).Value = 1;
            s.GetItem(2, 7).Value = 2;
            s.GetItem(3, 6).Value = 9;
            s.GetItem(7, 7).Value = 3;
            s.GetItem(8, 7).Value = 4;
            s.GetItem(9, 4).Value = 9;

            /**
             * - - -| - - -| - - -| 
             * - - -| - - -| - - -|
             * - - -| - - -| - - -|
             * --------------------
             * - - -| - - -| - - 9|
             * - - -| - - -| - - -|
             * - - 9| - - -| - - -|
             * --------------------
             * 1 2 -| - - -| 3 4 -|
             * - - -| T T T| - - -|
             * - - -| T T T| - - -|
             * */
            s.Items.ForEach(i => s.NackedSingleTest(i));
            s.BlockBlockInteractionTest();
            Assert.IsFalse(s.Items.Where(i => i.Block == 8 && i.Row > 7).Any(i => i.ContainsPossibleNumber(9)));
        }
    }
}