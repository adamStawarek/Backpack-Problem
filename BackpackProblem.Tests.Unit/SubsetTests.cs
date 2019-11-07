using NUnit.Framework;
using System.Collections.Generic;

namespace BackpackProblem.Tests.Unit
{
    [TestFixture]
    public class SubsetTests
    {
        private readonly Subset _subset = new Subset();
        private readonly Item _item1 = new Item(1,0,0);
        private readonly Item _item2 = new Item(0, 1, 0);
        private readonly Item _item3 = new Item(0, 0, 1);

        [SetUp]
        public void Init()
        {
            _subset.Items.Add(_item1);
            _subset.Items.Add(_item2);
            _subset.Items.Add(_item3);
        }

        [Test]
        public void GeneratePermutations_Returns_All_Variations_Of_Subset_Items()
        {
            var permutations = _subset.GetPermutations();
            var expected = new List<List<Item>>
            {
                new List<Item>(){_item1,_item2, _item3},
                new List<Item>(){_item1,_item3, _item2},
                new List<Item>(){_item2,_item1, _item3},
                new List<Item>(){_item2,_item3, _item1},
                new List<Item>(){_item3,_item1, _item2},
                new List<Item>(){_item3,_item2, _item1},
            };
            CollectionAssert.AreEquivalent(expected, permutations);
        }
    }
}
