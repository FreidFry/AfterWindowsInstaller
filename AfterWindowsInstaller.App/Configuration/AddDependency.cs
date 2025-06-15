using AfterWindowsInstaller.App.Factories;
using AfterWindowsInstaller.Core.Interfaces;
using AfterWindowsInstaller.infrastructure.Data;
using AfterWindowsInstaller.infrastructure.Persistance.Models;
using AfterWindowsInstaller.infrastructure.Services;

using Microsoft.Extensions.DependencyInjection;

namespace AfterWindowsInstaller.App.Configuration
{
    internal static class AddDependency
    {
        internal static ServiceCollection AddDependencyes(this ServiceCollection services)
        {
            services.AddSingleton<IDownloadListStorage, DownloadListStorage>();

            services.AddScoped<IRepositoriesService, RepositoriesService>();
            services.AddScoped<IDownloadUrlModel, DownloadUrlModel>();
            services.AddScoped<IDownloadService, DownloadService>();

            services.AddScoped<IWindowFactory, ConfirmWindowFactory>();

            services.AddTransient<ConfirmExecuteWindow>();
            services.AddTransient<MainWindow>();

            return services;
        }
    }
}
