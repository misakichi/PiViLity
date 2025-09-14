using PiViLityCore.Plugin;
using PiViLityCore.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Media.PlayTo;

namespace PiViLityCore
{
    static public class Global
    {
        internal static Resource.Manager Resource = new(Resources.Resource.ResourceManager);
        private static SynchronizationContext? _syncContext;

        static Global()
        {
            _syncContext = SynchronizationContext.Current;
        }

        public static void InvokeMainThread(Action action)
        {
            _syncContext?.Post((o) => action(), null);
        }

    }


    public class ModuleInformation : IModuleInformation
    {
        public string Name { get; set; } = "Core";
        public string Description { get; set; } = "Core Module";
        public string OptionItemName { get; set; } = "";
        public ModuleInformation()
        {
        }

        public bool Initialize() => true;

        public bool Terminate() => true;
    }
}
