using PiViLityCore.Option;
using PiViLityPlugin.Difinition;
using PiViLityPlugin.Option;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace PiViLity.Option
{
    [Serializable, Option]
    public class SusiePluginSettings : SettingBase
    {
        static public readonly SusiePluginSettings Instance = new();
        public override string CategoryText => "Susie Plugin";

        public override string CategoryName => "Susie Plugin";

        public override ushort GroupUIOrder => 100;

        public override Control? SettingControl() => new Forms.SusiePluginSetting();

        public override ResourceManager? SettingResource => null;

        
    }
}
