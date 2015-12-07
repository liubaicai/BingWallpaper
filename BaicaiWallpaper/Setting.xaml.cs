using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Windows.Storage;
using System.IO;
using System.IO.IsolatedStorage;
using Coding4Fun.Phone.Controls;
using BaicaiWallpaper.Helpers;
using System.ComponentModel;
using BaicaiWallpaper.Resources;

namespace BaicaiWallpaper
{
    public partial class Setting : PhoneApplicationPage
    {
        private string pinurl = "/OneKeyChange.xaml";
        private string pinImage = "/Assets/Tiles/OneKeyChange.png";
        private string pinTitle = AppResources.OneKeyPinTitle;
        public Setting()
        {
            InitializeComponent();
            this.Loaded += Setting_Loaded;
            DataContext = this;
        }

        void Setting_Loaded(object sender, RoutedEventArgs e)
        {
            #region 更新设置
            IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();
            try
            {
                if (isolatedStorageFile.DirectoryExists("Setting") && isolatedStorageFile.FileExists("Setting\\pinlv"))
                {
                    IsolatedStorageFileStream stream = isolatedStorageFile.OpenFile("Setting\\pinlv", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    StreamReader sr = new StreamReader(stream);
                    String str = sr.ReadToEnd();
                    switch (str)
                    {
                        case "1":
                            pinlv.SelectedIndex = 0;
                            break;
                        case "2":
                            pinlv.SelectedIndex = 1;
                            break;
                        case "6":
                            pinlv.SelectedIndex = 2;
                            break;
                        case "12":
                            pinlv.SelectedIndex = 3;
                            break;
                        case "24":
                            pinlv.SelectedIndex = 4;
                            break;
                        default:
                            pinlv.SelectedIndex = 0;
                            break;
                    }
                    sr.Close();
                    stream.Close();
                }
            }
            catch { }
            try
            {
                if (isolatedStorageFile.DirectoryExists("Setting") && isolatedStorageFile.FileExists("Setting\\fangshi"))
                {
                    IsolatedStorageFileStream stream = isolatedStorageFile.OpenFile("Setting\\fangshi", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    StreamReader sr = new StreamReader(stream);
                    String str = sr.ReadToEnd();
                    switch (str)
                    {
                        case "suiji":
                            fangshi.SelectedIndex = 0;
                            break;
                        case "shunxu":
                            fangshi.SelectedIndex = 1;
                            break;
                        default:
                            fangshi.SelectedIndex = 0;
                            break;
                    }
                    sr.Close();
                    stream.Close();
                }
            }
            catch { }
            pinlv.SelectionChanged += pinlv_SelectionChanged;
            fangshi.SelectionChanged += fangshi_SelectionChanged;
            #endregion
        }

        #region 更新设置
        void fangshi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ListPickerItem item = fangshi.SelectedItem as ListPickerItem;
                string pl = item.Tag.ToString();
                IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();
                if (!isolatedStorageFile.DirectoryExists("Setting"))
                {
                    isolatedStorageFile.CreateDirectory("Setting");
                }
                IsolatedStorageFileStream stream = isolatedStorageFile.OpenFile("Setting\\fangshi", FileMode.Create, FileAccess.ReadWrite);
                StreamWriter sw = new StreamWriter(stream);
                sw.Write(pl);
                sw.Flush();
                sw.Close();
                stream.Close();
            }
            catch { }
        }

        void pinlv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ListPickerItem item = pinlv.SelectedItem as ListPickerItem;
                string pl = item.Tag.ToString();
                IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();
                if (!isolatedStorageFile.DirectoryExists("Setting"))
                {
                    isolatedStorageFile.CreateDirectory("Setting");
                }
                IsolatedStorageFileStream stream1 = isolatedStorageFile.OpenFile("Setting\\pinlv", FileMode.Create, FileAccess.ReadWrite);
                StreamWriter sw1 = new StreamWriter(stream1);
                sw1.Write(pl);
                sw1.Flush();
                sw1.Close();
                stream1.Close();
                if (!isolatedStorageFile.DirectoryExists("Setting") || !isolatedStorageFile.FileExists("Setting\\time"))
                {
                    IsolatedStorageFileStream stream2 = isolatedStorageFile.OpenFile("Setting\\time", FileMode.Create, FileAccess.ReadWrite);
                    StreamWriter sw2 = new StreamWriter(stream2);
                    sw2.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    sw2.Flush();
                    sw2.Close();
                    stream2.Close();
                }
            }
            catch { }
        }
        #endregion

        #region 按钮点击
        private void wfk_Click(object sender, RoutedEventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("tj_wfk");
            MarketplaceDetailTask task = new MarketplaceDetailTask();
            task.ContentIdentifier = "86f26f7d-c4de-4a49-ac51-9ae63fc334f6";
            task.ContentType = MarketplaceContentType.Applications;
            task.Show();
        }

        private void xzm_Click(object sender, RoutedEventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("tj_xzm");
            MarketplaceDetailTask task = new MarketplaceDetailTask();
            task.ContentIdentifier = "ff8aca92-f19c-4910-bb5f-fc29a33468ee";
            task.ContentType = MarketplaceContentType.Applications;
            task.Show();
        }

        private void Button_About_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.RelativeOrAbsolute));
        }

        private async void Button_Set_Click(object sender, RoutedEventArgs e)
        {
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
        #endregion

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

        #region 数据绑定
        public bool IsOneKeyPinEnabled
        {
            get
            {
                return ShellTileHelper.IsPinned(new Uri(pinurl,UriKind.Relative));
            }
            set
            {
                if (value)
                {
                    if (!ShellTileHelper.IsPinned(new Uri(pinurl, UriKind.Relative)))
                    {
                        StandardTileData NewTileData = new StandardTileData
                        {
                            BackgroundImage = new Uri(pinImage, UriKind.Relative),
                            Title = pinTitle,
                        };
                        ShellTileHelper.Pin(new Uri(pinurl, UriKind.Relative), NewTileData);
                    }
                }
                else
                {
                    if (ShellTileHelper.IsPinned(new Uri(pinurl, UriKind.Relative)))
                    {
                        ShellTileHelper.UnPin(pinurl);
                    }
                }
            }
        }
        #endregion
    }
}