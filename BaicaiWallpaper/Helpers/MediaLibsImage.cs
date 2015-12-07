using Microsoft.Phone;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Windows.Storage;

namespace BaicaiWallpaper.Helpers
{
    public class MediaLibsImage : BitmapSource
    {
        public MediaLibsImage(Picture picture, string filename)
        {
            try
            {
                Stream stream = picture.GetThumbnail();
                SetSource(stream);
                stream.Close();
            }
            catch { }
        }
    }
}
