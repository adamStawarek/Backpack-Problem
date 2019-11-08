using BackpackProblem.WebApi.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BackpackProblem.WebApi.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BackpackController : ControllerBase
    {
        [HttpGet]
        public JsonResult GetFromFile(string dataSet)
        {
            var container = ContainerFactory.ReadFromCsv(dataSet);

            var watch = System.Diagnostics.Stopwatch.StartNew();

            container.GeneratePowerSet();
            container.SortSubsets();
            var subset = container.FindBestSubset();

            watch.Stop();
            long elapsedMs = watch.ElapsedMilliseconds;

            return new JsonResult(new
            {
                container.Items,
                ContainerWidth = container.Width,
                ContainerHeight = container.Height,
                SelectedSubset = subset,
                ExecutionTime = elapsedMs
            });
        }

        [HttpGet]
        public JsonResult GetRandom([FromQuery]GetRandomModel model)
        {
            var container = new ContainerBuilder()
                .WithContainerDimensions(model.ContainerWidth, model.ContainerHeight)
                .WithItemMaxDimensions(model.MaxItemWidth, model.MaxItemHeight)
                .WithItemMaxValue(model.MaxItemValue)
                .WithItems(model.NumberOfItems)
                .Build();

            var watch = System.Diagnostics.Stopwatch.StartNew();

            container.GeneratePowerSet();
            container.SortSubsets();
            var subset = container.FindBestSubset();

            watch.Stop();
            long elapsedMs = watch.ElapsedMilliseconds;

            return new JsonResult(new
            {
                container.Items,
                ContainerWidth = container.Width,
                ContainerHeight = container.Height,
                SelectedSubset = subset,
                ExecutionTime = elapsedMs
            });
        }

        [HttpGet]
        public IEnumerable<string> GetAvailableDataSets()
        {
            return new string[]
            {
                "data-5.csv",
                "data-10.csv",
                "data-20.csv",
                "data-25.csv"
            };
        }
    }
}
