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
            toolStrip1 = new ToolStrip();
            pnlContainer = new Panel();
            picImage = new PictureBox();
            pnlContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picImage).BeginInit();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(453, 25);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
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
            picImage.SizeChanged += picImage_SizeChanged;
            // 
            // ImageViewer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pnlContainer);
            Controls.Add(toolStrip1);
            Name = "ImageViewer";
            Size = new Size(453, 311);
            pnlContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picImage).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ToolStrip toolStrip1;
        private Panel pnlContainer;
        private PictureBox picImage;
    }
}
