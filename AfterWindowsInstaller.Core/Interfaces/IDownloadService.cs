namespace AfterWindowsInstaller.Core.Interfaces
{
    public interface IDownloadService
    {
        Task DownloadFileFromGitAsync(KeyValuePair<string, IDownloadUrlModel> DownloadUrlModel, string outputPath, CancellationToken cancellationToken);
        Task DownloadFileAllowPathAsync(KeyValuePair<string, IDownloadUrlModel> DownloadUrlModel, string outputPath, CancellationToken cancellationToken);
        Task DownloadWingetAsync(KeyValuePair<string, IDownloadUrlModel> downloadUrlModel, string outputPath, CancellationToken cancellationToken);
    }
}