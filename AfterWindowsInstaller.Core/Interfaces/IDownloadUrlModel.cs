namespace AfterWindowsInstaller.Core.Interfaces
{
    public interface IDownloadUrlModel
    {
        /// <summary>
        /// Gets the URL of the repositories.
        /// </summary>
        string? Url { get; }

        /// <summary>
        /// Gets the repository name.
        /// </summary>
        string? Repo { get; }

        /// <summary>
        /// Gets the owner of the repository.
        /// </summary>
        string? Owner { get; }

        /// <summary>
        /// Description of the repository.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Icon of the repository in Base64 format.
        public string IconPath { get; }
    }
}
