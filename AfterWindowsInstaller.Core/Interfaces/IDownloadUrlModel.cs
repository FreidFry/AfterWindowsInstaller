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
        /// Winget URL of the repository, if applicable.
        /// </summary>
        string? WingetUrl { get; }

        /// <summary>
        /// Description of the repository.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Icon of the repository in Base64 format.
        /// </summary>
        string IconPath { get; }

        /// <summary>
        /// Verb for the installation command, if applicable.
        /// </summary>
        string? Verb { get; }

        /// <summary>
        /// Arguments for the installation command, if applicable.
        /// </summary>
        string? Arguments { get; }

        /// <summary>
        /// File path for the downloaded file, if applicable.
        /// </summary>  
        string? FilePath { get; set; }
    }
}
