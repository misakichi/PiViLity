using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

        public MethodInfo? MethodInfo { get; set; } = null;
    }

    public interface IShotcutCommandSupport
    {
        string TargetName { get; }

        public static List<ShorcutTrigger> ShortCutTriggers { get; set; } = new();

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ShortCutCommand : Attribute
    {
        string NameText { get; } = "";
        string NameResId { get; } = "";
    }
}
