using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AfterWindowsInstaller.App.Design
{
    public static class Extensions
    {
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T t)
                    yield return t;

                foreach (T childOfChild in FindVisualChildren<T>(child))
                    yield return childOfChild;
            }
        }

        public static Progress<double> CreateProgressBar(ProgressBar progressBar) => new(value => progressBar.Value = value * 100);

        public static void ReportTextBlock(this TextBlock textBlock, string text)
        {
            if (textBlock == null) return;
            textBlock.Text = text;
        }
    }
}
