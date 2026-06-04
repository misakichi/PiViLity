using PiVilityNative;
using System;
using System.Collections.Generic;
using System.Text;

namespace PiViLity.COM
{
    internal class SusiePluginInstance : IDisposable
    {
        private PiVilityNative.SusiePluginCom _com;
        private string Path { get; set; }

        public SusiePluginInstance(PiVilityNative.SusiePluginCom plugin, string path)
        {
            _com = plugin;
            Path = path;
        }

        public void Dispose()
        {
            _com.Dispose();
        }

        public SPIPictureInfo? GetFileInfo()
        {
            if (File.Exists(Path) == false)
                return null;
            SPIPictureInfo? info = null;
            if (_com.GetPictureInfoFile(Path, ref info))
            {
                return info;
            }
            return null;
        }

        public bool IsSupport()
        {
            if (File.Exists(Path) == false)
                return false;

            var length = new System.IO.FileInfo(Path).Length;
            byte[] buffer = new byte[Math.Min(512,length)];
            using var reader = new FileStream(Path, FileMode.Open, FileAccess.Read);
            Task t = reader.ReadExactlyAsync(buffer).AsTask();
            t.Wait();
            if (_com.IsSupportedBuffer(Path, buffer))
            {
                return true;
            }

            return false;
        }

        public Image? GetImage()
        {
            if (File.Exists(Path) == false)
                return null;

            var length = new System.IO.FileInfo(Path).Length;

            try
            {
                return _com.GetPictureFileToBmp(Path);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SusiePluginInstance.GetImage Error {ex}");
                return null;
            }
        }
        public Image? GetPreviewImage()
        {
            if (File.Exists(Path) == false)
                return null;

            var length = new System.IO.FileInfo(Path).Length;

            try
            {
                return _com.GetPreviewFileToBmp(Path);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SusiePluginInstance.GetPreviewImage Error {ex}");
                return null;
            }
        }
    }
}
