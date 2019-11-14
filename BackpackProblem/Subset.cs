using System.Collections.Generic;
using System.Linq;

namespace BackpackProblem
{
    public class Subset
    {
        public Subset()
        {
            Items = new List<Item>();
        }

        public List<Item> Items { get; set; }
        public int TotalArea => Items.Sum(i => i.Area);
        public int TotalValue => Items.Sum(i => i.Value);

        public override string ToString()
        {
            return $"[{ string.Join("\n", Items.Select(i => $"( {i.ToLongString()} )")) }]";
        }
    }
}