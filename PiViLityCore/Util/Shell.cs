using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Util
{
    internal static class Shell
    {
        /// <summary>
        /// ドライブの種類を文字列で返します。
        /// </summary>
        /// <param name="driveType"></param>
        /// <returns></returns>
        static public string DriveTypeName(DriveType driveType)
        {
            switch (driveType)
            {
                case DriveType.Unknown:
                    return "";
                case DriveType.NoRootDirectory:
                    return "";
                case DriveType.Removable:
                    return Global.GetResourceString("DriveTypeRemovable");
                case DriveType.Fixed:
                    return Global.GetResourceString("DriveTypeFixed");
                case DriveType.Network:
                    return Global.GetResourceString("Network");
                case DriveType.CDRom:
                    return "光学ドライブ";
                case DriveType.Ram:
                    return "Ramディスク";
            }
            return "";
        }
    }
}
