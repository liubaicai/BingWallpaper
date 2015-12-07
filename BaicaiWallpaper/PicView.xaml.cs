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
using System.IO;
using BaicaiWallpaper.Helpers;

namespace BaicaiWallpaper
{
    public partial class PicView : PhoneApplicationPage
    {
        public PicView()
        {
            InitializeComponent();
            this.Loaded += PicView_Loaded;
        }

        void PicView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (NavigationContext.QueryString.ContainsKey("filename"))
                {
                    string filename = HttpUtility.UrlDecode(NavigationContext.QueryString["filename"]);
                    IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();
                    imageView.Source = new StorageImage(filename);
                }
            }
            catch
            {
            }
        }
    }
}