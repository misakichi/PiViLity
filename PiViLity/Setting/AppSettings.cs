using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLity.Setting
{
    [Serializable]
    public class AppSettings : PiViLityCore.Plugin.SettingBase
    {
        static public readonly AppSettings Instance = new();

        public List<FileView> FileViews { get; set; } = new();

        public string CacheDb = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\thumbnail.db" ?? "";
        public View FileListViewStyle = View.Tile;

        public FormWindowState WindowState = FormWindowState.Normal;
        public Point WindowPosition = new();
        public Size WindowSize = new();
        public Size ThumbnailSize = new(384, 273);

        public override string Name { get; set; } = "PiViLity App";


    }
}
