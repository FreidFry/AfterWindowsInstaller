using System.Windows;

namespace AfterWindowsInstaller.Core.Interfaces
{
    public interface IWindowFactory
    {
        T Create<T>() where T : Window;
    }
}
