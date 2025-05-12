using PiViLityCore.Option;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLity.Option
{
    [Serializable, Option(NoOption = true)]
    public class AppSettings : PiViLityCore.Plugin.SettingBase
    {
        static public readonly AppSettings Instance = new();

        public override string CategoryText { get => "PiViLity App"; }
        public override string CategoryName { get => "PiViLity App"; }

        [OptionItem]
        public List<TvLvTabPage> TvLvTabPages { get; set; } = new();

        [OptionItem]
        public string CacheDb = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\thumbnail.db" ?? "";

        [OptionItem]
        public FormWindowState WindowState = FormWindowState.Normal;

        [OptionItem]
        public Point WindowPosition = new();

        [OptionItem]
        public Size WindowSize = new();


    }
}
