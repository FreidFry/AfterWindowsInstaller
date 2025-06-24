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
                var uriString = string.Empty;
                if (relativePath.StartsWith("http")) uriString = relativePath;
                else
                {
                    string assemblyName = "AfterWindowsInstaller.App";

                    var path = relativePath.Replace('\\', '/');
                    uriString = $"pack://application:,,,/{assemblyName};component/{path}";
                }
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(uriString);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    return bitmap;
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
