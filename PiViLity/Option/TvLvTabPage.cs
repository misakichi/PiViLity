using Microsoft.VisualBasic.FileIO;
using PiViLity.Controls;
using PiViLityCore.Option;
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
        [OptionItem]
        public PiViLityCore.Controls.FileListViewSettings FileListView = new();


        //タブ内容全体の設定
        [OptionItem]
        public int SplitDirWidth = 200;
        [OptionItem]
        public int SplitListHeight = 400;
    }
}
