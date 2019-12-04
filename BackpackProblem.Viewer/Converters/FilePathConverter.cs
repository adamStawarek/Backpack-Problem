using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace BackpackProblem.Viewer.Converters
{
    public class FilePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string path)
            {
                return path.Split(@"\").Last();
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
