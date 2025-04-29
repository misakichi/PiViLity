using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiViLityCore.Option;
using PiViLityCore.Plugin;

namespace PiViLity.Setting
{
    [Serializable, Option(NameTextResouceId= "ShellSetting.Name")]
    public class ShellSettings : SettingBase
    {
        static public readonly ShellSettings Instance = new();

        [OptionItem(NoOption = true)]
        public override string Name { get => "Filter Setting"; }


        [OptionItemSize(
        NameTextResouceId = "ShellSetting.ThumbnailSize",
        DescriptionTextResouceId = "",
        MinWidth = 32, MinHeight = 32, MaxWidth = 1024, MaxHeight = 1024)]
        public Size ThumbnailSize = new(384, 273);

    }
}
