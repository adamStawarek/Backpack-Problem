using System;
using System.Collections.Generic;

namespace BackpackProblem
{
    public class ItemDimensionalComparer : IEqualityComparer<Item>
    {
        public bool Equals(Item i1, Item i2)
        {
            return (i1.Height == i2.Height && i1.Width == i2.Width) ||
                   (i1.Height == i2.Width && i1.Width == i2.Height);
        }

        public int GetHashCode(Item obj)
        {
            throw new NotImplementedException();
        }
    }
}
