using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Controls
{
    public class PictureBox : System.Windows.Forms.PictureBox
    {
        public PictureBox()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
        }
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case Windows.Message.WM_MOUSEHOWHEEL:
                    {
                        int delta = (short)((m.WParam >> 16) & 0xffff);
                        MouseEventArgs e = new MouseEventArgs(MouseButtons.None, 0, (short)(m.LParam&0xffff), (short)((m.LParam>>16) & 0xffff), delta);
                        MouseHWheel?.Invoke(this, e);
                        break;
                    }
                default:
                    break;
            }
        
            base.WndProc(ref m);
        }

        public event MouseEventHandler? MouseHWheel;
    }
}
