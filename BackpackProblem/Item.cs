using System.Drawing;

namespace BackpackProblem
{
    public class Item
    {
        public int Id { get; }
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
            Id = -1;
        }

        public Item(int width, int height, int value, int id)
        {
            Width = width;
            Height = height;
            Value = value;
            Id = id;
        }

        public Item Clone()
        {
            return new Item(Width, Height, Value, Id)
            {
                DimensionsSwapped = DimensionsSwapped,
                UpperLeftCornerPoint = UpperLeftCornerPoint
            };
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