using AfterWindowsInstaller.App.Design;
using AfterWindowsInstaller.Core.Interfaces;
using AfterWindowsInstaller.infrastructure;
using AfterWindowsInstaller.infrastructure.Extensions;

using System.ComponentModel;
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
        private readonly IInstallService _installService;
        private readonly IRemoveFilesService _removeFilesService;

        private readonly ProgressBar _commonProgressBar;

        private CancellationTokenSource? _cts;
        private bool _isProcessing = false;

        private readonly bool _onlyDownload;
        private readonly double _totalSteps = 0;

        public ConfirmExecuteWindow(bool onlyDownload, IDownloadService downloadService, IDownloadListStorage downloadListStorage, IInstallService installService, IRemoveFilesService removeFilesService)
        {
            _downloadService = downloadService;
            _downloadListStorage = downloadListStorage;
            _installService = installService;
            _removeFilesService = removeFilesService;

            InitializeComponent();

            ConfirmGrid.Visibility = Visibility.Collapsed;
            ProgressGrid.Visibility = Visibility.Collapsed;

            ToggleGridFunc();

            ConfirmProgramList.ItemsSource = downloadListStorage.GetDownloadFile();

            _commonProgressBar = TotalSteps;

            _onlyDownload = onlyDownload;

            if (onlyDownload)
            {
                _totalSteps = _downloadListStorage.DownloadList.Count;
                Title.Text = "Confirm Download";
                Confirm_Message.Text = "You are about to download the following programs.";
                ConfirmButton.Content = "Download";
            }
            else
            {
                _totalSteps = _downloadListStorage.DownloadList.Count * 2;
                Title.Text = "Confirm Download and Install";
                Confirm_Message.Text = "You are about to download and install the following programs.";
                ConfirmButton.Content = "Download and Install";
            }
        }

        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            _isProcessing = true;
            ToggleGridFunc();
            var path = USERDATA.USER_DOWNLOADS_PATH.CreateDirectoryIfNotExists();

            _cts = new CancellationTokenSource();
            double progressvalue = 0;
            IProgress<double> commonProgress = CreateProgressBar(_commonProgressBar);
            try
            {
                foreach (var program in _downloadListStorage.DownloadList)
                {
                    if (_cts.IsCancellationRequested) break;
                    progressvalue = _downloadListStorage.DownloadList.IndexOf(program) + 1;

                    base.Title = $"Progress {progressvalue}/{_totalSteps}";

                    TotalStepTextBlock.ReportTextBlock($"Downloading... {progressvalue}/{_totalSteps}");
                    CurrentStepTextBlock.ReportTextBlock($"Downloading {program.Name}...");

                    var item = KeyValuePair.Create(program.Name, program.Model);
                    await DownloadAsync(item, path);

                    commonProgress.Report(progressvalue / _downloadListStorage.DownloadList.Count);
                }

                if (!_onlyDownload)
                {
                    foreach (var file in _downloadListStorage.DownloadList)
                    {
                        progressvalue += 1;
                        base.Title = $"Progress {progressvalue}/{_totalSteps}";
                        TotalStepTextBlock.ReportTextBlock($"Installing... {progressvalue}/{_totalSteps}");

                        commonProgress.Report(progressvalue / _downloadListStorage.DownloadList.Count);

                        try
                        {
                            var item = KeyValuePair.Create(file.Name, file.Model);

                            CurrentStepTextBlock.ReportTextBlock($"Installing {Path.GetFileNameWithoutExtension(file.Name)}...");

                            await InstallAsync(item, file);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error installing {file}: {ex.Message}", "Installation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    _removeFilesService.RemoveAllFiles().Wait();
                }
                if (!_cts.IsCancellationRequested) MessageBox.Show("All successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Downloaded error!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _isProcessing = false;
                this.Close();
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
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_isProcessing) СancellationProgress_Click(null, null);
            else
            {
                _cts?.Cancel();
                _cts?.Dispose();
            }

        }

        private void СancellationProgress_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("You realy want to cancel the download?", "Cancel download", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) _cts?.Cancel();
        }

        private async Task DownloadAsync(KeyValuePair<string, IDownloadUrlModel> item, string path)
        {
            if (_cts == null) return;
            if (item.Value.WingetUrl != null && _onlyDownload)
                await _downloadService.DownloadWingetAsync(item, path, _cts.Token);
            else if (item.Value.Owner != null || item.Value.Repo != null)
                await _downloadService.DownloadFileFromGitAsync(item, path, _cts.Token);
            else if (item.Value.Url != null)
                await _downloadService.DownloadFileAllowPathAsync(item, path, _cts.Token);
        }

        private async Task InstallAsync(KeyValuePair<string, IDownloadUrlModel> item, IDownloadItem file)
        {
            if (_cts == null) return;
            if (item.Value.WingetUrl != null)
            {
                await _installService.WingetInstall(item, _cts.Token);
                return;
            }
            if (!File.Exists(file.Model.FilePath)) return;

            await _installService.Install(item, _cts.Token);
        }
    }
}
