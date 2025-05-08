using PiViLityCore;
using System.Drawing;
using System.Windows.Forms;
using Windows.Devices.Enumeration;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using PiViLity.Option;

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

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeAndViewTab_Load(object sender, EventArgs e)
        {
            List<Tuple<TvLvTabPage, Controls.TreeAndView>> loadSettingPair = new();

            //TreeAndViewTabを追加する
            treeAndViewTab.Dock = DockStyle.Fill;
            panel.Controls.Add(treeAndViewTab);

            //設定ファイルを元にタブを追加する
            AppSettings.Instance.TvLvTabPages.ForEach(fileViewSetting =>
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
                TvLvTabPage newSetting = new();
                AppSettings.Instance.TvLvTabPages.Add(newSetting);

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
            btnSmallIconView.Click += (s, e) => SetViewType(View.SmallIcon);
            btnLargeIconView.Click += (s, e) => SetViewType(View.LargeIcon);
            btnListView.Click += (s, e) => SetViewType(View.List);
            btnTileView.Click += (s, e) => SetViewType(View.Tile);
            btnDetailView.Click += (s, e) => SetViewType(View.Details);

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
            AppSettings.Instance.WindowSize = Size;
            AppSettings.Instance.WindowPosition = new Point(Left, Top);
            AppSettings.Instance.WindowState = WindowState;
            AppSettings.Instance.TvLvTabPages.Clear();
            for (int tabIndex = 0; tabIndex < treeAndViewTab.TabCount; tabIndex++)
            {
                var tab = treeAndViewTab.GetTab(tabIndex);
                if (tab != null)
                {
                    TvLvTabPage fileView = new();
                    tab.SaveSettings(fileView);
                    AppSettings.Instance.TvLvTabPages.Add(fileView);
                }
            }

        }

        private void SetViewType(View view)
        {
            if (treeAndViewTab.CurrentTreeAndView != null)
            {
                PiViLityCore.Option.ShellSettings.Instance.FileListViewStyle = view;
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

            treeAndViewTab.CurrentTreeAndView.View = PiViLityCore.Option.ShellSettings.Instance.FileListViewStyle;
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

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Option option = new Option();
            option.ShowDialog();
        }
    }
}
