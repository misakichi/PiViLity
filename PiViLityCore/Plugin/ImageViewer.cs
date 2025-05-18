using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// 画像ビューワーの基底クラスです
    /// </summary>
    public interface IImageViewer: IViewer
    {

        public ViewModeStyle ViewMode { get; set; }

    }
}
