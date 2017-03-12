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
            Grid g = new Grid();
            g.GetItem(5, 1).Value = 1;
            g.GetItem(5, 2).Value = 2;
            g.GetItem(3, 4).Value = 3;
            g.GetItem(4, 4).Value = 4;
            g.GetItem(4, 5).Value = 5;
            g.GetItem(4, 6).Value = 6;
            g.GetItem(5, 7).Value = 7;
            g.GetItem(7, 4).Value = 8;
            Solver s = new Solver(g);
            s.NackedSingle(g.GetItem(5, 4));
            Assert.AreEqual(9, g.GetItem(5, 4).Value);
        }

        public int GetIndex(int c, int r)
        {
            return (r - 1) * 9 + c - 1;
        }

        [TestMethod]
        public void HiddenSingle_BlockValidTest_IncludeNackedTest()
        {
            Grid g = new Grid();
            g.GetItem(4, 3).Value = 1;
            g.GetItem(5, 5).Value = 2;
            g.GetItem(5, 6).Value = 3;
            g.GetItem(6, 8).Value = 1;
            Solver s = new Solver(g);
            s.SimpleSolveRotation();
            Assert.AreEqual(1, g.GetItem(5, 4).Value);
            int[] check = new int[] { GetIndex(4, 3), GetIndex(5, 4), GetIndex(5, 5), GetIndex(5, 6), GetIndex(6, 8) };
            for (int i = 0; i < g.Count(); i++)
            {
                if (check.Contains(i))
                    continue;
                else
                    Assert.AreEqual(0, g.GetItem(i).Value, g.GetItem(i).ToString());
            }
        }

        [TestMethod]
        public void HiddenSingle_RowValidTest_IncludeNackedTest()
        {
            Grid g = new Grid();
            g.GetItem(4, 3).Value = 1;
            g.GetItem(7, 2).Value = 1;
            g.GetItem(1, 1).Value = 3;
            g.GetItem(2, 1).Value = 2;
            Solver s = new Solver(g);
            s.SimpleSolveRotation();
            Assert.AreEqual(1, g.GetItem(3, 1).Value);
            int[] check = new int[] { GetIndex(4, 3), GetIndex(7, 2), GetIndex(1, 1), GetIndex(2, 1), GetIndex(3, 1) };
            for (int i = 0; i < g.Count(); i++)
            {
                if (check.Contains(i))
                    continue;
                else
                    Assert.AreEqual(0, g.GetItem(i).Value, g.GetItem(i).ToString());
            }
        }

        [TestMethod]
        public void NackedSubset_BlockValidTest()
        { 
            Grid g = new Grid();
            for (byte i = 4; i < 10; i++)
                g.GetItem(5, 4).RemovePossibleNumber(i);

            for (byte i = 4; i < 10; i++)
                g.GetItem(4, 6).RemovePossibleNumber(i);

            for (byte i = 3; i < 10; i++) // Some special
                g.GetItem(6, 6).RemovePossibleNumber(i);

            /**
            * - - -| - - -| - - -| 
            * - - -| - - -| - - -|
            * - - -| - - -| - - -|
            * --------------------
            * - - -| - X -| - - -|
            * - - -| X - -| - - -|
            * - - -| - - X| - - -|
            * --------------------
            * - - -| - - -| - - -|
            * - - -| - - -| - - -|
            * - - -| - - -| - - -|
            * */
            Solver s = new Solver(g);
            s.NacketSubset();

            var testElements = g.Where(i => i.Block == 5 && i != g.GetItem(5, 4) && i != g.GetItem(4, 6) && i != g.GetItem(6, 6));
            foreach (var element in testElements)
            {
                if (element.ContainsPossibleNumber(1) || element.ContainsPossibleNumber(2) || element.ContainsPossibleNumber(3))
                    Assert.Fail("The Test Failed" + element.ToString());
            }
        }

        [TestMethod] // TODO: Move
        public void LoadSudokuCSV_ValidValues()
        {
            Grid g = new Grid();
            g.LoadSudokuCSV("test-001.csv");
        }

        [TestMethod]
        public void SimpleSolveRotation_ValidValues_EasyCase_Tagesspiegel()
        {
            // Tagesspiegel.de is the test source. It seems to be to easy!
            Grid g = new Grid();
            g.LoadSudokuCSV("test-001.csv");
            Solver s = new Solver(g);
            s.SimpleSolve();
            FindDoubles(g);
        }

        [TestMethod]
        public void SimpleSolveRotation_ValidValues_MediumCase_Tagesspiegel()
        {
            Grid g = new Grid();
            g.LoadSudokuCSV("test-002.csv");
            Solver s = new Solver(g);
            s.SimpleSolve();
            FindDoubles(g);
        }

        [TestMethod]
        public void SimpleSolveRotation_ValidValues_HardCase_Tagesspiegel()
        {
            Grid g = new Grid();
            g.LoadSudokuCSV("test-003.csv");
            Solver s = new Solver(g);
            s.SimpleSolve();
            FindDoubles(g);
        }

        [TestMethod]
        public void SimpleSolveRotation_ValidValues_VeryHardCase_Tagesspiegel()
        {
            // Tagesspiegel.de is the test source. It seems to be to easy! The programm just need nacked and hidden single.
            Grid g = new Grid();
            g.LoadSudokuCSV("test-004.csv");
            Solver s = new Solver(g);
            s.SimpleSolve();
            FindDoubles(g);
        }

        private static void FindDoubles(Grid g)
        {
            for (int i = 1; i < 10; i++)
            {
                if (g.Any(CellStatus.Unsolved))
                    Assert.Fail("Not Solved");
                // Find doubles in Row
                var row = g.GetRow(i);
                for (int c = 1; c < 10; c++)
                {
                    var shouldCountOne = row.Where(sco => sco.Value == c);
                    if (shouldCountOne.Count() > 1)
                        Assert.Fail("Found Double" + shouldCountOne.ToString());
                }

                // Find doubles in Column
                var column = g.GetColumn(i);
                for (int c = 1; c < 10; c++)
                {
                    var shouldCountOne = column.Where(sco => sco.Value == c);
                    if (shouldCountOne.Count() > 1)
                        Assert.Fail("Found Double" + shouldCountOne.ToString());
                }

                // Find doubles in Block
                var block = g.GetBlock(i);
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
            Grid g = new Grid();
            foreach (var item in g.GetBlock(1))
            {
                if (item == g.GetItem(2, 1) || item == g.GetItem(3, 3))
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
            Solver s = new Solver(g);
            s.HiddenSubset();
            Assert.AreEqual(2, g.GetItem(2, 1).PossibleNumbers.Count, 2);
            Assert.IsTrue(g.GetItem(2, 1).PossibleNumbers.Contains(1));
            Assert.IsTrue(g.GetItem(2, 1).PossibleNumbers.Contains(4));
            Assert.AreEqual(2, g.GetItem(3, 3).PossibleNumbers.Count);
            Assert.IsTrue(g.GetItem(3, 3).PossibleNumbers.Contains(1));
            Assert.IsTrue(g.GetItem(3, 3).PossibleNumbers.Contains(4));

        }

        [TestMethod]
        public void BlockLineInteraction_ValidValues()
        {
            Grid g = new Grid();
            g.GetItem(1, 1).Value = 1;
            g.GetItem(3, 1).Value = 2;
            g.GetItem(1, 3).Value = 3;
            g.GetItem(3, 3).Value = 4;
            g.GetItem(4, 2).Value = 9;
            for (byte i = 1; i < 4; i++)
                g.GetItem(i, 2).RemovePossibleNumber(9);
            Solver s = new Solver(g);
            s.BlockLineInteraction();

            if (g.Any(i => i.Column == 2 && i.Block != 1 && i.ContainsPossibleNumber(9)))
                Assert.Fail();
        }

        [TestMethod]
        public void BlockBlockInteraction_ValidValues_Vertical()
        {
            Grid g = new Grid();
            g.GetItem(3, 1).Value = 1;
            g.GetItem(3, 2).Value = 2;
            g.GetItem(4, 3).Value = 9;
            g.GetItem(3, 7).Value = 3;
            g.GetItem(3, 8).Value = 4;
            g.GetItem(4, 9).Value = 9;

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
            Solver s = new Solver(g);
            g.ForEach(i => s.NackedSingle(i));
            s.BlockBlockInteractions();
            Assert.IsFalse(g.Where(i => i.Block == 4 && i.Column < 3).Any(i => i.ContainsPossibleNumber(9)));
        }

        [TestMethod]
        public void BlockBlockInteraction_ValidValues_Horizontal()
        {
            Grid g = new Grid();
            g.GetItem(1, 7).Value = 1;
            g.GetItem(2, 7).Value = 2;
            g.GetItem(3, 6).Value = 9;
            g.GetItem(7, 7).Value = 3;
            g.GetItem(8, 7).Value = 4;
            g.GetItem(9, 4).Value = 9;

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
            Solver s = new Solver(g);
            g.ForEach(i => s.NackedSingle(i));
            s.BlockBlockInteractions();
            Assert.IsFalse(g.Where(i => i.Block == 8 && i.Row > 7).Any(i => i.ContainsPossibleNumber(9)));
            Assert.IsTrue(g.Where(i => i.Row < 6 && i.Column < 8 && i.Column != 3 && i.Row != 4).Any(i => i.ContainsPossibleNumber(9)));
        }

        [TestMethod]
        public void BlockBlockInteraction_BlocksWithNoImpact()
        {
            Grid g = new Grid();
            g.GetItem(1, 1).Value = 1;
            g.GetItem(2, 1).Value = 2;
            g.GetItem(3, 6).Value = 9;
            g.GetItem(7, 7).Value = 3;
            g.GetItem(8, 7).Value = 4;
            g.GetItem(9, 4).Value = 9;

            /**
             * 1 2 -| - - -| - - -| 
             * - - -| - - -| - - -|
             * - - -| - - -| - - -|
             * --------------------
             * - - -| - - -| - - 9|
             * - - -| - - -| - - -|
             * - - 9| - - -| - - -|
             * --------------------
             * - - -| - - -| 3 4 -|
             * - - -| - - -| - - -|
             * - - -| - - -| - - -|
             * */
            Solver s = new Solver(g);
            g.ForEach(i => s.NackedSingle(i));
            s.BlockBlockInteractions(1,9);
            Assert.IsTrue(g.Where(i => i.Column != 3 && i.Column != 9 && i.Row != 4 && i.Row != 6 && i.Value == 0).Any(i => i.ContainsPossibleNumber(9)));
        }
    }
}