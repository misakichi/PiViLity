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

namespace PiViLity.Forms
{
    public partial class ViewerForm : Form
    {
        public ViewerForm()
        {
            InitializeComponent();
            if (!DesignMode)
            {
                status.Renderer = new ToolStripProfessionalRenderer();

                status.Items.Add(imgViewer.ResolutionStatus);
                status.Items.Add(imgViewer.ScaleStatus);
            }
        }

        public bool LoadFile(string filename)
        {
            if (imgViewer.LoadImage(filename))
            {
                Text = filename;
                return true;
            }
            return false;
        }

        private void ViewerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Dispose();
            GC.Collect();
        }
    }
}
