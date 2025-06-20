using AfterWindowsInstaller.Core.Interfaces;

using System.Diagnostics;
using System.IO;
using System.Windows;

namespace AfterWindowsInstaller.infrastructure.Services
{
    public class InstallService : IInstallService
    {
        public Task Install(KeyValuePair<string, IDownloadUrlModel> item, CancellationToken cancellationToken)
        {
            switch (Path.GetExtension(item.Value.FilePath)?.ToLower())
            {
                case "exe":
                    return InstallExe(item, cancellationToken);
                case "msi":
                    return InstallMsi(item, cancellationToken);
                default:
                    MessageBox.Show($"Unsupported file type: {item.Value.FilePath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return Task.CompletedTask;
            }
        }

        public Task InstallExe(KeyValuePair<string, IDownloadUrlModel> item, CancellationToken cancellationToken)
        {
            var model = item.Value;

            Process.Start(new ProcessStartInfo
            {
                FileName = item.Key,
                UseShellExecute = true,
                Verb = model.Verb ?? string.Empty,
                Arguments = model.Arguments ?? string.Empty,
            });

            return Task.CompletedTask;
        }
        public Task InstallMsi(KeyValuePair<string, IDownloadUrlModel> item, CancellationToken cancellationToken)
        {
            var model = item.Value;

            Process.Start(new ProcessStartInfo
            {
                FileName = "msiexec",
                UseShellExecute = true,
                Verb = model.Verb ?? string.Empty,
                Arguments = $"/i {model.FilePath} /qn /norestart"
            });

            return Task.CompletedTask;
        }
    }
}
