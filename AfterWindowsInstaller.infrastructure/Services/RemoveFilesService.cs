using AfterWindowsInstaller.Core.Interfaces;

using System.IO;
using System.Windows;

namespace AfterWindowsInstaller.infrastructure.Services
{
    public class RemoveFilesService(IDownloadListStorage downloadListStorage) : IRemoveFilesService
    {
        private readonly IDownloadListStorage _downloadListStorage = downloadListStorage;

        public Task RemoveAllFiles()
        {
            foreach (var item in _downloadListStorage.DownloadList)
                if (item.Model.FilePath != null && File.Exists(item.Model.FilePath))
                    try
                    {
                        File.Delete(item.Model.FilePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to delete installer {item.Name}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

            return Task.CompletedTask;
        }
    }
}
