using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLity.Setting
{
    [Serializable]
    public class FileView
    {
        public FileView()
        { 
        }
        public string CacheDbDirectory = Path.GetDirectoryName(Application.ExecutablePath) ?? "";
        public View FileListViewStyle = View.Tile;
    }
}
