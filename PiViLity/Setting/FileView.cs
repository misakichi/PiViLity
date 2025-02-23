using Microsoft.VisualBasic.FileIO;
using PiViLity.Controls;
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

        //ファイルビューの設定
        public string Path = SpecialDirectories.MyPictures;
        public int[] SubItemWidth = new int[(int)FileListViewSubItemBit.Max];


        //タブ内容全体の設定
        public int SplitDirWidth = 200;
        public int SplitListHeight = 400;
    }
}
