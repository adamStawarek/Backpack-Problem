using System;

namespace BackpackProblem
{
    public class ContainerBuilder
    {
        private int _containerWidth;
        private int _containerHeight;
        private int _maxItemWidth;
        private int _maxItemHeight;
        private int _maxItemValue;
        private int _itemsCount;
        private readonly Random _random;

        public ContainerBuilder()
        {
            _random = new Random();
        }

        public ContainerBuilder WithContainerDimensions(int width, int height)
        {
            _containerWidth = width;
            _containerHeight = height;
            return this;
        }

        public ContainerBuilder WithItemMaxDimensions(int maxWidth, int maxHeight)
        {
            _maxItemHeight = maxHeight;
            _maxItemWidth = maxWidth;
            return this;
        }

        public ContainerBuilder WithItemMaxValue(int maxValue)
        {
            _maxItemValue = maxValue;
            return this;
        }

        public ContainerBuilder WithItems(int numberOfItems)
        {
            _itemsCount = numberOfItems;
            return this;
        }

        public Container Build()
        {
            var container = new Container(_containerWidth, _containerHeight);
            for (int i = 0; i < _itemsCount; i++)
            {
                int itemWidth = _random.Next(1, _maxItemWidth);
                int itemHeight = _random.Next(1, _maxItemHeight);
                int itemValue = _random.Next(1, _maxItemValue);
                container.AddItem(new Item(itemWidth, itemHeight, itemValue));
            }

            return container;
        }
    }
}
