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
        public async Task DownloadFileFromGitAsync(KeyValuePair<string, IDownloadUrlModel> DownloadUrlModel, string outputPath, IProgress<double> currentBar, CancellationToken cancellationToken)
        {
            using var client = CreateHttpClient();

            var owner = DownloadUrlModel.Value.Owner;
            var repo = DownloadUrlModel.Value.Repo;

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

        public async Task DownloadFileAllowPathAsync(KeyValuePair<string, IDownloadUrlModel> DownloadUrlModel, string outputPath, IProgress<double> currentBar, CancellationToken cancellationToken)
        {
            using var client = CreateHttpClient();

            var url = DownloadUrlModel.Value.Url;

            var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            var ProgramName = DownloadUrlModel.Key;

            var filename = Path.Combine(outputPath, ProgramName + ".exe");
            long total = response.Content.Headers.ContentLength ?? -1;

            await DownloadFileAsync(filename, response, total, currentBar, cancellationToken);
        }

        static async Task DownloadFileAsync(string filename, HttpResponseMessage responseMessage, long total, IProgress<double> currentBar, CancellationToken cancellationToken)
        {
            var canReportProgress = CanReportProgress(total, currentBar);

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

        static HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("MyApp", "1.0"));

            return client;
        }

        static bool CanReportProgress(long total, IProgress<double> currentBar) => total != -1 && currentBar != null;
    }
}