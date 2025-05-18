using PiViLityCore.Plugin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PiViLityCore.Forms
{
    public partial class ViewerForm : Form
    {
        private Plugin.IViewer? _viewer;

        public ViewerForm()
        {
            InitializeComponent();
            if (!DesignMode)
            {
                status.Renderer = new ToolStripProfessionalRenderer();
                viewToolStrip.Renderer = new ToolStripProfessionalRenderer();
                //status.Items.Add(imgViewer.ResolutionStatus);
                //status.Items.Add(imgViewer.ScaleStatus);
            }
        }

        public bool LoadFile(string filename)
        {
            var reader = PluginManager.Instance.GetImageReader(filename);
            bool ret=false;
            if (reader is PiViLityCore.Plugin.IImageReader)
            {
                if (_viewer is not PiViLityCore.Plugin.IViewer)
                {
                    status.Items.Clear();
                    viewToolStrip.Items.Clear();
                    status.Items.Add(toolStripStatusLabel1);
                    _viewer?.Dispose();
                    _viewer = null;
                    if (PluginManager.Instance.ImageViewers.Count > 0)
                    {
                        var type = PluginManager.Instance.ImageViewers[0];
                        if (Activator.CreateInstance(type) is IImageViewer viewer)
                        {
                            _viewer = viewer;
                            status.Items.AddRange(_viewer.StatusBarItems.ToArray());
                            viewToolStrip.Items.AddRange(_viewer.ToolBarItems.ToArray());
                            viewPanel.Controls.Clear();
                            var viewerControl = _viewer.GetViewer();
                            viewerControl.Dock = DockStyle.Fill;
                            viewPanel.Controls.Add(viewerControl);
                        }
                    }
                }
                ret = _viewer?.LoadFile(filename) ?? false;
            }
            if (ret && _viewer!=null)
            {
                Text = $"{_viewer.Path} - PiViLity";
            }
            return ret;
        }

        private void ViewerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Dispose();
            GC.Collect();
        }
    }
}
