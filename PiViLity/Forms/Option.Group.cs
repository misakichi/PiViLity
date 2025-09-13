using PiViLityCore.Option;
using PiViLityCore.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Management.Deployment.Preview;

namespace PiViLity.Forms
{
    public partial class Option
    {
        /// <summary>
        /// 項目ごとのパネル
        /// </summary>
        private class OptionItemPanel : Panel
        {
            public required OptionItemAttribute OptionAttribute;
            public UInt64 UIOrder = UInt64.MaxValue;
            public Label? ItemNameLabel = null;
        }

        /// <summary>
        /// 不明型の列挙型オプション項目用
        /// </summary>
        private class EnunmOptionItem
        {
            public object Value;
            public string Text;
            public EnunmOptionItem(object value, string text)
            {
                Value = value;
                Text = text;
            }
            public override string ToString()
            {
                return Text;
            }
        }

        

        /// <summary>
        /// グループ１つを表すノード
        /// SttingBase継承クラス１つとイコールではない。複数のSettingBase継承が同一グループのこともある。
        /// </summary>
        internal class OptionGroup : TreeNode
        {
            public OptionGroup(string name)
            {
                Text = name;
            }

            //グループのUI表示順序
            public int GroupUIOrder = int.MaxValue;

            //グループパネル
            public Panel Panel = new();

            //オプション項目パネル
            private List<OptionItemPanel> Items = new();

            public void AddSetting(SettingBase setting)
            {
                var fields = setting.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetField);
                var properties = setting.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);


                GroupUIOrder = Math.Min(GroupUIOrder, setting.GroupUIOrder);

                //項目単位ではプラグイン→グループ→項目の順でソートする
                var pluginIndex = PluginManager.Instance.Plugins.FindIndex(plugin => plugin.settings.Contains(setting));
                if (setting.GetType().Assembly == typeof(App).Assembly || setting.GetType().Assembly == typeof(PiViLityCore.Global).Assembly)
                {
                    pluginIndex = 0;
                }
                else
                {
                    pluginIndex = Math.Max(1, pluginIndex + 1);
                }

                //フィールド、プロパティを列挙し、UIOrderに応じてソートしてから追加
                foreach (var field in fields)
                {
                    var panel = AddSettingItem(setting, field);
                    if(panel == null)
                        continue;
                    panel.UIOrder+= (UInt64)pluginIndex << 32;
                    Items.Add(panel);
                }
                foreach (var prop in properties)
                {
                    if (prop.CanWrite)
                    {
                        var panel = AddSettingItem(setting, prop);
                        if (panel == null)
                            continue;
                        panel.UIOrder+= (UInt64)pluginIndex << 32;
                        Items.Add(panel);
                    }
                }

            }
            /// <summary>
            /// 設定１要素のUIを生成し、パネルにまとめて返す
            /// 現在サポートしている型はstring,int,bool,Size
            /// </summary>
            /// <param name="res"></param>
            /// <param name="settingInstance"></param>
            /// <param name="member"></param>
            /// <returns></returns>
            /// <exception cref="HasMultipleOptionAttributeException"></exception>
            /// <exception cref="NotImplementedException"></exception>
            private OptionItemPanel? AddSettingItem(SettingBase settingInstance, MemberInfo member)
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
                        if (settingAttr.TextResouceId.Length > 0)
                            itemText = settingInstance.SettingResource.GetString(settingAttr.TextResouceId);
                        optionItemAttribute = settingAttr;
                    }
                }
                //表示対象外なら何もしない
                if (!isShow)
                    return null;
                //属性が無い場合はデフォルトの属性を生成
                if (optionItemAttribute == null)
                    optionItemAttribute = new OptionItemAttribute();

                var prop = member as PropertyInfo;
                var field = member as FieldInfo;
                Type? targetType = prop?.PropertyType ?? field?.FieldType ?? null;
                if (targetType == null)
                    return null;

                //生成する１項目を構成するコントロールを格納するリスト
                List<Control> addControls = new();
                void AddControlToPanel(Control control)
                {
                    control.PerformLayout();
                    control.Dock = DockStyle.Left;
                    addControls.Add(control);
                }

                Label? nameLabel = new Label { AutoSize = true, Text = itemText };
                AddControlToPanel(nameLabel);

                //設定項目の型に応じてコントロールを生成
                //string
                if (targetType == typeof(string))
                {
                    var strInput = new TextBox()
                    {
                        Text = PiViLityCore.Util.Member.GetClassValue<string>(settingInstance, member)
                    };
                    AddControlToPanel(strInput);
                    strInput.TextChanged += (s, e) =>
                    {
                        if (s is TextBox tb)
                        {
                            PiViLityCore.Util.Member.SetClassValue(settingInstance, member, tb.Text);
                        }
                    };
                }
                //int
                else if (targetType == typeof(int))
                {
                    var numInput = new NumericUpDown()
                    {
                        Value = PiViLityCore.Util.Member.GetValue<int>(settingInstance, member)
                    };
                    AddControlToPanel(numInput);
                    numInput.ValueChanged += (s, e) =>
                    {
                        if (s is NumericUpDown num)
                        {
                            PiViLityCore.Util.Member.SetValue(settingInstance, member, (int)num.Value);
                        }
                    };

                }
                //bool
                else if (targetType == typeof(bool))
                {
                    var checkBox = new CheckBox()
                    {
                        AutoSize = true,
                        Text = itemText,
                        Checked = PiViLityCore.Util.Member.GetValue<bool>(settingInstance, member)
                    };
                    nameLabel.Text = "";
                    AddControlToPanel(checkBox);
                    checkBox.CheckedChanged += (s, e) =>
                    {
                        if (s is CheckBox checkbox)
                        {
                            PiViLityCore.Util.Member.SetValue(settingInstance, member, checkbox.Checked);
                        }
                    };
                }
                //Size型
                else if (targetType == typeof(Size))
                {
                    var widthInput = new NumericUpDown();
                    var heightInput = new NumericUpDown();
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
                    Size value = PiViLityCore.Util.Member.GetValue(settingInstance, member);
                    void OnChangeSizeValue(object? sender, EventArgs e)
                    {
                        if (sender is NumericUpDown num)
                        {
                            PiViLityCore.Util.Member.SetValue(settingInstance, member, new Size((int)widthInput.Value, (int)heightInput.Value));
                        }
                    }
                    widthInput.Value = value.Width;
                    heightInput.Value = value.Height;
                    widthInput.ValueChanged += OnChangeSizeValue;
                    heightInput.ValueChanged += OnChangeSizeValue;
                }
                //列挙型
                else if (targetType.IsEnum)
                {
                    //列挙型の値と表示名のペアを生成
                    List<EnunmOptionItem> enumList = new();
                    foreach (var item in Enum.GetValues(targetType))
                    {
                        var enumMember = targetType.GetMember(item.ToString() ?? "") ?? Array.Empty<MemberInfo>();
                        if(enumMember.Length == 0)
                            continue;
                        var enumAttr = enumMember[0].GetCustomAttribute<OptionItemAttribute>();
                        string text = item.ToString() ?? "";
                        if (enumAttr!=null && enumAttr.TextResouceId!="")
                        {
                            text = settingInstance.SettingResource.GetString(enumAttr.TextResouceId);
                        }
                        enumList.Add(new EnunmOptionItem(item, text));
                    }
                    var currentValue = PiViLityCore.Util.Member.GetValueObject(settingInstance, member);

                    //コンボボックスを生成
                    var comboBox = new ComboBox()
                    {
                        DropDownStyle = ComboBoxStyle.DropDownList,
                    };
                    comboBox.Items.AddRange(enumList.ToArray());
                    //名称とコンボをパネルにまとめる
                    var selected = enumList.FindIndex(e => e.Value.Equals(currentValue));
                    comboBox.SelectedIndex = Math.Max(0,selected);
                    comboBox.SelectedIndexChanged += (s, e) =>
                    {
                        if (s is ComboBox cb)
                        {
                            if (cb.SelectedItem is EnunmOptionItem item)
                            {
                                PiViLityCore.Util.Member.SetValueObject(settingInstance, member, item.Value);
                            }
                        }
                    };

                    AddControlToPanel(comboBox);

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
                itemPanel.UIOrder = optionItemAttribute.UIOrder + (UInt32)(settingInstance.GroupUIOrder) << 16;
                return itemPanel;
            }

            /// <summary>
            /// 用意された項目パネルをグループパネルに追加し、表示順序を整える
            /// </summary>
            public void SettingPanelFinalize()
            {
                Panel.Dock = DockStyle.Top;
                Panel.PerformLayout();
                foreach (Control control in Panel.Controls)
                {
                    Panel.Height = Math.Max(Panel.Height, control.Bottom);
                }
                Items.Sort((a, b) => b.OptionAttribute.UIOrder - a.OptionAttribute.UIOrder);
                Panel.Controls.AddRange(Items.ToArray());

                setTabIndex(Panel);
                Panel.PerformLayout();

                //パネルの高さを調整
                int nameMaxWidth = 0;
                foreach (var panel in Items)
                {
                    nameMaxWidth = Math.Max(nameMaxWidth, panel.ItemNameLabel?.Width ?? 0);
                    Panel.Height = Math.Max(Panel.Height, panel.Bottom);
                }

                //名称ラベルの幅を揃える
                foreach (var panel in Items)
                {
                    if (panel.ItemNameLabel == null)
                        continue;
                    panel.ItemNameLabel.AutoSize = false;
                    panel.ItemNameLabel.Width = nameMaxWidth;
                }
            }

            /// <summary>
            /// タブインデックスを設定し順序を整える関数
            /// </summary>
            /// <param name="control"></param>
            private void setTabIndex(Control control)
            {
                int tabIndex = 10;
                control.TabIndex = tabIndex;
                tabIndex++;
                if (control is OptionItemPanel itemPanel)
                {
                    for (int i = (control.Controls.Count) - (1); i >= 0; i--)
                    {
                        setTabIndex(control.Controls[i]);
                    }
                }
                else
                {
                    foreach (Control child in control.Controls)
                    {
                        setTabIndex(child);
                    }
                }
            }
        }

    }
}
