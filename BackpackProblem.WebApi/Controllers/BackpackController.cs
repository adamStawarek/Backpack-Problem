﻿using BackpackProblem.WebApi.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BackpackProblem.WebApi.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BackpackController : ControllerBase
    {
        private readonly string _dataSetDirectory = Directory.GetCurrentDirectory() + "/DataSets";

        [HttpGet]
        public async Task<JsonResult> GetFromDataSet([FromQuery]GetFromFileModel model)
        {
            var container = ContainerFactory.ReadFromFile($"{_dataSetDirectory}/{model.DataSet}");

            var watch = System.Diagnostics.Stopwatch.StartNew();

            container.GeneratePowerSet();
            container.SortSubsets();
            var subset = model.ExecuteAsync ? await container.FindBestSubsetAsync() :
                container.FindBestSubset();

            watch.Stop();
            long elapsedMs = watch.ElapsedMilliseconds;

            return new JsonResult(new
            {
                Items = container.Items.Select(s => new
                {
                    s.Width,
                    s.Height,
                    s.Value
                }),
                ContainerWidth = container.Width,
                ContainerHeight = container.Height,
                SelectedSubset = subset.Items.Select(s => new
                {
                    s.Width,
                    s.Height,
                    s.Value,
                    s.UpperLeftCornerPoint,
                    s.DimensionsSwapped
                }),
                ExecutionTime = elapsedMs
            });
        }

        [HttpGet]
        [ValidateModelState]
        public async Task<JsonResult> GetRandom([FromQuery]GetRandomModel model)
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
            var subset = model.ExecuteAsync ? await container.FindBestSubsetAsync() :
                container.FindBestSubset();

            watch.Stop();
            long elapsedMs = watch.ElapsedMilliseconds;

            if (model.SaveItems)
            {
                this.SaveItemsToFile(container.Width, container.Height, container.Items);
            }

            return new JsonResult(new
            {
                Items = container.Items.Select(s => new
                {
                    s.Width,
                    s.Height,
                    s.Value
                }),
                ContainerWidth = container.Width,
                ContainerHeight = container.Height,
                SelectedSubset = subset.Items.Select(s => new
                {
                    s.Width,
                    s.Height,
                    s.Value,
                    s.UpperLeftCornerPoint,
                    s.DimensionsSwapped
                }),
                ExecutionTime = elapsedMs
            });
        }

        [HttpPost]
        public async Task<IActionResult> UploadDataSet(IFormFile file)
        {
            using (var fileStream = new FileStream(Path.Combine(_dataSetDirectory, file.FileName),
                FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
                return Ok();
            }
        }

        [HttpGet]
        public IEnumerable<string> GetAvailableDataSets()
        {
            return Directory.GetFiles(_dataSetDirectory).Select(f => f.Split(@"\").Last());
        }

        private void SaveItemsToFile(int containerWidth, int containerHeight, List<Item> containerItems)
        {
            Directory.CreateDirectory("Datasets");
            using (var file = new StreamWriter($"{_dataSetDirectory}/backpack_{DateTime.Now.ToString("s").Replace(":", "-")}_{containerWidth}-{containerHeight}-{containerItems.Count}.txt"))
            {
                file.WriteLine($"{containerWidth} {containerHeight}");
                file.WriteLine(containerItems.Count.ToString());
                containerItems.ForEach(item => file.WriteLine($"{item.Width} {item.Height} {item.Height}"));
            }
        }

    }
}
