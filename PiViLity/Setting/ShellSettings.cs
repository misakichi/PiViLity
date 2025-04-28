using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLity.Setting
{
    [Serializable, PiViLityCore.Plugin.Setting(NameTextResouceId= "ShellSetting.Name")]
    public class ShellSettings : PiViLityCore.Plugin.SettingBase
    {
        static public readonly ShellSettings Instance = new();

        [PiViLityCore.Plugin.Setting(NoOption = true)]
        public override string Name { get => "Filter Setting"; }


        [PiViLityCore.Plugin.Setting(NameTextResouceId = "ShellSetting.ThumbnailSize", DescriptionTextResouceId ="")]
        public Size ThumbnailSize = new(384, 273);
    }
}
