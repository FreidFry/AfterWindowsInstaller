namespace AfterWindowsInstaller.infrastructure.Extensions
{
    public static class UrlExtensions
    {
        public static string GetGitUrl(string owner, string repo) => $"https://api.github.com/repos/{owner}/{repo}/releases/latest";
    }
}
