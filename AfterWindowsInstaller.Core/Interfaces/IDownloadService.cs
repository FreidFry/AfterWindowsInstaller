using System.Windows.Controls;

namespace AfterWindowsInstaller.Core.Interfaces
{
    public interface IDownloadService
    {
        Task DownloadFileFromGitAsync(string owner, string repo, string outputPath, IProgress<double> currentBar, CancellationToken cancellationToken);
        Task DownloadFileAllowPathAsync(string url, string outputPath, IProgress<double> currentBar, CancellationToken cancellationToken);
    }
}