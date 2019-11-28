using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackProblem.StatisticsCollector
{
    class Program
    {
        private static readonly string ResultsDirectory = Directory.GetCurrentDirectory() + "/Results";

        static void Main()
        {
            var configurations = new List<(int containerWidth, int containerHeight, int minItemWidth, int maxItemWidth,
                int minItemHeight, int maxItemHeight, int minItemValue, int maxItemValue, int itemCount)>
            {
                (10,10,1,4,1,4,1,10,18),
                (10,10,4,9,4,9,1,10,18),
                (10,10,1,4,1,4,1,2,18),
                (10,10,4,9,4,9,1,2,18)
            };

            foreach (var config in configurations)
            {
                try
                {
                    CollectStatistics(config.containerWidth, config.containerHeight,
                        config.minItemWidth, config.minItemHeight,
                        config.minItemValue, config.maxItemWidth,
                        config.maxItemHeight, config.maxItemValue,
                        config.itemCount, 30,
                        25 * 60 * 1000, false);
                }
                catch
                {
                    // ignored
#if Debug
               global::System.Console.WriteLine("Exception occured at calculating statisics");
#endif
                }
            }
        }

        public static void CollectStatistics(int containerWidth, int containerHeight,
            int minItemWidth, int minItemHeight, int minItemValue,
            int maxItemWidth, int maxItemHeight, int maxItemValue,
            int itemCount, int iterationCount, int timeout, bool shuffle)
        {
            var sb = new StringBuilder();
            sb.AppendLine("containerArea itemsCount resultItemsCount resultTotalValue averageItemAreaToContainerAreaRatio averageOfItemValueToItemAreaRatio operations time");

            sb.AppendLine();

            for (int i = 0; i < iterationCount; i++)
            {
                var container = new ContainerBuilder()
                    .WithContainerDimensions(containerWidth, containerHeight)
                    .WithItemMinDimensions(minItemWidth, minItemHeight)
                    .WithItemMinValue(minItemValue)
                    .WithItemMaxDimensions(maxItemWidth, maxItemHeight)
                    .WithItemMaxValue(maxItemValue)
                    .WithItems(itemCount)
                    .Build();

                var task = Task.Run(() =>
                {
                    container.GeneratePowerSet();
                    container.SortSubsets();
                    return container.FindBestSubset();
                });

                var watch = System.Diagnostics.Stopwatch.StartNew();
                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(timeout));
                watch.Stop();
                long elapsedMs = watch.ElapsedMilliseconds;
                sb.Append(
                    isCompletedSuccessfully
                        ? $"{container.Area} " +
                          $"{container.AllItems.Count} " +
                          $"{task.Result?.Items.Count} " +
                          $"{task.Result?.TotalValue} " +
                          $"{container.AllItems.Average(item => item.Area) / (double)container.Area} " +
                          $"{container.AllItems.Average(item => item.Value / (double)item.Area)} " +
                          $"{container.Subsets.IndexOf(task.Result)} " +
                          $"{container.Counter} " +
                          $"{elapsedMs}"
                        : "[Timeout]");


                if (shuffle)
                {
                    var container2 = container.CloneWithItems();
                    container2.Shuffle();
                    var task2 = Task.Run(() =>
                    {
                        container2.GeneratePowerSet();
                        container2.SortSubsets();
                        return container2.FindBestSubset();
                    });

                    var watch2 = System.Diagnostics.Stopwatch.StartNew();
                    var isCompletedSuccessfully2 = task2.Wait(TimeSpan.FromMilliseconds(timeout));
                    watch2.Stop();
                    var elapsedMs2 = watch.ElapsedMilliseconds;
                    sb.Append(
                        isCompletedSuccessfully2
                            ? $"[{task2.Result?.Items.Count} {task2.Result?.TotalValue} {task2.Result?.TotalArea} {elapsedMs2}]"
                            : "[Timeout]");

                    if (isCompletedSuccessfully)
                    {
                        sb.Append($"[{task.Result?.Items.Count.Equals(task2.Result?.Items.Count)}" +
                                  $" {task.Result?.TotalArea.Equals(task2.Result?.TotalArea)}" +
                                  $" {task.Result?.TotalValue.Equals(task2.Result?.TotalValue)}]");
                    }
                }

                sb.AppendLine();
            }

            Directory.CreateDirectory(ResultsDirectory);
            using (var file = new StreamWriter($"{ResultsDirectory}/" +
                                               $"Result_{DateTime.Now.ToString("s").Replace(":", "-")}.txt"))
            {
                file.Write(sb);
            }
        }
    }
}
