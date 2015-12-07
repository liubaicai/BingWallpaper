using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace BingDailyWallpaper.Control
{
    public class PopupFrame
    {
        private static PopupFrame _instance;
        public static PopupFrame Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PopupFrame(Colors.White);
                }
                return _instance;
            }
        }

        private static PopupFrame _instance260;
        public static PopupFrame Instance260 => _instance260 ?? (_instance260 = new PopupFrame(Color.FromArgb(255, 51, 51, 51), 260));

        readonly Popup _popup = new Popup();
        readonly Grid grid = new Grid();
        readonly Frame frame = new Frame();

        private PopupFrame(Color backcolor, int width = 600)
        {
            _popup.ChildTransitions = new TransitionCollection() { new EntranceThemeTransition() { FromHorizontalOffset = width, IsStaggeringEnabled = false } };

            frame.Background = new SolidColorBrush(backcolor);
            frame.Width = width;
            frame.HorizontalAlignment = HorizontalAlignment.Right;
            frame.Height = Window.Current.Bounds.Height;
            frame.Margin = new Thickness(48, 1, 0, 0);
            frame.Tapped += Frame_Tapped;
            grid.Background = new SolidColorBrush(Colors.Transparent);
            grid.Width = Window.Current.Bounds.Width;
            grid.Height = Window.Current.Bounds.Height;
            grid.Tapped += Grid_Tapped;
            grid.Children.Add(frame);
            _popup.Child = grid;
            Window.Current.SizeChanged += Current_SizeChanged;
        }

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            frame.Height = Window.Current.Bounds.Height;
            grid.Width = Window.Current.Bounds.Width;
            grid.Height = Window.Current.Bounds.Height;
            _popup.Width = Window.Current.Bounds.Width;
            _popup.Height = Window.Current.Bounds.Height;
        }

        private void Frame_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void Grid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Hide();
        }

        public EventHandler<bool> VisibleChanged;

        public bool IsOpen { get { return _popup.IsOpen; } }

        public void Show(Type type, object param = null, HorizontalAlignment direction = HorizontalAlignment.Right)
        {
            frame.HorizontalAlignment = direction;
            if (IsOpen)
                Hide();
            _popup.IsOpen = true;
            VisibleChanged?.Invoke(Instance, true);
            Navigate(type, param);
        }

        public void Hide()
        {
            _popup.IsOpen = false;
            frame.Content = null;
            VisibleChanged?.Invoke(Instance, false);
        }

        private void Navigate(Type type, object param)
        {
            frame.Navigate(type, param);
        }

        public void GoBack()
        {
            while (frame.CanGoBack)
            {
                frame.GoBack();
            }
            frame.Content = null;
            Hide();
        }
    }
}
