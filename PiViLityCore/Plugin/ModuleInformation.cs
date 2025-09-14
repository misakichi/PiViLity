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

        public bool Initialize();
        public bool Terminate();

    }

}