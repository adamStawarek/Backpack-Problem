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
        public List<Subset> Subsets { get; }
        public List<Item> Items { get; private set; }
        public List<Item> AllItems { get; private set; }
        public int[,] Fields { get; private set; }
        public int Area => Width * Height;
        public int Counter { get; set; }

        public Container(int width, int height)
        {
            Width = width;
            Height = height;
            Items = new List<Item>();
            AllItems = new List<Item>();
            Subsets = new List<Subset>();
            Fields = new int[height, width];
            Counter = 0;
        }

        public void AddItem(Item item)
        {
            Counter++;
            AllItems.Add(item);
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
            if (subset.TotalArea > Area) return;

            var duplicate = Subsets.FirstOrDefault(s =>
                s.Items.SequenceEqual(subset.Items, new ItemDimensionalComparer()));

            if (duplicate != null)
            {
                if (duplicate.TotalValue >= subset.TotalValue) return;

                Subsets.Remove(duplicate);
                Subsets.Add(subset);
            }
            else
            {
                Subsets.Add(subset);
            }
        }

        public void SortSubsets()
        {
            Subsets.Sort(delegate (Subset A, Subset B)
            {
                Counter++;
                if (A.TotalValue == B.TotalValue)
                {
                    if (A.TotalArea == B.TotalArea)
                        return 0;

                    if (A.TotalArea > B.TotalArea)
                        return 1;

                    return -1;
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
            foreach (var subset in Subsets)
            {
                Counter++;
                var result = CheckIfSubsetFits(subset);
                if (result.canFit)
                {
                    subset.Items = result.items.ToList();
                    return subset;
                }
            }
            return null;
        }

        public async Task<Subset> FindBestSubsetAsync()
        {
            int numberOfThreads = 4;
            for (int i = 0; i < Subsets.Count; i += numberOfThreads)
            {
                var tasks = new Task<(Subset subset, IEnumerable<Item> items)>[numberOfThreads];
                for (int j = 0; j < numberOfThreads; j++)
                {
                    var subset = Subsets[i + j];
                    Task<(Subset, IEnumerable<Item>)> task = Task.Factory.StartNew(() =>
                        {
                            var (canFit, items) = CheckIfSubsetFits(subset);
                            return (subset, items);
                        });
                    tasks[j] = task;
                }
                var results = await Task.WhenAll(tasks);

                if (results.Any(r => r.items != null))
                {
                    var result = results.First(r => r.items != null);
                    result.subset.Items = result.items.ToList();
                    return result.subset;
                }
            }

            return null;
        }

        public (bool canFit, IEnumerable<Item> items) CheckIfSubsetFits(Subset subset)
        {
            return (CanFit(new Stack<Item>(subset.Items.Select(i => i.Clone())), this, out List<Item> changedItems), changedItems);
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

        public bool CanFit(Stack<Item> items, Container container, out List<Item> changedItems)
        {
            Counter++;
            var item = items.Pop();
            var places = container.GetPlacesForItem(item).ToArray();
            var placesWhenDimensionsSwapped = item.Width.Equals(item.Height)
                ? Array.Empty<Point>()
                : container.GetPlacesForItem(new Item(item.Height, item.Width, item.Value)).ToArray();


            if (!places.Any() && !placesWhenDimensionsSwapped.Any())
            {
#if DEBUG
                Console.WriteLine($"Can't fit item: {item}");
#endif
                changedItems = null;
                return false;
            }

            if (!items.Any())
            {
                var place = places.Any() ? places.First() : placesWhenDimensionsSwapped.First();
#if DEBUG
                Console.WriteLine(item + ": " + place + (places.Any() ? "" : " (swapped)"));
#endif
                item.UpperLeftCornerPoint = place;
                changedItems = new List<Item> { item };
                return true;
            };

            foreach (var place in places)
            {
                var newContainer = container.Clone();
                newContainer.Update(item, place);
                if (newContainer.CanFit(new Stack<Item>(items.Select(i => i.Clone())), newContainer, out changedItems))
                {
#if DEBUG
                    Console.WriteLine(item + ": " + place);
#endif
                    item.UpperLeftCornerPoint = place;
                    changedItems.Add(item);
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
                    if (newContainer.CanFit(new Stack<Item>(items.Select(i => i.Clone())), newContainer, out changedItems))
                    {
#if DEBUG
                        Console.WriteLine(item + ": " + place + " (swapped)");
#endif
                        item.UpperLeftCornerPoint = place;
                        changedItems.Add(item);
                        return true;
                    }
                }
                item.SwapDimensions();
            }

            changedItems = null;
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

        public void Shuffle()
        {
            var oldAllItems = new List<Item>(this.AllItems);
            while (oldAllItems.SequenceEqual(this.AllItems))
            {
                this.AllItems.Sort(((i1, i2) => Guid.NewGuid().CompareTo(Guid.NewGuid())));
            }

            var oldItems = new List<Item>(this.Items);
            while (oldItems.SequenceEqual(this.Items))
            {
                this.Items.Sort(((i1, i2) => Guid.NewGuid().CompareTo(Guid.NewGuid())));
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

        public Container CloneWithItems()
        {
            var newContainer = new Container(this.Width, this.Height)
            {
                Fields = this.Fields.Clone() as int[,],
                AllItems = this.AllItems.Select(i => i.Clone()).ToList(),
                Items = this.Items.Select(i => i.Clone()).ToList()
            };
            return newContainer;
        }

        public IEnumerable<Point> GetPlacesForItem(Item item)
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
                    Counter++;
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