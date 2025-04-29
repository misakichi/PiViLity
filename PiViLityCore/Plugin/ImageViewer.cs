using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Plugin
{
    public enum ViewModeStyle
    {
        Fixed,
        AutoScale,
    }
    /// <summary>
    /// 画像ビューワーのインターフェースです。
    /// </summary>
    public interface ImageViewer
    {
        /// <summary>
        /// 画像をロード（表示）します
        /// </summary>
        /// <param name="filePath"></param>
        bool LoadImage(string filePath);

        /// <summary>
        /// ビューアーのコントロールを取得します。
        /// </summary>
        /// <returns></returns>
        Control GetViewer();

        ViewModeStyle ViewMode { get; set; }
    }
}
