using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Reflection;
using Microsoft.Phone.Tasks;

namespace BaicaiWallpaper
{
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            InitializeComponent();
            this.Loaded += About_Loaded;
        }

        void About_Loaded(object sender, RoutedEventArgs e)
        {
            vision.Text = new AssemblyName(Assembly.GetExecutingAssembly().FullName).Version.ToString();
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask ect = new EmailComposeTask();
            ect.To = "liushuai.baicai@hotmail.com";
            ect.Show();
        }

        private void ButtonEx_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MarketplaceReviewTask task = new MarketplaceReviewTask();
                task.Show();
            }
            catch { }
        }

        private void WeiboHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri("http://weibo.com/liubaicai");
            task.Show();
        }
    }
}