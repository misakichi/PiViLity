using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Plugin
{
	public abstract class ImageReaderBase : IImageReader, IDisposable
    {
		public virtual ThumbnailQualities ThumbnailQuality { get; set; } = ThumbnailQualities.UseThumbnail;
		public virtual ThumbnailTypes ThumbnailType { get; set; } = ThumbnailTypes.Centering;

        public abstract void Dispose();
        public abstract Image? GetImage() ;
        public virtual Image? GetPreviewImage()=> GetImage();

        public abstract Size GetImageSize();
        public abstract List<string> GetSupportedExtensions();
        public abstract Image? GetThumbnailImage(Size size);
        public abstract bool IsSupported();
        public abstract bool SetFilePath(string filePath);

        public System.Drawing.Rectangle GetThumbnailDrawRect(Size imageSize, Size thumbnailSize)
        {
            if (ThumbnailType == ThumbnailTypes.KeepAspectRatio)
            {
                float aspectRatio = (float)(imageSize.Width) / imageSize.Height;
                int newWidth = thumbnailSize.Width;
                int newHeight = (int)(thumbnailSize.Width / aspectRatio);

                if (newHeight > thumbnailSize.Height)
                {
                    newHeight = thumbnailSize.Height;
                    newWidth = (int)(thumbnailSize.Height * aspectRatio);
                }
                int x = (thumbnailSize.Width - newWidth) / 2;
                int y = (thumbnailSize.Height - newHeight) / 2;
                return new(x, y, newWidth, newHeight);
            }
            else if (ThumbnailType == ThumbnailTypes.Centering)
            {
                var dstRatio = thumbnailSize.Height / (float)(thumbnailSize.Width);
                var srcRatio = imageSize.Height / (float)(imageSize.Width);
                int newWidth = thumbnailSize.Width;
                int newHeight = thumbnailSize.Height;
                if (dstRatio > srcRatio)
                {
                    newWidth = (int)(thumbnailSize.Height / srcRatio);
                }
                else
                {
                    newHeight = (int)(thumbnailSize.Width * srcRatio);
                }

                int x = (thumbnailSize.Width - newWidth) / 2;
                int y = (thumbnailSize.Height - newHeight) / 2;
                return new(x, y, newWidth, newHeight);
            }
            return new(new Point(0,0), thumbnailSize);
        }
    }
}
