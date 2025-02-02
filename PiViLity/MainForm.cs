using PiViLityCore;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace PiViLity
{
    public partial class MainForm : Form
    {
        TreeAndViewTab treeAndViewTab;
        ToolStripButton btnIconView = new();
        ToolStripButton btnSmallIconView = new();
        ToolStripButton btnLargeIconView = new();
        ToolStripButton btnListView = new();
        ToolStripButton btnDetailView = new();
        ToolStripButton btnTileView = new();

        public MainForm()
        {
            InitializeComponent();
            toolStrip.Renderer = new ToolStripProfessionalRenderer();
            stsStrip.Renderer = new ToolStripProfessionalRenderer();

            List<ToolStripItem> items = new List<ToolStripItem>();
            btnSmallIconView.Image = Global.GetResourceIcon(Resource.ResourceManager, "Icon")?.ToBitmap();
            btnLargeIconView.Image = Global.GetResourceIcon(Resource.ResourceManager, "LargeIcon")?.ToBitmap();
            btnListView.Image = Global.GetResourceIcon(Resource.ResourceManager, "List")?.ToBitmap();
            btnDetailView.Image = Global.GetResourceIcon(Resource.ResourceManager, "Detail")?.ToBitmap();
            btnTileView.Image = Global.GetResourceIcon(Resource.ResourceManager, "Thumb")?.ToBitmap();

            //TreeAndViewTabを追加する
            treeAndViewTab = new();
            treeAndViewTab.Dock = DockStyle.Fill;
            panel.Controls.Add(treeAndViewTab);

            //設定ファイルを元にタブを追加する
            Setting.AppSettings.Instance.FileViews.ForEach( fileViewSetting =>
            {
                if(!System.IO.Directory.Exists(fileViewSetting.Path))
                {
                    return;
                }

                var newTreeView = treeAndViewTab.AddTab(fileViewSetting.Path);
                newTreeView.AfterSelect += (s, e) =>
                {
                    if(s is TreeAndView newView)
                        fileViewSetting.Path = newView.SelectedPath;
                };

            });

            //タブがない場合新規追加
            if (treeAndViewTab.TabCount == 0)
            {
                Setting.FileView newSetting = new()
                {
                    Path = System.IO.Directory.GetCurrentDirectory()
                };
                Setting.AppSettings.Instance.FileViews.Add(newSetting);

                var newTreeView = treeAndViewTab.AddTab(System.IO.Directory.GetCurrentDirectory());
                newTreeView.AfterSelect += (s, e) =>
                {
                    if (s is TreeAndView newView)
                        newSetting.Path = newView.SelectedPath;
                };

            }
            treeAndViewTab.SelectedIndex = 0;

            ////リストビュー表示タイプ切り替えボタン
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

            RefreshViewTypeBtnChecked();


            //各コントロールのフォントをSystem準拠にする
            PiViLityCore.Util.Forms.FormInitializeSystemTheme(this);

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
            btnLargeIconView.Checked = treeAndViewTab.CurrentTreeAndView.View== View.LargeIcon;
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
    }
}
