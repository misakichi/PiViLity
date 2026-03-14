using PiViLityPlugin.Difinition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiViLityPlugin.Option;

namespace PiViLityCore.Option
{
    [Serializable, Option]
    public class ShellSettings : Setting<ShellSettings>
    {
        public override void Dispose() { }

        public override string CategoryText
        {
            get => Option.Resource.ShellSetting_Name;
        }
        public override string CategoryName
        {
            get => "ShellSetting";
        }

        public override System.Resources.ResourceManager? SettingResource => Option.Resource.ResourceManager;
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
