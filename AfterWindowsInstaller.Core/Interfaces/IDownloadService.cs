using System.Collections;
using System.Windows.Controls;

namespace AfterWindowsInstaller.Core.Interfaces
{
    public interface IDownloadService
    {
        Task DownloadFileFromGitAsync(KeyValuePair<string, IDownloadUrlModel> DownloadUrlModel, string outputPath, IProgress<double> currentBar, CancellationToken cancellationToken);
        Task DownloadFileAllowPathAsync(KeyValuePair<string, IDownloadUrlModel> DownloadUrlModel, string outputPath, IProgress<double> currentBar, CancellationToken cancellationToken);
    }
}