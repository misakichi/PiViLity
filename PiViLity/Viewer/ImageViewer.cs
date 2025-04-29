using PiViLityCore.Plugin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PiViLity.Viewer
{
    public partial class ImageViewer : UserControl, PiViLityCore.Plugin.ImageViewer
    {
        public ImageViewer()
        {
            InitializeComponent();
        }

        public bool LoadImage(string filePath)
        {
            Image? image = null;
            var imageReader = PluginManager.Instance.GetImageReader(filePath);
            if (imageReader?.SetFilePath(filePath) ?? false)
            {
                image = imageReader.GetImage();
            }
            SetImage(image);
            return image != null;
        }

        private void SetImage(Image? image)
        {
            pnlImage.BackgroundImage = image;
            if(ViewMode== ViewModeStyle.AutoScale)
            {
                if (image != null)
                {
                    pnlImage.Dock= DockStyle.Fill;
                    pnlImage.BackgroundImageLayout = ImageLayout.Zoom;
                }
            }
            else
            {
                pnlImage.Dock = DockStyle.None;
                pnlImage.BackgroundImageLayout = ImageLayout.None;
            }
        }
        public Control GetViewer()
        {
            // Return the control for displaying the image
            return this;
        }

        [DefaultValue(ViewModeStyle.AutoScale)]
        public ViewModeStyle ViewMode { get; set; } = ViewModeStyle.AutoScale;

    }
}
