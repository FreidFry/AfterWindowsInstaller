using System.IO;

namespace AfterWindowsInstaller.infrastructure.Extensions
{
    public static class DicrectoryExtensions
    {
        public static string CreateDirectoryIfNotExists(this string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            return path;
        }
    }
}
