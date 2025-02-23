using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Shell
{
    public interface IIconStore
    {
        ImageList SmallIconList { get; }
        ImageList LargeIconList { get; }
        ImageList TileIconList { get; }


        void GetIcon(Environment.SpecialFolder specialFolder, Action<int>? returnAction);

        void GetIcon(string path, Action<int>? returnAction);

        public void Clear(Size? largeSize = null, Size? smallSize = null, Size? tileSize = null);

    }
}
