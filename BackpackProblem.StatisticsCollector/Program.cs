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
                int minItemHeight, int maxItemHeight, int minItemValue, int maxItemValue, int itemCount, bool onlySquares)>
            {
                //small value/large area , large cointainer
                (20,20,5,9,5,9,1,3,5, false),
                (20,20,5,9,5,9,1,3,10, false),
                (20,20,5,9,5,9,1,3,15, false),
                (20,20,5,9,5,9,1,3,20, false),
                (20,20,5,9,5,9,1,3,25, false),

               //large value/small area, large cointainer
               (20,20,1,5,1,5,50,100,5, false),
               (20,20,1,5,1,5,50,100,10, false),
               (20,20,1,5,1,5,50,100,15, false),
               (20,20,1,5,1,5,50,100,20, false),
               (20,20,1,5,1,5,50,100,25, false),

               //small value/large area, small cointainer
               (10,10,5,9,5,9,1,3,5, false),
               (10,10,5,9,5,9,1,3,10, false),
               (10,10,5,9,5,9,1,3,15, false),
               (10,10,5,9,5,9,1,3,20, false),
               (10,10,5,9,5,9,1,3,25, false),

               //large value/small area, small cointainer
               (10,10,1,5,1,5,50,100,5, false),
               (10,10,1,5,1,5,50,100,10, false),
               (10,10,1,5,1,5,50,100,15, false),
               (10,10,1,5,1,5,50,100,20, false),
               (10,10,1,5,1,5,50,100,25, false),

               //sqares
               (10,10,1,10,-1,-1,1,10,5, true),
               (10,10,1,10,-1,-1,1,10,10, true),
               (10,10,1,10,-1,-1,1,10,15, true),
               (10,10,1,10,-1,-1,1,10,20, true),
               (10,10,1,10,-1,-1,1,10,25, true),

               //sqares
               (20,20,1,10,-1,-1,1,10,5, true),
               (20,20,1,10,-1,-1,1,10,10, true),
               (20,20,1,10,-1,-1,1,10,15, true),
               (20,20,1,10,-1,-1,1,10,20, true),
               (20,20,1,10,-1,-1,1,10,25, true),
            };

            foreach (var config in configurations)
            {
                try
                {
                    CollectStatistics(config.containerWidth, config.containerHeight,
                        config.minItemWidth, config.minItemHeight,
                        config.minItemValue, config.maxItemWidth,
                        config.maxItemHeight, config.maxItemValue,
                        config.itemCount, 3,
                        1000, config.onlySquares);
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
            int itemCount, int iterationCount, int timeout, bool onlySquares)
        {
            var sb = new StringBuilder();
            sb.AppendLine("containerArea itemsCount resultItemsCount resultTotalValue averageItemAreaToContainerAreaRatio averageOfItemValueToItemAreaRatio operations time");

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

            Directory.CreateDirectory(ResultsDirectory);
            using (var file = new StreamWriter($"{ResultsDirectory}/" +
                                               $"Result_{DateTime.Now.ToString("s").Replace(":", "-")}.txt"))
            {
                file.Write(sb);
            }
        }
    }
}
