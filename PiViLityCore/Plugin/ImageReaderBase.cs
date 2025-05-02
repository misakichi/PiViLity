using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Plugin
{
	public abstract class ImageReaderBase : IImageReader
	{
		public virtual ThumbnailQualities ThumbnailQuality { get; set; } = ThumbnailQualities.UseThumbnail;
		public virtual ThumbnailTypes ThumbnailType { get; set; } = ThumbnailTypes.Centering;

        public abstract void Dispose();
        public abstract Image? GetImage() ;
        public abstract Size GetImageSize();
        public abstract List<string> GetSupportedExtensions();
        public abstract Image? GetThumbnailImage(Size size);
        public abstract bool IsSupported();
        public abstract bool SetFilePath(string filePath);
    }
}
