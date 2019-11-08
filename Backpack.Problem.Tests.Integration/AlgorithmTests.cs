using BackpackProblem;
using NUnit.Framework;

namespace Backpack.Problem.Tests.Integration
{
    [TestFixture]
    public class AlgorithmTests
    {
        [Test]
        public void SmallDataSet()
        {
            var container = ContainerFactory.ReadFromFile("data-5.csv");
            container.GeneratePowerSet();
            container.SortSubsets();
            var subset = container.FindBestSubset();

            Assert.AreEqual(255, subset.TotalArea);
        }
    }
}
