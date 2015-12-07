using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System;
using Windows.Storage;
using System.IO.IsolatedStorage;
using System.IO;

namespace BaicaiWallpaperTask
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        /// <remarks>
        /// ScheduledAgent 构造函数，初始化 UnhandledException 处理程序
        /// </remarks>
        static ScheduledAgent()
        {
            // 订阅托管的异常处理程序
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        /// 出现未处理的异常时执行的代码
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // 出现未处理的异常；强行进入调试器
                Debugger.Break();
            }
        }

        /// <summary>
        /// 运行计划任务的代理
        /// </summary>
        /// <param name="task">
        /// 调用的任务
        /// </param>
        /// <remarks>
        /// 调用定期或资源密集型任务时调用此方法
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            //TODO: 添加用于在后台执行任务的代码
            try
            {
                IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();
                if (!isolatedStorageFile.DirectoryExists("Setting"))
                {
                    isolatedStorageFile.CreateDirectory("Setting");
                }
                DateTime dt = DateTime.Now;
                if (isolatedStorageFile.DirectoryExists("Setting") && isolatedStorageFile.FileExists("Setting\\time"))
                {
                    IsolatedStorageFileStream stream = isolatedStorageFile.OpenFile("Setting\\time", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    StreamReader sr = new StreamReader(stream);
                    String str = sr.ReadToEnd();
                    dt = DateTime.Parse(str);
                    sr.Close();
                    stream.Close();
                }
                else
                {
                    if (!isolatedStorageFile.DirectoryExists("Setting"))
                    {
                        isolatedStorageFile.CreateDirectory("Setting");
                    }
                    IsolatedStorageFileStream stream = isolatedStorageFile.OpenFile("Setting\\time", FileMode.Create, FileAccess.ReadWrite);
                    StreamWriter sw = new StreamWriter(stream);
                    sw.Write(DateTime.Now.AddMinutes(-30).ToString("yyyy-MM-dd HH:mm:ss"));
                    sw.Flush();
                    sw.Close();
                    stream.Close();
                }
                if (isolatedStorageFile.DirectoryExists("Setting") && isolatedStorageFile.FileExists("Setting\\pinlv"))
                {
                    IsolatedStorageFileStream stream = isolatedStorageFile.OpenFile("Setting\\pinlv", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    StreamReader sr = new StreamReader(stream);
                    String str = sr.ReadToEnd();
                    if (str != null && str.Length > 0)
                    {
                        int temp = 1;
                        try
                        {
                            temp = Int32.Parse(str);
                        }
                        catch { }
                        Debug.WriteLine("分钟数"+temp * 30);
                        //dt = dt.AddSeconds(5);
                        dt = dt.AddMinutes(temp*30);
                    }
                    sr.Close();
                    stream.Close();
                }
                Debug.WriteLine(dt.ToString());
                Debug.WriteLine(DateTime.Now.ToString());
                if (DateTime.Compare(dt,DateTime.Now)<=0)
                {
                    if (isolatedStorageFile.DirectoryExists("Setting") && isolatedStorageFile.FileExists("Setting\\time"))
                    {
                        IsolatedStorageFileStream stream = isolatedStorageFile.OpenFile("Setting\\time", FileMode.Create, FileAccess.ReadWrite);
                        StreamWriter sw = new StreamWriter(stream);
                        sw.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        sw.Flush();
                        sw.Close();
                        stream.Close();
                    }
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
                            Debug.WriteLine("顺序");
                        }
                        else
                        {
                            suijiInvoke();
                            Debug.WriteLine("随机");
                        }
                    }
                    else
                    {
                        suijiInvoke();
                        Debug.WriteLine("随机");
                    }
                }
                else
                {
                    Debug.WriteLine("尚未执行");
                }
            }
            catch { }

            NotifyComplete();
        }

        private async void shunxuInvoke()
        {
            try
            {
                var isProvider = Windows.Phone.System.UserProfile.LockScreenManager.IsProvidedByCurrentApplication;
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