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
    public interface ISetting
    {
        event EventHandler Changed;
        void RaiseChanged(EventArgs e);

        string CategoryText { get; }

        /// <summary>
        /// 設定項目の名称（管理用）
        /// </summary>
        string CategoryName { get; }

        /// <summary>
        /// 非nullの場合、そのコントロールが設定の責任を負う。
        /// 自動UI生成を行わずにコントロールがfillでが表示される。
        /// </summary>
        /// <returns>パネル</returns>
        Control? SettingControl();

        /// <summary>
        /// カテゴリ表示順ソート
        /// また、同一カテゴリ内でのクラス単位ソートに用いる
        /// </summary>
        UInt16 GroupUIOrder { get; }

        /// <summary>
        /// OptionItem属性がテキストリソース参照を行う際はこのリソースを参考する。
        /// </summary>
        ResourceManager? SettingResource { get; }
    }

    /// <summary>
    /// 設定クラスの基底クラス
    /// </summary>
    public abstract class Setting<T> : Singleton<T>, ISetting where T : class, IDisposable, new()
    {
        /// <summary>
        /// 値が変わったとき（確定時）に呼ばれるイベント
        /// </summary>
        public event EventHandler Changed = delegate { };

        public void RaiseChanged(EventArgs e) =>Changed?.Invoke(this, e);

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
