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

        public int Width { get; }
        public int Height { get; }
        public int Value { get; }

        public int Area => Width * Height;
        public double Ratio => (double)Area / Value;

        public int SelectionCounter { get; set; }
        public Space Space { get; set; }

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