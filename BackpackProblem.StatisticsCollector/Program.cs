using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BackpackProblem.StatisticsCollector
{
    class Program
    {
        private static readonly string ResultsDirectory = Directory.GetCurrentDirectory() + "/Results";
        
        static void Main()
        {
            try
            {
                CollectStatistics(10, 10, 10, 10,
                        10, 8, 100, 3000, true);
            }
            catch
            {
                // ignored
#if Debug
               global::System.Console.WriteLine("Exception occured at calculating first statisics");
#endif
            }

            try
            {
                CollectStatistics(10, 10, 10, 10,
                    10, 16, 100, 5000, true);
            }
            catch
            {
                // ignored
#if Debug
               global::System.Console.WriteLine("Exception occured at calculating second statisics");
#endif
            }

            try
            {
                CollectStatistics(10, 10, 3, 3,
                    3, 8, 100, 3000, true);
            }
            catch
            {
                // ignored
#if Debug
               global::System.Console.WriteLine("Exception occured at calculating third statisics");
#endif
            }

            try
            {
                CollectStatistics(10, 10, 3, 3,
                    3, 20, 100, 5000, true);
            }
            catch
            {
                // ignored
#if Debug
               global::System.Console.WriteLine("Exception occured at calculating fourth statisics");
#endif
            }
        }

        public static void CollectStatistics(int containerWidth, int containerHeight,
            int maxItemWidth, int maxItemHeight, int maxItemValue, int itemCount,
            int iterationCount, int timeout, bool shuffle)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"({nameof(containerWidth)}){containerWidth} | ({nameof(containerHeight)}){containerHeight} |" +
                          $" ({nameof(maxItemWidth)}){maxItemWidth} | ({nameof(maxItemHeight)}){maxItemHeight} |" +
                          $" ({nameof(maxItemValue)}){maxItemValue} | ({nameof(itemCount)}){itemCount} |" +
                          $" ({nameof(iterationCount)}){iterationCount} | ({nameof(timeout)}){timeout}");
            
            sb.AppendLine();

            for (int i = 0; i < iterationCount; i++)
            {
                var container = new ContainerBuilder()
                    .WithContainerDimensions(containerWidth, containerHeight)
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
                        ? $"[{task.Result?.Items.Count} {task.Result?.TotalValue} {task.Result?.TotalArea} {elapsedMs}]"
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
