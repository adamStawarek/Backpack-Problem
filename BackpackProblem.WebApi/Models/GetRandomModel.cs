namespace BackpackProblem.WebApi.Models
{
    public class GetRandomModel
    {
        public int MaxItemWidth { get; set; }
        public int MaxItemHeight { get; set; }
        public int MaxItemValue { get; set; }
        public int ContainerWidth { get; set; }
        public int ContainerHeight { get; set; }
        public int NumberOfItems { get; set; }
    }
}
