using AfterWindowsInstaller.Core.Interfaces;
using AfterWindowsInstaller.infrastructure;

using System.IO;
using System.Text.RegularExpressions;
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
        private readonly ProgressBar _currentProgressBar;

        private CancellationTokenSource? _cts;

        private bool _onlyDownload;
        private double _totalSteps = 0;

        public ConfirmExecuteWindow(bool onlyDownload, IDownloadService downloadService, IDownloadListStorage downloadListStorage, IInstallService installService, IRemoveFilesService removeFilesService)
        {
            _downloadService = downloadService;
            _downloadListStorage = downloadListStorage;
            _installService = installService;
            _removeFilesService = removeFilesService;

            InitializeComponent();
            ToggleGridFunc();

            ConfirmProgramList.ItemsSource = downloadListStorage.GetDownloadFile();

            _commonProgressBar = TotalSteps;
            _currentProgressBar = CurrentSteps;

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
            ToggleGridFunc();
            if (!Directory.Exists(USERDATA.USER_DOWNLOADS_PATH))
                Directory.CreateDirectory(USERDATA.USER_DOWNLOADS_PATH);
            var path = USERDATA.USER_DOWNLOADS_PATH;

            _cts = new CancellationTokenSource();
            double progressvalue = 0;
            IProgress<double> commonProgress = CreateProgressBar(_commonProgressBar);
            try
            {
                foreach (var program in _downloadListStorage.DownloadList)
                {
                    if (_cts.IsCancellationRequested) break;
                    progressvalue = _downloadListStorage.DownloadList.IndexOf(program) + 1;
                    TotalStepTextBlock.ReportTextBlock($"Downloading... {progressvalue - 1}/{_totalSteps}");

                    var progress = CreateProgressBar(_currentProgressBar);
                    var item = KeyValuePair.Create(program.Name, program.Model);
                    if (item.Value.Owner != null || item.Value.Repo != null)
                        await _downloadService.DownloadFileFromGitAsync(item, path, progress, _cts.Token);
                    else if (item.Value.WingetUrl != null)
                        await _downloadService.DownloadWingetAsync(item, path, progress, _cts.Token);
                    else
                        await _downloadService.DownloadFileAllowPathAsync(item, path, progress, _cts.Token);


                    commonProgress.Report(progressvalue / _downloadListStorage.DownloadList.Count);
                    CurrentStepTextBlock.ReportTextBlock($"Downloading {program.Name}...");
                }

                commonProgress.Report(progressvalue / _downloadListStorage.DownloadList.Count);

                if (!_onlyDownload)
                {
                    foreach (var file in _downloadListStorage.DownloadList)
                    {
                        TotalStepTextBlock.ReportTextBlock($"Installing... {progressvalue - 1}/{_totalSteps}");

                        commonProgress.Report(progressvalue / _downloadListStorage.DownloadList.Count);
                        progressvalue += 1;

                        try
                        {
                            var item = KeyValuePair.Create(file.Name, file.Model);
                            CurrentStepTextBlock.ReportTextBlock($"Installing {Path.GetFileNameWithoutExtension(file.Name)}...");
                            if (!File.Exists(file.Model.FilePath)) continue;

                            await _installService.Install(item, _cts.Token);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error installing {file}: {ex.Message}", "Installation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }

                    bool isDelete = MessageBox.Show($"Do you want to delete the downloaded files?\nPath: {USERDATA.USER_DOWNLOADS_PATH}", "Delete files", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
                    if (isDelete) _removeFilesService.RemoveAllFiles().Wait();
                }

                MessageBox.Show("All successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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
