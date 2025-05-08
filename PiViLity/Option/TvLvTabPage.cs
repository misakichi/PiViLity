using Microsoft.VisualBasic.FileIO;
using PiViLity.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLity.Option
{
    [Serializable]
    public class TvLvTabPage
    {
        public TvLvTabPage()
        { 
        }

        //ファイルビューの設定
        public PiViLityCore.Controls.FileListViewSettings FileListView = new();


        //タブ内容全体の設定
        public int SplitDirWidth = 200;
        public int SplitListHeight = 400;
    }
}
