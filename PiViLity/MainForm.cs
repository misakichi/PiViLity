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
            //TreeAndViewTab��ǉ�����
            treeAndViewTab = new();
            treeAndViewTab.Dock = DockStyle.Fill;
            panel.Controls.Add(treeAndViewTab);

            items.Add(btnSmallIconView);
            items.Add(btnLargeIconView);
            items.Add(btnListView);
            items.Add(btnDetailView);
            items.Add(btnTileView);

            toolStrip.Items.AddRange(items.ToArray());

            btnSmallIconView.Click += (s, e) =>
            {
                if (treeAndViewTab.CurrentTreeAndView != null)
                {
                    treeAndViewTab.CurrentTreeAndView.View = View.SmallIcon;
                    RefreshViewTypeBtnChecked();
                }
            };
            btnLargeIconView.Click += (s, e) =>
            {
                if (treeAndViewTab.CurrentTreeAndView != null)
                {
                    treeAndViewTab.CurrentTreeAndView.View = View.LargeIcon;
                    RefreshViewTypeBtnChecked();
                }
            };
            btnListView.Click += (s, e) =>
            {
                if (treeAndViewTab.CurrentTreeAndView != null)
                {
                    treeAndViewTab.CurrentTreeAndView.View = View.List;
                    RefreshViewTypeBtnChecked();
                }
            };
            btnTileView.Click += (s, e) =>
            {
                if (treeAndViewTab.CurrentTreeAndView != null)
                {
                    treeAndViewTab.CurrentTreeAndView.View = View.Tile;
                    RefreshViewTypeBtnChecked();
                }
            };

            btnDetailView.Click += (s, e) =>
            {
                if (treeAndViewTab.CurrentTreeAndView != null)
                {
                    treeAndViewTab.CurrentTreeAndView.View = View.Details;
                    RefreshViewTypeBtnChecked();
                }
            };

            btnTileView.Checked = true;
            treeAndViewTab.TabIndexChanged += TreeAndViewTab_TabIndexChanged;

            //�e�R���g���[���̃t�H���g��System�����ɂ���
            PiViLityCore.Util.Forms.FormInitializeSystemTheme(this);

        }

        /// <summary lang="ja-JP">
        /// ���X�g�r���[�\���^�C�v�؂�ւ��{�^��������ɍ��킹��
        /// </summary>
        private void RefreshViewTypeBtnChecked()
        {
            if (treeAndViewTab.CurrentTreeAndView == null)
                return;

            btnListView.Checked = treeAndViewTab.CurrentTreeAndView.View == View.List;
            btnSmallIconView.Checked = treeAndViewTab.CurrentTreeAndView.View == View.SmallIcon;
            btnLargeIconView.Checked = treeAndViewTab.CurrentTreeAndView.View== View.LargeIcon;
            btnTileView.Checked = treeAndViewTab.CurrentTreeAndView.View == View.Tile;
            btnDetailView.Checked = treeAndViewTab.CurrentTreeAndView.View == View.Details;

        }

        /// <summary lang="ja-JP">
        /// �^�u���؂�ւ�����Ƃ��̓���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeAndViewTab_TabIndexChanged(object? sender, EventArgs e)
        {
            RefreshViewTypeBtnChecked();

        }
    }
}
