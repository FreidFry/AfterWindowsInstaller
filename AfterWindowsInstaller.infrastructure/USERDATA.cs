using System.IO;

namespace AfterWindowsInstaller.infrastructure
{
    public static class USERDATA
    {
        public static string WINDOWS_VERSION { get; } = Environment.OSVersion.Version.ToString();
        public static bool IS_64_BIT_SYSTEM { get; } = Environment.Is64BitOperatingSystem;
        public static string USER_DOWNLOADS_PATH { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
    }
}
