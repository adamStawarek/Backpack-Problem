using System;
using System.Globalization;
using System.Windows.Data;
using BackpackProblem.Viewer.ViewModel;

namespace BackpackProblem.Viewer.Converters
{
    public class DimensionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var locator = new ViewModelLocator();
            if (locator.Main.Container == null) return null;

            var area = locator.Main.Container.Area;
            var coef = 0;
            if (area <= 100)
            {
                coef = 40;
            }
            else if (area <= 200)
            {
                coef = 30;
            }else if (area <= 300)
            {
                coef = 20;
            }
            else if (area <= 400)
            {
                coef = 15;
            }
            else
            {
                coef = 10;
            }

            if (value is int val)
            {
                return val * coef;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}