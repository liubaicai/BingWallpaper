using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BaicaiWallpaper.Models;
using System.Collections.ObjectModel;
using System.Threading;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;
using System.ComponentModel;

namespace BaicaiWallpaper
{
    public partial class PicsFolders : PhoneApplicationPage
    {
        public ObservableCollection<LibsFolder> listFolders { get; set; }

        public PicsFolders()
        {
            listFolders = new ObservableCollection<LibsFolder>();
            InitializeComponent();
            DataContext = this;

            this.Loaded += PicsFolders_Loaded;
        }

        void PicsFolders_Loaded(object sender, RoutedEventArgs e)
        {
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode != NavigationMode.Back)
            {
                Thread t = new Thread(LoadFolder);
                t.Start();
            }
        }

        private void LoadFolder()
        {
            //try
            //{
                MediaLibrary mediaLibrary = new MediaLibrary();
                List<PictureAlbum> folders = new List<PictureAlbum>();
                foreach (var pic in mediaLibrary.Pictures)
                {
                    if (!folders.Contains(pic.Album))
                    {
                        folders.Add(pic.Album);
                    }
                }

                Dispatcher.BeginInvoke(() =>
                {
                    foreach (var folder in folders)
                    {
                        listFolders.Add(new LibsFolder(folder));
                    }
                    NotifyPropertyChanged("listFolders");
                });
            //}
            //catch { }
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

        private void folderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (listBox.SelectedIndex != -1)
            {
                LibsFolder folder = listBox.SelectedItem as LibsFolder;
                Datas.folder = folder.folder;
                NavigationService.Navigate(new Uri("/PicsAdd.xaml", UriKind.RelativeOrAbsolute));
                listBox.SelectedIndex = -1;
            }
        }
    }
}