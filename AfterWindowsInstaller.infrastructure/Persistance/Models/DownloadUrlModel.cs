using AfterWindowsInstaller.Core.Interfaces;

namespace AfterWindowsInstaller.infrastructure.Persistance.Models
{
    public class DownloadUrlModel : IDownloadUrlModel
    {
        #region url

        public string? Url { get; set; }
        #endregion

        #region github
        public string? Repo { get; set; }
        public string? Owner { get; set; }
        #endregion

        #region Winget
        public string? WingetUrl { get; set; }
        #endregion
        public string Description { get; set; } = string.Empty;
        public string IconPath { get; set; } = string.Empty;

        #region Installation Command
        public string? Verb { get; set; }
        public string? Arguments { get; set; }
        #endregion
        public string? FilePath { get;set; }
    }
}
