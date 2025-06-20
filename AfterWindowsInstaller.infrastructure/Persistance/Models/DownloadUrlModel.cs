using AfterWindowsInstaller.Core.Interfaces;

namespace AfterWindowsInstaller.infrastructure.Persistance.Models
{
    public class DownloadUrlModel : IDownloadUrlModel
    {
        public string? Url { get; set; }
        public string? Repo { get; set; }
        public string? Owner { get; set; }
        public string Description { get; set; } = string.Empty;
        public string IconPath { get; set; } = string.Empty;
        public string? Verb { get; set; }
        public string? Arguments { get; set; }
        public string? FilePath { get;set; }
    }
}
