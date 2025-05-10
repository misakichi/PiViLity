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
            status = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            imgViewer = new PiViLity.Viewer.ImageViewer();
            status.SuspendLayout();
            SuspendLayout();
            // 
            // status
            // 
            status.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            status.Location = new Point(0, 428);
            status.Name = "status";
            status.Size = new Size(800, 22);
            status.TabIndex = 0;
            status.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(36, 17);
            toolStripStatusLabel1.Text = "None";
            // 
            // imgViewer
            // 
            imgViewer.Dock = DockStyle.Fill;
            imgViewer.Location = new Point(0, 0);
            imgViewer.Name = "imgViewer";
            imgViewer.Size = new Size(800, 428);
            imgViewer.TabIndex = 2;
            // 
            // ViewerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(imgViewer);
            Controls.Add(status);
            Name = "ViewerForm";
            Text = "ViewerForm";
            FormClosed += ViewerForm_FormClosed;
            status.ResumeLayout(false);
            status.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private StatusStrip status;
        private Viewer.ImageViewer imgViewer;
        private ToolStripStatusLabel toolStripStatusLabel1;
    }
}