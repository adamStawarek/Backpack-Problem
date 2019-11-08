using System.Drawing;

namespace BackpackProblem
{
    public class Item
    {
        public Item(int width, int height, int value)
        {
            Width = width;
            Height = height;
            Value = value;
        }

        public int Width { get; set; }
        public int Height { get; set; }
        public int Value { get; }

        public int Area => Width * Height;
        public double Ratio => (double)Area / Value;

        public Point UpperLeftCornerPoint { get; set; }
        public bool DimensionsSwaped { get; set; }

        public void SwapDimensions()
        {
            var tmp = Width;
            Width = Height;
            Height = tmp;
            DimensionsSwaped = !DimensionsSwaped;
        }

        public string ToLongString()
        {
            return $"Width: {Width}; Height: {Height}; Value: {Value}; Area: {Area}; Ratio: {Ratio}";
        }

        public override string ToString()
        {
            return $"({Width}, {Height}, {Value})";
        }
    }
}