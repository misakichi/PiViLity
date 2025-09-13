using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Option
{
    public class OptionAttribute : Attribute
    {
        public OptionAttribute()
        {
        }

        public bool NoOption = false;

        public Type? ParentType = null;
    }

    public class OptionItemAttribute : Attribute
    {
        static public bool MemberIsNoOption(MemberInfo member)
        {
            var attrs = member.GetCustomAttributes(typeof(OptionItemAttribute), true);
            bool noOption = false;
            foreach(var attr in attrs)
            {
                if (attr is OptionItemAttribute optAttr)
                {
                    noOption |= optAttr.NoOption;
                }
            }
            return noOption;
        }
        static public bool MemberIsSave(MemberInfo member)
        {
            var attrs = member.GetCustomAttributes(typeof(OptionItemAttribute), true);
            bool isSave = true;
            foreach(var attr in attrs)
            {
                if (attr is OptionItemAttribute optAttr)
                {
                    isSave &= (optAttr.NoSave==false);
                }
            }
            return isSave;
        }

        public OptionItemAttribute()
        {
        }

        /// <summary>
        /// 設定項目の名称を取得するためのリソースID
        /// </summary>
        public string TextResouceId = "";

        /// <summary>
        /// 項目の詳細を取得するためのリソースID
        /// </summary>
        public string DescriptionTextResouceId = "";

        public bool NoOption = false;

        public bool NoSave = false;

        public UInt16 UIOrder = 0;


    }


    public class OptionItemSizeAttribute : OptionItemAttribute
    {
        public OptionItemSizeAttribute() : base() { }
        public int MinWidth = 0;
        public int MinHeight = 0;
        public int MaxWidth = 9999;
        public int MaxHeight = 9999;
    }
}
