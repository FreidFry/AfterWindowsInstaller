using AfterWindowsInstaller.Core.Interfaces;

using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

using static AfterWindowsInstaller.infrastructure.Extensions.UrlExtensions;

namespace AfterWindowsInstaller.infrastructure.Services
{
    public class DownloadService : IDownloadService
    {
        public async Task DownloadFileFromGitAsync(string owner, string repo, string outputPath, IProgress<double> currentBar, CancellationToken cancellationToken)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("MyApp", "1.0"));

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

            if (downloadUrl == null) throw new Exception("Asset не найден.");

            using var fileResponse = await client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            fileResponse.EnsureSuccessStatusCode();

            var filename = Path.Combine(outputPath, Path.GetFileName(downloadUrl));

            await DownloadFileAsync(filename, fileResponse, total, currentBar, cancellationToken);
        }




        public async Task DownloadFileAllowPathAsync(string url, string outputPath, IProgress<double> currentBar, CancellationToken cancellationToken)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("MyApp", "1.0"));

            var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            var filename = Path.Combine(outputPath, "testDownload.exe");
            long total = response.Content.Headers.ContentLength ?? -1;

            await DownloadFileAsync(filename, response, total, currentBar, cancellationToken);
        }

        static async Task DownloadFileAsync(string filename, HttpResponseMessage responseMessage, long total, IProgress<double> currentBar, CancellationToken cancellationToken)
        {
            var canReportProgress = total != -1 && currentBar != null;

            await using var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
            await using var fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);

            var buffer = new byte[81920];
            long totalRead = 0;
            int read;
            while ((read = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken)) > 0)
            {
                await fileStream.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
                totalRead += read;
                if (canReportProgress)
                    currentBar.Report((double)totalRead / total);
            }
        }
    }
}