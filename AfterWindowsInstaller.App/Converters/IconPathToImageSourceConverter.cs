using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace AfterWindowsInstaller.App.Converters
{
    public class IconPathToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Diagnostics.Debug.WriteLine("IconPathToImageSourceConverter called with value: " + value);
            if (value is string relativePath && !string.IsNullOrEmpty(relativePath))
            {
                string assemblyName = "AfterWindowsInstaller.App";
                
                var path = relativePath.Replace('\\', '/');
                var uriString = $"pack://application:,,,/{assemblyName};component/{path}";
                try
                {
                    return new BitmapImage(new Uri(uriString));
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
