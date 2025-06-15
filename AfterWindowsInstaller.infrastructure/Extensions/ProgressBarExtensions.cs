using System.Windows.Controls;

namespace AfterWindowsInstaller.infrastructure.Extensions
{
    public static class ProgressBarExtensions
    {
        public static Progress<double> CreateProgressBar(ProgressBar progressBar) => new(value => progressBar.Value = value * 100);
    }
}
