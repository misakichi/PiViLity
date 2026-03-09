using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Plugin
{
    public enum PropertyGroup
    {
        File,
        Image,
        Others = 999
    }
    public class Property
    {
        public PropertyGroup Group = PropertyGroup.File;
        public string Name = string.Empty;
        public string Value = string.Empty;
    }
    public interface IPropertyReader : IReader
    {
        public List<Property> ReadProperties();

    }
}
