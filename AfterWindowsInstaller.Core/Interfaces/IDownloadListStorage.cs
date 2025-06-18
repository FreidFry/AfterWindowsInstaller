using System.Collections.ObjectModel;

namespace AfterWindowsInstaller.Core.Interfaces
{
    public interface IDownloadListStorage
    {
        ObservableCollection<IDownloadItem> DownloadList { get; }

        void AddToDownloadList(IDownloadItem programModel);
        void RemoveFromDownloadList(string programName);
        ObservableCollection<IDownloadItem> GetDownloadFile();
    }
}
