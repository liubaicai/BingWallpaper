using Microsoft.Phone;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Windows.Storage;

namespace BaicaiWallpaper.Helpers
{
    public class SmallImageLoad
    {
        public static WriteableBitmap LoadStorageImage(string filename)
        {
            try
            {
                if (filename != null)
                {
                    StorageFolder local = ApplicationData.Current.LocalFolder;
                    if (local != null)
                    {
                        IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();
                        if (isolatedStorageFile.FileExists("LockImages\\" + filename))
                        {
                            IsolatedStorageFileStream stream = isolatedStorageFile.OpenFile("LockImages\\" + filename, FileMode.Open, FileAccess.Read);
                            WriteableBitmap bitmap = PictureDecoder.DecodeJpeg(stream, App.Width, App.Height);
                            stream.Close();
                            return bitmap;
                        }
                    }
                }
                return null;
            }
            catch { return null; }
        }
    }
}
