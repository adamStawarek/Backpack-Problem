using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace BackpackProblem
{
    public class Container
    {
        public int Width { get; }
        public int Height { get; }
        public List<Item> Items { get; }
        public List<Subset> Subsets { get; set; }
        public int Area => Width * Height;
        public int[,] Fields { get; set; }

        public Container(int width, int height)
        {
            Width = width;
            Height = height;
            Items = new List<Item>();
            Subsets = new List<Subset>();
            Fields = new int[height, width];
        }

        public void AddItem(Item item)
        {
            if ((item.Height <= Height && item.Width <= Width) || (item.Height <= Width && item.Width <= Height))
                Items.Add(item);
        }

        public void AddItems(IEnumerable<Item> items)
        {
            foreach (var item in items)
            {
                AddItem(item);
            }
        }

        public void AddSubset(Subset subset)
        {
            if (subset.TotalArea <= Area)
                Subsets.Add(subset);
        }

        public void SortSubsets()
        {
            Subsets.Sort(delegate (Subset A, Subset B)
            {
                if (A.TotalValue == B.TotalValue)
                {
                    return 0;
                }

                if (A.TotalValue > B.TotalValue)
                {
                    return -1;
                }

                return 1;
            });
        }

        public Subset FindBestSubset()
        {
            return Subsets.FirstOrDefault(CheckIfSubsetFits);
        }

        public async Task<Subset> FindBestSubsetAsync()
        {
            int numberOfThreads = Environment.ProcessorCount;
            for (int i = 0; i < Subsets.Count; i += numberOfThreads)
            {
                var tasks = new Task<Subset>[numberOfThreads];
                for (int j = 0; j < numberOfThreads; j++)
                {
                    var subset = Subsets[i + j];
                    Task<Subset> task = Task.Factory.StartNew(() => CheckIfSubsetFits(subset)
                        ? subset : null);
                    tasks[j] = task;
                }
                var results = await Task.WhenAll(tasks);

                if (results.Any(r => r != null))
                {
                    return results.First(r => r != null);
                }
            }

            return null;
        }

        public bool CheckIfSubsetFits(Subset subset)
        {
            return CanFit(new Stack<Item>(subset.Items), this);
        }

        public bool CheckIfItemFits(Item item, Point point)
        {
            if (item.Width + point.X > Width || item.Height + point.Y > Height) return false;

            for (int i = 0; i < item.Width; i++)
            {
                for (int j = 0; j < item.Height; j++)
                {
                    if (Fields[point.Y + j, point.X + i] == 1)
                        return false;
                }
            }

            return true;
        }

        public bool CanFit(Stack<Item> items, Container container)
        {
            var item = items.Pop();
            var places = container.GetPlacesForItems(item).ToArray();
            var placesWhenDimensionsSwapped = item.Width.Equals(item.Height)
                ? Array.Empty<Point>()
                : container.GetPlacesForItems(new Item(item.Height, item.Width, item.Value)).ToArray();


            if (!places.Any() && !placesWhenDimensionsSwapped.Any())
            {
#if DEBUG
                Console.WriteLine($"Can't fit item: {item}");
#endif
                return false;
            }

            if (!items.Any())
            {
                var place = places.Any() ? places.First() : placesWhenDimensionsSwapped.First();
#if DEBUG
                Console.WriteLine(item + ": " + place); 
#endif
                item.UpperLeftCornerPoint = place;
                return true;
            };

            foreach (var place in places)
            {
                var newContainer = container.Clone();
                newContainer.Update(item, place);
                if (CanFit(new Stack<Item>(items), newContainer))
                {
#if DEBUG
                    Console.WriteLine(item + ": " + place); 
#endif
                    item.UpperLeftCornerPoint = place;
                    return true;
                }
            }

            if (placesWhenDimensionsSwapped.Any())
            {
                item.SwapDimensions();
                foreach (var place in placesWhenDimensionsSwapped)
                {
                    var newContainer = container.Clone();
                    newContainer.Update(item, place);
                    if (CanFit(new Stack<Item>(items), newContainer))
                    {
#if DEBUG
                        Console.WriteLine(item + ": " + place+" (swapped)"); 
#endif
                        item.UpperLeftCornerPoint = place;
                        return true;
                    }
                }
                item.SwapDimensions();
            }

            return false;
        }

        public void Update(Item item, Point point)
        {
#if DEBUG
            Console.WriteLine($"Update item: {item} at {point}");
#endif
            for (int i = 0; i < item.Width; i++)
            {
                for (int j = 0; j < item.Height; j++)
                {
                    Fields[point.Y + j, point.X + i] = 1;
                }
            }
        }

        public Container Clone()
        {
            var newContainer = new Container(this.Width, this.Height)
            {
                Fields = this.Fields.Clone() as int[,]
            };
            return newContainer;
        }

        public IEnumerable<Point> GetPlacesForItems(Item item)
        {
            var upperLeftCorners = new List<Point>();
            for (int i = 0; i <= Width - item.Width; i++)
            {
                for (int j = 0; j <= Height - item.Height; j++)
                {
                    upperLeftCorners.Add(new Point(i, j));
                }
            }

            return upperLeftCorners.Where(p => CheckIfItemFits(item, p));
        }

        public void GeneratePowerSet()
        {
            int n = Items.Count;

            // Run a loop from 0 to 2^n
            for (int i = 0; i < 1 << n; i++)
            {
                var subset = new Subset();
                int m = 1; // m is used to check set bit in binary representation.
                for (int j = 0; j < n; j++)
                {
                    if ((i & m) > 0)
                    {
                        subset.Items.Add(Items[j]);
                    }
                    m <<= 1;
                }
                AddSubset(subset);
            }
        }

        public override string ToString()
        {
            return $"Width: {Width}; Height: {Height}; Area: {Area};Total items: {Items.Count}; Total items area: {Items.Sum(i => i.Area)}";
        }
    }
}