using AfterWindowsInstaller.Core.Interfaces;

namespace AfterWindowsInstaller.infrastructure.Data
{
    public class DownloadListStorage : IDownloadListStorage
    {
        public List<string> DownloadList { get; set; } = [];

        public void AddToDownloadList(string url) => DownloadList.Add(url);
        public void RemoveFromDownloadList(string url) => DownloadList.Remove(url);
        public List<string> GetDownloadListFromFile() => DownloadList;

    }
}
