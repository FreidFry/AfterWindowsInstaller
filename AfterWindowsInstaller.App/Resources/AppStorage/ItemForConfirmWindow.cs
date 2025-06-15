using AfterWindowsInstaller.Core.Interfaces;

namespace AfterWindowsInstaller.App.Resources.AppStorage
{
    public static class ItemForConfirmWindow
    {
        public static Dictionary<string, IDownloadUrlModel> LocalItems { get; set; } = [];

        public static void AddItem(string key, IDownloadUrlModel item) => LocalItems.TryAdd(key, item);
        public static void RemoveItem(string key) => LocalItems.Remove(key);
    }
}
