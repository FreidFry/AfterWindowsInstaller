using System.Windows;

namespace AfterWindowsInstaller.Core.Interfaces
{
    public interface IWindowFactory
    {
        T Create<T>(bool onlyDownload) where T : Window;
    }
}
