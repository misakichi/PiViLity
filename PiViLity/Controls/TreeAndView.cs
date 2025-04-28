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
        private List<string> _directoryRecent = new();
        private int _currentRecentIndex = -1;
        private bool _isInPathSetter = false;

        public event EventHandler? DirectoryChanged;

        private ToolStripButton BackDirectoryBtn = new();
        private ToolStripButton PreviousDirectoryBtn = new();
        private ToolStripButton NextDirectoryBtn = new();

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
            lsvFile.ThumbnailIconStore = new IconStoreThumbnail(Setting.ShellSettings.Instance.ThumbnailSize);

            if (!DesignMode)
            {
                lsvFile.TileSize = Setting.ShellSettings.Instance.ThumbnailSize;
            }

            //ツールストリップの基本機能を追加
            BackDirectoryBtn.Text = "↑";
            BackDirectoryBtn.Click += (s, e) =>
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

            PreviousDirectoryBtn.Text = "←";
            PreviousDirectoryBtn.Click += (s, e) =>
            {
                if(_currentRecentIndex > 0)
                {
                    _currentRecentIndex--;
                    lsvFile.Path = _directoryRecent[_currentRecentIndex];
                }
                if (_currentRecentIndex == 0)
                {
                    PreviousDirectoryBtn.Enabled = _currentRecentIndex > 0;
                    NextDirectoryBtn.Enabled = _currentRecentIndex < _directoryRecent.Count - 1;
                }
            };
            NextDirectoryBtn.Text = "→";
            NextDirectoryBtn.Click += (s, e) =>
            {
                if (_currentRecentIndex < _directoryRecent.Count - 1)
                {
                    _currentRecentIndex++;
                    lsvFile.Path = _directoryRecent[_currentRecentIndex];
                }
                if(_currentRecentIndex == _directoryRecent.Count - 1)
                {
                    PreviousDirectoryBtn.Enabled = _currentRecentIndex > 0;
                    NextDirectoryBtn.Enabled = _currentRecentIndex < _directoryRecent.Count - 1;
                }
            };
            toolStrip.Items.Add(BackDirectoryBtn);
            toolStrip.Items.Add(PreviousDirectoryBtn);
            toolStrip.Items.Add(NextDirectoryBtn);
        }

        /// <summary lang="ja-JP">
        /// 選択されたノードが変更されたとき、ファイルの一覧を変更する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnAfterSelectDir(object? sender, EventArgs e)
        {
            lsvFile.Path = tvwDirMain.Path;
        }

        private void FlvOnDirectoryChanged(object? sender, EventArgs e)
        {
            DirectoryChanged?.Invoke(this, EventArgs.Empty);
            if (!_isInPathSetter)
            {
                AddRecent();
            }
        }

        private void AddRecent()
        {
            _directoryRecent.RemoveRange(_currentRecentIndex + 1, _directoryRecent.Count - _currentRecentIndex - 1);
            _directoryRecent.Add(lsvFile.Path);
            _currentRecentIndex = _directoryRecent.Count - 1;
            PreviousDirectoryBtn.Enabled = _currentRecentIndex > 0;
            NextDirectoryBtn.Enabled = _currentRecentIndex < _directoryRecent.Count - 1;

        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string DirectoryPath
        {
            set { _isInPathSetter = true;  tvwDirMain.Path = value; _isInPathSetter = false; }
            get => tvwDirMain.Path;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Path
        {
            set { _isInPathSetter = true;  lsvFile.Path = value; _isInPathSetter = false; }
            get => lsvFile.Path;
        }


        public string SelectedText => System.IO.Path.GetFileName(lsvFile.Path) ?? "";

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

        public void Initialize(string path)
        {
            DirectoryPath = path;
            _directoryRecent.Clear();
            _currentRecentIndex = -1;
            AddRecent();
        }
    }
}
