using BackpackProblem;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Backpack.Problem.Tests.Integration
{
    [TestFixture]
    public class AlgorithmTests
    {
        [Test]
        public void MicroDataSet()
        {
            var container = ContainerFactory.ReadFromFile("data-3.csv");
            container.GeneratePowerSet();
            container.SortSubsets();
            var subset = container.FindBestSubset();

            Assert.AreEqual(subset.TotalValue, 3);
        }

        [Test]
        public async Task MicroDataSet_Async()
        {
            var container = ContainerFactory.ReadFromFile("data-3.csv");
            container.GeneratePowerSet();
            container.SortSubsets();
            var subset = await container.FindBestSubsetAsync();

            Assert.AreEqual(subset.TotalValue, 3);
        }

        [Test]
        public void SmallDataSet()
        {
            var container = ContainerFactory.ReadFromFile("data-5.csv");
            container.GeneratePowerSet();
            container.SortSubsets();
            var subset = container.FindBestSubset();

            Assert.AreEqual(255, subset.TotalArea);
        }

        [Test]
        public async Task SmallDataSet_Async()
        {
            var container = ContainerFactory.ReadFromFile("data-5.csv");
            container.GeneratePowerSet();
            container.SortSubsets();
            var subset = await container.FindBestSubsetAsync();

            Assert.AreEqual(255, subset.TotalArea);
        }

        [Test]
        public void BigDataSet()
        {
            var container = ContainerFactory.ReadFromFile("data-20.csv");
            container.GeneratePowerSet();
            container.SortSubsets();
            var subset = container.FindBestSubset();

            Assert.AreEqual(100, subset.TotalArea);
        }

        [Test]
        public async Task BigDataSet_Async()
        {
            var container = ContainerFactory.ReadFromFile("data-20.csv");
            container.GeneratePowerSet();
            container.SortSubsets();
            var subset = await container.FindBestSubsetAsync();

            Assert.AreEqual(100, subset.TotalArea);
        }
    }
}
