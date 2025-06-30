namespace AfterWindowsInstaller.Core.Interfaces
{
    public interface IInstallService
    {
        Task Install(KeyValuePair<string, IDownloadUrlModel> item, CancellationToken cancellationToken);
        Task WingetInstall(KeyValuePair<string, IDownloadUrlModel> item0, CancellationToken cancellationToken);
    }
}
