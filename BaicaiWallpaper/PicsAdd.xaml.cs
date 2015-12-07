using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.ComponentModel;
using System.Collections.ObjectModel;
using BaicaiWallpaper.Models;
using Microsoft.Xna.Framework.Media;
using Windows.Storage;
using System.Windows.Media.Imaging;
using Microsoft.Phone;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading;
using System.Diagnostics;
using BaicaiWallpaper.Resources;

namespace BaicaiWallpaper
{
    public partial class PicsAdd : PhoneApplicationPage, INotifyPropertyChanged
    {
        public ObservableCollection<LibsImage> listImages { get; set; }
        bool isInput = false;
        private ApplicationBar appbar;

        int num = 0;
        int pagenum = 9;

        public PicsAdd()
        {
            appbar = new ApplicationBar();
            listImages = new ObservableCollection<LibsImage>();
            DataContext = this;

            InitializeComponent();

            this.Loaded += PicsAdd_Loaded;

            StoryboardClose.Completed += new EventHandler((sd, ev) =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    Thread t = new Thread(LoadPic);
                    t.Start();
                });
            });
        }

        void PicsAdd_Loaded(object sender, RoutedEventArgs e)
        {
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                progressBar.Visibility = Visibility.Visible;
                Thread t = new Thread(LoadPic);
                t.Start();

                num = 0;

                string ratio = (App.Width_d / App.Height_d).ToString("0.00");
                title.Text = ratio;

                InitAppBar();
                this.ApplicationBar = appbar;
            }
        }

        ApplicationBarIconButton shang = new ApplicationBarIconButton();
        ApplicationBarIconButton xia = new ApplicationBarIconButton();
        void InitAppBar()
        {
            appbar.Buttons.Clear();

            shang.IconUri = new Uri("/Assets/AppBar/back.png", UriKind.RelativeOrAbsolute);
            shang.Text = AppResources.PageUp;
            shang.Click += new EventHandler((sender, e) =>
            {
                try
                {
                    num = num - pagenum;
                    StoryboardClose.Begin();
                }
                catch { }
            });

            xia.IconUri = new Uri("/Assets/AppBar/next.png", UriKind.RelativeOrAbsolute);
            xia.Text = AppResources.PageDown;
            xia.Click += new EventHandler((sender, e) =>
            {
                try
                {
                    num = num + pagenum;
                    StoryboardClose.Begin();
                }
                catch { }
            });

            appbar.Buttons.Add(shang);
            appbar.Buttons.Add(xia);
        }

        void LoadPic()
        {
            Debug.WriteLine(num + "");
            try
            {
                PictureAlbum folder = Datas.folder;
                Dispatcher.BeginInvoke(() =>
                {
                    listImages.Clear();
                    progressBar.Visibility = Visibility.Visible;
                });
                for (int i = num; i < (num + pagenum) && i < folder.Pictures.Count; i++)
                {
                    Picture pic = folder.Pictures[i];
                    Dispatcher.BeginInvoke(() =>
                    {
                        try
                        {
                            bool isSave = CheckisSave(pic.Album.Name + "." + pic.Name);
                            listImages.Add(new LibsImage(Guid.NewGuid().ToString(), pic, isSave));

                            if (num == 0)
                            {
                                shang.IsEnabled = false;
                            }
                            else
                            {
                                shang.IsEnabled = true;
                            }
                            if (num + pagenum >= folder.Pictures.Count)
                            {
                                xia.IsEnabled = false;
                            }
                            else
                            {
                                xia.IsEnabled = true;
                            }
                        }
                        catch { }
                    });
                }
                Dispatcher.BeginInvoke(() =>
                {
                    NotifyPropertyChanged("listImages");
                    progressBar.Visibility = Visibility.Collapsed;
                    StoryboardOpen.Begin();
                });
            }
            catch { }
        }

        bool CheckisSave(string filename)
        {
            try
            {
                var isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();
                if (isolatedStorageFile.FileExists("LockImages/" + filename))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch { return false; }
        }

        #region 绑定接口实现
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        private static readonly object _readLock = new object();
        private void imageList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ListBox listBox = sender as ListBox;
                if (listBox.SelectedIndex != -1)
                {
                    isInput = true;
                    LibsImage image = listBox.SelectedItem as LibsImage;
                    if (!image.isSave)
                    {
                        if (Datas.picCount >= Datas.maxCount)
                        {
                            MessageBox.Show(AppResources.MainImageTooMore);
                            return;
                        }
                        string filename = image.fileName;
                        lock (_readLock)
                        {
                            var isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();
                            if (!isolatedStorageFile.DirectoryExists("LockImages"))
                            {
                                isolatedStorageFile.CreateDirectory("LockImages");
                            }
                            using (var fileStream = isolatedStorageFile.OpenFile("LockImages/" + image.fileFolder + "." + image.fileName, FileMode.OpenOrCreate, FileAccess.Write))
                            {
                                BitmapImage bitmap = new BitmapImage();
                                Stream stream = image.picture.GetImage();

                                int quality = 60;
                                long streamLength = stream.Length;
                                if (streamLength > 1500000)
                                {
                                    quality = 15;
                                }
                                else if(streamLength > 1000000)
                                {
                                    quality = 20;
                                }
                                else if (streamLength > 500000)
                                {
                                    quality = 25;
                                }
                                else if (streamLength > 200000)
                                {
                                    quality = 30;
                                }
                                else if (streamLength > 100000)
                                {
                                    quality = 40;
                                }
                                else if (streamLength > 50000)
                                {
                                    quality = 55;
                                }

                                bitmap.SetSource(stream);
                                WriteableBitmap wb = new WriteableBitmap(bitmap);
                                Extensions.SaveJpeg(wb, fileStream, wb.PixelWidth, wb.PixelHeight, 0, quality);
                                stream.Close();
                                fileStream.Close();
                                Datas.picCount++;
                            }
                        }

                        for (int i = 0; i < listImages.Count; i++)
                        {
                            if (listImages[i].fileIDTemp.Equals(image.fileIDTemp))
                            {
                                LibsImage temp = listImages[i];
                                temp.isSave = true;
                                listImages.RemoveAt(i);
                                listImages.Insert(i, temp);
                                NotifyPropertyChanged("listImages");
                            }
                        }
                    }
                    else
                    {
                        var isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();
                        if (isolatedStorageFile.FileExists("LockImages/" + image.fileFolder + "." + image.fileName))
                        {
                            isolatedStorageFile.DeleteFile("LockImages/" + image.fileFolder + "." + image.fileName);
                        }
                        Datas.picCount--;
                        for (int i = 0; i < listImages.Count; i++)
                        {
                            if (listImages[i].fileIDTemp.Equals(image.fileIDTemp))
                            {
                                LibsImage temp = listImages[i];
                                temp.isSave = false;
                                listImages.RemoveAt(i);
                                listImages.Insert(i, temp);
                                NotifyPropertyChanged("listImages");
                            }
                        }
                    }
                    listBox.SelectedIndex = -1;
                }
            }
            catch { }
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            try
            {
                foreach (var item in listImages)
                {
                    try
                    {
                        item.picture.Dispose();
                    }
                    catch { }
                }
            }
            catch { }
            Datas.isRef = isInput;
            base.OnBackKeyPress(e);
        }
    }
}