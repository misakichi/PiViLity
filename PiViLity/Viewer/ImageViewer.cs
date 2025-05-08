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
        Image? viewImage = new Bitmap(1, 1);

        public ImageViewer()
        {
            InitializeComponent();
            picImage.Dock = DockStyle.Fill;
            picImage.Paint += PnlImage_Paint;
        }

        private void PnlImage_Paint(object? sender, PaintEventArgs e)
        {

            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;

            if (viewImage != null)
            {
                e.Graphics.DrawImage(viewImage, new Rectangle(0, 0, picImage.Width, picImage.Height), new Rectangle(0, 0, viewImage.Width, viewImage.Height), GraphicsUnit.Pixel);
            }
        }

        public bool LoadImage(string filePath)
        {
            Image? image = null;
            using (var imageReader = PluginManager.Instance.GetImageReader(filePath))
            {
                if (imageReader?.SetFilePath(filePath) ?? false)
                {
                    image = imageReader.GetImage();
                }
            }
            SetImage(image);
            return image != null;
        }

        private void SetImage(Image? image)
        {
            viewImage = image;
            picImage.Invalidate();
            //if(ViewMode== ViewModeStyle.AutoScale)
            //{
            //    if (image != null)
            //    {
            //        pnlImage.Dock= DockStyle.Fill;
            //        pnlImage.BackgroundImageLayout = ImageLayout.Zoom;
            //    }
            //}
            //else
            //{
            //    pnlImage.Dock = DockStyle.None;
            //    pnlImage.BackgroundImageLayout = ImageLayout.None;
            //}
        }
        public Control GetViewer()
        {
            // Return the control for displaying the image
            return this;
        }

        private void picImage_SizeChanged(object sender, EventArgs e)
        {
            picImage.Invalidate();
        }

        [DefaultValue(ViewModeStyle.AutoScale)]
        public ViewModeStyle ViewMode { get; set; } = ViewModeStyle.AutoScale;

    }
}
