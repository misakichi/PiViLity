using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Util
{
    static public class String
    {
        public static string GetEasyReadFileSize(long size, bool dontUseByte = true)
        {
            if(size<1024 && !dontUseByte)
                return $"{size} B";
            else if (size < 1024 * 1024)
                return $"{size/1024} KB";
            else if (size < 1024 * 1024 * 1024)
                return $"{size/1024/1024} MB";
            else if (size < 1024L * 1024 * 1024 * 1024)
                return $"{size / 1024 / 1024 / 1024} GB";
            else 
                return $"{size / 1024 / 1024 / 1024 / 1024} TB";
        }
    }
}
