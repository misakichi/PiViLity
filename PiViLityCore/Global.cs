using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PiViLityCore
{
    static public class Global
    {
        static ResourceManager resourceMgr = new ("PiViLityCore.Resources.Resource", Assembly.GetExecutingAssembly());
        static CultureInfo culture = new CultureInfo(CultureInfo.CurrentCulture.Name);
        static Global()
        {

        }

        public static void SetResourceLanguage(string language)
        {
            culture = new CultureInfo(language);            
        }

        public static string GetResourceString(ResourceManager rm, string name)
        {
            return rm.GetString(name,culture)??"";
        }
        public static Icon? GetResourceIcon(ResourceManager rm, string name)
        {
            var data = rm.GetObject(name) as byte[];
            return data != null ? new Icon(new MemoryStream(data)) : null;
        }
        public static object? GetResourceObject(ResourceManager rm, string name)
        {
            return rm.GetObject(name, culture);
        }
        public static UnmanagedMemoryStream? GetResourceStream(ResourceManager rm, string name)
        {
            return rm.GetStream(name, culture);
        }
        public static string GetResourceString(string name)
        {
            return GetResourceString(resourceMgr,name);
        }

        public static EnvironmentSettings settings = new();
    }
}
