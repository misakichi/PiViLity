namespace PiViLity.Forms
{
    partial class ViewerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            imgViewer = new PiViLity.Viewer.ImageViewer();
            SuspendLayout();
            // 
            // imgViewer
            // 
            imgViewer.Dock = DockStyle.Fill;
            imgViewer.Location = new Point(0, 0);
            imgViewer.Name = "imgViewer";
            imgViewer.Size = new Size(800, 450);
            imgViewer.TabIndex = 0;
            // 
            // ViewerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(imgViewer);
            Name = "ViewerForm";
            Text = "ViewerForm";
            ResumeLayout(false);
        }

        #endregion

        private Viewer.ImageViewer imgViewer;
    }
}