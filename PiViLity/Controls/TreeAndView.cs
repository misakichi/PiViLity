using PiViLityCore.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PiViLity.Controls.TreeAndView;

namespace PiViLity.Controls
{
    public partial class TreeAndView : UserControl
    {

        private PiViLityCore.Shell.DirTree dirTree = new();
        private DirTreeViewControl dirTreeViewMgr = new();

        public event EventHandler? AfterSelect;

        public TreeAndView()
        {
            InitializeComponent();


            //各コントロールのフォントをSystem準拠にする
            PiViLityCore.Util.Forms.FormInitializeSystemTheme(this);

            //ツリービューセットアップ
            dirTreeViewMgr.Bind(dirTree, tvwDirMain);
            tvwDirMain.FullRowSelect = true;
            tvwDirMain.HideSelection = false;

            dirTreeViewMgr.AfterSelect += DirTreeViewMgr_AfterSelect;
            tvwDirMain.HotTracking = true;

            lsvFile.LabelEdit = true;

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
            AfterSelect?.Invoke(this, EventArgs.Empty);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SelectedPath
        {
            set { if (dirTreeViewMgr != null) { dirTreeViewMgr.SelectedPath = value; } }
            get => dirTreeViewMgr?.SelectedPath ?? "";
        }

        public string SelectedName => dirTreeViewMgr.SelectedName;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public View View
        {
            get => lsvFile?.View ?? View.LargeIcon;
            set { if (lsvFile != null) { lsvFile.View = value; } }
        }

        /// <summary>
        /// 設定を復元する
        /// </summary>
        /// <param name="fileView"></param>
        public void RestoreSettings(Setting.FileView fileView)
        {
            splitDirView.SplitPosition = fileView.SplitDirWidth;
            splitViewInfo.SplitPosition = fileView.SplitListHeight;
            lsvFile.RestoreSettings(fileView);
        }

        /// <summary>
        /// 設定を保存する
        /// </summary>
        /// <param name="fileView"></param>
        public void SaveSettings(Setting.FileView fileView)
        {
            fileView.SplitDirWidth = splitDirView.SplitPosition;
            fileView.SplitListHeight = splitViewInfo.SplitPosition;
            lsvFile.SaveSettings(fileView);
        }

        private void lsvFile_ColumnClick(object sender, ColumnClickEventArgs e)
        {

        }
    }
}
