using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Plugin
{
    /// <summary>
    /// パネルそのままを設定画面として利用する場合の基底クラス
    /// </summary>
    public abstract class SettingPanelBase : SettingBase
    {
        public abstract Panel Panel { get; protected set; }
    }
}
