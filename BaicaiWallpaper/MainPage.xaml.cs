using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BaicaiWallpaper.Resources;
using BaicaiWallpaperTask;
using Microsoft.Phone.Scheduler;
using System.Collections.ObjectModel;
using BaicaiWallpaper.Models;
using BaicaiWallpaper.Controls;
using Windows.Storage;
using System.IO.IsolatedStorage;
using System.ComponentModel;
using BaicaiWallpaper.Helpers;
using System.Diagnostics;
using System.Windows.Threading;
using Coding4Fun.Phone.Controls;
using System.Windows.Media;
using AwesomeMenuForWindowsPhone;
using System.Threading;

namespace BaicaiWallpaper
{
    public partial class MainPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        public ObservableCollection<ImageInfo> listImages { get; set; }
        public int width = App.Width;
        public int height = App.Height;

        // 构造函数
        public MainPage()
        {
            InitializeComponent();
            listImages = new ObservableCollection<ImageInfo>();
            this.Loaded += MainPage_Loaded;
            DataContext = this;
        }

        #region 按钮事件
        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (menu == null)
            {
                InitMenu();
            }
        }

        AwesomeMenu menu;
        void InitMenu()
        {
            Rect rc = new Rect(0, 740, 456, 770);
            Point pt = new Point(406, 720);
            var items = new List<AwesomMenuItem>();

            AwesomMenuItem itemadd = new AwesomMenuItem("/Assets/AppBar/add.png", "AwesomeMenu/Images/bg-menuitem.png");
            AwesomMenuItem itemfolder = new AwesomMenuItem("/Assets/AppBar/folder.png", "AwesomeMenu/Images/bg-menuitem.png");
            AwesomMenuItem itemdel = new AwesomMenuItem("/Assets/AppBar/delete.png", "AwesomeMenu/Images/bg-menuitem.png");
            AwesomMenuItem itemset = new AwesomMenuItem("/Assets/AppBar/settings.png", "AwesomeMenu/Images/bg-menuitem.png");

            itemadd.ClickMenuItem += itemadd_ClickMenuItem;
            itemfolder.ClickMenuItem += itemfolder_ClickMenuItem;
            itemdel.ClickMenuItem += itemdel_ClickMenuItem;
            itemset.ClickMenuItem += itemset_ClickMenuItem;

            items.Add(itemadd);
            items.Add(itemfolder);
            items.Add(itemdel);
            items.Add(itemset);

            menu = new AwesomeMenu(rc, items, "AwesomeMenu/Images/icon-plus.png", "AwesomeMenu/Images/bg-addbutton.png", pt);
            menu.TapToDissmissItem = false;
            Grid.SetRow(menu, 1);
            menu.Margin = new Thickness(0, 0, 30, 30);
            menu.ActionClosed += (item) =>
            {
                Dispatcher.BeginInvoke(delegate
                {
                    try
                    {
                        this.LayoutRoot.Children.Remove(menu);
                        if (menu != null)
                            menu.Children.Clear();
                        menu = null;
                    }
                    catch { }
                });
            };
            LayoutRoot.Children.Add(menu);
        }

        private void itemset_ClickMenuItem(AwesomMenuItem item)
        {
            try
            {
                NavigationService.Navigate(new Uri("/Setting.xaml", UriKind.RelativeOrAbsolute));
                Dispatcher.BeginInvoke(delegate
                {
                    try
                    {
                        this.LayoutRoot.Children.Remove(menu);
                        if (menu != null)
                            menu.Children.Clear();
                        menu = null;
                    }
                    catch { }
                });
            }
            catch { }
        }

        private void itemdel_ClickMenuItem(AwesomMenuItem item)
        {
            try
            {
                if (MessageBox.Show(AppResources.MainClearImage, "", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                {
                    return;
                }
                using (IsolatedStorageFile isFile = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (isFile.DirectoryExists("LockImages"))
                    {
                        foreach (String str in isFile.GetFileNames("LockImages/*.*"))
                        {
                            try
                            {
                                isFile.DeleteFile("LockImages/" + str);
                            }
                            catch { }
                        }
                    }
                }
                if (listImages != null)
                {
                    listImages.Clear();
                }
                Datas.picCount = 0;
                NotifyPropertyChanged("listImages");
                var schema = "ms-appx:///DefaultLockScreen.jpg";
                var uri = new Uri(schema, UriKind.Absolute);
                Windows.Phone.System.UserProfile.LockScreen.SetImageUri(uri);

                ToastPrompt toast = new ToastPrompt
                {
                    Message = AppResources.MainClearImageDown
                };
                toast.Show();
                Dispatcher.BeginInvoke(delegate
                {
                    try
                    {
                        this.LayoutRoot.Children.Remove(menu);
                        if (menu != null)
                            menu.Children.Clear();
                        menu = null;
                    }
                    catch { }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void itemfolder_ClickMenuItem(AwesomMenuItem item)
        {
            try
            {
                if (listImages.Count >= Datas.maxCount)
                {
                    MessageBox.Show(AppResources.MainImageTooMore);
                    return;
                }
                else
                {
                    Datas.picCount = listImages.Count;
                    NavigationService.Navigate(new Uri("/PicsFolders.xaml", UriKind.RelativeOrAbsolute));
                }
                Dispatcher.BeginInvoke(delegate
                {
                    try
                    {
                        this.LayoutRoot.Children.Remove(menu);
                        if (menu != null)
                            menu.Children.Clear();
                        menu = null;
                    }
                    catch { }
                });
            }
            catch { }
        }

        void itemadd_ClickMenuItem(AwesomMenuItem item)
        {
            try
            {
                if (listImages.Count >= Datas.maxCount)
                {
                    MessageBox.Show(AppResources.MainImageTooMore);
                    return;
                }
                else
                {
                    PhotoUpload pu = new PhotoUpload(width, height);
                    pu.photoUpload();
                    //string filename = pu.filename;
                    //listImages.Add(new ImageInfo(filename));
                    //NotifyPropertyChanged("listImages");

                    timer = new DispatcherTimer();
                    timer.Interval = TimeSpan.FromMilliseconds(500);
                    timer.Tick += timer_Tick;
                    timer.Start();
                }
                Dispatcher.BeginInvoke(delegate
                {
                    try
                    {
                        this.LayoutRoot.Children.Remove(menu);
                        if (menu != null)
                            menu.Children.Clear();
                        menu = null;
                    }
                    catch { }
                });
            }
            catch { }
        }
        #endregion

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode != NavigationMode.Back||Datas.isRef)
            {
                Datas.isRef = false;
                StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                if (local != null)
                {
                    listImages.Clear();
                    StorageFolder lockimageFolder = await local.CreateFolderAsync("LockImages", CreationCollisionOption.OpenIfExists);
                    foreach (var file in await lockimageFolder.GetFilesAsync())
                    {
                        listImages.Add(new ImageInfo(file.Name, file.DateCreated.ToString("MM-dd HH:mm")));
                    }
                    Datas.picCount = listImages.Count;
                    NotifyPropertyChanged("listImages");
                }
                int flag = 0;
                var isProvider = Windows.Phone.System.UserProfile.LockScreenManager.IsProvidedByCurrentApplication;
                if (!isProvider)
                {
                    flag++;
                    // If you're not the provider, this call will prompt the user for permission.
                    // Calling RequestAccessAsync from a background agent is not allowed.
                    var op = await Windows.Phone.System.UserProfile.LockScreenManager.RequestAccessAsync();

                    // Only do further work if the access was granted.
                    isProvider = op == Windows.Phone.System.UserProfile.LockScreenRequestResult.Granted;
                }
                if (isProvider)
                {
                    TaskFuc.startTask();
                    TaskFuc.setLock();
                    if (flag == 1)
                    {
                        ToastPrompt toast = new ToastPrompt
                        {
                            Message = AppResources.MainLaunchNotice
                        };
                        toast.Show();
                    }
                }
                else
                {
                    MessageBox.Show(AppResources.SettingSetLockFail);
                }
            }
        }

        private void ButtonEx_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ButtonEx bt = sender as ButtonEx;
                string filename = bt.Tag.ToString();
                StorageFolder local = ApplicationData.Current.LocalFolder;
                if (local != null)
                {
                    IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();
                    if (isolatedStorageFile.FileExists("LockImages\\" + filename))
                    {
                        isolatedStorageFile.DeleteFile("LockImages\\" + filename);
                    }
                }
                foreach (var item in listImages)
                {
                    if (item.filename.Equals(filename))
                    {
                        listImages.Remove(item);
                        Datas.picCount = listImages.Count;
                        NotifyPropertyChanged("listImages");
                        return;
                    }
                }
            }
            catch { }
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

        DispatcherTimer timer;

        private void timer_Tick(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(async () =>
            {
                try
                {
                    StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                    if (local != null)
                    {
                        listImages.Clear();
                        StorageFolder lockimageFolder = await local.CreateFolderAsync("LockImages", CreationCollisionOption.OpenIfExists);
                        foreach (var file in await lockimageFolder.GetFilesAsync())
                        {
                            listImages.Add(new ImageInfo(file.Name, file.DateCreated.ToString("MM-dd HH:mm")));
                        }
                        Datas.picCount = listImages.Count;
                        NotifyPropertyChanged("listImages");
                        TaskFuc.setLock();
                    }
                }
                catch { }
                if (timer != null)
                {
                    timer.Stop();
                }
            });
        }

        private void LockImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                Image image = sender as Image;
                string filename = image.Tag.ToString();
                NavigationService.Navigate(new Uri("/PicView.xaml?filename=" + HttpUtility.UrlEncode(filename), UriKind.RelativeOrAbsolute));
            }
            catch { }
        }

        #region back按钮事件
        bool isBackQuit = false;
        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            try
            {
                while (this.NavigationService.CanGoBack)
                {
                    this.NavigationService.RemoveBackEntry();
                }
                if (isBackQuit)
                {
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                    isBackQuit = true;
                    ToastPrompt toast = new ToastPrompt
                    {
                        Message = AppResources.MainBackNotice
                    };
                    toast.Show();
                    Thread t = new Thread(backWait);
                    t.Start();
                }
            }
            catch { e.Cancel = true; }
        }
        private void backWait()
        {
            Thread.Sleep(3000);
            isBackQuit = false;
        }
        #endregion
    }
}