using AfterWindowsInstaller.Core.Interfaces;

using System.Windows;

namespace AfterWindowsInstaller.App.Factories
{
    public class ConfirmWindowFactory : IWindowFactory
    {
        private readonly IDownloadService _downloadService;

        public ConfirmWindowFactory(IDownloadService downloadService)
        {
            _downloadService = downloadService;
        }

        public T Create<T>() where T : Window
        {
            return new ConfirmExecuteWindow(_downloadService) as T;
        }
    }
}
