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
        List<string> directoryRecent_ = new();
        int currentRecentIndex_ = -1;

        public event EventHandler? DirectoryChanged;

        public TreeAndView()
        {
            InitializeComponent();

            //各コントロールのフォントをSystem準拠にする
            //PiViLityCore.Util.Forms.FormInitializeSystemTheme(this);

            var rootNode = new DirectoryTreeNode(tvwDirMain);
            rootNode.SetType(DirTreeNodeType.SpecialFolderMyComputer, null);
            tvwDirMain.Nodes.Add(rootNode);
            tvwDirMain.AfterSelect += OnAfterSelectDir;
            lsvFile.DirectoryChanged += FlvOnDirectoryChanged;
            lsvFile.LabelEdit = true;
            lsvFile.ThumbnailIconStore = new IconStoreThumbnail(Setting.AppSettings.Instance.ThumbnailSize);

            if (!DesignMode)
            {
                lsvFile.TileSize = Setting.AppSettings.Instance.ThumbnailSize;
            }

            //ツールストリップの基本機能を追加
            {
                var upDdir = new ToolStripButton("↑");
                upDdir.Click += (s, e) =>
                {
                    var path = lsvFile.Path;
                    if (path != null)
                    {
                        var dir = new DirectoryInfo(path);
                        if (dir.Parent is DirectoryInfo parentDir)
                        {
                            lsvFile.Path = parentDir.FullName;
                        }
                    }
                };
                toolStrip.Items.Add(upDdir);
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
            lsvFile.Path = tvwDirMain.SelectedPath;
        }

        private void FlvOnDirectoryChanged(object? sender, EventArgs e)
        {
            DirectoryChanged?.Invoke(this, EventArgs.Empty);
            directoryRecent_.RemoveRange(currentRecentIndex_ + 1, directoryRecent_.Count - currentRecentIndex_ - 1);
            directoryRecent_.Add(lsvFile.Path);
            currentRecentIndex_ = directoryRecent_.Count - 1;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string DirectoryPath
        {
            set { if (tvwDirMain != null) { tvwDirMain.SelectedPath = value; } }
            get => lsvFile.Path ?? "";
        }

        public string SelectedText => Path.GetFileName(lsvFile.Path) ?? "";

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
