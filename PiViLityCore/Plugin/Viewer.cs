using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Plugin
{
    public interface IViewer : IDisposable
    {

        bool LoadFile(string filePath);
        public Control GetViewer();

        event EventHandler? FileLoaded;
        
        string Path { get; }

        public enum ViewType
        {
            Image,

        }

        ViewType SupportViewType { get; }


        IEnumerable<ToolStripItem> ToolBarItems { get; }
        IEnumerable<ToolStripItem> StatusBarItems { get; }


        bool NextFile();

        bool PreviousFile();
        bool FirstFile();
        bool LastFile();
    }
}
