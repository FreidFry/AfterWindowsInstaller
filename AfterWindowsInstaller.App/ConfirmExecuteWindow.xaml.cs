using AfterWindowsInstaller.Core.Interfaces;
using AfterWindowsInstaller.infrastructure;

using System.IO;
using System.Windows;
using System.Windows.Controls;

using static AfterWindowsInstaller.App.Design.Extensions;

namespace AfterWindowsInstaller.App
{
    /// <summary>
    /// Interaction logic for ConfirmExecuteWindow.xaml
    /// </summary>
    public partial class ConfirmExecuteWindow : Window
    {
        private readonly IDownloadService _downloadService;
        private readonly IDownloadListStorage _downloadListStorage;
        private readonly ProgressBar _commonProgressBar;
        private readonly ProgressBar _currentProgressBar;

        private CancellationTokenSource? _cts;

        public ConfirmExecuteWindow(IDownloadService downloadService, IDownloadListStorage downloadListStorage)
        {
            _downloadService = downloadService;
            _downloadListStorage = downloadListStorage;

            InitializeComponent();
            ToggleGridFunc();

            ConfirmProgramList.ItemsSource = downloadListStorage.GetDownloadFile();

            _commonProgressBar = TotalSteps;
            _currentProgressBar = CurrentSteps;
        }

        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleGridFunc();
            var path = USERDATA.USER_DOWNLOADS_PATH;

            _cts = new CancellationTokenSource();

            IProgress<double> commonProgress = CreateProgressBar(_commonProgressBar);
            try
            {
                foreach (var program in _downloadListStorage.DownloadList)
                {
                    var progress = CreateProgressBar(_currentProgressBar);
                    var item = KeyValuePair.Create(program.Name, program.Model);
                    if (item.Value.Owner != null || item.Value.Repo != null)
                        await _downloadService.DownloadFileFromGitAsync(item, path, progress, _cts.Token);
                    else
                        await _downloadService.DownloadFileAllowPathAsync(item, path, progress, _cts.Token);
                    commonProgress.Report(1.0 / _downloadListStorage.DownloadList.Count);
                }
                MessageBox.Show("All files have been downloaded successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Downloaded canceled!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            finally
            {
                this.Close();
                ToggleGridFunc();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) => this.Close();

        private void ToggleGridFunc()
        {
            if (ConfirmGrid.Visibility == Visibility.Visible)
            {
                ConfirmGrid.Visibility = Visibility.Collapsed;
                ProgressGrid.Visibility = Visibility.Visible;
            }
            else
            {
                ConfirmGrid.Visibility = Visibility.Visible;
                ProgressGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void CancelProgress_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("You realy want to cancel the download?", "Cancel download", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) _cts?.Cancel();
        }
    }
}
