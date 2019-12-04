using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BackpackProblem.Viewer.Converters
{
    public class IntegerToBrushesConverter : IValueConverter
    {
        private List<Color> colors = new List<Color>
        {
            Color.FromRgb(128, 0, 0),
            Color.FromRgb(170, 110, 40),
            Color.FromRgb(128, 128, 0),
            Color.FromRgb(0, 128, 128),
            Color.FromRgb(0, 0, 128),
            Color.FromRgb(230, 25, 75),
            Color.FromRgb(245, 130, 48),
            Color.FromRgb(210, 245, 60),
            Color.FromRgb(60, 180, 75),
            Color.FromRgb(70, 240, 240),
            Color.FromRgb(0, 130, 200),
            Color.FromRgb(145, 30, 180),
            Color.FromRgb(240, 50, 230),
            Color.FromRgb(128, 128, 128),
            Color.FromRgb(250, 190, 190),
            Color.FromRgb(255, 215, 180),
            Color.FromRgb(170, 255, 195),
            Color.FromRgb(230, 190, 255),
            Color.FromRgb(200, 205, 0),
            Color.FromRgb(60, 190, 200),
            Color.FromRgb(0, 60, 60),
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int i)
            {
                var c = colors[i % colors.Count];
                return new SolidColorBrush(System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B));
            }
               
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private Color GetRandColor(int index)
        {
            byte red = 0;
            byte green = 0;
            byte blue = 0;

            for (int t = 0; t <= index / 8; t++)
            {
                int index_a = (index + t) % 8;
                int index_b = index_a / 2;

                //Color writers, take on values of 0 and 1
                int color_red = index_a % 2;
                int color_blue = index_b % 2;
                int color_green = ((index_b + 1) % 3) % 2;

                int add = 255 / (t + 1);

                red = (byte)(red + color_red * add);
                green = (byte)(green + color_green * add);
                blue = (byte)(blue + color_blue * add);
            }

            Color color = Color.FromRgb(red,green,blue);
            return color;
        }
    }
}