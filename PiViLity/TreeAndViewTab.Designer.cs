namespace PiViLity
{
    partial class TreeAndViewTab
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
            tabView = new TabControl();
            SuspendLayout();
            // 
            // tabView
            // 
            tabView.Dock = DockStyle.Fill;
            tabView.Location = new Point(0, 0);
            tabView.Name = "tabView";
            tabView.SelectedIndex = 0;
            tabView.Size = new Size(800, 450);
            tabView.TabIndex = 7;
            // 
            // TreeAndViewTab
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(800, 450);
            Controls.Add(tabView);
            Name = "TreeAndViewTab";
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabView;
    }
}