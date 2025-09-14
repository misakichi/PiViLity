using PiViLity.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PiViLity.Dock
{
    /// <summary>
    /// ファイルリストビューのドッキングコンテナ（ドキュメント専用）
    /// </summary>
    public partial class FileListViewContent : DockContent
    {
        FileListViewApp _fileListView;
        public FileListViewContent(FileListViewApp flv)
        {
            _fileListView = flv;
            InitializeComponent();
            Icon = null;
            DockAreas = DockAreas.Document | DockAreas.Float;
        }



        private void FileListViewContent_Load(object sender, EventArgs e)
        {
            if (DesignMode)
                return;

            _fileListView.Dock = DockStyle.Fill;
            _flvPanel.Controls.Add(_fileListView);

            _toolStrip.Renderer = new ToolStripProfessionalRenderer();

            //ツールストリップボタンのイベント設定
            _fileListView.DirectoryNavigatorButtonStatusChanged += _fileListView_DirectoryNavigatorButtonStatusChanged;
            _parentDirectoryBtn.Click += (s, e) => _fileListView.MoveParentDirectory();
            _previousDirectoryBtn.Click += (s, e) => _fileListView.MovePreviousDirectory();
            _nextDirectoryBtn.Click += (s, e) => _fileListView.MoveNextDirectory();
            _btnSmallIconView.Click += (s, e) => SetViewType(View.SmallIcon);
            _btnLargeIconView.Click += (s, e) => SetViewType(View.LargeIcon);
            _btnListView.Click += (s, e) => SetViewType(View.List);
            _btnTileView.Click += (s, e) => SetViewType(View.Tile);
            _btnDetailView.Click += (s, e) => SetViewType(View.Details);

            RefreshNavigationButtonStatus();
        }

        /// <summary>
        /// ナビゲーションボタンの状況を変えるべきとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _fileListView_DirectoryNavigatorButtonStatusChanged(object? sender, EventArgs e)=> RefreshNavigationButtonStatus();

        private void RefreshNavigationButtonStatus()
        {
            _parentDirectoryBtn.Enabled = _fileListView.CanParentDirectoryEnabled;
            _previousDirectoryBtn.Enabled = _fileListView.CanPreviousDirectoryEnabled;
            _nextDirectoryBtn.Enabled = _fileListView.CanNextDirectoryEnabled;
        }

        /// <summary>
        /// ビュー表示タイプ設定
        /// </summary>
        /// <param name="view"></param>
        private void SetViewType(View view)
        {
            PiViLityCore.Option.ThumbnailSettings.Instance.FileListViewStyle = view;
            _fileListView.View = view;
            RefreshViewTypeBtnChecked();
        }

        /// <summary lang="ja-JP">
        /// リストビュー表示タイプ切り替えボタンを現状に合わせる
        /// </summary>
        private void RefreshViewTypeBtnChecked()
        {
            var viewType = _fileListView.View;
            _btnListView.Checked = viewType == View.List;
            _btnSmallIconView.Checked = viewType == View.SmallIcon;
            _btnLargeIconView.Checked = viewType == View.LargeIcon;
            _btnTileView.Checked = viewType == View.Tile;
            _btnDetailView.Checked = viewType == View.Details;
        }


        protected override string GetPersistString()
        {
            return $"FileListView_{SaveID}";
        }


        public string PersistString => GetPersistString();

        [DefaultValue(0)]
        public int SaveID { get; set; } = 0;
    }
}
