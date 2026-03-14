using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiViLityPlugin.Difinition;
using PiViLityPlugin.Option;

namespace PiViLityCore.Option
{
    /// <summary>
    /// シェル設定クラス
    /// </summary>
    [Serializable, Option(ParentType = typeof(ShellSettings))]
    public class ThumbnailSettings : Setting<ThumbnailSettings>
    {
        public override void Dispose(){}

        public override string CategoryText { 
            get=>Option.Resource.ThumbnailSetting_Name;
        }

        public override string CategoryName
        {
            get => "ThumbnailSetting";
        }

        public override System.Resources.ResourceManager? SettingResource => Option.Resource.ResourceManager;

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
