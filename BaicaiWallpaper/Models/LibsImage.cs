using BaicaiWallpaper.Helpers;
using BaicaiWallpaper.Resources;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace BaicaiWallpaper.Models
{
    public class LibsImage
    {
        public string fileIDTemp { get; set; }
        public bool isSave { get; set; }
        public Picture picture { get; set; }

        public string fileName 
        { 
            get 
            { 
                return picture.Name; 
            } 
        }
        public string fileFolder 
        { 
            get 
            { 
                return picture.Album.Name; 
            } 
        }
        public Visibility isCheck 
        {
            get
            {
                return isSave ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public string showName
        {
            get
            {
                string showname = AppResources.ModelFenbianlv + picture.Width + "*" + picture.Height;
                return showname;
            }
        }
        public string showRatio
        {
            get
            {
                return AppResources.ModelKuangaobi + ((float)(picture.Width) / (float)picture.Height).ToString("0.00");
            }
        }

        public BitmapSource image
        {
            get
            {
                return new MediaLibsImage(picture, fileName);
            }
        }


        public LibsImage(string fileIDTemp, Picture picture, bool isSave)
        {
            this.fileIDTemp = fileIDTemp;
            this.picture = picture;
            this.isSave = isSave;
        }
    }
}
