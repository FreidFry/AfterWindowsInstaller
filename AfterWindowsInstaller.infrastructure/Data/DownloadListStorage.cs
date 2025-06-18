using AfterWindowsInstaller.Core.Interfaces;

using System.Collections.ObjectModel;

namespace AfterWindowsInstaller.infrastructure.Data
{
    public class DownloadListStorage : IDownloadListStorage
    {
        public ObservableCollection<IDownloadItem> DownloadList { get; } = [];

        public void AddToDownloadList(IDownloadItem programModel)
            => DownloadList.Add(programModel);

        public void RemoveFromDownloadList(string programName)
        {
            var item = DownloadList.FirstOrDefault(x => x.Name == programName);
            if (item != null) DownloadList.Remove(item);
        }

        public ObservableCollection<IDownloadItem> GetDownloadFile() => DownloadList;
    }
}
