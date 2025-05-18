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
            pnlContainer = new Panel();
            picImage = new PiViLityCore.Controls.PictureBox();
            tlblResolutionStatus = new ToolStripStatusLabel();
            tlblScaleStatus = new ToolStripStatusLabel();
            pnlContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picImage).BeginInit();
            SuspendLayout();

            // 
            // pnlContainer
            // 
            pnlContainer.AutoScroll = true;
            pnlContainer.Controls.Add(picImage);
            pnlContainer.Dock = DockStyle.Fill;
            pnlContainer.Location = new Point(0, 0);
            pnlContainer.Name = "pnlContainer";
            pnlContainer.Size = new Size(453, 311);
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
            Name = "ImageViewer";
            Size = new Size(453, 311);
            pnlContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picImage).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlContainer;
        private PiViLityCore.Controls.PictureBox picImage;

    }
}
