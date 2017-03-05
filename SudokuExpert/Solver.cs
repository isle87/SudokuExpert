using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuExpert
{
    /// <summary>
    /// <see cref="http://www.hib-wien.at/leute/wurban/mathematik/sudoku_strategie.pdf"/>
    /// </summary>
    public class Solver
    {
        public Solver()
        {
            create();
        }

        public List<SudokuItem> Items = new List<SudokuItem>();
        public IEnumerable<SudokuItem> UnsolvedItems = new List<SudokuItem>();

        private void create()
        {
            for (int i = 0; i <= 80; i++)
            {
                Items.Add(new SudokuItem(0, (byte)i));
            }
        }

        public SudokuItem ItemGet(byte Column, byte Row)
        {
            return Items.ElementAt((Row - 1) * 9 + Column - 1);
        }


        private void nackedSingle(SudokuItem item)
        {
            var rowItems = Items.Where(i => i.Row == item.Row && i.Value != 0);
            foreach (var rowItem in rowItems)
                item.DeletePossibleNumber(rowItem.Value);

            var columnItems = Items.Where(i => i.Column == item.Column && i.Value != 0);
            foreach (var colItem in columnItems)
                item.DeletePossibleNumber(colItem.Value);

            var blockItems = Items.Where(i => i.Block == item.Block && i.Value != 0);
            foreach (var bItem in blockItems)
                item.DeletePossibleNumber(bItem.Value);
        }

        private void hiddenSingle(SudokuItem item)
        {
            if (item.IsSolved)
                return;

            var bitems = UnsolvedItems.Where(b => b.Block == item.Block && b.Value == 0);
            hiddenSingleHelper(item, bitems);
            bitems = UnsolvedItems.Where(b => b.Row == item.Row && b.Value == 0);
            hiddenSingleHelper(item, bitems);
            bitems = UnsolvedItems.Where(b => b.Column == item.Column && b.Value == 0);
        }

        private void hiddenSingleHelper(SudokuItem item, IEnumerable<SudokuItem> bitems)
        {
            bool found = false;
            foreach (byte thisNumber in item.PossibleNumbers) // Durchlaufe die möglichen Zahlen
            {
                for (int i = 0; i < bitems.Count(); i++) // Durchlaufe alle Items im Bereich
                {
                    if (bitems.ElementAt(i) == item && bitems.Count() - 1 != i)
                        continue;
                    if (bitems.ElementAt(i).GotPossibleNumber(thisNumber)) // Enthält das Element die mögliche Zahl?
                    {
                        found = true;
                        break;
                    }
                    else if (i == bitems.Count() - 1)
                        item.Value = thisNumber;
                }
                if (found)
                {
                    found = false;
                    continue;
                }
                break;
            }
        }

        private void nackedSubset(SudokuItem item)
        {
            if (item.IsSolved)
                return;

            var bitems = UnsolvedItems.Where(b => b.Block == item.Block && b.Value == 0);
            nackedSubsetHelper(item, bitems);

            bitems = UnsolvedItems.Where(b => b.Row == item.Row && b.Value == 0);
            nackedSubsetHelper(item, bitems);

            bitems = UnsolvedItems.Where(b => b.Column == item.Column && b.Value == 0);
            nackedSubsetHelper(item, bitems);
        }

        private void nackedSubsetHelper(SudokuItem item, IEnumerable<SudokuItem> bItems)
        {
            var possibleElements = bItems.Where(b => b.PossibleNumbers.Count <= item.PossibleNumbers.Count);
            if (possibleElements.Count() != item.PossibleNumbers.Count || possibleElements.Count() > 7 || possibleElements.Count() == bItems.Count())
                return;

            foreach (var element in possibleElements)
            {
                if (element == item)
                    continue;

                int MissCount = 0; //Counts if something doesn't fit
                foreach (var number in item.PossibleNumbers)
                {
                    if (!(element.GotPossibleNumber(number)))
                        MissCount++;
                    if (MissCount + element.PossibleNumbers.Count() > item.PossibleNumbers.Count)
                        return;
                }
            }
            // All elements accomplish the conditions

            // Delete the elements from bItems. To find out where we have to delete the possible Numbers
            bItems = bItems.Where(b => !possibleElements.Contains(b));

            // Delete the possible number
            foreach (var b in bItems)
                foreach (var pN in item.PossibleNumbers)
                    b.DeletePossibleNumber(pN);
        }

        // TODO: HiddenSubset

        #region TestRegion
#if DEBUG
        public void SolveRotation()
        {
            UnsolvedItems = Items.Where(i => i.Value == 0);

            foreach (var item in UnsolvedItems)
                nackedSingle(item);

            foreach (var item in UnsolvedItems)
                hiddenSingle(item);
        }

        public void NackedSingle(SudokuItem item)
        {
            nackedSingle(item);
        }

        public void nackedSubsetTest()
        {
            UnsolvedItems = Items.Where(i => i.Value == 0);

            foreach (var item in UnsolvedItems)
                nackedSubset(item);
        }
#endif
        #endregion
    }
}
