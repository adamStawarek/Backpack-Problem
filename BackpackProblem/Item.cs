using System.Drawing;

namespace BackpackProblem
{
    public class Item
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Value { get; }
        public int Area => Width * Height;
        public Point UpperLeftCornerPoint { get; set; }
        public bool DimensionsSwapped { get; set; }

        public Item(int width, int height, int value)
        {
            Width = width;
            Height = height;
            Value = value;
        }

        public void SwapDimensions()
        {
            var tmp = Width;
            Width = Height;
            Height = tmp;
            DimensionsSwapped = !DimensionsSwapped;
        }

        public string ToLongString()
        {
            return $"Width: {Width}; Height: {Height}; Value: {Value}; Area: {Area};";
        }

        public override string ToString()
        {
            return $"({Width}, {Height}, {Value})";
        }
    }
}