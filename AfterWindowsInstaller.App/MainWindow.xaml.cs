using AfterWindowsInstaller.Core.Interfaces;
using AfterWindowsInstaller.infrastructure.Persistance.Models;
 
using System.Windows;
using System.Windows.Controls;

using static AfterWindowsInstaller.App.Design.Extensions;

namespace AfterWindowsInstaller.App
{
    public partial class MainWindow : Window
    {

        readonly IRepositoriesService _repositoriesService;
        readonly IDownloadListStorage _downloadListStorage;
        readonly IWindowFactory _windowFactory;

        public MainWindow(IRepositoriesService repositoriesService, IDownloadListStorage downloadListStorage, IWindowFactory windowFactory)
        {
            _repositoriesService = repositoriesService;
            _downloadListStorage = downloadListStorage;
            _windowFactory = windowFactory;

            InitializeComponent();
            itemsControl.ItemsSource = _repositoriesService.RegistrationRepositories();

#if DEBUG
            OnlyDownloadCheckBox.IsChecked = true;
#endif
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb && cb.DataContext is KeyValuePair<string, IDownloadUrlModel> kvp)
            {
                var item = new DownloadItem
                {
                    Name = kvp.Key,
                    Model = kvp.Value
                };
                _downloadListStorage.AddToDownloadList(item);
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb && cb.DataContext is KeyValuePair<string, IDownloadUrlModel> item)
            {
                _downloadListStorage.RemoveFromDownloadList(item.Key);
            }
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            if (sender is not Expander expandedExpander)
                return;

            foreach (var item in FindVisualChildren<Expander>(this))
                if (item != expandedExpander)
                    item.IsExpanded = false;
        }

        private void IsContinue_Click(object sender, RoutedEventArgs e)
        {
            _windowFactory.Create<ConfirmExecuteWindow>(OnlyDownloadCheckBox.IsChecked == true).ShowDialog();
        }

        private void OnlyDownloadCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb)
            {
                if (cb.IsChecked == true)
                    Continue_Button.Content = "Download apps";
                else
                    Continue_Button.Content = "Install apps";
            }
        }

        private void OpenAboutWindow_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow
            {
                Owner = this
            };
            aboutWindow.ShowDialog();
        }
    }
}