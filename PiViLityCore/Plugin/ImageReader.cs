using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Plugin
{
    public interface IImageReader
    {
        public List<string> GetSupportExtensions();

        public bool IsSupport(string filePath);

        public System.Drawing.Image? ReadImage(string filePath);

    }
}
