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
using System.Resources;
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



        static void SetValueObject(object instance, MemberInfo member, object value)
        {
            if (member is PropertyInfo prop)
            {
                prop.SetValue(instance, value);
            }
            else if (member is FieldInfo field)
            {
                field.SetValue(instance, value);
            }
        }
        static object? GetValueObject(object instance, MemberInfo member)
        {
            if (member is PropertyInfo prop)
            {
                return prop.GetValue(instance);
            }
            else if (member is FieldInfo field)
            {
                return field.GetValue(instance);
            }
            return null;
        }

        static void SetValue(object instance, MemberInfo member, Size size) => SetValueObject(instance, member, size);

        static Size GetValue(object instance, MemberInfo member)
        {
            object? obj = GetValueObject(instance, member);
            if (obj is Size size)
            {
                return size;
            }
            else
            {
                return new();
            }
        }


        static void SetValue<T>(object instance, MemberInfo member, T value) where T : struct => SetValueObject(instance, member, value);
        static void SetClassValue<T>(object instance, MemberInfo member, T value) where T : class => SetValueObject(instance, member, value);


        static T GetValue<T>(object instance, MemberInfo member) where T : struct
        {
            object? obj = GetValueObject(instance, member);
            if (obj is T value)
            {
                return value;
            }
            else
            {
                return default;
            }
        }

        static T GetClassValue<T>(object instance, MemberInfo member) where T : class
        {
            object? obj = GetValueObject(instance, member);
            if (obj is T value)
            {
                return value;
            }
            else
            {
                if (typeof(T) == typeof(string))
                {
                    return (T)(object)string.Empty;
                }
                return (T)Activator.CreateInstance(typeof(T))!;
            }
        }


        private class OptionItemPanel : Panel
        {
            public required OptionItemAttribute OptionAttribute;
        }
        OptionItemPanel? AddSettingItem(ResourceManager? res, SettingBase settingInstance, MemberInfo member)
        {
            bool isShow = false;
            string itemText = member.Name;

            OptionItemAttribute? optionItemAttribute = null;
            var attrs = Attribute.GetCustomAttributes(member).Where(attr => attr is OptionItemAttribute);

            if (attrs.Count() > 1)
                throw new HasMultipleOptionAttributeException("Has multiple OptionItemAttribute.");

            foreach (var attr in attrs)
            {
                //表示対象に対して項目セットアップ
                if (attr is OptionItemAttribute settingAttr)
                {
                    isShow |= settingAttr.NoOption == false;
                    if (res != null && settingAttr.NameTextResouceId.Length > 0)
                        itemText = Global.GetResourceString(res, settingAttr.NameTextResouceId);
                    optionItemAttribute = settingAttr;
                }
            }
            if (!isShow)
                return null;
            if (optionItemAttribute == null)
                optionItemAttribute = new OptionItemAttribute();

            var prop = member as PropertyInfo;
            var field = member as FieldInfo;
            Type? targetType = prop?.PropertyType ?? field?.FieldType ?? null;
            if (targetType == null)
                return null;

            List<Control> addControls = new();
            void AddControlToPanel(Control control)
            {
                control.PerformLayout();
                control.Dock = DockStyle.Left;
                addControls.Add(control);
            }

            //設定項目の型に応じてコントロールを生成
            if (targetType == typeof(string))
            {
                var strInput = new TextBox()
                {
                    Text = GetClassValue<string>(settingInstance, member)
                };
                AddControlToPanel(new Label { AutoSize = true, Text = itemText });
                AddControlToPanel(strInput);
                strInput.TextChanged += (s, e) =>
                {
                    if (s is TextBox tb)
                    {
                        SetClassValue(settingInstance, member, tb.Text);
                    }
                };
            }
            else if (targetType == typeof(int))
            {
                var numInput = new NumericUpDown()
                {
                    Value = GetValue<int>(settingInstance, member)
                };
                AddControlToPanel(new Label { AutoSize = true, Text = itemText });
                AddControlToPanel(numInput);
                numInput.ValueChanged += (s, e) =>
                {
                    if (s is NumericUpDown num)
                    {
                        SetValue(settingInstance, member, (int)num.Value);
                    }
                };

            }
            else if (targetType == typeof(bool))
            {
                var checkBox = new CheckBox()
                {
                    AutoSize = true,
                    Text = itemText,
                    Checked = GetValue<bool>(settingInstance, member)
                };
                AddControlToPanel(checkBox);
                checkBox.CheckedChanged += (s, e) =>
                {
                    if (s is CheckBox checkbox)
                    {
                        SetValue(settingInstance, member, checkbox.Checked);
                    }
                };
            }
            else if (targetType == typeof(Size))
            {

                var widthInput = new NumericUpDown();
                var heightInput = new NumericUpDown();
                AddControlToPanel(new Label { AutoSize = true, Text = itemText });
                AddControlToPanel(widthInput);
                AddControlToPanel(new Label { AutoSize = true, Text = "x" });
                AddControlToPanel(heightInput);
                foreach (var attr in attrs)
                {
                    if (attr is OptionItemSizeAttribute sizeAttr)
                    {
                        widthInput.Minimum = sizeAttr.MinWidth;
                        widthInput.Maximum = sizeAttr.MaxWidth;
                        heightInput.Minimum = sizeAttr.MinHeight;
                        heightInput.Maximum = sizeAttr.MaxHeight;
                    }
                }
                Size value = GetValue(settingInstance, member);
                void OnChangeSizeValue(object? sender, EventArgs e)
                {
                    if (sender is NumericUpDown num)
                    {
                        SetValue(settingInstance, member, new Size((int)widthInput.Value, (int)heightInput.Value));
                    }
                }
                widthInput.Value = value.Width;
                heightInput.Value = value.Height;
                widthInput.ValueChanged += OnChangeSizeValue;
                heightInput.ValueChanged += OnChangeSizeValue;
            }
            else
            {
                throw new NotImplementedException($"Is not implements type of option item member.({targetType.Name})");
            }

            //項目に必要なコントロールをまとめたパネルを生成
            var itemPanel = new OptionItemPanel()
            {
                OptionAttribute = optionItemAttribute,
                Size = new Size(0, 0),
                Dock = DockStyle.Top,
                Padding = new Padding(ItemUiPadding),

            };
            //高さを先に確保しておく必要あり
            int height = itemPanel.Height;
            foreach (var control in addControls)
            {
                height = Math.Max(height, control.Bottom);
            }
            itemPanel.Height = height + ItemUiPadding * 2;
            addControls.Reverse();
            foreach (var control in addControls)
            {
                itemPanel.Controls.Add(control);
            }
            itemPanel.PerformLayout();
            return itemPanel;

        }

        private Control CreateSettingPanel(ResourceManager? res, SettingBase setting)
        {
            var settingPanel = new Panel();
            settingPanel.Dock = DockStyle.Top;
            var fields = setting.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetField);
            var properties = setting.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);

#if true
            settingPanel.PerformLayout();
            List<OptionItemPanel> panels = new();
            foreach (var field in fields)
            {
                var panel = AddSettingItem(res, setting, field);
                if (panel != null)
                    panels.Add(panel);
            }
            foreach (var prop in properties)
            {
                if (prop.CanWrite)
                {
                    var panel = AddSettingItem(res, setting, prop);
                    if (panel != null)
                        panels.Add(panel);
                }
            }
            panels.Sort((a, b) => b.OptionAttribute.UIOrder - a.OptionAttribute.UIOrder);
            settingPanel.Controls.AddRange(panels.ToArray());
            settingPanel.PerformLayout();
            foreach (var panel in panels)
            {
                settingPanel.Height = Math.Max(settingPanel.Height, panel.Bottom);
            }
            //settingPanel.Width = Math.Max(itemPanel.Width, itemPanel.Width);
#endif

            return settingPanel;
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
            PluginManager.Instance.Plugins.ForEach(plugin =>
            {
                var moduleItem = new TreeNode(plugin.information.OptionItemName);
                var assemName = plugin.assembly.GetName();
                string moduleInfoText = plugin.information.Description;
                if (assemName != null)
                {
                    moduleInfoText = $"Module:{assemName.Name}\nVersion:{assemName.Version?.Major ?? 0}.{assemName.Version?.Minor ?? 0}.{assemName.Version?.Revision ?? 0}.{assemName.Version?.Build ?? 0}\n";
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
                moduleNodes.Add(moduleItem);

                //プラグイン中の設定情報を捜査
                plugin.settings.ForEach(setting =>
                {
                    var attrs = Attribute.GetCustomAttributes(setting.GetType());
                    foreach (var attr in attrs.Where(attr => attr is OptionAttribute))
                    {
                        //表示対象に対して項目セットアップ
                        if (attr is OptionAttribute settingAttr)
                        {
                            if (settingAttr.NoOption == false)
                            {
                                string name = setting.CategoryText;
                                var settingItem = new TreeNode(name);
                                settingItem.Tag = CreateSettingPanel(plugin.information.ResourceManager, setting);

                                if (plugin.information.OptionItemName == "")
                                {
                                    tvOptions.Nodes.Add(settingItem);
                                }
                                else
                                {
                                    moduleItem.Nodes.Add(settingItem);
                                }
                            }
                        }
                    }
                });
                moduleItem.ExpandAll();

                tvOptions.AfterSelect += (s, e) =>
                {
                    var tag = e?.Node?.Tag;
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
            tvOptions.Nodes.AddRange(moduleNodes.ToArray());

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
