using PiViLityCore;
using PiViLityCore.Option;
using PiViLityCore.Plugin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Management.Deployment.Preview;
using Windows.UI.ApplicationSettings;

namespace PiViLity.Forms
{
    public partial class Option : Form
    {
        //項目ごとの隙間
        private const int ItemUiPadding = 10;

        public Option()
        {
            InitializeComponent();
        }


        MemoryStream settingBackup = new();

        private void CancelOption()
        {
            settingBackup.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(settingBackup);
            PluginManager.Instance.LoadSettings(reader);
            settingBackup.Position = 0;
        }

        private void Option_Load(object sender, EventArgs e)
        {
            var writer = new StreamWriter(settingBackup);
            settingBackup.Seek(0, SeekOrigin.Begin);
            PluginManager.Instance.SaveSettings(writer);
            writer.Flush();
            List<TreeNode> moduleNodes = new();
            string moduleInfoText = "";

            PluginManager.Instance.Plugins.ForEach(plugin =>
            {
                var assemName = plugin.assembly.GetName();
                moduleInfoText += plugin.information.Description;
                if (assemName != null)
                {
                    moduleInfoText +=
                    $"Module:{assemName.Name}\n" +
                    $"Version:{assemName.Version?.Major ?? 0}.{assemName.Version?.Minor ?? 0}.{assemName.Version?.Revision ?? 0}.{assemName.Version?.Build ?? 0}\n" +
                    $"Company:{plugin.assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? ""}\n" +
                    $"Copyright:{plugin.assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? ""}\n" +
                    $"Description:{plugin.information.Description}\n\n";

                }

            //カテゴリ名と親カテゴリ毎にグループ化
            
                Dictionary<(string Name, Type Type, Type? Parent), OptionGroup> appSettingGroup = new();
                Dictionary<(string Name, Type Type, Type? Parent), OptionGroup> pluginSsettingGroup = new();

                //プラグイン中の設定情報を捜査
                plugin.settings.ForEach(setting =>
                {
                    var settingAsm = setting.GetType().Assembly;
                    var settingGroup = settingAsm==typeof(PiViLityCore.Global).Assembly || settingAsm==typeof(App).Assembly
                        ? appSettingGroup
                        : pluginSsettingGroup;
                    var attrs = Attribute.GetCustomAttributes(setting.GetType());
                    foreach (var attr in attrs.Where(attr => attr is OptionAttribute))
                    {
                        //表示対象に対して項目セットアップ
                        if (attr is OptionAttribute optAttr)
                        {
                            if (optAttr.NoOption == false)
                            {
                                var parentType = PiViLityCore.Util.Types.HasParentType(optAttr.ParentType, typeof(SettingBase)) ? optAttr.ParentType : null;
                                var key = (setting.CategoryName, setting.GetType(), optAttr.ParentType);
                                string name = setting.CategoryName;
                                string text = setting.CategoryText;
                                if(!settingGroup.TryGetValue(key, out var optionGroup))
                                {
                                    optionGroup = new OptionGroup(text);
                                    settingGroup[key] = optionGroup;
                                }
                                optionGroup.AddSetting(setting);
                            }
                        }
                    }
                });

                void AddGroupToNode(Dictionary<(string Name, Type Type, Type? Parent), OptionGroup> _groups)
                {
                    var groups = _groups.ToList();
                    groups.Sort((a, b) => a.Value.GroupUIOrder- b.Value.GroupUIOrder);
                    foreach (var group in groups)
                    {
                        //パネル内を完成させる
                        group.Value.SettingPanelFinalize();

                        //親カテゴリがある場合は親カテゴリに追加
                        if (group.Key.Parent != null)
                        {
                            if( groups.Find(g => g.Key.Type == group.Key.Parent) is var parentGroup)
                            {

                                parentGroup.Value.Nodes.Add(group.Value);
                                continue;
                            }
                        }
                        //親カテゴリがない場合はルートに追加                        
                        moduleNodes.Add(group.Value);
                    }
                }
                AddGroupToNode(appSettingGroup);
                AddGroupToNode(pluginSsettingGroup);

            });
            tvOptions.AfterSelect += (s, e) =>
            {
                var tag = e?.Node?.Tag;
                if (e?.Node?.Tag is Label label)
                {
                    pnlContent.AutoScroll = true;
                    pnlContent.Controls.Clear();
                    pnlContent.Controls.Add(label);
                }
                else if (e?.Node is OptionGroup group)
                {
                    pnlContent.AutoScroll = true;
                    pnlContent.Controls.Clear();
                    pnlContent.Controls.Add(group.Panel);
                }
            };
            //モジュール情報ノード追加
            var moduleInfoLabel = new Label()
            {
                Text = moduleInfoText,
                AutoSize = true,
                Dock = DockStyle.None,
                Padding = new Padding(10),
            };
            moduleInfoLabel.PerformLayout();
            var labelSize = moduleInfoLabel.Size;
            //moduleInfoLabel.AutoSize = false;
            moduleInfoLabel.Size = labelSize;
            var moduleInfoNode = new TreeNode("Module Information")
            {
                Tag = moduleInfoLabel,
            };
            moduleNodes.Add(moduleInfoNode);



            tvOptions.Nodes.AddRange(moduleNodes.ToArray());
            tvOptions.ExpandAll();


        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            PiViLityCore.Event.Option.UpdateSettings();
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void Option_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(DialogResult == DialogResult.Cancel)
            {
                CancelOption();
            }
        }
    }
}
