using PiViLityCore.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicPlugin
{
    class Plugin : IModuleInformation
    {
        public string Name => "Basic Plugin";
        public string Description => "Basic Plugin for PiViLity";
        public string OptionItemName => "Basic Plugin";
        public System.Resources.ResourceManager? ResourceManager => null;
    }
}
