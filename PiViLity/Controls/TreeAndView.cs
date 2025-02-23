using PiViLityCore.Controls;
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
using Windows.UI.StartScreen;
using static PiViLity.Controls.TreeAndView;

namespace PiViLity.Controls
{
    public partial class TreeAndView : UserControl
    {


        public event EventHandler? AfterSelect;

        public TreeAndView()
        {
            InitializeComponent();

            //各コントロールのフォントをSystem準拠にする
            //PiViLityCore.Util.Forms.FormInitializeSystemTheme(this);

            var rootNode = new DirectoryTreeNode(tvwDirMain);
            rootNode.SetType(DirTreeNodeType.SpecialFolderMyComputer, null);
            tvwDirMain.Nodes.Add(rootNode);
            tvwDirMain.AfterSelect += OnAfterSelectDir;
            lsvFile.LabelEdit = true;
            lsvFile.ThumbnailIconStore = new IconStoreThumbnail(Setting.AppSettings.Instance.ThumbnailSize);

            if (!DesignMode)
            {
                lsvFile.TileSize = Setting.AppSettings.Instance.ThumbnailSize;
            }


        }

        /// <summary lang="ja-JP">
        /// 選択されたノードが変更されたとき、ファイルの一覧を変更する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnAfterSelectDir(object? sender, EventArgs e)
        {
            lsvFile.Path = SelectedPath;
            AfterSelect?.Invoke(this, EventArgs.Empty);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SelectedPath
        {
            set { if (tvwDirMain != null) { tvwDirMain.SelectedPath = value; } }
            get => tvwDirMain?.SelectedPath ?? "";
        }

        public string SelectedText => tvwDirMain.SelectedNode?.Text ?? "";

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
        public void RestoreSettings(Setting.TvLvTabPage pageSetting)
        {
            splitDirView.SplitPosition = pageSetting.SplitDirWidth;
            splitViewInfo.SplitPosition = pageSetting.SplitListHeight;
            lsvFile.RestoreSettings(pageSetting.FileListView);
        }

        /// <summary> 
        /// 設定を保存する
        /// </summary>
        /// <param name="fileView"></param>
        public void SaveSettings(Setting.TvLvTabPage pageSetting)
        {
            pageSetting.SplitDirWidth = splitDirView.SplitPosition;
            pageSetting.SplitListHeight = splitViewInfo.SplitPosition;
            lsvFile.SaveSettings(pageSetting.FileListView);
        }

        private void lsvFile_ColumnClick(object sender, ColumnClickEventArgs e)
        {

        }
    }
}
