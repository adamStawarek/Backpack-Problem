using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace BackpackProblem.WebApi.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BackpackController : ControllerBase
    {
        [HttpGet]
        public JsonResult GetBestSetOfItems(string dataSet)
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
