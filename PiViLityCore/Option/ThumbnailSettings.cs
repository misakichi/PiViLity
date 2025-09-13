using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiViLityCore.Plugin;
using PiViLityCore.Resource;
using PiViLityCore.Resources;

namespace PiViLityCore.Option
{
    /// <summary>
    /// シェル設定クラス
    /// </summary>
    [Serializable, Option(ParentType = typeof(ShellSettings))]
    public class ThumbnailSettings : SettingBase
    {
        public static readonly ThumbnailSettings Instance = new();
        private static PiViLityCore.Resource.Manager _resource = new (Option.Resource.ResourceManager);
        public override string CategoryText { 
            get=>Option.Resource.ThumbnailSetting_Name;
        }

        public override string CategoryName
        {
            get => "ThumbnailSetting";
        }

        public override Manager SettingResource => _resource;

        public override ushort GroupUIOrder => 20;


        /// <summary>
        /// デフォルト値として記憶されるファイルリストの表示形式
        /// </summary>
        [OptionItem(NoOption = true)]
        public View FileListViewStyle = View.Tile;

        [OptionItemSize
            (
                TextResouceId = "ThumbnailSetting.ThumbnailSize",
                DescriptionTextResouceId = "",
                MinWidth = 64, MinHeight = 32, MaxWidth = 1024, MaxHeight = 1024
            )
        ]
        public Size ThumbnailSize = new(384, 273);

    }
}
