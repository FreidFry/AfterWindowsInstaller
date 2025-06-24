using System.Diagnostics;
using System.Windows;

namespace AfterWindowsInstaller.App
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();

            Info.Text = $"After Windows Installer v{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}\n";
            var CurrentYear = DateTime.Now.Year.ToString();

            Copyrignt.Text = $"© {CurrentYear} Politov Michail\n" +
                "All rights reserved.\n\n";
        }

        private void PayPalDonateButton_Click(object sender, RoutedEventArgs e)
        {
            OpenBrowser("https://www.paypal.com/donate/?hosted_button_id=PMZZY5MTVUH8Y");
        }

        private void BoostyDonateButton_Click(object sender, RoutedEventArgs e)
        {
            OpenBrowser("https://boosty.to/freid4/donate");
        }

        private void MonoBankJarButton_Click(object sender, RoutedEventArgs e)
        {
            OpenBrowser("https://send.monobank.ua/jar/53EokQuT4A");
        }

        private static void OpenBrowser(string url)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
