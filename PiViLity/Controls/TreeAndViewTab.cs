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

namespace PiViLity.Controls
{
    public partial class TreeAndViewTab : UserControl
    {
        public event EventHandler? SelectedIndexChanged;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TreeAndViewTab()
        {
            InitializeComponent();


            //各コントロールのフォントをSystem準拠にする
            PiViLityCore.Util.Forms.FormInitializeSystemTheme(this);

             tabView.SelectedIndexChanged += TabView_SelectedIndexChanged;
        }

#if true
        /// <summary>
        /// タブにTreeAndViewを追加します。
        /// </summary>
        /// <param name="path"></param>
        public TreeAndView AddTab(string path)
        {
            TabPage tabPage = new TabPage(Path.GetFileName(path));
            
            tabView.TabPages.Add(tabPage);
            //タブページへTreeAndViewを登録
            var newView = new TreeAndView();
            newView.Dock = DockStyle.Fill;
            newView.Initialize(path);
            newView.DirectoryChanged += (s, e) =>
            {
                tabPage.Text = newView.SelectedText;
            };
            tabPage.Text = newView.SelectedText;
            tabPage.Controls.Add(newView);
            tabPage.Tag = newView;
            newView.Size = tabPage.ClientSize;

            return newView;
        }

        /// <summary>
        /// タブ個数を返します
        /// </summary>
        public int TabCount => tabView.TabCount;

        public TreeAndView? GetTab(int index)
        {
            if (index < 0 || index >= tabView.TabCount)
                return null;
            return tabView.TabPages[index].Tag as TreeAndView;
        }

        /// <summary>
        /// 選択されたタブのインデックスを取得または設定します。
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedIndex
        {
            get => tabView.SelectedIndex;
            set => tabView.SelectedIndex = value;
        }

        /// <summary>
        /// タブの選択が変更されたときに発生します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabView_SelectedIndexChanged(object? sender, EventArgs e)
        {
            SelectedIndexChanged?.Invoke(this, e);
        }

        /// <summary>
        /// 現在選択されているTreeAndViewを取得します。
        /// </summary>
        public TreeAndView? CurrentTreeAndView
        {
            get => tabView.SelectedTab?.Tag as TreeAndView;
        }
#endif
    }
}
