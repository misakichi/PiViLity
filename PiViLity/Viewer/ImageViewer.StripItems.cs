using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLity.Viewer
{
    partial class ImageViewer
    {
        void InitializeToolStripItems()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageViewer));

            // 
            // lblResolution
            // 
            tlblResolutionStatus.AutoSize = false;
            tlblResolutionStatus.BorderSides = ToolStripStatusLabelBorderSides.Right;
            tlblResolutionStatus.BorderStyle = Border3DStyle.Etched;
            tlblResolutionStatus.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tlblResolutionStatus.Name = "lblResolution";
            tlblResolutionStatus.Size = new Size(120, 17);
            tlblResolutionStatus.Text = "999999 x 999999";
            // 
            // lblScale
            // 
            tlblScaleStatus.AutoSize = false;
            tlblScaleStatus.BorderSides = ToolStripStatusLabelBorderSides.Right;
            tlblScaleStatus.BorderStyle = Border3DStyle.Etched;
            tlblScaleStatus.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tlblScaleStatus.Name = "lblScale";
            tlblScaleStatus.Size = new Size(80, 17);
            tlblScaleStatus.Text = "100%";
            // 
            // tlblSpacer
            // 
            tlblSpacer.AutoSize = false;
            tlblSpacer.BorderSides = ToolStripStatusLabelBorderSides.None;
            tlblSpacer.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tlblSpacer.Spring = true;

            tlblPixelColor.AutoSize = true;
            tlblPixelColor.Alignment = ToolStripItemAlignment.Right;
            tlblPixelColor.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tlblPixelColor.Text = "■";
            tlblPixelColor.ForeColor = Color.Black;

            // 
            // tlblPixelInfo
            // 
            tlblPixelInfo.AutoSize = true;
            tlblPixelInfo.Alignment = ToolStripItemAlignment.Right;
            tlblPixelInfo.BorderSides = ToolStripStatusLabelBorderSides.Right;
            tlblPixelInfo.BorderStyle = Border3DStyle.Etched;
            tlblPixelInfo.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tlblPixelInfo.Name = "tlblPixelInfo";
            tlblPixelInfo.Size = new Size(500, 17);
            tlblPixelInfo.Text = "";



            // 
            // tbtnFitSize
            // 
            tbtnFitSize.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tbtnFitSize.Image = (Image?)resources.GetObject("tbtnFitSize.Image");
            tbtnFitSize.ImageTransparentColor = Color.Magenta;
            tbtnFitSize.Name = "tbtnFitSize";
            tbtnFitSize.Size = new Size(24, 22);
            tbtnFitSize.Text = "Fit";
            tbtnFitSize.ToolTipText = "Fit to window size.";
            tbtnFitSize.Click += tbtnFitSize_Click;
            // 
            // tbtnZoomOut
            // 
            tbtnZoomOut.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tbtnZoomOut.Image = resources.GetObject("tbtnZoomOut.Image") as Image;
            tbtnZoomOut.ImageTransparentColor = Color.Magenta;
            tbtnZoomOut.Name = "tbtnZoomOut";
            tbtnZoomOut.Size = new Size(23, 22);
            tbtnZoomOut.Text = "-";
            tbtnZoomOut.Click += tbtnZoomOut_Click;
            // 
            // tbtnZoom100
            // 
            tbtnZoom100.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tbtnZoom100.Image = resources.GetObject("tbtnZoom100.Image") as Image;
            tbtnZoom100.ImageTransparentColor = Color.Magenta;
            tbtnZoom100.Name = "tbtnZoom100";
            tbtnZoom100.Size = new Size(39, 22);
            tbtnZoom100.Text = "100%";
            tbtnZoom100.Click += tbtnZoom100_Click;
            // 
            // tbtnZoomIn
            // 
            tbtnZoomIn.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tbtnZoomIn.Image = resources.GetObject("tbtnZoomIn.Image") as Image;
            tbtnZoomIn.ImageTransparentColor = Color.Magenta;
            tbtnZoomIn.Name = "tbtnZoomIn";
            tbtnZoomIn.Size = new Size(23, 22);
            tbtnZoomIn.Text = "+";
            tbtnZoomIn.ToolTipText = "Zoom in";
            tbtnZoomIn.Click += tbtnZoomIn_Click;
            //
            // tbtnCopy
            //
            tbtnCopy.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tbtnCopy.Image = resources.GetObject("tbtnCopy.Image") as Image;
            tbtnCopy.ImageTransparentColor = Color.Magenta;
            tbtnCopy.Name = "tbtnCopy";
            tbtnCopy.Size = new Size(23, 22);
            tbtnCopy.Text = "Copy";
            tbtnCopy.ToolTipText = "Copy to clipboard";
            tbtnCopy.Click += tbtnCopy_Click;
            tbtnCopy.Enabled = false;



        }
        public IEnumerable<ToolStripItem> ToolBarItems => [tbtnFitSize, tbtnZoomOut, tbtnZoom100, tbtnZoomIn, separaterZoom, tbtnCopy];

        public IEnumerable<ToolStripItem> StatusBarItems => [tlblResolutionStatus, tlblScaleStatus, tlblSpacer, tlblPixelColor, tlblPixelInfo];


        private ToolStripButton tbtnFitSize = new();
        private ToolStripButton tbtnZoomOut = new();
        private ToolStripButton tbtnZoom100 = new();
        private ToolStripButton tbtnZoomIn = new();
        private ToolStripSeparator separaterZoom = new();
        private ToolStripButton tbtnCopy = new();

        private ToolStripStatusLabel tlblResolutionStatus = new();
        private ToolStripStatusLabel tlblScaleStatus = new();
        private ToolStripStatusLabel tlblSpacer = new();
        private ToolStripStatusLabel tlblPixelColor = new();
        private ToolStripStatusLabel tlblPixelInfo = new();

    }
}
