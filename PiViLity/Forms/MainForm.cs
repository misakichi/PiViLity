using PiViLityCore;
using System.Drawing;
using System.Windows.Forms;
using Windows.Devices.Enumeration;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace PiViLity.Forms

{
    public partial class MainForm : Form
    {
        Controls.TreeAndViewTab treeAndViewTab = new();
        ToolStripButton btnSmallIconView = new();
        ToolStripButton btnLargeIconView = new();
        ToolStripButton btnListView = new();
        ToolStripButton btnDetailView = new();
        ToolStripButton btnTileView = new();

        public MainForm()
        {
            InitializeComponent();

            if (!DesignMode)
            {
                toolStrip.Renderer = new ToolStripProfessionalRenderer();
                stsStrip.Renderer = new ToolStripProfessionalRenderer();

                btnSmallIconView.Image = Global.GetResourceIcon(Resource.ResourceManager, "Icon")?.ToBitmap();
                btnLargeIconView.Image = Global.GetResourceIcon(Resource.ResourceManager, "LargeIcon")?.ToBitmap();
                btnListView.Image = Global.GetResourceIcon(Resource.ResourceManager, "List")?.ToBitmap();
                btnDetailView.Image = Global.GetResourceIcon(Resource.ResourceManager, "Detail")?.ToBitmap();
                btnTileView.Image = Global.GetResourceIcon(Resource.ResourceManager, "Thumb")?.ToBitmap();

                if (Setting.AppSettings.Instance.WindowSize.Width <= 0 || Setting.AppSettings.Instance.WindowSize.Height <= 0)
                {
                    // フォームのサイズを現在ディスプレイの大きさの半分にしてセンタリングで表示する
                    var screen = Screen.PrimaryScreen?.WorkingArea ?? new Rectangle(0, 0, 800, 600);
                    this.Size = new Size(screen.Width / 2, screen.Height / 2);
                    this.StartPosition = FormStartPosition.CenterScreen;
                }
                else
                {
                    Size = Setting.AppSettings.Instance.WindowSize;
                    Left = Setting.AppSettings.Instance.WindowPosition.X;
                    Top = Setting.AppSettings.Instance.WindowPosition.Y;
                    WindowState = Setting.AppSettings.Instance.WindowState;
                }


            }

        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeAndViewTab_Load(object sender, EventArgs e)
        {
            List<Tuple<Setting.TvLvTabPage, Controls.TreeAndView>> loadSettingPair = new();

            //TreeAndViewTabを追加する
            treeAndViewTab.Dock = DockStyle.Fill;
            panel.Controls.Add(treeAndViewTab);

            //設定ファイルを元にタブを追加する
            Setting.AppSettings.Instance.TvLvTabPages.ForEach(fileViewSetting =>
            {
                if (!System.IO.Directory.Exists(fileViewSetting.FileListView.Path))
                {
                    return;
                }

                var newTreeView = treeAndViewTab.AddTab(fileViewSetting.FileListView.Path);
                loadSettingPair.Add(new(fileViewSetting, newTreeView));
            });

            //タブがない場合新規追加
            if (treeAndViewTab.TabCount == 0)
            {
                Setting.TvLvTabPage newSetting = new();
                Setting.AppSettings.Instance.TvLvTabPages.Add(newSetting);

                var newTreeView = treeAndViewTab.AddTab(System.IO.Directory.GetCurrentDirectory());
            }
            treeAndViewTab.SelectedIndex = 0;

            ////リストビュー表示タイプ切り替えボタン
            List<ToolStripItem> items = new List<ToolStripItem>();
            items.Add(btnSmallIconView);
            items.Add(btnLargeIconView);
            items.Add(btnListView);
            items.Add(btnDetailView);
            items.Add(btnTileView);

            toolStrip.Items.AddRange(items.ToArray());
            btnSmallIconView.Click += (s, e) => SetVireType(View.SmallIcon);
            btnLargeIconView.Click += (s, e) => SetVireType(View.LargeIcon);
            btnListView.Click += (s, e) => SetVireType(View.List);
            btnTileView.Click += (s, e) => SetVireType(View.Tile);
            btnDetailView.Click += (s, e) => SetVireType(View.Details);

            treeAndViewTab.TabIndexChanged += TreeAndViewTab_TabIndexChanged;

            Show();
            RefreshViewTypeBtnChecked();

            //コントロールのレイアウトを行いサイズを現状に合わせる
            PerformLayout();
            Application.DoEvents();

            //各コントロールのフォントをSystem準拠にする
            PiViLityCore.Util.Forms.FormInitializeSystemTheme(this);
            //設定ファイルを元に各タブの設定を復元する
            BeginInvoke(new Action(() =>
            {
                loadSettingPair.ForEach(pair =>
                {
                    pair.Item2.RestoreSettings(pair.Item1);
                });
            }));
        }


        public void SaveSettings()
        {
            Setting.AppSettings.Instance.WindowSize = Size;
            Setting.AppSettings.Instance.WindowPosition = new Point(Left, Top);
            Setting.AppSettings.Instance.WindowState = WindowState;

            Setting.AppSettings.Instance.TvLvTabPages.Clear();
            for (int tabIndex = 0; tabIndex < treeAndViewTab.TabCount; tabIndex++)
            {
                var tab = treeAndViewTab.GetTab(tabIndex);
                if (tab != null)
                {
                    Setting.TvLvTabPage fileView = new();
                    tab.SaveSettings(fileView);
                    Setting.AppSettings.Instance.TvLvTabPages.Add(fileView);
                }
            }

        }

        private void SetVireType(View view)
        {
            if (treeAndViewTab.CurrentTreeAndView != null)
            {
                Setting.AppSettings.Instance.FileListViewStyle = view;
                RefreshViewTypeBtnChecked();
            }
        }

        /// <summary lang="ja-JP">
        /// リストビュー表示タイプ切り替えボタンを現状に合わせる
        /// </summary>
        private void RefreshViewTypeBtnChecked()
        {
            if (treeAndViewTab.CurrentTreeAndView == null)
                return;

            treeAndViewTab.CurrentTreeAndView.View = Setting.AppSettings.Instance.FileListViewStyle;
            btnListView.Checked = treeAndViewTab.CurrentTreeAndView.View == View.List;
            btnSmallIconView.Checked = treeAndViewTab.CurrentTreeAndView.View == View.SmallIcon;
            btnLargeIconView.Checked = treeAndViewTab.CurrentTreeAndView.View == View.LargeIcon;
            btnTileView.Checked = treeAndViewTab.CurrentTreeAndView.View == View.Tile;
            btnDetailView.Checked = treeAndViewTab.CurrentTreeAndView.View == View.Details;

        }

        /// <summary lang="ja-JP">
        /// タブが切り替わったときの動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeAndViewTab_TabIndexChanged(object? sender, EventArgs e)
        {
            RefreshViewTypeBtnChecked();

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }

    }
}
