namespace PiViLity.Dock
{
    partial class FilePropertyContent
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
            tree = new TreeView();
            SuspendLayout();
            // 
            // tree
            // 
            tree.Dock = DockStyle.Fill;
            tree.Location = new Point(0, 0);
            tree.Name = "tree";
            tree.ShowLines = false;
            tree.Size = new Size(800, 450);
            tree.TabIndex = 0;
            // 
            // FilePropertyContent
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tree);
            Name = "FilePropertyContent";
            Text = "FilePropertyContent";
            ResumeLayout(false);
        }

        #endregion

        private TreeView tree;
    }
}