using Microsoft.Phone;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Windows.Storage;
using System.IO;
using System.Windows;

namespace BaicaiWallpaper.Helpers
{
    public class PhotoUpload
    {
        public string filename;
        private int width=480;
        private int height=800;

        public PhotoUpload(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public void photoUpload()
        {
            try
            {
                PhotoChooserTask task = new PhotoChooserTask();
                task.PixelHeight = height;
                task.PixelWidth = width;
                task.ShowCamera = true;
                task.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
                task.Show();
            }
            catch { }
        }

        async void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            try
            {
                if (e.Error == null && e.TaskResult == TaskResult.OK)
                {
                    filename = DateTime.Now.ToString("yyyyMMddHHmmss");

                    StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                    if (local != null)
                    {
                        WriteableBitmap bmp = PictureDecoder.DecodeJpeg(e.ChosenPhoto);
                        StorageFolder lockimageFolder = await local.CreateFolderAsync("LockImages", CreationCollisionOption.OpenIfExists);
                        StorageFile file = await lockimageFolder.CreateFileAsync(filename,CreationCollisionOption.GenerateUniqueName);
                        using (var s = await file.OpenStreamForWriteAsync())
                        {
                            Extensions.SaveJpeg(bmp, s, bmp.PixelWidth, bmp.PixelHeight, 0, 40);
                            s.Close();
                        }
                    }

                    //PhotoStream = isf.OpenFile(filename, FileMode.Open, FileAccess.Read);   //读取刚刚保存的图片的文件流
                    //BitmapImage bmp1 = new BitmapImage();
                    //bmp1.SetSource(PhotoStream);  //把文件流转换为图片
                    //PhotoStream.Close();    //读取完毕，关闭文件流
                }
                if (e.ChosenPhoto == null && e.TaskResult == TaskResult.Cancel && e.Error.Message == "InvalidOperationException")
                {
                    MessageBox.Show("图片库打开失败！", "提示", MessageBoxButton.OKCancel);
                }
            }
            catch { }
        }
    }
}
