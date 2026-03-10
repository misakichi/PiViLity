using PiViLityCore.Option;
using PiViLityPlugin.Difinition;
using PiViLityPlugin.Option;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace PiViLity.Option
{

    public enum Language
    {
        [OptionItem(TextResouceId = "Application.Language.SystemDefault")]
        SystemDefault = 0,
        [OptionItem(TextResouceId = "Application.Language.Japanese")]
        Japanese = 1,
        [OptionItem(TextResouceId = "Application.Language.English")]
        English = 2,
    }
    public enum ColorTheme
    {
        [OptionItem(TextResouceId = "Application.Theme.SystemDefault")]
        SystemDefault = 0,
        [OptionItem(TextResouceId = "Application.Theme.Light")]
        Light = 1,
        [OptionItem(TextResouceId = "Application.Theme.Dark")]
        Dark = 2,
    }


    [Serializable, Option]
    public class AppSettings : SettingBase
    {
        static public readonly AppSettings Instance = new();

        public override string CategoryText { get => SettingResource.GetString("Application") ?? ""; }
        public override string CategoryName { get => "PiViLity App"; }

        public string ApplicationDirectory = System.IO.Path.GetDirectoryName(Application.ExecutablePath) ?? "";

        public string DockLayoutFile = (System.IO.Path.GetDirectoryName(Application.ExecutablePath) ?? "") + "\\layout.config";

        [OptionItem(TextResouceId = "Application.Language", DescriptionTextResouceId ="Application.Language.Desc", UIOrder = 1)]
        public Language AppLanguage = Language.SystemDefault;

        [OptionItem(TextResouceId = "Application.Theme", DescriptionTextResouceId = "Application.Theme.Desc", UIOrder = 2)]
        public ColorTheme Theme = ColorTheme.SystemDefault;

        [OptionItem(NoOption = true)]
        public List<TvLvTabPage> TvLvTabPages { get; set; } = new();

        public override System.Resources.ResourceManager SettingResource => Option.Resource.ResourceManager;

        public override ushort GroupUIOrder => 0;

        [OptionItem(NoOption = true)]
        public string CacheDb = (System.IO.Path.GetDirectoryName(Application.ExecutablePath) ?? "") + "\\thumbnail.db";

        [OptionItem(NoOption = true)]
        public FormWindowState WindowState = FormWindowState.Normal;

        [OptionItem(NoOption = true)]
        public Point WindowPosition = new();

        [OptionItem(NoOption = true)]
        public Size WindowSize = new();


    }
}
