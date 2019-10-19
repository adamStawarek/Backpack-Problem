using FizzWare.NBuilder;
using NUnit.Framework;
using System;

namespace BackpackProblem.Tests.Unit
{
    [TestFixture]
    public class ContainerTests
    {
        private Container _container;

        [SetUp]
        public void Init()
        {
            _container = new Container(10, 10);
        }

        [Test]
        public void Given_Item_Fitting_Container_AddItem_Increase_Number_Of_Items_In_Container_By_One()
        {
            int previousItemsCount = _container.Items.Count;

            _container.AddItem(new Item(1, 1, 1));

            Assert.AreEqual(previousItemsCount + 1, _container.Items.Count);
        }

        [Test]
        public void Given_Item_NOT_Fitting_Container_AddItem_Does_Not_Increase_Number_Of_Items_In_Container()
        {
            int previousItemsCount = _container.Items.Count;

            _container.AddItem(new Item(11, 1, 1));

            Assert.AreEqual(previousItemsCount, _container.Items.Count);
        }

        [TestCase(3)]
        [TestCase(2)]
        [TestCase(1)]
        public void Given_N_Items_GeneratePowerSets_Returns_2_To_N_Power_Subsets(int itemsCount)
        {
            var items = Builder<Item>.CreateListOfSize(itemsCount)
                .All().WithFactory(() => new Item(1, 1, 1))
                .Build();
            _container.AddItems(items);

            _container.GeneratePowerSet();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(itemsCount, _container.Items.Count);
                Assert.AreEqual((int)Math.Pow(2, itemsCount), _container.Subsets.Count);
            });
        }

        [Test]
        public void Given_Generated_Subsets_SortSubsets_Change_Order_Descending_Subsets_By_Total_Items_Value()
        {
            var subset1 = new Subset();
            subset1.Items.Add(new Item(1, 1, 1));
            subset1.Items.Add(new Item(1, 1, 6));

            var subset2 = new Subset();
            subset2.Items.Add(new Item(1, 1, 2));

            var subset3 = new Subset();
            subset3.Items.Add(new Item(1, 1, 24));

            _container.AddSubset(subset1);
            _container.AddSubset(subset2);
            _container.AddSubset(subset3);

            _container.SortSubsets();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(subset3, _container.Subsets[0]);
                Assert.AreEqual(subset1, _container.Subsets[1]);
                Assert.AreEqual(subset2, _container.Subsets[2]);
            });
        }

        [Test]
        public void Given_Subset_Which_Items_Can_Fit_Container_CheckIfSubsetFits_Returns_True()
        {
            var subset = new Subset();
            subset.Items.Add(new Item(5, 5, 1));
            subset.Items.Add(new Item(5, 5, 1));
            subset.Items.Add(new Item(5, 5, 1));
            subset.Items.Add(new Item(4, 4, 1));
            _container.AddSubset(subset);

            var (canFit, spaceRemaining) = _container.CheckIfSubsetFits(subset);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(canFit);
                Assert.AreEqual(9, spaceRemaining);
            });
        }

        [Test]
        public void Given_Subset_Which_Items_Can_NOT_Fit_Container_CheckIfSubsetFits_Returns_False()
        {
            var subset = new Subset();
            subset.Items.Add(new Item(5, 5, 1));
            subset.Items.Add(new Item(5, 5, 1));
            subset.Items.Add(new Item(5, 5, 1));
            subset.Items.Add(new Item(6, 4, 1));
            _container.AddSubset(subset);

            var (canFit, spaceRemaining) = _container.CheckIfSubsetFits(subset);

            Assert.Multiple(() =>
            {
                Assert.IsFalse(canFit);
                Assert.IsNull(spaceRemaining);
            });
        }
        [Test]
        public void Given_Subsets_Which_Items_Fits_Container_FindBestSubset_Returns_Subset_With_Highest_TotalValue()
        {
            var subset1 = new Subset();
            subset1.Items.Add(new Item(1, 1, 1));
            subset1.Items.Add(new Item(1, 1, 6));

            var subset2 = new Subset();
            subset2.Items.Add(new Item(1, 1, 2));

            var subset3 = new Subset();
            subset3.Items.Add(new Item(1, 1, 24));

            var subset4 = new Subset();
            subset4.Items.Add(new Item(1, 1, 4));
            subset4.Items.Add(new Item(1, 1, 14));
            subset4.Items.Add(new Item(1, 1, 8));

            _container.AddSubset(subset1);
            _container.AddSubset(subset2);
            _container.AddSubset(subset3);

            _container.SortSubsets();

            var subset = _container.FindBestSubset();

            Assert.AreEqual(subset3, subset);
        }
    }
}