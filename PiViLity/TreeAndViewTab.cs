using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PiViLity
{
    public partial class TreeAndViewTab : UserControl
    {
        public event EventHandler? SelectedIndexChanged;

        public TreeAndViewTab()
        {
            InitializeComponent();


            //各コントロールのフォントをSystem準拠にする
            PiViLityCore.Util.Forms.FormInitializeSystemTheme(this);

            {
                //初期タブ
                TabPage tabPage = new TabPage("<PC>");
                tabView.TabPages.Add(tabPage);
                //タブページへTreeAndViewを登録
                var newView = new TreeAndView();
                newView.Dock = DockStyle.Fill;
                newView.dirTreeViewMgr.AfterSelect += (s, e) =>
                {
                    tabPage.Text = e.dirTreeNode?.Name ?? "";
                };
                tabPage.Text = newView.SelectedName;
                tabPage.Controls.Add(newView);
                tabPage.Tag = newView;
            }
            //test
            {
                //初期タブ
                TabPage tabPage = new TabPage("<PC>");
                tabView.TabPages.Add(tabPage);
                //タブページへTreeAndViewを登録
                var newView = new TreeAndView();
                newView.Dock = DockStyle.Fill;
                newView.dirTreeViewMgr.AfterSelect += (s, e) =>
                {
                    tabPage.Text = e.dirTreeNode?.Name ?? "";
                };
                tabPage.Text = newView.SelectedName;
                tabPage.Controls.Add(newView);
                tabPage.Tag = newView;
            }

            tabView.SelectedIndexChanged += TabView_SelectedIndexChanged;
        }

        private void TabView_SelectedIndexChanged(object? sender, EventArgs e)
        {
            SelectedIndexChanged?.Invoke(this, e);
        }

        public TreeAndView? CurrentTreeAndView
        {
            get => tabView.SelectedTab?.Tag as TreeAndView;
        }
    }
}
