namespace PiViLity
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
            panel = new Panel();
            stsStrip.SuspendLayout();
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
            resources.ApplyResources(mnuForm, "mnuForm");
            mnuForm.Name = "mnuForm";
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
            stsStrip.ResumeLayout(false);
            stsStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ToolStrip toolStrip;
        private StatusStrip stsStrip;
        private MenuStrip mnuForm;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private Panel panel;
    }
}
