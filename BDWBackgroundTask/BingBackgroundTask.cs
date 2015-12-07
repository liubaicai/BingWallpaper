using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Storage;
using Windows.System.UserProfile;

namespace BDWBackgroundTask
{
    public sealed class BingBackgroundTask : IBackgroundTask
    {
        private const string Url = "http://www.bing.com/hpimagearchive.aspx?format=xml&idx=0&n=1&mbl=1&mkt=zh-cn";
        private IAsyncOperation<StorageFolder> CacheImageFolder => ApplicationData.Current.LocalFolder.CreateFolderAsync("Xmls", CreationCollisionOption.OpenIfExists);

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral _deferral = taskInstance.GetDeferral();
            try
            {
                var str = await GetCacheXml();
                var regEx = new Regex(@"(\<urlBase\>(?<urlbase>.*?)\<\/urlBase\>)",
                    RegexOptions.IgnoreCase | RegexOptions.Singleline);
                foreach (Match match in regEx.Matches(str))
                {
                    var url = "http://cn.bing.com" + match.Groups["urlbase"].Value + "_1920x1080.jpg";
                    Debug.WriteLine("============" + url);
                    var filename = url.Split('/').Last();
                    StorageFolder storageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);
                    var b = await IsFileExist(storageFolder, filename);
                    if (!b)
                    {
                        Debug.WriteLine("=====DownLoadImage");
                        var tmpfile = await storageFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                        var http = new HttpClient();
                        var data = await http.GetByteArrayAsync(url);
                        await FileIO.WriteBytesAsync(tmpfile, data);
                    }
                    var file = await storageFolder.GetFileAsync(filename);
                    using (var stream = await file.OpenReadAsync())
                    {
                        if (stream.Size <= 0)
                        {
                            Debug.WriteLine("=====DownLoadImage");
                            var tmpfile = await storageFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                            var http = new HttpClient();
                            var data = await http.GetByteArrayAsync(url);
                            await FileIO.WriteBytesAsync(tmpfile, data);
                        }
                    }
                    if (Settings.Get("SetLockScreen", "0").ToString() == "1")
                    {
                        var result = await UserProfilePersonalizationSettings.Current.TrySetLockScreenImageAsync(file);
                        Debug.WriteLine("============LockScreen:" + result);
                    }
                    if (Settings.Get("SetWallpaper", "0").ToString() == "1")
                    {
                        var result = await UserProfilePersonalizationSettings.Current.TrySetWallpaperImageAsync(file);
                        Debug.WriteLine("============Wallpaper:" + result);
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
            finally
            {
                _deferral.Complete();
            }
        }

        private async Task<string> GetCacheXml()
        {
            var date = GetDateInt();
            var folder = await CacheImageFolder;
            var b = await IsFileExist(folder, date + ".xml");
            if (b)
            {
                var file = await folder.GetFileAsync(date + ".xml");
                var xml = await FileIO.ReadTextAsync(file);
                return xml;
            }
            else
            {
                var http = new HttpClient();
                var xml = await http.GetStringAsync(string.Format(Url));
                if (xml.Length > 50)
                {
                    var file = await folder.CreateFileAsync(date + ".xml", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(file, xml);
                }
                return xml;
            }
        }

        private async Task<bool> IsFileExist(StorageFolder storageFolder, string filename)
        {
            return (await storageFolder.GetFilesAsync()).Any(file => file.Name == filename);
        }

        private int GetDateInt()
        {
            var dt = DateTime.Now;
            return dt.Year * 1000 + dt.DayOfYear;
        }
    }
}
