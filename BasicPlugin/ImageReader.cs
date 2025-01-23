
using System.Drawing;

namespace BasicPlugin
{
    public class ImageReader : PiViLityCore.Plugin.IImageReader
    {
        static List<string> supportExtensions = new List<string>()
        {
            "bmp","jpg","png","tiff"
        };

        public List<string> GetSupportExtensions()
        {
            return supportExtensions;
        }

        public bool IsSupport(string filePath)
        {
            var fileExt = Path.GetExtension(filePath).ToLower();
            if (fileExt.Length > 0)
            {
                fileExt = fileExt.Substring(1);
            }

            foreach (string ext in supportExtensions)
            {
                if (fileExt == ext)
                    return true;
            }
            return false;
        }

        public Image? ReadImage(string filePath)
        {
            if (!IsSupport(filePath) || !File.Exists(filePath))
                return null;
            return Image.FromFile(filePath);
        }
    }
}
