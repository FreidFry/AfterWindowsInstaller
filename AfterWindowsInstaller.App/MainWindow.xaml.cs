using AfterWindowsInstaller.Core.Interfaces;
using static AfterWindowsInstaller.App.Design.Extensions;

using System.Windows;
using System.Windows.Controls;
using AfterWindowsInstaller.App.Resources.AppStorage;

namespace AfterWindowsInstaller.App
{
    public partial class MainWindow : Window
    {
        readonly IRepositoriesService _repositoriesService;
        readonly IDownloadListStorage _downloadListStorage;



        IWindowFactory _windowFactory;
        public MainWindow(IRepositoriesService repositoriesService, IDownloadListStorage downloadListStorage, IWindowFactory windowFactory)
        {
            _repositoriesService = repositoriesService;
            _downloadListStorage = downloadListStorage;
            _windowFactory = windowFactory;

            InitializeComponent();
            itemsControl.ItemsSource = _repositoriesService.RegistrationRepositories();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb && cb.DataContext is KeyValuePair<string, IDownloadUrlModel> item)
            {
                _downloadListStorage.AddToDownloadList(item.Value.Url);
                ItemForConfirmWindow.AddItem(item.Key, item.Value);
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb && cb.DataContext is KeyValuePair<string, IDownloadUrlModel> item)
            {
                _downloadListStorage.RemoveFromDownloadList(item.Value.Url);
                ItemForConfirmWindow.RemoveItem(item.Key);
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
            _windowFactory.Create<ConfirmExecuteWindow>().ShowDialog();
        }
    }
}