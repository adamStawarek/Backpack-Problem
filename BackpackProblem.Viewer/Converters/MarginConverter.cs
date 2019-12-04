using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using BackpackProblem.Viewer.ViewModel;
using Point = System.Drawing.Point;

namespace BackpackProblem.Viewer.Converters
{
    public class MarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Point point)
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
                }
                else if (area <= 300)
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
                return new Thickness(point.X * coef, point.Y * coef, 0, 0);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}