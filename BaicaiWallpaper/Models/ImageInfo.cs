using BaicaiWallpaper.Controls;
using BaicaiWallpaper.Helpers;
using BaicaiWallpaper.Helpers.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Windows.Storage;

namespace BaicaiWallpaper.Models
{
    public class ImageInfo
    {
        public string filename { get; set; }
        public string fileChangeTime { get; set; }

        public bool isSave { get; set; }

        public BitmapSource image
        {
            get
            {
                return SmallImageLoad.LoadStorageImage(filename);
            }
        }
        //public BitmapSource image
        //{
        //    get
        //    {
        //        return new StorageImage(filename);
        //    }
        //}

        public ImageInfo(string filename, string fileChangeTime)
        {
            this.filename = filename;
            this.fileChangeTime = fileChangeTime;
        }
    }
}
