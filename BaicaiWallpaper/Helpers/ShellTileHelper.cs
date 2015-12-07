using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaicaiWallpaper.Helpers
{
    public class ShellTileHelper
    {
        public static bool IsPinned(Uri uri)
        {
            var item = ShellTile.ActiveTiles.FirstOrDefault
                (x => x.NavigationUri == uri);

            return item != null;
        }

        public static bool IsPinned(string uniqueId)
        {
            var item = ShellTile.ActiveTiles.FirstOrDefault
                (x => x.NavigationUri.ToString().Contains(uniqueId));

            return item != null;
        }

        public static void Pin(Uri uri, ShellTileData initialData)
        {
            ShellTile.Create(uri, initialData);
        }

        public static void UnPin(string uniqueId)
        {
            var item = ShellTile.ActiveTiles.FirstOrDefault
               (x => x.NavigationUri.ToString().Contains(uniqueId));

            if (item != null)
                item.Delete();
        }
    }
}
