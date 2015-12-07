using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace BingDailyWallpaper.Control
{
    public class StorageImage : BitmapSource
    {
        public StorageImage(StorageFile file)
        {
            SetSource(file);
        }

        private async void SetSource(StorageFile file)
        {
            IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read);
            await SetSourceAsync(fileStream);
        }
    }
}
