namespace PiViLity.Viewer
{
    partial class ImageViewer
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageViewer));
            toolStrip1 = new ToolStrip();
            tbtnFitSize = new ToolStripButton();
            tbtnZoomOut = new ToolStripButton();
            tbtnZoom100 = new ToolStripButton();
            tbtnZoomIn = new ToolStripButton();
            pnlContainer = new Panel();
            picImage = new PiViLityCore.Controls.PictureBox();
            statusStrip1 = new StatusStrip();
            ResolutionStatus = new ToolStripStatusLabel();
            ScaleStatus = new ToolStripStatusLabel();
            toolStrip1.SuspendLayout();
            pnlContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picImage).BeginInit();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { tbtnFitSize, tbtnZoomOut, tbtnZoom100, tbtnZoomIn });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(453, 25);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
            // 
            // tbtnFitSize
            // 
            tbtnFitSize.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tbtnFitSize.Image = (Image)resources.GetObject("tbtnFitSize.Image");
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
            tbtnZoomOut.Image = (Image)resources.GetObject("tbtnZoomOut.Image");
            tbtnZoomOut.ImageTransparentColor = Color.Magenta;
            tbtnZoomOut.Name = "tbtnZoomOut";
            tbtnZoomOut.Size = new Size(23, 22);
            tbtnZoomOut.Text = "-";
            tbtnZoomOut.Click += tbtnZoomOut_Click;
            // 
            // tbtnZoom100
            // 
            tbtnZoom100.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tbtnZoom100.Image = (Image)resources.GetObject("tbtnZoom100.Image");
            tbtnZoom100.ImageTransparentColor = Color.Magenta;
            tbtnZoom100.Name = "tbtnZoom100";
            tbtnZoom100.Size = new Size(39, 22);
            tbtnZoom100.Text = "100%";
            tbtnZoom100.Click += tbtnZoom100_Click;
            // 
            // tbtnZoomIn
            // 
            tbtnZoomIn.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tbtnZoomIn.Image = (Image)resources.GetObject("tbtnZoomIn.Image");
            tbtnZoomIn.ImageTransparentColor = Color.Magenta;
            tbtnZoomIn.Name = "tbtnZoomIn";
            tbtnZoomIn.Size = new Size(23, 22);
            tbtnZoomIn.Text = "+";
            tbtnZoomIn.ToolTipText = "Zoom in";
            tbtnZoomIn.Click += tbtnZoomIn_Click;
            // 
            // pnlContainer
            // 
            pnlContainer.AutoScroll = true;
            pnlContainer.Controls.Add(picImage);
            pnlContainer.Dock = DockStyle.Fill;
            pnlContainer.Location = new Point(0, 25);
            pnlContainer.Name = "pnlContainer";
            pnlContainer.Size = new Size(453, 286);
            pnlContainer.TabIndex = 1;
            // 
            // picImage
            // 
            picImage.Location = new Point(208, 143);
            picImage.Name = "picImage";
            picImage.Size = new Size(100, 50);
            picImage.TabIndex = 1;
            picImage.TabStop = false;
            picImage.MouseHWheel += picImage_MouseHWheel;
            picImage.SizeChanged += picImage_SizeChanged;
            picImage.MouseDown += picImage_MouseDown;
            picImage.MouseMove += picImage_MouseMove;
            picImage.MouseUp += picImage_MouseUp;
            // 
            // ImageViewer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pnlContainer);
            Controls.Add(toolStrip1);
            Name = "ImageViewer";
            Size = new Size(453, 311);
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            pnlContainer.ResumeLayout(false);
            pnlContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picImage).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ToolStrip toolStrip1;
        private Panel pnlContainer;
        private PiViLityCore.Controls.PictureBox picImage;
        private ToolStripButton tbtnFitSize;
        private ToolStripButton tbtnZoomOut;
        private ToolStripButton tbtnZoom100;
        private ToolStripButton tbtnZoomIn;
        private StatusStrip statusStrip1;
    }
}
