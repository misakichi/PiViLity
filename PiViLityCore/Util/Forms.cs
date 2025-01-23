using PiViLityCore.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Util
{
    static public class Forms
    {
        public static void CurrentFirstTraversalControl(Control control, Action<Control, int> action)
        {
            Queue<Tuple<Control, int>> stack = new();
            stack.Enqueue(new Tuple<Control, int>(control, 0));

            Tuple<Control, int>? cur;
            while (stack.TryDequeue(out cur))
            {
                action(cur.Item1, cur.Item2);
                foreach (Control child in cur.Item1.Controls)
                    stack.Enqueue(new Tuple<Control, int>(child, cur.Item2 + 1));

            }
        }

        public static void FormInitializeSystemTheme(Control control)
        {
            CurrentFirstTraversalControl(control, (c, d) =>
            {
                if (c != null)
                {
                    control.Font = SystemFonts.MessageBoxFont;
                    //if (ShelAPIHelper.SystemColor.IsDarkMode())
                    //{
                    //    control.BackColor = Color.FromArgb(255, 32, 32, 32);
                    //    control.ForeColor = Color.FromArgb(255, 0xCC, 0xCC, 0xCC);
                    //}
                    //else
                    //{
                    //    control.BackColor = SystemColors.Control;
                    //    control.ForeColor = SystemColors.ControlText;
                    //}
                }
            });
        }
    }
}
