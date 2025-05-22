using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PiViLityCore.Plugin
{
    public class ShorcutTrigger
    {
        public Keys Key { get; set; } = Keys.None;
        public Keys Modifiers { get => Key & (Keys.Modifiers); }
        public Keys KeyCode { get => Key & (Keys.KeyCode); }
        public bool Shift { get => (Modifiers & Keys.Shift) == Keys.Shift; }
        public bool Control { get => (Modifiers & Keys.Control) == Keys.Control; }
        public bool Alt { get => (Modifiers & Keys.Alt) == Keys.Alt; }

        public string MethodName { get; set; } = string.Empty;

        public MethodInfo? MethodInfo { get; set; } = null;
    }

    public interface IShotcutCommandSupport
    {
        string TargetName { get; }

        List<ShorcutTrigger> ShortCutTriggers { get; set; }

        public void ResolveCommandMethods()
        {
            var type = GetType();
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute(typeof(ShortCutCommand), true);
                if (attr != null)
                {
                    var commandAttr = (ShortCutCommand)attr;
                    foreach (var item in ShortCutTriggers.Where(x => x.MethodName == method.Name))
                    {
                        item.MethodInfo = method;
                    }
                }
            }
        }

        void OnKeyDown(KeyEventArgs e)
        {
            foreach (var item in ShortCutTriggers)
            {
                if (item.KeyCode == e.KeyCode)
                {
                    if (item.Modifiers==Keys.None || item.Modifiers == e.Modifiers)
                    {
                        item.MethodInfo?.Invoke(this, null);
                        break;
                    }
                }
            }
        }

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ShortCutCommand : Attribute
    {
        public string NameText = string.Empty;
        public string NameResId = string.Empty;

    }
}
