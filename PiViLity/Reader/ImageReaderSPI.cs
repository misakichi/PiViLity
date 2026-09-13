using ABI.System;
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
            return CallSpiThreadProcClass(() => _plugin?.GetImage());
        }

        public override Size GetImageSize()
        {
            return CallSpiThreadProc(() =>
            {
                SPIPictureInfo? info = _plugin?.GetFileInfo();
                if (info != null)
                {
                    return new Size(info.width, info.height);
                }
                return new Size();
            });
        }

        public override IEnumerable<string> GetSupportedExtensions()
        {
            return CallSpiThreadProcClass(()=>SusiePluginManager.Instance.Extensions.ToList()) ?? [];
        }

        public override Image? GetThumbnailImage(Size size)
        {
            return CallSpiThreadProcClass(() =>
            {
                var img = _plugin?.GetPreviewImage();
                if (img == null)
                    img = GetImage();
                if (img == null)
                    return (Image?)null;

                var thumbnailDrawRect = GetThumbnailDrawRect(img.Size, size);
                var thumb = new Bitmap(size.Width, size.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (var g = Graphics.FromImage(thumb))
                {
                    g.DrawImage(img, thumbnailDrawRect);
                }
                return thumb;
            });
        }

        public override bool IsSupported()
        {
            return CallSpiThreadProc(()=> _plugin?.IsSupport()) ?? false;
        }

        public override bool SetFilePath(string filePath)
        {            
            return CallSpiThreadProc(
                () =>
                {
                    _plugin = SusiePluginManager.Instance.GetPluginInstanceForFile(filePath);
                    if (_plugin != null)
                    {
                        _path = filePath;
                        return true;
                    }
                    return false;
                }
                );;
        }

        private T CallSpiThreadProc<T>(Func<T> func) where T : struct
        {
#if true
            return func();
#else
            T ret = default;
            using var job = SusiePluginManager.Instance.AddJobSync(() => ret = func());
            job.Wait();
            return ret;
#endif
        }
        private T? CallSpiThreadProc<T>(Func<T?> func) where T : struct
        {
#if true
            return func();
#else
          return func();
            T? ret = null;
            using var job = SusiePluginManager.Instance.AddJobSync(() => ret = func());
            job.Wait();
            return ret;
#endif
        }

        private T? CallSpiThreadProcClass<T>(Func<T?> func) where T : class
        {
#if true
            return func();
#else
        return func();
            T? ret = null;
            using var job = SusiePluginManager.Instance.AddJobSync(() => ret = func());
            job.Wait();
            return ret;
#endif
        }

        private SusiePluginInstance? _plugin;
        private string _path = "";
    }
#endif
        }
