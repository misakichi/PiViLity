using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Shell
{
    public interface IFileSystemItem
    {
        bool IsSpecialFolder { get; }
        Environment.SpecialFolder SpecialFolder { get; }
        string Path { get; set; }

        bool HasPath { get; }

        FileSystemInfo? GetFileSystemInfo();
    }
}
