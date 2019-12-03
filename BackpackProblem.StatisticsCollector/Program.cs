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

        static async Task Main()
        {
            var configurations = new List<(int containerWidth, int containerHeight, int minItemWidth, int maxItemWidth,
                int minItemHeight, int maxItemHeight, int minItemValue, int maxItemValue, int itemCount, bool onlySquares)>
            {
                //small value/large area , large cointainer
                (20,20,5,9,5,9,1,3,6, false),
                (20,20,5,9,5,9,1,3,8, false),
                (20,20,5,9,5,9,1,3,10, false),
                (20,20,5,9,5,9,1,3,12, false),
                (20,20,5,9,5,9,1,3,14, false),
                (20,20,5,9,5,9,1,3,16, false),

               //large value/small area, large cointainer
               (20,20,1,5,1,5,50,100,6, false),
               (20,20,1,5,1,5,50,100,8, false),
               (20,20,1,5,1,5,50,100,10, false),
               (20,20,1,5,1,5,50,100,12, false),
               (20,20,1,5,1,5,50,100,14, false),
               (20,20,1,5,1,5,50,100,16, false),

               //small value/large area, small cointainer
               (10,10,5,9,5,9,1,3,6, false),
               (10,10,5,9,5,9,1,3,8, false),
               (10,10,5,9,5,9,1,3,10, false),
               (10,10,5,9,5,9,1,3,12, false),
               (10,10,5,9,5,9,1,3,14, false),
               (10,10,5,9,5,9,1,3,16, false),

               //large value/small area, small cointainer
               (10,10,1,5,1,5,50,100,6, false),
               (10,10,1,5,1,5,50,100,8, false),
               (10,10,1,5,1,5,50,100,10, false),
               (10,10,1,5,1,5,50,100,12, false),
               (10,10,1,5,1,5,50,100,14, false),
               (10,10,1,5,1,5,50,100,16, false),

               //sqares
               (10,10,1,10,-1,-1,1,10,6, true),
               (10,10,1,10,-1,-1,1,10,8, true),
               (10,10,1,10,-1,-1,1,10,10, true),
               (10,10,1,10,-1,-1,1,10,12, true),
               (10,10,1,10,-1,-1,1,10,14, true),
               (10,10,1,10,-1,-1,1,10,16, true),

               //sqares
               (20,20,1,10,-1,-1,1,10,6, true),
               (20,20,1,10,-1,-1,1,10,8, true),
               (20,20,1,10,-1,-1,1,10,10, true),
               (20,20,1,10,-1,-1,1,10,12, true),
               (20,20,1,10,-1,-1,1,10,14, true),
               (20,20,1,10,-1,-1,1,10,16, true)
            };

            foreach (var config in configurations)
            {
                try
                {
                    await CollectStatistics(config.containerWidth, config.containerHeight,
                        config.minItemWidth, config.minItemHeight,
                        config.minItemValue, config.maxItemWidth,
                        config.maxItemHeight, config.maxItemValue,
                        config.itemCount, 1,
                        10*60*1000, config.onlySquares,
                        configurations.IndexOf(config));
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

        public static async Task CollectStatistics(int containerWidth, int containerHeight,
            int minItemWidth, int minItemHeight, int minItemValue,
            int maxItemWidth, int maxItemHeight, int maxItemValue,
            int itemCount, int iterationCount, int timeout, bool onlySquares,
            int index)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{index} | " +
                          $"{containerWidth} {containerHeight} " +
                          $"{minItemWidth} {maxItemWidth} " +
                          $"{minItemHeight} {maxItemHeight} " +
                          $"{minItemValue} {maxItemValue} " +
                          $"{itemCount} {onlySquares}");

            sb.AppendLine();

            for (int i = 0; i < iterationCount; i++)
            {
                var cb = new ContainerBuilder()
                    .WithContainerDimensions(containerWidth, containerHeight)
                    .WithItemMinDimensions(minItemWidth, minItemHeight)
                    .WithItemMinValue(minItemValue)
                    .WithItemMaxDimensions(maxItemWidth, maxItemHeight)
                    .WithItemMaxValue(maxItemValue)
                    .WithItems(itemCount);

                var container = onlySquares ? cb.WithOnlySquares().Build() : cb.Build();

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

                sb.AppendLine();
            }

            await Task.Delay(1000);

            Directory.CreateDirectory(ResultsDirectory);
            using (var file = new StreamWriter($"{ResultsDirectory}/" +
                                               $"Result_{DateTime.Now.ToString("s").Replace(":", "-")}.txt"))
            {
                file.Write(sb);
            }
        }
    }
}
