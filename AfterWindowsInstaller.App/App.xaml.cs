using AfterWindowsInstaller.App.Configuration;

using Microsoft.Extensions.DependencyInjection;

using System.Windows;

namespace AfterWindowsInstaller.App
{

    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;

        public App()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddDependencyes();

            _serviceProvider = serviceCollection.BuildServiceProvider();




        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = _serviceProvider?.GetRequiredService<MainWindow>();
            mainWindow?.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            _serviceProvider?.Dispose();
        }
    }

}
