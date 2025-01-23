using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Util
{
    internal static class Shell
    {
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
