using PiViLity.Controls;
using PiViLity.Option;
using PiViLityCore;
using PiViLityCore.Controls;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Intrinsics.Arm;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using PiViLity.Dock;
using Windows.Devices.Enumeration;
using Windows.UI.StartScreen;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace PiViLity.Forms

{
    public partial class MainForm : Form
    {
        private ToolStripButton _btnSmallIconView = new();
        private ToolStripButton _btnLargeIconView = new();
        private ToolStripButton _btnListView = new();
        private ToolStripButton _btnDetailView = new();
        private ToolStripButton _btnTileView = new();
        private ToolStripButton _parentDirectoryBtn = new();
        private ToolStripButton _previousDirectoryBtn = new();
        private ToolStripButton _nextDirectoryBtn = new();

        class DirectoryTab : IDisposable
        {
            public DirectoryTab()
            {
                var rootNode = new DirectoryTreeNode(FolderTreeView);
                rootNode.SetType(DirTreeNodeType.SpecialFolderMyComputer, null);
                FolderTreeView.Nodes.Add(rootNode);

                FileListContent.Controls.Clear();
                FileListContent.Controls.Add(FileListView);
                FileListContent.IsFloat = false;
                FileListContent.DockAreas = DockAreas.Document | DockAreas.Float;

                FileListView.Dock = DockStyle.Fill;
                FolderTreeView.Dock = DockStyle.Fill;

                FileListContent.Icon = null;

           }

            public DirectoryTab(string path) : this()
            {
                FileListView.Path = path;
                FolderTreeView.Path = path;
                FileListContent.Text = System.IO.Path.GetFileName(path);
                FileListContent.Icon = PiVilityNative.FileInfo.GetFileSmallIcon(path);
            }

            public FileListViewApp FileListView { get; set; } = new();
            public DirectoryTreeViewApp FolderTreeView { get; set; } = new();

            public FileListViewContent FileListContent = new();

            public void Dispose()
            {
                FileListContent.Hide();
                FileListView.Dispose();
                FolderTreeView.Dispose();
                FileListContent.Dispose();
            }
        }

        DirectoryTab? _currentTab = null;
        List<DirectoryTab> _directoryTabs = new();

        DireectoryTreeViewContent _directoryTreeDock = new();
        FilePreviewContent _previewDock = new();

        private DirectoryTab CreateTab(string path)
        {
            DirectoryTab tab = new(path);
            tab.FileListView.ItemDoubleClick += LsvFile_ItemDoubleClick;
            tab.FileListView.SelectItemsChanged += LsvFile_SelectIItemsChanged;
            tab.FileListView.DirectoryChanged += (s, e) =>
            {
                tab.FileListContent.Text = System.IO.Path.GetFileName(tab.FileListView.Path);
                tab.FileListContent.Icon = PiVilityNative.FileInfo.GetFileSmallIcon(tab.FileListView.Path);
            };
            tab.FolderTreeView.AfterSelect += OnAfterSelectDir;
            tab.FileListContent.FormClosed += OnFileListViewClosed;
            tab.FileListView.View = PiViLityCore.Option.ShellSettings.Instance.FileListViewStyle;

            _directoryTabs.Add(tab);
            return tab;
        }

        bool _isInCreateAddTab = false;
        private void CreateAddTab()
        {
            if(_isInCreateAddTab)
                return;

            //追加用タブがない場合に追加用タブを作成する
            var activePaneContents = dockPanel.ActiveDocumentPane?.Contents;
            if (activePaneContents == null)
                return;
            for (int i = activePaneContents.Count - 1; i >= 0; i--)
            {
                var item = activePaneContents[i];
                {
                    if (item is FileListAddContent)
                    {
                        return;
                    }
                }
            }

            _isInCreateAddTab = true;
            FileListAddContent addTab = new();
            var prepareActive = dockPanel.ActiveDocument as DockContent;
            addTab.Show(dockPanel.ActiveDocumentPane, null);
            prepareActive?.Activate();
            _isInCreateAddTab = false;
        }


        private void ApplyActiveTab()
        {
            var it = _directoryTabs.FindIndex(it => it.FileListContent == dockPanel.ActiveDocument);
            if (it >= 0)
            {
                setTabContent(it);
            }
        }

        private void OnFileListViewClosed(object? sender, FormClosedEventArgs e)
        {
            if (sender is DockContent dc)
            {
                var it = _directoryTabs.FindIndex(it => it.FileListContent == dc);
                if (it >= 0)
                {
                    var tab = _directoryTabs[it];
                    _directoryTabs.RemoveAt(it);
                    tab.Dispose();

                    ApplyActiveTab();
                }
            }
        }

        void setTabContent(int index)
        {
            if (index < 0 || index >= _directoryTabs.Count)
                return;
            var tab = _directoryTabs[index];
            _directoryTreeDock.Controls.Clear();
            _directoryTreeDock.Controls.Add(tab.FolderTreeView);
            

            _currentTab?.FileListView?.ToDisable();
            _currentTab = tab;
            _currentTab?.FileListView?.ToEnable((s, e) =>
            {
                _parentDirectoryBtn.Enabled = _currentTab.FileListView.IsParentDirectoryEnabled;
                _previousDirectoryBtn.Enabled = _currentTab.FileListView.IsPreviousDirectoryEnabled;
                _nextDirectoryBtn.Enabled = _currentTab.FileListView.IsNextDirectoryEnabled;
            });
            RefreshViewTypeBtnChecked();
        }

        public MainForm()
        {
            InitializeComponent();

            if (!DesignMode)
            {
                toolStrip.Renderer = new ToolStripProfessionalRenderer();
                stsStrip.Renderer = new ToolStripProfessionalRenderer();

                _btnSmallIconView.Image = Global.GetResourceIcon(Resource.ResourceManager, "Icon")?.ToBitmap();
                _btnLargeIconView.Image = Global.GetResourceIcon(Resource.ResourceManager, "LargeIcon")?.ToBitmap();
                _btnListView.Image = Global.GetResourceIcon(Resource.ResourceManager, "List")?.ToBitmap();
                _btnDetailView.Image = Global.GetResourceIcon(Resource.ResourceManager, "Detail")?.ToBitmap();
                _btnTileView.Image = Global.GetResourceIcon(Resource.ResourceManager, "Thumb")?.ToBitmap();

                //ディレクトリ操作ツールストリップアイテム
                _parentDirectoryBtn.Text = "↑";
                _parentDirectoryBtn.Click += (s,e)=> _currentTab?.FileListView.MoveParentDirectory();

                _previousDirectoryBtn.Text = "←";
                _previousDirectoryBtn.Click += (s,e)=>_currentTab?.FileListView.MovePreviousDirectory();
                _nextDirectoryBtn.Text = "→";
                _nextDirectoryBtn.Click += (s,e) =>_currentTab?.FileListView.MoveNextDirectory();
                toolStrip.Items.Insert(0,_parentDirectoryBtn);
                toolStrip.Items.Insert(1, _previousDirectoryBtn);
                toolStrip.Items.Insert(2, _nextDirectoryBtn);
                toolStrip.Items.Insert(3, new ToolStripSeparator());


                if (AppSettings.Instance.WindowSize.Width <= 0 || AppSettings.Instance.WindowSize.Height <= 0)
                {
                    // フォームのサイズを現在ディスプレイの大きさの半分にしてセンタリングで表示する
                    var screen = Screen.PrimaryScreen?.WorkingArea ?? new Rectangle(0, 0, 800, 600);
                    this.Size = new Size(screen.Width / 2, screen.Height / 2);
                    this.StartPosition = FormStartPosition.CenterScreen;
                }
                else
                {
                    Size = AppSettings.Instance.WindowSize;
                    Left = AppSettings.Instance.WindowPosition.X;
                    Top = AppSettings.Instance.WindowPosition.Y;
                    WindowState = AppSettings.Instance.WindowState;
                }


            }

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                TreeAndViewTab_Load(sender, e);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeAndViewTab_Load(object sender, EventArgs e)
        {
            List<Tuple<bool, DirectoryTab>> loadSettingPair = new();

            dockPanel.Theme = new WeifenLuo.WinFormsUI.Docking.VS2015DarkTheme(); // 例：テーマを設定
            dockPanel.DocumentStyle = DocumentStyle.DockingWindow;
            dockPanel.ActiveDocumentChanged += DockPanel_ActiveDocumentChanged;
            dockPanel.ActivePaneChanged += DockPanel_ActivePaneChanged;

            //設定ファイルを元にファイルリストビューを追加する
            AppSettings.Instance.TvLvTabPages.ForEach(fileViewSetting =>
            {
                if (System.IO.Directory.Exists(fileViewSetting.FileListView.Path))
                {
                    var newTreeView = CreateTab(fileViewSetting.FileListView.Path);
                    newTreeView.FileListView.RestoreSettings(fileViewSetting.FileListView);
                    newTreeView.FileListContent.SaveID = loadSettingPair.Count;
                    loadSettingPair.Add(new(false, newTreeView));
                }
            });

            if (System.IO.File.Exists(AppSettings.Instance.DockLayoutFile))
            {
                dockPanel.LoadFromXml(AppSettings.Instance.DockLayoutFile, new DeserializeDockContent(s =>
                {
                    if (s.StartsWith("FileListView_"))
                    {
                        var tab = loadSettingPair.Find(it => it.Item2.FileListContent.PersistString == s);
                        if (tab != null)
                        {
                            return tab.Item2.FileListContent;
                        }
                    }
                    else if(s == "FileListAddContent")
                    {
                        return new FileListAddContent();
                    }
                    else if (s == "DireectoryTreeViewContent")
                    {
                        return _directoryTreeDock;
                    }
                    else if (s == "FilePreviewContent")
                    {
                        return _previewDock;
                    }
                    else if (s == "FilePropertyContent")
                    {
                        //将来実装
                        //return null;
                        throw new System.NotImplementedException("FilePropertyContent is not implemented yet.");
                    }
                    //????
                    throw new System.ArgumentOutOfRangeException("Unknown DockContent");
                }));
            }

            //タブがない場合新規追加
            if (loadSettingPair.Count == 0)
            {
                TvLvTabPage newSetting = new();
                AppSettings.Instance.TvLvTabPages.Add(newSetting);

                CreateTab(System.IO.Directory.GetCurrentDirectory());
            }

            //各ドックコンテントがまだ表示されてないなら表示
            foreach (var tab in _directoryTabs)
            {
                if(tab.FileListContent.IsHidden)
                    tab.FileListContent.Show(dockPanel, DockState.Document);
            }
            ApplyActiveTab();

            //非表示の場合標準位置へ
            if(_directoryTreeDock.IsHidden)
                _directoryTreeDock.Show(dockPanel, DockState.DockLeft);

            if(_previewDock.IsHidden)
                _previewDock.Show(dockPanel, DockState.DockBottom);

            //追加用タブを追加
            foreach (var pane in dockPanel.Panes)
            {
                if (pane.DockState == DockState.Document)
                {
                    CreateAddTab();
                }
            }
            //各アクティブタブをアクティブにする            
            for(int i= dockPanel.Panes.Count - 1; i >= 0; i--)
            {
                var pane = dockPanel.Panes[i];
                if (pane.DockState == DockState.Document)
                {
                    if(pane.Contents.Count>0)
                    {
                        if (pane.Contents[0] is DockContent dock)
                        {
                            dock.Activate();
                        }
                    }
                }
            }

            ////リストビュー表示タイプ切り替えボタン
            List<ToolStripItem> items = new List<ToolStripItem>();
            items.Add(_btnSmallIconView);
            items.Add(_btnLargeIconView);
            items.Add(_btnListView);
            items.Add(_btnDetailView);
            items.Add(_btnTileView);

            toolStrip.Items.AddRange(items.ToArray());
            _btnSmallIconView.Click += (s, e) => SetViewType(View.SmallIcon);
            _btnLargeIconView.Click += (s, e) => SetViewType(View.LargeIcon);
            _btnListView.Click += (s, e) => SetViewType(View.List);
            _btnTileView.Click += (s, e) => SetViewType(View.Tile);
            _btnDetailView.Click += (s, e) => SetViewType(View.Details);

            Show();
            RefreshViewTypeBtnChecked();

            //コントロールのレイアウトを行いサイズを現状に合わせる
            PerformLayout();
            Application.DoEvents();

            //各コントロールのフォントをSystem準拠にする
            PiViLityCore.Util.Forms.FormInitializeSystemTheme(this);
            ////設定ファイルを元に各タブの設定を復元する
            //BeginInvoke(new Action(() =>
            //{
            //    loadSettingPair.ForEach(pair =>
            //    {
            //        pair.Item2.RestoreSettings(pair.Item1);
            //    });
            //}));
        }

        /// <summary>
        /// フォーム閉じるとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }

        /// <summary>
        /// フォーム設定の保存
        /// </summary>
        public void SaveSettings()
        {
            AppSettings.Instance.WindowSize = Size;
            AppSettings.Instance.WindowPosition = new Point(Left, Top);
            AppSettings.Instance.WindowState = WindowState;
            AppSettings.Instance.TvLvTabPages.Clear();

            //各タブの設定を保存する
            int id= 0;
            foreach (var tab in _directoryTabs)
            {
                tab.FileListContent.SaveID = id++;
                TvLvTabPage fileView = new();
                tab.FileListView.SaveSettings(fileView.FileListView);
                AppSettings.Instance.TvLvTabPages.Add(fileView);
            }

            //ドックパネルの状態を保存する
            dockPanel.SaveAsXml(AppSettings.Instance.DockLayoutFile);
        }

        /// <summary>
        /// アクティブパネル変更時の処理
        /// 追加タブがない場合に追加タブを作成する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DockPanel_ActivePaneChanged(object? sender, EventArgs e)
        {
            if (sender is DockPanel dp)
            {
                if (dp.ActiveDocument is DockContent dc)
                {
                    if (dc.DockState == DockState.Document)
                    {
                        //追加用タブがない場合に追加用タブを作成する
                        CreateAddTab();
                    }
                }
            }
        }

        /// <summary>
        /// アクティブドキュメント変更時の処理
        /// 追加用タブがアクティブになった場合新規タブを追加する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DockPanel_ActiveDocumentChanged(object? sender, EventArgs e)
        {
            if(_isInCreateAddTab)
                return;

            if (sender is DockPanel dp)
            {
                if(dp.ActiveDocument is FileListAddContent dc)
                {
                    //追加用タブがアクティブになった場合新規タブを追加する
                    if (dp.ActiveDocumentPane.Contents.Count == 1)
                    {
                        bool foundOtherTab = false;

                        foreach (var pane in dp.Panes)
                        {
                            if(pane.DockState== DockState.Document && pane.Contents.Count > 1)
                            {
                                foundOtherTab = true;
                                break;
                            }
                        }

                        if (foundOtherTab)
                        {
                            dc.Hide();
                        return;
                        }
                    }

                    var addTab = CreateTab(_currentTab?.FolderTreeView.Path ?? "");
                    var activePane = dp.ActiveDocumentPane;
                    addTab.FileListContent.Show(activePane, dc);
                }
            }
            ApplyActiveTab();
        }


        /// <summary lang="ja-JP">
        /// ツリーの選択されたノードが変更されたとき、ファイルの一覧を変更する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnAfterSelectDir(object? sender, EventArgs e)
        {
            if (sender is DirectoryTreeView dtv)
            {
                if (_currentTab?.FolderTreeView == dtv)
                {
                    var flvFile = _currentTab.FileListView;
                    if (flvFile.TileSize != PiViLityCore.Option.ShellSettings.Instance.ThumbnailSize)
                    {
                        flvFile.ThumbnailIconStore = new IconStoreThumbnail(PiViLityCore.Option.ShellSettings.Instance.ThumbnailSize);
                    }
                    flvFile.TileSize = PiViLityCore.Option.ShellSettings.Instance.ThumbnailSize;

                    flvFile.Path = dtv.Path;
                }
            }
        }

        /// <summary>
        /// Handles the double-click event for a file item in the file list view.
        /// </summary>
        /// <remarks>If the double-clicked item represents a file, this method attempts to display the
        /// file in a viewer. If the file cannot be displayed, it is opened using the default application associated
        /// with its type.</remarks>
        /// <param name="item">The event arguments containing information about the double-clicked file item. The <see
        /// cref="FileListViewItemEventArgs.Item"/> property provides details about the file.</param>
        private void LsvFile_ItemDoubleClick(FileListViewItemEventArgs item)
        {
            if (item.Item.IsFile)
            {
                if (PiViLityCore.Util.Forms.ShowFileOnView(item.Item.Path, this))
                {
                    item.Default = false;
                }
                else
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = item.Item.Text,
                        UseShellExecute = true
                    });
                }
            }
        }
        private void LsvFile_SelectIItemsChanged(FileListViewItemsEventArgs item)
        {
            if (item.Items.Count > 0)
            {
                _previewDock.SetFile(item.Items[0].Path);
            }
            else
            {
                _previewDock.SetFile("");
            }
        }

        private void SetViewType(View view)
        {
            if (_currentTab != null)
            {
                PiViLityCore.Option.ShellSettings.Instance.FileListViewStyle = view;
                _currentTab.FileListView.View = view;
                RefreshViewTypeBtnChecked();
            }
        }

        /// <summary lang="ja-JP">
        /// リストビュー表示タイプ切り替えボタンを現状に合わせる
        /// </summary>
        private void RefreshViewTypeBtnChecked()
        {
            if (_currentTab == null)
                return;

            var viewType = _currentTab.FileListView.View;
            _btnListView.Checked = viewType == View.List;
            _btnSmallIconView.Checked = viewType == View.SmallIcon;
            _btnLargeIconView.Checked = viewType == View.LargeIcon;
            _btnTileView.Checked = viewType == View.Tile;
            _btnDetailView.Checked = viewType == View.Details;

        }


        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Option option = new Option();
            option.ShowDialog();
        }
    }
}
