namespace PiViLity.Forms
{
    partial class MainForm
    {
        /// <summary lang="ja">
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary lang="ja">
        ///  Clean up any resources being used.
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

        /// <summary lang="ja">
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            toolStrip = new ToolStrip();
            stsStrip = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            mnuForm = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            quitToolStripMenuItem = new ToolStripMenuItem();
            toolToolStripMenuItem = new ToolStripMenuItem();
            optionsToolStripMenuItem = new ToolStripMenuItem();
            panel = new Panel();
            stsStrip.SuspendLayout();
            mnuForm.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip
            // 
            resources.ApplyResources(toolStrip, "toolStrip");
            toolStrip.Name = "toolStrip";
            // 
            // stsStrip
            // 
            stsStrip.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            resources.ApplyResources(stsStrip, "stsStrip");
            stsStrip.Name = "stsStrip";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            resources.ApplyResources(toolStripStatusLabel1, "toolStripStatusLabel1");
            // 
            // mnuForm
            // 
            mnuForm.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, toolToolStripMenuItem });
            resources.ApplyResources(mnuForm, "mnuForm");
            mnuForm.Name = "mnuForm";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { quitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // quitToolStripMenuItem
            // 
            quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            resources.ApplyResources(quitToolStripMenuItem, "quitToolStripMenuItem");
            // 
            // toolToolStripMenuItem
            // 
            toolToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { optionsToolStripMenuItem });
            toolToolStripMenuItem.Name = "toolToolStripMenuItem";
            resources.ApplyResources(toolToolStripMenuItem, "toolToolStripMenuItem");
            // 
            // optionsToolStripMenuItem
            // 
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            resources.ApplyResources(optionsToolStripMenuItem, "optionsToolStripMenuItem");
            optionsToolStripMenuItem.Click += optionsToolStripMenuItem_Click;
            // 
            // panel
            // 
            resources.ApplyResources(panel, "panel");
            panel.Name = "panel";
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(panel);
            Controls.Add(stsStrip);
            Controls.Add(toolStrip);
            Controls.Add(mnuForm);
            MainMenuStrip = mnuForm;
            Name = "MainForm";
            FormClosing += MainForm_FormClosing;
            Load += TreeAndViewTab_Load;
            SizeChanged += TreeAndViewTab_TabIndexChanged;
            stsStrip.ResumeLayout(false);
            stsStrip.PerformLayout();
            mnuForm.ResumeLayout(false);
            mnuForm.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ToolStrip toolStrip;
        private StatusStrip stsStrip;
        private MenuStrip mnuForm;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private Panel panel;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem quitToolStripMenuItem;
        private ToolStripMenuItem toolToolStripMenuItem;
        private ToolStripMenuItem optionsToolStripMenuItem;
    }
}
