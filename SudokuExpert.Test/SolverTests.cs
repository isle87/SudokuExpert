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
            s.NackedSingle(s.GetItem(5, 4));
            Assert.AreEqual(9, s.GetItem(5, 4).Value);
        }

        public int getIndex(int c, int r)
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
            s.GetItem(4, 3).Value = 1;
            s.GetItem(7, 2).Value = 1;
            s.GetItem(1, 1).Value = 3;
            s.GetItem(2, 1).Value = 2;
            s.SimpleSolveRotation();
            Assert.AreEqual(1, s.GetItem(3, 1).Value);
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
                s.GetItem(5, 4).DeletePossibleNumber(i);

            for (byte i = 4; i < 10; i++)
                s.GetItem(4, 6).DeletePossibleNumber(i);

            for (byte i = 3; i < 10; i++) // Some special
                s.GetItem(6, 6).DeletePossibleNumber(i);

            s.nackedSubsetTest();

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
            findDoubles(s);
        }

        [TestMethod]
        public void SimpleSolveRotation_ValidValues_MediumCase_Tagesspiegel()
        {
            Solver s = new Solver();
            s.LoadSudokuCSV("test-002.csv");
            s.SimpleSolve();
            findDoubles(s);
        }

        [TestMethod]
        public void SimpleSolveRotation_ValidValues_HardCase_Tagesspiegel()
        {
            Solver s = new Solver();
            s.LoadSudokuCSV("test-003.csv");
            s.SimpleSolve();
            findDoubles(s);
        }

        [TestMethod]
        public void SimpleSolveRotation_ValidValues_VeryHardCase_Tagesspiegel()
        {
            // Tagesspiegel.de is the test source. It seems to be to easy! The programm just need nacked and hidden single.
            Solver s = new Solver();
            s.LoadSudokuCSV("test-004.csv");
            s.SimpleSolve();
            findDoubles(s);
        }

        private static void findDoubles(Solver s)
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
    }
}