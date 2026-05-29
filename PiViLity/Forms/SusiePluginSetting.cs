using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PiViLity.Forms
{
    public partial class SusiePluginSetting : UserControl
    {
        public SusiePluginSetting()
        {
            InitializeComponent();
        }

        private void SusiePluginSetting_Load(object sender, EventArgs e)
        {
            txtPluginDirectory.Text = PiViLity.Option.SusiePluginSettings.Instance.PluginPath;
        }

        private void btnDirRef_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = PiViLity.Option.SusiePluginSettings.Instance.PluginPath;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtPluginDirectory.Text = dlg.SelectedPath;
            }
        }

        private void txtPluginDirectory_TextChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(txtPluginDirectory.Text))
            {
                PiViLity.Option.SusiePluginSettings.Instance.PluginPath = txtPluginDirectory.Text;
            }
        }
    }
}
