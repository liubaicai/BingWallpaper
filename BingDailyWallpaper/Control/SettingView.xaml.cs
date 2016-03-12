using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace BingDailyWallpaper.Control
{
    public sealed partial class SettingView : Page, INotifyPropertyChanged
    {
        public bool SetLockScreen
        {
            get { return Settings.Get(nameof(SetLockScreen), "0").ToString() == "1"; }
            set
            {
                Settings.Set(nameof(SetLockScreen), value ? "1" : "0");
                NotifyPropertyChanged(nameof(SetLockScreen));
                if (value)
                {
                    MainPage.Instance.SetBingImage();
                }
            }
        }

        public bool SetWallpaper
        {
            get { return Settings.Get(nameof(SetWallpaper), "0").ToString() == "1"; }
            set
            {
                Settings.Set(nameof(SetWallpaper), value ? "1" : "0");
                NotifyPropertyChanged(nameof(SetWallpaper));
                if (value)
                {
                    MainPage.Instance.SetBingImage();
                }
            }
        }

        public bool AutoSaveToLib
        {
            get { return Settings.Get(nameof(AutoSaveToLib), "0").ToString() == "1"; }
            set
            {
                Settings.Set(nameof(AutoSaveToLib), value ? "1" : "0");
                NotifyPropertyChanged(nameof(AutoSaveToLib));
                if (value)
                {
                    MainPage.Instance.SaveBingImage();
                }
            }
        }

        public SettingView()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
