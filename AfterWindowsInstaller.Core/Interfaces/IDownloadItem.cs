namespace AfterWindowsInstaller.Core.Interfaces
{
    public interface IDownloadItem
    {
        public string Name { get; set; }
        public IDownloadUrlModel Model { get; set; }
    }
}
