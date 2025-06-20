using AfterWindowsInstaller.Core.Interfaces;

using System.Windows;

namespace AfterWindowsInstaller.App.Factories
{
    public class ConfirmWindowFactory(
        IDownloadService downloadService,
        IDownloadListStorage downloadListStorage,
        IInstallService installService
        ) : IWindowFactory
    {
        private readonly IDownloadService _downloadService = downloadService;
        private readonly IDownloadListStorage _downloadListStorage = downloadListStorage;
        private readonly IInstallService _installService = installService;

        public T Create<T>(bool onlyDownload) where T : Window
        {
            return new ConfirmExecuteWindow(onlyDownload, _downloadService, _downloadListStorage, _installService) as T;
        }
    }
}
