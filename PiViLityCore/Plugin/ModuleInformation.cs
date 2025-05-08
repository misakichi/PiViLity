using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Plugin
{
    public interface IModuleInformation
    {
        public string Name { get; }
        public string Description { get; }
        public string OptionItemName { get; }

        public System.Resources.ResourceManager? ResourceManager { get; }
    }

    public class ModuleInformation : IModuleInformation
    {
        public string Name { get; set; } = "Core";
        public string Description { get; set; } = "Core Module";
        public string OptionItemName { get; set; } = "";
        public System.Resources.ResourceManager? ResourceManager { get; set; } = null;
        public ModuleInformation()
        {
        }
    }
}