using AfterWindowsInstaller.Core.Interfaces;
using AfterWindowsInstaller.infrastructure.Extensions;

using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Windows;

using static AfterWindowsInstaller.infrastructure.Extensions.UrlExtensions;

namespace AfterWindowsInstaller.infrastructure.Services
{
    public class DownloadService : IDownloadService
    {
        public async Task DownloadFileFromGitAsync(KeyValuePair<string, IDownloadUrlModel> DownloadUrlModel, string outputPath, CancellationToken cancellationToken)
        {
            using var client = CreateHttpClient();

            var owner = DownloadUrlModel.Value.Owner;
            var repo = DownloadUrlModel.Value.Repo;

            if (string.IsNullOrEmpty(owner) || string.IsNullOrEmpty(repo))
                return;

            var response = await client.GetAsync(GetGitUrl(owner, repo), cancellationToken);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            using var doc = JsonDocument.Parse(json);
            var assets = doc.RootElement.GetProperty("assets");

            string? downloadUrl = null;
            long total = 1;

            foreach (var asset in assets.EnumerateArray())
                if (asset.GetProperty("name").ToString().EndsWith(".exe"))
                {
                    downloadUrl = asset.GetProperty("browser_download_url").GetString();
                    total = asset.GetProperty("size").GetInt64();
                    break;
                }

            if (downloadUrl == null)
            {
                MessageBox.Show("Not found any .exe files in the latest release of the repository.");
                return;
            }

            using var fileResponse = await client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            fileResponse.EnsureSuccessStatusCode();

            var filename = Path.Combine(outputPath, Path.GetFileName(downloadUrl));

            await DownloadFileAsync(filename, fileResponse, cancellationToken);
        }

        public async Task DownloadFileAllowPathAsync(KeyValuePair<string, IDownloadUrlModel> downloadUrlModel, string outputPath, CancellationToken cancellationToken)
        {
            try
            {
                using var client = CreateHttpClient();

                var url = downloadUrlModel.Value.Url;
                if (string.IsNullOrEmpty(url))
                    return;

                var programName = GetName(url, downloadUrlModel.Key);
                var filename = Path.Combine(outputPath, programName);
                var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"{programName} HTTP error: {response.StatusCode}");
                    return;
                }

                long total = response.Content.Headers.ContentLength ?? -1;
                await DownloadFileAsync(filename, response, cancellationToken);
                downloadUrlModel.Value.FilePath = filename;
            }
            catch
            {
                MessageBox.Show($"Error downloading {downloadUrlModel.Key} out of reach URL: {downloadUrlModel.Value.Url}");
            }
        }

        public async Task DownloadWingetAsync(KeyValuePair<string, IDownloadUrlModel> downloadUrlModel, string outputPath, CancellationToken cancellationToken)
        {
            try
            {
                var url = downloadUrlModel.Value.WingetUrl;
                if (string.IsNullOrEmpty(url))
                    return;

                await WinGetDownloadAsync(url, outputPath, cancellationToken);

                var filePath = Path.Combine(outputPath, url.Split('.')[0]);
                downloadUrlModel.Value.FilePath = filePath;
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Download canceled by user.");
            }
            catch
            {
                MessageBox.Show($"Error downloading {downloadUrlModel.Key} from the Winget URL: {downloadUrlModel.Value.WingetUrl}");
            }
            finally
            {
                if (Directory.Exists(USERDATA.USER_DOWNLOADS_PATH))
                {
                    string[] files = Directory.GetFiles(USERDATA.USER_DOWNLOADS_PATH, "*.yaml", SearchOption.AllDirectories);
                    foreach (var file in files) File.Delete(file);
                }
            }
        }

        static async Task DownloadFileAsync(string filename, HttpResponseMessage responseMessage, CancellationToken cancellationToken)
        {
            await using var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
            await using var fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);

            var buffer = new byte[81920];
            int read;

            while ((read = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken)) > 0)
            {
                await fileStream.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
            }
        }

        static async Task WinGetDownloadAsync(string url, string outputPath, CancellationToken cancellationToken)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "winget",
                    Arguments = $"download -q \"{url}\" -d \"{outputPath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true

            };
            process.Start();

            await process.ListenProcessAsync(cancellationToken);
        }

        static string GetName(string url, string Name)
        {
            var Extension = Path.GetExtension(url);
            if (string.IsNullOrEmpty(Extension)) Extension = ".exe";

            return Name + Extension;
        }

        static HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("MyApp", "1.0"));

            return client;
        }
    }
}