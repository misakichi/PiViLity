using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace PiViLity.COM
{
    internal class SusiePluginCom : IDisposable
    {
        //static readonly Guid comCLSID = new ("35841F94-BF72-4EA7-92C0-439BF050556D");
        static readonly Guid comCLSID = new ("6FD27283-665F-47F8-A627-CEBF40C0B95D");
        dynamic? _com;

        public SusiePluginCom()
        {
            // CLSID から Type を取得
            if (Type.GetTypeFromCLSID(comCLSID) is Type type)
            {
                _com = Activator.CreateInstance(type);
            }

            if (_com is null)
                new System.Runtime.InteropServices.COMException();

        }

        public bool Load(string pluginPath)
        {
            if(!File.Exists(pluginPath)) 
                return false;
            if(_com is null)
                return false;

            return _com.Load(pluginPath) == 0;
        }

        public void Dispose()
        {
            if(_com is not null)
                Marshal.ReleaseComObject(_com);
            _com = null;
        }
    }
}
