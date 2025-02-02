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

        public string CacheDbDirectory = System.IO.Path.GetDirectoryName(Application.ExecutablePath) ?? "";
        public View FileListViewStyle = View.Tile;

        public FormWindowState WindowState = FormWindowState.Normal;
        public Point WindowPosition = new Point();
        public Size WindowSize = new Size();

        public override string Name { get; set; } = "PiViLity App";


    }
}
