using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace PiViLityCore.Resource
{
    public class Manager
    {
        private static CultureInfo culture = CultureInfo.CurrentCulture;
        private ResourceManager resourceMgr;

        public Manager(string baseName, Assembly assembly)
        {
            resourceMgr = new ResourceManager(baseName, assembly);
        }
        public Manager(ResourceManager resourceManager)
        {
            resourceMgr = resourceManager;
        }

        public static void SetLanguage(string language)
        {
            culture = new CultureInfo(language);
        }
        public static void SetCulture(CultureInfo ci)
        {
            culture = ci;
        }

        public string GetString(string name)
        {
            return GetString(resourceMgr, name) ?? "";
        }
        public Icon? GetIcon(string name)
        {
            return GetIcon(resourceMgr, name);
        }
        public object? GetObject(string name)
        {
            return GetObject(resourceMgr, name);
        }
        public UnmanagedMemoryStream? GetStream(string name)
        {
            return GetStream(resourceMgr, name);
        }

        public static string GetString(ResourceManager rm, string name)
        {
            return rm.GetString(name, culture) ?? "";
        }
        public static Icon? GetIcon(ResourceManager rm, string name)
        {
            var data = rm.GetObject(name) as byte[];
            return data != null ? new Icon(new MemoryStream(data)) : null;
        }
        public static object? GetObject(ResourceManager rm, string name)
        {
            return rm.GetObject(name, culture);
        }
        public static UnmanagedMemoryStream? GetStream(ResourceManager rm, string name)
        {
            return rm.GetStream(name, culture);
        }


    }
}
