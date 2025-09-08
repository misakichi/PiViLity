using PiViLity.Viewer;
using PiViLityCore.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;
using static System.Net.Mime.MediaTypeNames;

namespace PiViLity.Dock
{
    internal class FilePreviewContent : DockContent
    {
        public FilePreviewContent()
        {
            Text = "Preview";
            DockAreas = DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockTop | DockAreas.DockBottom | DockAreas.Float;
        }
        protected override string GetPersistString()
        {
            return "FilePreviewContent";
        }

        IPreViewer? _currentViewer = null;

        public void SetFile(string filePath)
        {
            if(filePath=="" || File.Exists(filePath) == false)
            {
                Controls.Clear();
                return;
            }

            if (PluginManager.Instance.GetImageReader(filePath) is IImageReader reader)
            {
                if (reader.SetFilePath(filePath))
                {
                    if (!(_currentViewer is ImagePreViewer preview))
                    { 
                        preview = new ImagePreViewer();
                    }
                    preview.LoadFile(filePath);
                    preview.Dock = System.Windows.Forms.DockStyle.Fill;
                    _currentViewer = preview;
                    Controls.Clear();
                    Controls.Add(preview);
                }
                else
                {
                    Controls.Clear();
                }
            }
            else
            {
                Controls.Clear();
            }
        }
    }

}
