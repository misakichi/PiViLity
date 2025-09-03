using System.Windows.Forms;

namespace PiViLityCore.Forms
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
            viewPanel = new Panel();
            viewToolStrip = new ToolStrip();
            status.SuspendLayout();
            SuspendLayout();
            // 
            // status
            // 
            status.ImageScalingSize = new Size(24, 24);
            status.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            status.Location = new Point(0, 718);
            status.Name = "status";
            status.Padding = new Padding(1, 0, 20, 0);
            status.Size = new Size(1143, 32);
            status.TabIndex = 0;
            status.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(55, 25);
            toolStripStatusLabel1.Text = "None";
            // 
            // viewPanel
            // 
            viewPanel.Dock = DockStyle.Fill;
            viewPanel.Location = new Point(0, 25);
            viewPanel.Margin = new Padding(4, 5, 4, 5);
            viewPanel.Name = "viewPanel";
            viewPanel.Size = new Size(1143, 693);
            viewPanel.TabIndex = 2;
            // 
            // viewToolStrip
            // 
            viewToolStrip.ImageScalingSize = new Size(24, 24);
            viewToolStrip.Location = new Point(0, 0);
            viewToolStrip.Name = "viewToolStrip";
            viewToolStrip.Padding = new Padding(0, 0, 3, 0);
            viewToolStrip.Size = new Size(1143, 25);
            viewToolStrip.TabIndex = 3;
            viewToolStrip.Text = "toolStrip1";
            // 
            // ViewerForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1143, 750);
            Controls.Add(viewPanel);
            Controls.Add(viewToolStrip);
            Controls.Add(status);
            Margin = new Padding(4, 5, 4, 5);
            Name = "ViewerForm";
            Text = "ViewerForm";
            FormClosed += ViewerForm_FormClosed;
            KeyDown += ViewerForm_KeyDown;
            KeyUp += ViewerForm_KeyUp;
            status.ResumeLayout(false);
            status.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private StatusStrip status;
        private Panel viewPanel;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStrip viewToolStrip;
    }
}