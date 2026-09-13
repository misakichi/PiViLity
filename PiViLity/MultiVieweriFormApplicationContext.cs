using PiViLityCore.Plugin;
using System;
using System.Collections.Generic;
using System.Text;

namespace PiViLity
{
    internal class MultiVieweriFormApplicationContext : ApplicationContext
    {
        int openForms_ = 0;

        public MultiVieweriFormApplicationContext(IEnumerable<string> filepaths)
        {
            List<Form> forms = new();
            foreach (string path in filepaths)
            {
                var form = PiViLityCore.Util.Forms.CreateFileOnView(path);
                if (form is not null)
                    forms.Add(form);
            }

            if (forms.Count == 0)
            {
                throw new ApplicationException("Cannot open files.");
            }

            openForms_= forms.Count;
            foreach (var form in forms)
            {
                form.FormClosed += Form_FormClosed;
                form.Show();
            }
        }

        private void Form_FormClosed(object? sender, FormClosedEventArgs e)
        {
            if (Interlocked.Decrement(ref openForms_) == 0)
                ExitThread();
        }
    }
}
