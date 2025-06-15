namespace AfterWindowsInstaller.Core.Interfaces
{
    public interface IRepositoriesService
    {
        Dictionary<string, Dictionary<string, IDownloadUrlModel>> RegistrationRepositories();
    }
}