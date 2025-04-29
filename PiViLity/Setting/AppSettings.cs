using PiViLityCore.Option;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLity.Setting
{
    [Serializable, Option(NoOption = true)]
    public class AppSettings : PiViLityCore.Plugin.SettingBase
    {
        static public readonly AppSettings Instance = new();

        public override string Name { get => "PiViLity App"; }

        public List<TvLvTabPage> TvLvTabPages { get; set; } = new();

        public string CacheDb = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\thumbnail.db" ?? "";
        public View FileListViewStyle = View.Tile;

        public FormWindowState WindowState = FormWindowState.Normal;
        public Point WindowPosition = new();
        public Size WindowSize = new();


    }
}
