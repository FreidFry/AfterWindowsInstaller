using Microsoft.Win32;

using System.IO;

namespace AfterWindowsInstaller.infrastructure
{
    public static class USERDATA
    {
        public static string USER_DOWNLOADS_PATH { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "After Install Window");
    }
}
