using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace BingImage
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private string XmlUrl = "http://www.bing.com/hpimagearchive.aspx?format=xml&idx=0&n=1&mbl=1&mkt=zh-cn";

        private string FolderPath { get; } = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)+"\\BingImage\\Data";

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var http = new WebClient();
            http.DownloadStringAsync(new Uri(XmlUrl));
            http.DownloadStringCompleted += (o, args) =>
            {
                try
                {
                    var xml = args.Result;
                    var regEx = new Regex(@"(\<urlBase\>(?<urlbase>.*?)\<\/urlBase\>.*?\<copyright\>(?<copyright>.*?)\<\/copyright\>.*?\<copyrightlink\>(?<copyrightlink>.*?)\<\/copyrightlink\>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    foreach (Match match in regEx.Matches(xml))
                    {
                        var imgurl = "http://cn.bing.com" + match.Groups["urlbase"].Value + "_1920x1080.jpg";
                        BingImageControl.Source = new BitmapImage(new Uri(imgurl));
                        DownImage(imgurl);
                        break;
                    }
                }
                catch (Exception exception)
                {
                    Debug.WriteLine("DownloadStringAsync:" + exception.Message);
                }
            };
        }

        private void DownImage(string url)
        {
            var filename = System.IO.Path.GetFileName(url);
            if (string.IsNullOrEmpty(filename))
            {
                return;
            }
            var localpath = System.IO.Path.Combine(FolderPath, filename);
            var bmpPath = localpath.Replace(".jpg", ".bmp");
            if (File.Exists(bmpPath))
            {
                BingProgressBar.Visibility = Visibility.Collapsed;
                ChangeImg(bmpPath);
                Process.GetCurrentProcess().Kill();
            }
            else
            {
                var http = new WebClient();
                http.DownloadFileAsync(new Uri(url), localpath);
                http.DownloadFileCompleted += (sender, args) =>
                {
                    BingProgressBar.Visibility = Visibility.Collapsed;
                    var bitmap = new Bitmap(localpath);
                    bitmap.Save(bmpPath,ImageFormat.Bmp);
                    bitmap.Dispose();
                    ChangeImg(bmpPath);
                    try
                    {
                        File.Delete(localpath);
                    }
                    catch (Exception)
                    {
                    }
                    Process.GetCurrentProcess().Kill();
                };
            }
        }

        private void ChangeImg(string bmpPath)
        {
            if (File.Exists(bmpPath))
            {
                var nResult = Win32.SystemParametersInfo(20, 0, bmpPath, 0x02 | 0x02); //更换壁纸
                if (nResult == 0)
                {
                    Debug.WriteLine("没有更新成功!");
                }
                else
                {
                    RegistryKey hk = Registry.CurrentUser;
                    RegistryKey run = hk.CreateSubKey(@"Control Panel\Desktop\");
                    run?.SetValue("Wallpaper", bmpPath); //将新图片路径写入注册表
                }
                try
                {
                    Win32.SHChangeNotify(0x8000000, 0, IntPtr.Zero, IntPtr.Zero);
                }
                catch (Exception)
                {
                }
            }
            else
            {
                Debug.WriteLine("文件不存在!");
            }
        }
    }
}
