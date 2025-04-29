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
            pnlImage = new Panel();
            pnlContainer.SuspendLayout();
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
            pnlContainer.Controls.Add(pnlImage);
            pnlContainer.Dock = DockStyle.Fill;
            pnlContainer.Location = new Point(0, 25);
            pnlContainer.Name = "pnlContainer";
            pnlContainer.Size = new Size(453, 286);
            pnlContainer.TabIndex = 1;
            // 
            // pnlImage
            // 
            pnlImage.Location = new Point(0, 0);
            pnlImage.Margin = new Padding(0);
            pnlImage.Name = "pnlImage";
            pnlImage.Size = new Size(200, 100);
            pnlImage.TabIndex = 0;
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
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ToolStrip toolStrip1;
        private Panel pnlContainer;
        private Panel pnlImage;
    }
}
