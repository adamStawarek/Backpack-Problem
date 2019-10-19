using BackpackProblem;
using NUnit.Framework;
using System;

namespace Backpack.Problem.Tests.Integration
{
    [TestFixture]
    public class AlgorithmTests
    {
        [Test]
        public void SmallDataSet()
        {
            var container = ContainerFactory.ReadFromCsv("data-5.csv");
            container.GeneratePowerSet();
            container.SortSubsets();
            var subset = container.FindBestSubset();

            Assert.AreEqual(192, subset.TotalArea);
        }
    }
}
