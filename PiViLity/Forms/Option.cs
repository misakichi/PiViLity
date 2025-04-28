using PiViLityCore;
using PiViLityCore.Plugin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Management.Deployment.Preview;

namespace PiViLity.Forms
{
    public partial class Option : Form
    {
        public Option()
        {
            InitializeComponent();
        }

        private Control CreateSettingPanel(ResourceManager? res, SettingBase setting)
        {
            var settingPanel = new Panel();
            var fields = setting.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetField);
            var properties = setting.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
            void AddSettingItem(MemberInfo member)
            {
                bool isShow = false;
                string itemText = member.Name;
                var attrs = Attribute.GetCustomAttributes(member);
                foreach (var attr in attrs.Where(attr => attr is SettingAttribute))
                {
                    //表示対象に対して項目セットアップ
                    if (attr is SettingAttribute settingAttr)
                    {
                        isShow |= settingAttr.NoOption == false;
                        if(res!=null)
                            itemText = Global.GetResourceString(res, settingAttr.NameTextResouceId);
                    }
                }
                var prop = member as PropertyInfo;
                var field = member as FieldInfo;
                Type? targetType = prop?.PropertyType ?? field?.FieldType ?? null;
                if (targetType == null)
                    return;

                if (targetType == typeof(string))
                {
                    var textBox = new TextBox
                    {
                        Dock = DockStyle.Top,
                        Padding = new Padding(5),
                        Margin = new Padding(5)
                    };
                    settingPanel.Controls.Add(textBox);
                }
                else if (targetType == typeof(int))
                {
                    var numericUpDown = new NumericUpDown
                    {
                        Dock = DockStyle.Top,
                        Padding = new Padding(5),
                        Margin = new Padding(5)
                    };
                    settingPanel.Controls.Add(numericUpDown);
                }
                else if (targetType == typeof(bool))
                {
                    var checkBox = new CheckBox
                    {
                        Dock = DockStyle.Top,
                        Padding = new Padding(5),
                        Margin = new Padding(5)
                    };
                    settingPanel.Controls.Add(checkBox);
                }else if (targetType == typeof(Size))
                {
                    var widthInput = new NumericUpDown();
                    var heightInput = new NumericUpDown();
                    var sizePanel = new FlowLayoutPanel();
                    sizePanel.AutoSize= true;
                    sizePanel.WrapContents = false;
                    sizePanel.FlowDirection= FlowDirection.LeftToRight;
                    sizePanel.Controls.Add(new Label { Text = itemText });
                    sizePanel.Controls.Add(widthInput);
                    sizePanel.Controls.Add(new Label{ Text = "x" });
                    sizePanel.Controls.Add(heightInput);
                    sizePanel.PerformLayout();



                    settingPanel.Controls.Add(sizePanel);
                    settingPanel.Width=Math.Max(settingPanel.Width, sizePanel.Width);
                    settingPanel.PerformLayout();
                }


            }
            foreach (var field in fields)
            {
                AddSettingItem(field);
            }
            foreach (var prop in properties)
            {
                if(prop.CanWrite)
                {
                    AddSettingItem(prop);
                }
            }


            return settingPanel;
        }

        private void Option_Load(object sender, EventArgs e)
        {
            PluginManager.Instance.Plugins.ForEach(plugin =>
            {
                var moduleItem = new TreeNode(plugin.information.OptionItemName);
                var assemName = plugin.assembly.GetName();
                string moduleInfoText = plugin.information.Description;
                if (assemName != null) {
                    moduleInfoText = $"Module:{assemName.Name}\nVersion:{assemName.Version?.Major??0}.{assemName.Version?.Minor ?? 0}.{assemName.Version?.Revision ?? 0}.{assemName.Version?.Build ?? 0}\n";
                }
                var moduleInfo = new Label
                {
                    Text = moduleInfoText,
                    AutoSize = true,
                    Dock = DockStyle.Top,
                    Padding = new Padding(5),
                    Margin = new Padding(5)
                };

                moduleItem.Tag = moduleInfo;
                tvOptions.Nodes.Add(moduleItem);

                //プラグイン中の設定情報を捜査
                plugin.settings.ForEach(setting =>
                {
                    var attrs = Attribute.GetCustomAttributes(setting.GetType());
                    foreach (var attr in attrs.Where(attr => attr is SettingAttribute))
                    {
                        //表示対象に対して項目セットアップ
                        if(attr is SettingAttribute settingAttr)
                        {
                            if (settingAttr.NoOption == false)
                            {
                                string name = setting.Name;
                                if (settingAttr.NameTextResouceId != "" && plugin.information.ResourceManager != null)
                                {
                                    name = Global.GetResourceString(plugin.information.ResourceManager, settingAttr.NameTextResouceId);
                                }

                                var settingItem = new TreeNode(name);
                                settingItem.Tag = CreateSettingPanel(plugin.information.ResourceManager, setting);
                                moduleItem.Nodes.Add(settingItem);
                            }
                        }
                    }
                });

                tvOptions.AfterSelect += (s, e) =>
                {
                    var tag =e?.Node?.Tag;
                    if (tag is Label label)
                    {
                        pnlContent.Controls.Clear();
                        pnlContent.Controls.Add(label);
                    }
                    else if (tag is Panel panel)
                    {
                        pnlContent.Controls.Clear();
                        pnlContent.Controls.Add(panel);
                    }
                };
            });

        }
    }
}
