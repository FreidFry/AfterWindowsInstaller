using AfterWindowsInstaller.Core.Interfaces;

using System.IO;
using System.Windows;

namespace AfterWindowsInstaller.infrastructure.Services
{
    public class RemoveFilesService() : IRemoveFilesService
    {
        public Task RemoveAllFiles()
        {
            try
            {
                if (Directory.Exists(USERDATA.USER_DOWNLOADS_PATH))
                    Directory.Delete(USERDATA.USER_DOWNLOADS_PATH, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete installers from Directory{USERDATA.USER_DOWNLOADS_PATH}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return Task.CompletedTask;
        }
    }
}
