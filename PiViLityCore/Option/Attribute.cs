using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Option
{
    public class OptionAttribute : Attribute
    {
        public OptionAttribute()
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
    public class OptionItemAttribute : Attribute
    {
        public OptionItemAttribute()
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

        public int UIOrder = 0;


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
