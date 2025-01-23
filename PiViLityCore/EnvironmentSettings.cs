using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore
{
    public class EnvironmentSettings
    {
        public EnvironmentSettings() 
        { 
        }

        public bool IsVisibleHidden = false;
        public bool IsVisibleSystem = false;

        private bool IsVisibleEntry(FileSystemInfo fileInfo)
        {
            if (fileInfo.Attributes.HasFlag(FileAttributes.Hidden) && !IsVisibleHidden)
                return false;
            if (fileInfo.Attributes.HasFlag(FileAttributes.System) && !IsVisibleSystem)
                return false;
            return true;
        }

        public bool IsVisibleFile(FileInfo fileInfo)
        {
            return IsVisibleEntry(fileInfo);
        }
        public bool IsVisibleDirectory(DirectoryInfo dirInfo)
        {
            return IsVisibleEntry(dirInfo);

        }
    }
}
