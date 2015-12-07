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
    public class StorageImage : BitmapSource
    {
        static StorageImage()
        {
            using (var isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isolatedStorageFile.DirectoryExists("LockImages"))
                {
                    isolatedStorageFile.CreateDirectory("LockImages");
                }
            }
        }

        public StorageImage(string filename)
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
                            SetSource(stream);
                            stream.Close();
                        }
                    }
                }
            }
            catch { }
        }
    }
}
