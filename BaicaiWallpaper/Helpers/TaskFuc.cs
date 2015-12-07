//#define DEBUG_AGENT

using BaicaiWallpaper.Resources;
using Coding4Fun.Phone.Controls;
using Microsoft.Phone.Scheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Storage;

namespace BaicaiWallpaper.Helpers
{
    public class TaskFuc
    {
        static string taskName = "BaicaiWallpaper";
        static string description = AppResources.TaskNotice;

        public static void startTask()
        {
            PeriodicTask t;
            t = ScheduledActionService.Find(taskName) as PeriodicTask;
            bool found = (t != null);
            if (!found)
            {
                t = new PeriodicTask(taskName);
            }
            t.Description = description;
            //t.ExpirationTime = DateTime.Now.AddDays(10);
            try
            {
                if (!found)
                {
                    ScheduledActionService.Add(t);
                }
                else
                {
                    ScheduledActionService.Remove(taskName);
                    ScheduledActionService.Add(t);
                }
            }
            catch
            {
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    ToastPrompt toast1 = new ToastPrompt
                    {
                        Message = AppResources.TaskErrToast1
                    };
                    toast1.Completed += toast1_Completed;
                    toast1.Show();
                });
            }
#if(DEBUG_AGENT)
            ScheduledActionService.LaunchForTest(taskName, TimeSpan.FromSeconds(5));
#endif
        }

        static void toast1_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                ToastPrompt toast2 = new ToastPrompt
                {
                    Message = AppResources.TaskErrToast2
                };
                toast2.Show();
            });
        }

        public static async void setLock()
        {
            var isProvider = Windows.Phone.System.UserProfile.LockScreenManager.IsProvidedByCurrentApplication;
            if (isProvider)
            {
                var currentImageTemp = Windows.Phone.System.UserProfile.LockScreen.GetImageUri();
                if (currentImageTemp.ToString().Equals("ms-appx:///DefaultLockScreen.jpg"))
                {
                    StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                    if (local != null)
                    {
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
        }
    }
}
