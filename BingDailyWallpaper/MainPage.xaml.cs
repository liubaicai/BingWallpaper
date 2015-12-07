using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage.Streams;
using Windows.System;
using Windows.System.UserProfile;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using BingDailyWallpaper.Control;
using BingDailyWallpaper.Helper;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace BingDailyWallpaper
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private ImageSource _rootBackBrush;

        public ImageSource RootBackBrush
        {
            get { return _rootBackBrush; }
            set
            {
                _rootBackBrush = value;
                NotifyPropertyChanged(nameof(RootBackBrush));
            }
        }

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
            }
        }

        public string BingImageUrl { get; set; }
        public StorageFile BingImageFile { get; set; }

        private int BingImageIndex { get; set; } = 0;

        public static MainPage Instance { get; set; }

        public IAsyncOperation<StorageFolder> CacheImageFolder => ApplicationData.Current.LocalFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);

        public MainPage()
        {
            this.InitializeComponent();
            Instance = this;
            if (!DesignMode.DesignModeEnabled)
            {
                this.DataContext = this;
                LoadBackBrush();
                this.Loaded += OnLoaded;

                if (PlatformHelper.IsMobile)
                {
                    Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
                    Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
                    SetStatusBar();
                }
            }
        }

        private async void SetStatusBar()
        {
            StatusBar statusBar = StatusBar.GetForCurrentView();
            await statusBar.HideAsync();
        }

        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            if (PopupFrame.Instance260.IsOpen)
            {
                PopupFrame.Instance260.Hide();
                e.Handled = true;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            LoadBackTask();
        }

        private async void ShowPopup()
        {
            if (Settings.Get("IsShowSettingMessage", "1").ToString() == "1" && (!SetLockScreen || !SetWallpaper))
            {
                await Task.Delay(3000);
                MessageDialog dialog = new MessageDialog("是否将必应图片设置为锁屏和壁纸?\r\n稍后可在设置菜单中单独设置.", "提示");
                dialog.Commands.Add(new UICommand("确定", s =>
                {
                    Settings.Set("IsShowSettingMessage", "0");
                    SetLockScreen = true;
                    SetWallpaper = true;
                }));
                dialog.Commands.Add(new UICommand("取消", s =>
                {
                    Settings.Set("IsShowSettingMessage", "0");
                }));
                await dialog.ShowAsync();
            }
        }

        private async void LoadBackBrush(bool isNew = false)
        {
            if (RootBackBrush == null|| isNew)
            {
                var level = NetworkInformation.GetInternetConnectionProfile()?.GetNetworkConnectivityLevel();
                if (level == NetworkConnectivityLevel.None)
                {
                    return;
                }
                try
                {
                    RootBackBrush = null;
                    CopyrightLink.Content = "";
                    CopyrightLink.NavigateUri = null;
                    var str = await HtmlCacheHelper.Instance.GetCacheXml(BingImageIndex);
                    var regEx = new Regex(@"(\<urlBase\>(?<urlbase>.*?)\<\/urlBase\>.*?\<copyright\>(?<copyright>.*?)\<\/copyright\>.*?\<copyrightlink\>(?<copyrightlink>.*?)\<\/copyrightlink\>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    foreach (Match match in regEx.Matches(str))
                    {
                        BingImageUrl = "http://cn.bing.com" + match.Groups["urlbase"].Value + "_1920x1080.jpg";
                        var filename = BingImageUrl.Split('/').Last();
                        StorageFolder storageFolder = await CacheImageFolder;
                        var b = await IsFileExist(storageFolder, filename);
                        if (!b)
                        {
                            Debug.WriteLine("=====DownLoadImage:"+ BingImageIndex);
                            var tmpfile = await storageFolder.CreateFileAsync(filename);
                            var http = new HttpClient();
                            var data = await http.GetByteArrayAsync(BingImageUrl);
                            await FileIO.WriteBytesAsync(tmpfile, data);
                        }
                        var file = await storageFolder.GetFileAsync(filename);
                        using (var stream = await file.OpenReadAsync())
                        {
                            if (stream.Size <= 0)
                            {
                                Debug.WriteLine("=====DownLoadImage");
                                var tmpfile = await storageFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                                var http = new HttpClient();
                                var data = await http.GetByteArrayAsync(BingImageUrl);
                                await FileIO.WriteBytesAsync(tmpfile, data);
                            }
                        }
                        if (AutoSaveToLib)
                        {
                            b = await IsFileExist(KnownFolders.SavedPictures, filename);
                            if (!b)
                            {
                                await file.CopyAsync(KnownFolders.SavedPictures);
                            }
                        }
                        BingImageFile = file;
                        var t1 = DateTime.Now;
                        //IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read);
                        //BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);
                        //WriteableBitmap target = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                        //target.SetSource(fileStream);
                        //RootBackBrush = target;
                        RootBackBrush = new StorageImage(file);
                        var t2 = DateTime.Now;
                        CopyrightLink.Content = match.Groups["copyright"].Value;
                        CopyrightLink.NavigateUri = new Uri(match.Groups["copyrightlink"].Value, UriKind.Absolute);
                        Debug.WriteLine("=====SetImage:" + BingImageIndex + ",time=" + (t2 - t1).TotalMilliseconds);
                    }

                    if (BingImageFile != null && BingImageIndex == 0)
                    {
                        SetBingImage();
                    }
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception.Message);
                }
                ShowPopup();
            }
        }

        private async Task<bool> IsFileExist(StorageFolder storageFolder,string filename)
        {
            return (await storageFolder.GetFilesAsync()).Any(file => file.Name == filename);
        }

        private async Task<bool> IsFolderExist(StorageFolder storageFolder, string foldername)
        {
            return (await storageFolder.GetFoldersAsync()).Any(folder => folder.Name == foldername);
        }

        public async void SetBingImage()
        {
            try
            {
                if (Settings.Get("SetLockScreen", "0").ToString() == "1")
                {
                    var result = await UserProfilePersonalizationSettings.Current.TrySetLockScreenImageAsync(BingImageFile);
                    Debug.WriteLine("============LockScreen:" + result);
                }
                if (Settings.Get("SetWallpaper", "0").ToString() == "1")
                {
                    var result = await UserProfilePersonalizationSettings.Current.TrySetWallpaperImageAsync(BingImageFile);
                    Debug.WriteLine("============Wallpaper:" + result);
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
        }

        public async void SaveBingImage()
        {
            if (AutoSaveToLib && BingImageFile != null)
            {
                var b = await IsFileExist(KnownFolders.SavedPictures, BingImageUrl.Split('/').Last());
                if (!b)
                {
                    await BingImageFile.CopyAsync(KnownFolders.SavedPictures);
                }
            }
        }

        private async void LoadBackTask()
        {
            var result = await BackgroundExecutionManager.RequestAccessAsync();
            if (result != BackgroundAccessStatus.Denied)
            {
                var taskRegistered = false;
                var exampleTaskName = "BingBackgroundTask";

                foreach (var task in BackgroundTaskRegistration.AllTasks)
                {
                    if (task.Value.Name == exampleTaskName)
                    {
                        taskRegistered = true;
                        break;
                    }
                }

                if (!taskRegistered)
                {
                    var builder = new BackgroundTaskBuilder();

                    builder.Name = exampleTaskName;
                    builder.TaskEntryPoint = "BDWBackgroundTask.BingBackgroundTask";
                    builder.SetTrigger(new TimeTrigger(30, false));
                    builder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));

                    BackgroundTaskRegistration task = builder.Register();
                    task.Completed += (registration, args) =>
                    {
                        Debug.WriteLine("============" + registration.Name);
                    };
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AboutButtonOnClick(object sender, RoutedEventArgs e)
        {
            PopupFrame.Instance260.Show(typeof(SettingView));
        }

        private async void DownLoadButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (BingImageFile == null)
            {
                return;
            }
            FileSavePicker fsp = new FileSavePicker();
            fsp.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            fsp.FileTypeChoices.Add("Image", new List<string>() { ".jpg" });
            fsp.SuggestedFileName = BingImageUrl.Split('/').Last();
            StorageFile file = await fsp.PickSaveFileAsync();
            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);
                IBuffer buffer = await FileIO.ReadBufferAsync(BingImageFile);
                await FileIO.WriteBufferAsync(file, buffer);
                await CachedFileManager.CompleteUpdatesAsync(file);
            }
        }

        private void FirstButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (BingImageIndex != 0)
            {
                BingImageIndex = 0;
                LoadBackBrush(true);
            }
        }

        private void BackButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (BingImageIndex >= 0)
            {
                BingImageIndex--;
                LoadBackBrush(true);
            }
        }

        private void ForwardButtonOnClick(object sender, RoutedEventArgs e)
        {
            BingImageIndex++;
            LoadBackBrush(true);
        }

        private async void MarktReviewButtonOnClick(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9NBLGGH67TJK"));
        }
    }
}
