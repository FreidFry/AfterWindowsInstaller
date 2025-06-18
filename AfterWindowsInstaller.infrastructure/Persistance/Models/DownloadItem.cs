using AfterWindowsInstaller.Core.Interfaces;

namespace AfterWindowsInstaller.infrastructure.Persistance.Models
{
    public class DownloadItem : IDownloadItem
    {
        public string Name { get; set; }
        public IDownloadUrlModel Model { get; set; }
    }
}
