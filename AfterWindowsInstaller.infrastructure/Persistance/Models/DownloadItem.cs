using AfterWindowsInstaller.Core.Interfaces;

namespace AfterWindowsInstaller.infrastructure.Persistance.Models
{
    public class DownloadItem : IDownloadItem
    {
        public string Name { get; set; } = "Empty";
        public IDownloadUrlModel Model { get; set; } = new DownloadUrlModel
        {
            Url = string.Empty,
            Repo = string.Empty,
            Owner = string.Empty,
            WingetUrl = string.Empty,
            Description = string.Empty,
            IconPath = string.Empty,
            Verb = null,
            Arguments = null,
            FilePath = null
        };
    }
}
