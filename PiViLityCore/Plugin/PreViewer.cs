using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Plugin
{
    /// <summary>
    /// プレビュープラグインインターフェース
    /// </summary>
    public interface IPreViewer : IDisposable
    {
        bool LoadFile(string filePath);
        public Control GetPreViewer();

        event EventHandler? FileLoaded;

        string Path { get; }


        ViewType SupportViewType { get; }


        IEnumerable<ToolStripItem> ToolBarItems { get; }
        IEnumerable<ToolStripItem> StatusBarItems { get; }
    }
}
