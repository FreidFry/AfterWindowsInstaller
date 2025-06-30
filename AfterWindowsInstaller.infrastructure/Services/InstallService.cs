using AfterWindowsInstaller.Core.Interfaces;
using AfterWindowsInstaller.infrastructure.Extensions;

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

        public static Task InstallExe(KeyValuePair<string, IDownloadUrlModel> item, CancellationToken cancellationToken)
        {
            var model = item.Value;

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = item.Key,
                    UseShellExecute = true,
                    Verb = model.Verb ?? string.Empty,
                    Arguments = model.Arguments ?? string.Empty,
                },
                EnableRaisingEvents = true
            };

            process.Start();

            return process.ListenProcessAsync(cancellationToken);
        }
        public static async Task InstallMsi(KeyValuePair<string, IDownloadUrlModel> item, CancellationToken cancellationToken)
        {
            var model = item.Value;

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "msiexec",
                    UseShellExecute = true,
                    Verb = model.Verb ?? string.Empty,
                    Arguments = $"/i {model.FilePath} /qn /norestart"
                },
                EnableRaisingEvents = true
            };

            process.Start();

            await process.ListenProcessAsync(cancellationToken);

        }

        public async Task WingetInstall(KeyValuePair<string, IDownloadUrlModel> item, CancellationToken cancellationToken)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "winget",
                    Arguments = $"install -q \"{item.Value.WingetUrl}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                },
                EnableRaisingEvents = true
            };

            process.Start();
            process.StandardInput.WriteLine("y");

            await process.ListenProcessAsync(cancellationToken);

        }
    }
}
