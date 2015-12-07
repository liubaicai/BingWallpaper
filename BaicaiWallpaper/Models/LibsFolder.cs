using BaicaiWallpaper.Helpers;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BaicaiWallpaper.Models
{
    public class LibsFolder
    {
        public PictureAlbum folder { get; set; }

        public BitmapSource image
        {
            get
            {
                return new MediaLibsImage(folder.Pictures[0], folder.Pictures[0].Name);
            }
        }
        public string showName
        {
            get
            {
                return folder.Name;
            }
        }

        public LibsFolder(PictureAlbum folder)
        {
            this.folder = folder;
        }
    }
}
