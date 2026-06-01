using PiViLity.COM;
using PiViLity.Option;
using PiVilityNative;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLity.Reader
{
#if true
    internal class ImageReaderSPI : PiViLityPlugin.Difinition.ImageReaderBase
    {
        public override void Dispose()
        {
        }

        public override Image? GetImage()
        {
            return _plugin?.GetImage();
        }

        public override Size GetImageSize()
        {
            SPIPictureInfo? info = _plugin?.GetFileInfo();
            if (info  != null)
            {
                return new Size(info.width, info.height);
            }
            return new Size();
        }

        public override List<string> GetSupportedExtensions()
        {
            return SusiePluginManager.Instance.Extensions.ToList();
        }

        public override Image? GetThumbnailImage(Size size)
        {
            var img = _plugin?.GetPreviewImage();
            if (img == null)
                return null;
            var thumbnailDrawRect = GetThumbnailDrawRect(img.Size, size);
            var thumb = new Bitmap(size.Width, size.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(thumb))
            {
                g.DrawImage(img, thumbnailDrawRect);
            }
            return thumb;
        }

        public override bool IsSupported()
        {
            return _plugin?.IsSupport() ?? false;
        }

        public override bool SetFilePath(string filePath)
        {
            _plugin = SusiePluginManager.Instance.GetPluginInstanceForFile(filePath);
            if (_plugin != null)
            {
                _path = filePath;
                return true;
            }
            return false;
        }

        private SusiePluginInstance? _plugin;
        private string _path = "";
    }
#endif
}
