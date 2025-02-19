using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IWshRuntimeLibrary;
using PiVilityNative;

namespace PiViLityCore.Util
{
    public static class Shell
    {
        public static bool IsNetworkDriveAvailable(string path)
        {
            try
            {
                var driveLetter = Path.GetPathRoot(path)?.TrimEnd('\\');
                if (string.IsNullOrEmpty(driveLetter))
                    return false;

                var query = $"SELECT * FROM Win32_NetworkConnection WHERE LocalName = '{driveLetter}'";
                using (var searcher = new System.Management.ManagementObjectSearcher(query))
                using (var results = searcher.Get())
                {
                    foreach (var result in results)
                    {
                        var status = result["Status"]?.ToString();
                        if (status == "OK")
                            return true;
                    }
                }
            }
            catch
            {
                // エラーが発生した場合はネットワークドライブが利用不可とみなす
                return false;
            }

            return false;
        }
        public static void Copy(string srcPath, string path)
        {
            ShellAPI.FileOperationCopy([srcPath], path);
        }
        public static void Copy(IEnumerable<string> srcPath, string path)
        {
            ShellAPI.FileOperationCopy(srcPath, path);
        }
        public static void Move(string srcPath, string path)
        {
            ShellAPI.FileOperationMove([srcPath], path);
        }
        public static void Move(IEnumerable<string> srcPath, string path)
        {
            ShellAPI.FileOperationMove(srcPath, path);
        }
        public static void Delete(string path)
        {
            ShellAPI.FileOperationDelete([path]);
        }
        public static void Delete(IEnumerable<string> paths)
        {
            ShellAPI.FileOperationDelete(paths);
        }
        public static void CreateShortCut(string srcPath, string path, string description)
        {
            ShellAPI.CreateShortCut(srcPath, path, description);
        }

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

        static public bool IsDirectory(string path)
        {
            return System.IO.Directory.Exists(path);
        }
        static public bool IsExecute(string path)
        {
            if (System.IO.File.Exists(path))
            {
                var extension = Path.GetExtension(path).ToLower();
                if (extension == ".exe" || extension == ".com" || extension == ".bat")
                {
                    return true;
                }
            }
            return false;
        }
        static public bool IsShortCut(string path)
        {
            if (System.IO.File.Exists(path))
            {
                var extension = Path.GetExtension(path).ToLower();
                return extension == ".lnk";
            }
            return false;
        }

        static string? GetShortcutTarget(string path)
        {
            if (IsShortCut(path))
            {
                IWshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(path);
                return shortcut?.TargetPath;
            }
            return null;
        }
    }
}
