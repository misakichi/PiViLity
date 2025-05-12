using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiViLityCore.Plugin;

namespace PiViLityCore.Option
{
    /// <summary>
    /// シェル設定クラス
    /// </summary>
    [Serializable, Option]
    public class ShellSettings : SettingBase
    {
        static public readonly ShellSettings Instance = new();

        public override string CategoryText { 
            get=>Global.GetResourceString("ShellSetting.Name");
        }

        public override string CategoryName
        {
            get => "ShellSetting";
        }


        [OptionItem(NoOption = true)]
        public View FileListViewStyle = View.Tile;

        [OptionItemSize
            (
                NameTextResouceId = "ShellSetting.ThumbnailSize",
                DescriptionTextResouceId = "",
                MinWidth = 64, MinHeight = 32, MaxWidth = 1024, MaxHeight = 1024
            )
        ]
        public Size ThumbnailSize = new(384, 273);

    }
}
