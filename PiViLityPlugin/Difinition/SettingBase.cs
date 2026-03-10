using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityPlugin.Difinition
{

    /// <summary>
    /// 設定クラスの基底クラス
    /// </summary>
    public abstract class SettingBase
    {
        protected SettingBase() 
        {
        }

        /// <summary>
        /// 設定項目の名称（UI上での表示名）
        /// </summary>
        public abstract string CategoryText { get; }

        /// <summary>
        /// 設定項目の名称（管理用）
        /// </summary>
        public abstract string CategoryName { get; }

        /// <summary>
        /// 非nullの場合、そのコントロールが設定の責任を負う。
        /// 自動UI生成を行わずにコントロールがfillでが表示される。
        /// </summary>
        /// <returns>パネル</returns>
        public virtual Control? SettingControl() => null;

        /// <summary>
        /// カテゴリ表示順ソート
        /// また、同一カテゴリ内でのクラス単位ソートに用いる
        /// </summary>
        public abstract UInt16 GroupUIOrder { get; }

        /// <summary>
        /// OptionItem属性がテキストリソース参照を行う際はこのリソースを参考する。
        /// </summary>
        public abstract ResourceManager? SettingResource { get; }

    }
}
