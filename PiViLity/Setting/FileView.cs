using Microsoft.VisualBasic.FileIO;
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

        public string Path = SpecialDirectories.MyPictures;
    }
}
