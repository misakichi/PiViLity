using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;

namespace PiViLity.Dock
{
    internal class FileListViewContent : DockContent
    {
        public FileListViewContent()
        {
            Icon = null;
            DockAreas = DockAreas.Document | DockAreas.Float;
            DockStateChanged += OnDockStateChanged;

        }

        private void OnDockStateChanged(object? sender, EventArgs e)
        {

        }


        protected override string GetPersistString()
        {
            return $"FileListView_{SaveID}";
        }
        public string PersistString => GetPersistString();

        [DefaultValue(0)]
        public int SaveID { get; set; } = 0;

    }

}
