using System;
using System.Collections.Generic;
using System.Linq;

namespace BackpackProblem
{
    public class Container
    {
        public Container(int width, int height)
        {
            Width = width;
            Height = height;
            Items = new List<Item>();
            Subsets = new List<Subset>();
        }

        public int Width { get; }
        public int Height { get; }
        public List<Item> Items { get; }
        public List<Subset> Subsets { get; set; }
        public int Area => Width * Height;

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
            if (subset.TotalArea < Area)
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
                else if (A.TotalValue > B.TotalValue)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            });
        }

        public (bool canFit, int? spaceRemaining) CheckIfSubsetFits(Subset subset)
        {
            var spaces = new List<Space> { new Space(0, 0, Width, Height) };
            int counter = 0;
            
            foreach (var item in subset.Items.OrderByDescending(i => i.Area))
            {
                var space = spaces.Where(s => s.CanFit(item)).OrderBy(s=>s.Area).FirstOrDefault();
                if (space == null)
                {
                    return (false, null); //item does not fit space
                }
                else
                {
                    item.Space = new Space(space.X, space.Y,space.Width,space.Height);
                    item.SelectionCounter = ++counter;
                    if ((space.Width == item.Width && space.Height == item.Height) ||
                        (space.Height == item.Width && space.Width == item.Height))
                    {
                        spaces.Remove(space); //item cover all space
                    }
                    else if (item.Height == space.Height)
                    {
                        space.Width -= item.Width;
                        space.X += item.Width;
                    }
                    else if (item.Width == space.Width)
                    {
                        space.Height -= item.Height;
                        space.Y += item.Height;
                    }
                    else
                    {
                        spaces.Add(new Space(space.X, space.Y+item.Height, item.Width, space.Height - item.Height));
                        spaces.Add(new Space(space.X + item.Width, space.Y, space.Width- item.Width, space.Height));
                        spaces.Remove(space);
                    }
                }
            }

            return (true, spaces.Sum(s => s.Area));
        }
        public Subset FindBestSubset()
        {
            return Subsets.FirstOrDefault(s => CheckIfSubsetFits(s).canFit);
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