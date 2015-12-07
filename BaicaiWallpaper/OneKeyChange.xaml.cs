using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using Windows.Storage;
using System.Diagnostics;
using System.IO;
using BaicaiWallpaper.Helpers;
using BaicaiWallpaper.Resources;

namespace BaicaiWallpaper
{
    public partial class OneKeyChange : PhoneApplicationPage
    {
        public OneKeyChange()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var isProvider = Windows.Phone.System.UserProfile.LockScreenManager.IsProvidedByCurrentApplication;
            if (!isProvider)
            {
                MessageBox.Show(AppResources.OneKeyNoSetNotice);
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
                return;
            }
            else
            {
                try
                {
                    IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();
                    if (isolatedStorageFile.DirectoryExists("Setting") && isolatedStorageFile.FileExists("Setting\\fangshi"))
                    {
                        IsolatedStorageFileStream stream = isolatedStorageFile.OpenFile("Setting\\fangshi", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                        StreamReader sr = new StreamReader(stream);
                        String str = sr.ReadToEnd();
                        sr.Close();
                        stream.Close();
                        if (str.Equals("shunxu"))
                        {
                            shunxuInvoke();
                        }
                        else
                        {
                            suijiInvoke();
                        }
                    }
                    else
                    {
                        suijiInvoke();
                    }
                }
                catch { }
                var first = ApplicationSettingsHelper.GetValueOrDefault("onekeychange", 0);
                if (first == 0)
                {
                    ApplicationSettingsHelper.AddOrUpdateValue("onekeychange", 1);
                }
                else
                {
                    if (NavigationService.CanGoBack)
                    {
                        NavigationService.GoBack();
                    }
                    Application.Current.Terminate();
                }
                base.OnNavigatedTo(e);
            }
        }

        private async void shunxuInvoke()
        {
            try
            {
                var isProvider = Windows.Phone.System.UserProfile.LockScreenManager.IsProvidedByCurrentApplication;
                if (!isProvider)
                {
                    MessageBox.Show(AppResources.OneKeyNoSetNotice);
                }
                if (isProvider)
                {
                    StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                    if (local != null)
                    {
                        var currentImageTemp = Windows.Phone.System.UserProfile.LockScreen.GetImageUri();
                        StorageFolder lockimageFolder = await local.CreateFolderAsync("LockImages", CreationCollisionOption.OpenIfExists);
                        var files = await lockimageFolder.GetFilesAsync();
                        if (files.Count > 0)
                        {
                            bool isNext = false;
                            for (int i = 0; i < files.Count; i++)
                            {
                                if (isNext)
                                {
                                    isNext = false;
                                    var schema = "ms-appdata:///Local/LockImages/";
                                    var uri = new Uri(schema + files[i].Name, UriKind.Absolute);
                                    Debug.WriteLine(uri.ToString());
                                    Windows.Phone.System.UserProfile.LockScreen.SetImageUri(uri);
                                }
                                else
                                {
                                    var currentImage = Windows.Phone.System.UserProfile.LockScreen.GetImageUri();
                                    if (currentImage.ToString().Contains(files[i].Name))
                                    {
                                        isNext = true;
                                    }
                                }
                            }
                            if (isNext)
                            {
                                isNext = false;
                                var schema = "ms-appdata:///Local/LockImages/";
                                var uri = new Uri(schema + files[0].Name, UriKind.Absolute);
                                Debug.WriteLine(uri.ToString());
                                Windows.Phone.System.UserProfile.LockScreen.SetImageUri(uri);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private async void suijiInvoke()
        {
            try
            {
                var isProvider = Windows.Phone.System.UserProfile.LockScreenManager.IsProvidedByCurrentApplication;
                if (!isProvider)
                {
                    MessageBox.Show(AppResources.OneKeyNoSetNotice);
                }
                if (isProvider)
                {
                    StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                    if (local != null)
                    {
                        var currentImageTemp = Windows.Phone.System.UserProfile.LockScreen.GetImageUri();
                        StorageFolder lockimageFolder = await local.CreateFolderAsync("LockImages", CreationCollisionOption.OpenIfExists);
                        var files = await lockimageFolder.GetFilesAsync();
                        Random rd = new Random(DateTime.Now.Millisecond);
                        int count = rd.Next(files.Count);
                        for (int i = 0; i < files.Count; i++)
                        {
                            if (i == count)
                            {
                                var schema = "ms-appdata:///Local/LockImages/";
                                var uri = new Uri(schema + files[i].Name, UriKind.Absolute);
                                Debug.WriteLine(uri.ToString());
                                Windows.Phone.System.UserProfile.LockScreen.SetImageUri(uri);

                                var currentImage = Windows.Phone.System.UserProfile.LockScreen.GetImageUri();
                                Debug.WriteLine("The new lock screen background image is set to {0}", currentImage.ToString());
                            }
                        }
                    }
                }
            }
            catch { }
        }
    }
}