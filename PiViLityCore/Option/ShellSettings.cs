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
    }
}
