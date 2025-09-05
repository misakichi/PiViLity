using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Event
{
    public static class Option
    {
        public static EventHandler<EventArgs> ApplySettings = delegate{ };

        public static void UpdateSettings()
        {
            ApplySettings?.Invoke(null, EventArgs.Empty);
        }
    }
}
