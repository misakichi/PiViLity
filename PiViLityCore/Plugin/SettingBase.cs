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
        /// 設定名称
        /// </summary>
        [Option(NoOption=true)]
        public virtual string Name { get => ""; }

        [Option(NoOption = true)]
        public virtual bool IsUserOptions { get => false; }

        [Option(NoOption = true)]
        public virtual bool GetHasSettingDialog() => false;

        [Option(NoOption = true)]
        public virtual Form? SettingDialog() => null;

    }
}
