using PiViLity.Option;
using PiViLityCore.Controls;
using PiViLityCore.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.UI.StartScreen;
using XmpCore.Impl;

#if false
namespace PiViLity.Controls
{
    public partial class TreeAndView : UserControl
    {

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TreeAndView()
        {
            InitializeComponent();

            //各コントロールのフォントをSystem準拠にする
            //PiViLityCore.Util.Forms.FormInitializeSystemTheme(this);

            var rootNode = new DirectoryTreeNode(tvwDirMain);
            rootNode.SetType(DirTreeNodeType.SpecialFolderMyComputer, null);
            tvwDirMain.Nodes.Add(rootNode);

        }

    }
}
#endif