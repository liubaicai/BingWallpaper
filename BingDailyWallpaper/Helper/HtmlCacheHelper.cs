using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;

namespace BingDailyWallpaper.Helper
{
    public class HtmlCacheHelper
    {
        private static HtmlCacheHelper _instance;
        public static HtmlCacheHelper Instance => _instance ?? (_instance = new HtmlCacheHelper());
        private HtmlCacheHelper()
        {
        }

        private string Url = "http://www.bing.com/hpimagearchive.aspx?format=xml&idx={0}&n=1&mbl=1&mkt=zh-cn";

        public IAsyncOperation<StorageFolder> CacheImageFolder => ApplicationData.Current.LocalFolder.CreateFolderAsync("Xmls", CreationCollisionOption.OpenIfExists);

        public async Task<string> GetCacheXml(int index)
        {
            var date = GetDateInt(index);
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
                var xml = await http.GetStringAsync(string.Format(Url, index));
                if (xml.Length>50)
                {
                    var file = await folder.CreateFileAsync(date + ".xml", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(file, xml);
                }
                return xml;
            }
        }

        private int GetDateInt(int index)
        {
            var dt = DateTime.Now.AddDays(-index);
            return dt.Year*1000 + dt.DayOfYear;
        }

        private async Task<bool> IsFileExist(StorageFolder storageFolder, string filename)
        {
            return (await storageFolder.GetFilesAsync()).Any(file => file.Name == filename);
        }
    }
}
