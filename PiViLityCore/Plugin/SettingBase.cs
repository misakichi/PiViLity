using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Plugin
{
    public class SettingAttribute : Attribute
    {
        public SettingAttribute()
        {
        }

        /// <summary>
        /// 設定項目の名称を取得するためのリソースID
        /// </summary>
        public string NameTextResouceId = "";

        /// <summary>
        /// 項目の詳細を取得するためのリソースID
        /// </summary>
        public string DescriptionTextResouceId = "";

        public bool NoOption = false;
    }


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
        [Setting(NoOption=true)]
        public virtual string Name { get => ""; }

        [Setting(NoOption = true)]
        public virtual bool IsUserOptions { get => false; }

        [Setting(NoOption = true)]
        public virtual bool GetHasSettingDialog() => false;

        [Setting(NoOption = true)]
        public virtual Form? SettingDialog() => null;

    }
}
