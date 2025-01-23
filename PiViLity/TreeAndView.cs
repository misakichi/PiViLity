using PiViLityCore.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PiViLity.TreeAndView;

namespace PiViLity
{
    public partial class TreeAndView : UserControl
    {
        private PiViLityCore.Shell.DirTree dirTree = new();
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal DirTreeViewControl dirTreeViewMgr { get; private set; } = new();



        public TreeAndView()
        {
            InitializeComponent();

            //各コントロールのフォントをSystem準拠にする
            PiViLityCore.Util.Forms.FormInitializeSystemTheme(this);

            //ツリービューセットアップ
            dirTreeViewMgr.Bind(dirTree, tvwDirMain);

            dirTreeViewMgr.AfterSelect += DirTreeViewMgr_AfterSelect;
        }

        /// <summary lang="ja-JP">
        /// 選択されたノードが変更されたとき、ファイルの一覧を変更する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void DirTreeViewMgr_AfterSelect(DirTreeViewControl sender, DirTreeViewControl.DirTreeViewControlEventArgs e)
        {
            lsvFile.Path = SelectedPath;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SelectedPath
        {
            set => dirTreeViewMgr.SelectedPath = value;
            get => dirTreeViewMgr.SelectedPath;
        }

        public string SelectedName => dirTreeViewMgr.SelectedName;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public View View { get => lsvFile.View; set => lsvFile.View = value; }
    }
}
