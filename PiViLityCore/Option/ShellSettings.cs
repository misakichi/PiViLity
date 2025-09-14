using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Option
{
    [Serializable, Option]
    public class ShellSettings : PiViLityCore.Plugin.SettingBase
    {
        public static readonly ShellSettings Instance = new();
        private static PiViLityCore.Resource.Manager _resource = new(Option.Resource.ResourceManager);
        public override string CategoryText
        {
            get => Option.Resource.ShellSetting_Name;
        }
        public override string CategoryName
        {
            get => "ShellSetting";
        }
        public override PiViLityCore.Resource.Manager SettingResource => _resource;
        public override ushort GroupUIOrder => 10;


        [OptionItem(TextResouceId = "ShellSetting.ShowHiddenFiles", DescriptionTextResouceId = "ShellSetting.ShowHiddenFiles.Desc")]
        public bool IsVisibleHidden = false;
        [OptionItem(TextResouceId = "ShellSetting.ShowSystemFiles", DescriptionTextResouceId = "ShellSetting.ShowSystemFiles.Desc")]
        public bool IsVisibleSystem = false;

        public bool IsVisibleEntry(FileSystemInfo fileInfo)
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
