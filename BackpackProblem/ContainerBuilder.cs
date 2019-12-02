using System;

namespace BackpackProblem
{
    public class ContainerBuilder
    {
        private readonly Random _random;
        private int _containerWidth;
        private int _containerHeight;
        private int _maxItemWidth;
        private int _maxItemHeight;
        private int _maxItemValue;
        private int _itemsCount;
        private int _minItemHeight;
        private int _minItemWidth;
        private int _minItemValue;
        private bool _withSquares;


        public ContainerBuilder()
        {
            _random = new Random();
            _minItemWidth = 1;
            _maxItemHeight = 1;
            _minItemValue = 1;
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

        public ContainerBuilder WithItemMinDimensions(int minWidth, int minHeight)
        {
            _minItemHeight = minHeight;
            _minItemWidth = minWidth;
            return this;
        }

        public ContainerBuilder WithItemMaxValue(int maxValue)
        {
            _maxItemValue = maxValue;
            return this;
        }

        public ContainerBuilder WithItemMinValue(int minValue)
        {
            _minItemValue = minValue;
            return this;
        }

        public ContainerBuilder WithOnlySquares()
        {
            _withSquares = true;
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
                int itemWidth, itemHeight;
                if (_withSquares)
                {
                    itemHeight = itemWidth = _random.Next(_minItemWidth, _maxItemWidth);
                }
                else
                {
                    itemWidth = _random.Next(_minItemWidth, _maxItemWidth);
                    itemHeight = _random.Next(_minItemHeight, _maxItemHeight);
                }
                int itemValue = _random.Next(_minItemValue, _maxItemValue);
                container.AddItem(new Item(itemWidth, itemHeight, itemValue, i));
            }

            return container;
        }
    }
}
