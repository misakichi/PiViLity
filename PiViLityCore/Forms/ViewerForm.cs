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
            KeyPreview = true;
        }

        public bool LoadFile(string filename)
        {
            var reader = PluginManager.Instance.GetImageReader(filename);
            bool ret = false;
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
                            viewer.FileLoaded += Viewer_FileLoaded;
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
            return ret;
        }

        private void Viewer_FileLoaded(object? sender, EventArgs e)
        {
            Text = $"{_viewer?.Path ?? "no file"} - PiViLity";
        }

        private void ViewerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Dispose();
            GC.Collect();
        }

        private void ViewerForm_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void ViewerForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode==Keys.ShiftKey || e.KeyCode == Keys.ControlKey)
                return;
            if (e.KeyCode == Keys.Right)
            {
                _viewer?.NextFile();
            }
            else if (e.KeyCode == Keys.Left)
            {
                _viewer?.PreviousFile();
            }
            else if(_viewer is IShotcutCommandSupport commandSupport)
            {
                commandSupport.OnKeyDown(e);
            }

        }
    }
}
