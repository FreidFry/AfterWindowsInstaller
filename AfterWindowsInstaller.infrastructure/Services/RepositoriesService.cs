using AfterWindowsInstaller.Core.Interfaces;
using AfterWindowsInstaller.infrastructure.Persistance.Models;

using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace AfterWindowsInstaller.infrastructure.Services
{
    public class RepositoriesService : IRepositoriesService
    {
        public Dictionary<string, Dictionary<string, IDownloadUrlModel>> RegistrationRepositories()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var jsonResources = assembly.GetManifestResourceNames()
                .Where(r => r.EndsWith(".json"))
                .ToList();
            if (jsonResources.Count == 0)
                throw new InvalidOperationException("No JSON resources found for repositories.");

            Dictionary<string, Dictionary<string, DownloadUrlModel>> repositories = [];
            foreach (var resourceName in jsonResources)
            {
                using Stream stream = assembly.GetManifestResourceStream(resourceName);
                using StreamReader reader = new(stream);
                string jsonText = reader.ReadToEnd();

                var parsed = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, DownloadUrlModel>>>(jsonText)
                 ?? throw new InvalidOperationException("Failed to deserialize repositories from JSON.");

                repositories.Add(parsed.Last().Key, parsed.Last().Value);
            }

            return repositories
                .OrderBy(kvp => kvp.Key)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.ToDictionary(
                        innerKvp => innerKvp.Key,
                        innerKvp => (IDownloadUrlModel)innerKvp.Value
                    )
                );

        }
    }
}
