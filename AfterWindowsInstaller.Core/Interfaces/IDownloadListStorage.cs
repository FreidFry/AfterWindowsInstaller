namespace AfterWindowsInstaller.Core.Interfaces
{
    public interface IDownloadListStorage
    {
        List<string> DownloadList { get; set; }

        void AddToDownloadList(string url);
        void RemoveFromDownloadList(string url);
        List<string> GetDownloadListFromFile();
    }
}
