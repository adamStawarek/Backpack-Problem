using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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

        [Test]
        public void Given_3_Items_With_Different_Dimensions_GeneratePowerSets_Returns_8_Power_Subsets()
        {
            var items = new Item[]
            {
                new Item(1,1,1),
                new Item(1,2,1),
                new Item(1,3,1),
            };
            _container.AddItems(items);

            _container.GeneratePowerSet();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(3, _container.Items.Count);
                Assert.AreEqual((int)Math.Pow(2, 3), _container.Subsets.Count);
            });
        }

        [Test]
        public void Given_3_Items__Which_2_Have_Duplicate_Dimensions_GeneratePowerSets_Returns_6_Power_Subsets()
        {
            var items = new Item[]
            {
                new Item(1,1,1),
                new Item(1,2,1),
                new Item(2,1,2),
            };
            _container.AddItems(items);
            _container.SortSubsets();
            _container.GeneratePowerSet();

            //(1 1)(1 2)(2 1)
            //(1 1)(2 1)
            //(2 1)(1 2)
            //(1 1)
            //(2 1)
            //[]
            Assert.Multiple(() =>
            {
                Assert.AreEqual(3, _container.Items.Count);
                Assert.AreEqual(6, _container.Subsets.Count);
            });
        }

        [Test]
        public void Given_Generated_Subsets_SortSubsets_Change_Order_Descending_Subsets_By_Total_Items_Value()
        {
            var subset1 = new Subset();
            subset1.Items.Add(new Item(1, 1, 1));
            subset1.Items.Add(new Item(1, 1, 6));

            var subset2 = new Subset();
            subset2.Items.Add(new Item(1, 6, 2));

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

            var result = _container.CheckIfSubsetFits(subset);

            Assert.IsTrue(result.canFit);
        }

        [Test]
        public void Given_Subset_Which_Items_Can_NOT_Fit_Container_CheckIfSubsetFits_Returns_False()
        {
            var subset = new Subset();
            subset.Items.Add(new Item(5, 5, 1));
            subset.Items.Add(new Item(5, 5, 1));
            subset.Items.Add(new Item(5, 5, 1));
            subset.Items.Add(new Item(6, 5, 1));
            _container.AddSubset(subset);

            var result = _container.CheckIfSubsetFits(subset);

            Assert.IsFalse(result.canFit);
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

        [Test]
        public void Update_When_Item_Is_2x2_Square_And_IsInserted_In_Top_Left_Corner()
        {
            var place = new Point(0, 0);
            var item = new Item(2, 2, 1);
            this._container.Update(item, place);

            var expected = new int[10, 10];
            expected[0, 0] = 1;
            expected[0, 1] = 1;
            expected[1, 0] = 1;
            expected[1, 1] = 1;

            CollectionAssert.AreEqual(expected, _container.Fields);
        }

        [Test]
        public void Clone_Creates_New_Container_With_The_Same_Fields()
        {
            var newContainer = _container.Clone();
            CollectionAssert.AreEqual(_container.Fields, newContainer.Fields);
        }

        [Test]
        public void GetPlacesForItems_When_Item_Is_2x2_Square()
        {
            var tmpContainer = new Container(5, 5);
            var item1 = new Item(2, 2, 1);
            tmpContainer.Update(item1, new Point(2, 1));

            var item2 = new Item(2, 2, 1);
            var places = tmpContainer.GetPlacesForItem(item2);

            var expected = new List<Point>()
            {
                new Point(0,0),
                new Point(0,1),
                new Point(0,2),
                new Point(0,3),
                new Point(1,3),
                new Point(2,3),
                new Point(3,3)

            };
            CollectionAssert.AreEquivalent(expected, places);
        }

        [Test]
        public void CheckIfItemFits_When_Item_Can_Fit()
        {
            var place1 = new Point(0, 0);
            var item1 = new Item(2, 2, 1);
            this._container.Update(item1, place1);

            var place2 = new Point(2, 2);
            var item2 = new Item(2, 2, 1);

            var canFit = _container.CheckIfItemFits(item2, place2);
            Assert.IsTrue(canFit);
        }

        [Test]
        public void CheckIfItemFits_When_Item_Overlap_Another_Item()
        {
            var place1 = new Point(0, 0);
            var item1 = new Item(2, 2, 1);
            this._container.Update(item1, place1);

            var place2 = new Point(1, 1);
            var item2 = new Item(2, 2, 1);

            var canFit = _container.CheckIfItemFits(item2, place2);
            Assert.IsFalse(canFit);
        }

        [Test]
        public void CheckIfItemFits_When_Item_Is_Out_Of_Container()
        {
            var container = new Container(5, 5);
            var place = new Point(1, 3);
            var item = new Item(1, 3, 1);

            var canFit = container.CheckIfItemFits(item, place);
            Assert.IsFalse(canFit);
        }

        [Test]
        public void CanFit_Only_When_One_Item_Is_Swapped()
        {
            var items = new Stack<Item>();
            items.Push(new Item(4, 1, 1));
            items.Push(new Item(1, 2, 1));
            items.Push(new Item(1, 2, 1));

            var tmpContainer = new Container(2, 4);
            var canFit = tmpContainer.CanFit(items, tmpContainer, out List<Item> changedItems);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(canFit);
                Assert.IsTrue(changedItems.Single(r => r.Height == 4).DimensionsSwapped);
            });
        }

        [Test]
        [Repeat(500)]
        public void Shuffle_Change_Only_Order_Of_Items()
        {
            var container = new ContainerBuilder()
                .WithItemMaxValue(100)
                .WithContainerDimensions(10,10)
                .WithItems(10)
                .WithItemMaxDimensions(10,10)
                .Build();
            var oldItems = new List<Item>(container.AllItems);

            container.Shuffle(); 

            CollectionAssert.AreNotEqual(oldItems, container.AllItems);
            CollectionAssert.AreEquivalent(oldItems, container.AllItems);
        }
    }
}