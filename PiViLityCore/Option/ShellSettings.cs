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
    [Serializable, Option(NameTextResouceId= "ShellSetting.Name")]
    public class ShellSettings : SettingBase
    {
        static public readonly ShellSettings Instance = new();

        [OptionItem(NoOption = true)]
        public override string Name { get => "Filer Setting"; }

        [OptionItem(NoOption = true)]
        public View FileListViewStyle = View.Tile;

        [OptionItemSize(
        NameTextResouceId = "ShellSetting.ThumbnailSize",
        DescriptionTextResouceId = "",
        MinWidth = 64, MinHeight = 32, MaxWidth = 1024, MaxHeight = 1024)]
        public Size ThumbnailSize = new(384, 273);

    }
}
