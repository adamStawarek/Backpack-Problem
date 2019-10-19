namespace BackpackProblem
{
    public class Space
    {
        public Space(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Area => Width * Height;

        public bool CanFit(Item item)
        {
            return (item.Width <= Width && item.Height <= Height) ||
                   (item.Height <= Width && item.Width <= Height);
        }

    }
}