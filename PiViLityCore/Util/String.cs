using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PiViLityCore.Util
{
    static public class String
    {
        /// <summary>
        /// ファイルサイズを読みやすい形式で返します。
        /// </summary>
        /// <param name="size"></param>
        /// <param name="dontUseByte"></param>
        /// <returns></returns>
        public static string GetEasyReadFileSize(long size, bool dontUseByte = true)
        {
            if(size<1024 && !dontUseByte)
                return $"{size} {Global.Resource.GetString($"BytesStr")}";
            else if (size < 1024 * 1024)
                return $"{size/1024} KB";
            else if (size < 1024 * 1024 * 1024)
                return $"{size/1024/1024} MB";
            else if (size < 1024L * 1024 * 1024 * 1024)
                return $"{size / 1024 / 1024 / 1024} GB";
            else 
                return $"{size / 1024 / 1024 / 1024 / 1024} TB";
        }
        public static string GetEasyReadFileSizeF(long size, bool dontUseByte = true)
        {
            if (size<256 && !dontUseByte)
                return $"{size} {Global.Resource.GetString($"BytesStr")}";
            if (size < 1024 * 1024)
                return $"{size/1024.0f:N2} KB";
            else if (size < 1024.0f * 1024 * 1024)
                return $"{size/ 1024.0f / 1024:N2} MB";
            else if (size < 1024L * 1024 * 1024 * 1024)
                return $"{size / 1024.0f / 1024 / 1024:N2} GB";
            else 
                return $"{size / 1024.0f / 1024 / 1024 / 1024:N2} TB";
        }
    }
}
