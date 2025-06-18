using AfterWindowsInstaller.Core.Interfaces;

using System.Windows;

namespace AfterWindowsInstaller.App.Factories
{
    public class ConfirmWindowFactory(
        IDownloadService downloadService,
        IDownloadListStorage downloadListStorage
        ) : IWindowFactory
    {
        private readonly IDownloadService _downloadService = downloadService;
        private readonly IDownloadListStorage _downloadListStorage = downloadListStorage;

        public T Create<T>() where T : Window
        {
            return new ConfirmExecuteWindow(_downloadService, _downloadListStorage) as T;
        }
    }
}
