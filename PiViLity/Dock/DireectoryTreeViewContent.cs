using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;
using static System.Net.Mime.MediaTypeNames;

namespace PiViLity.Dock
{
    internal class DireectoryTreeViewContent : DockContent
    {
        public DireectoryTreeViewContent()
        {
            Icon = null;
            Text = "Explorer";
            this.ShowIcon = false;
            DockAreas = DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockTop | DockAreas.DockBottom | DockAreas.Float;
        }
        protected override string GetPersistString()
        {
            return "DireectoryTreeViewContent";
        }
    }

}
