using Microsoft.VisualBasic.FileIO;
using PiViLityCore.Option;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Plugin
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

        public virtual bool IsUserOptions { get => false; }

        public virtual bool GetHasSettingDialog() => false;

        public virtual Form? SettingDialog() => null;

        /// <summary>
        /// カテゴリ表示順ソート
        /// また、同一カテゴリ内でのクラス単位ソートに用いる
        /// </summary>
        public abstract UInt16 GroupUIOrder { get; }

        public abstract Resource.Manager SettingResource { get; }

    }
}
