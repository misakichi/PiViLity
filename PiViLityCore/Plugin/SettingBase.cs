using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Plugin
{
    public abstract class SettingBase
    {
        protected SettingBase() 
        {
        }

        /// <summary>
        /// 設定名称
        /// </summary>
        public abstract string Name { get; set; }

        public virtual bool GetHasSettingDialog() => false;

        public virtual Form? SettingDialog() => null;

    }
}
