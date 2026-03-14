using System;
using System.Collections.Generic;
using System.Text;

namespace PiViLityPlugin
{
    /// <summary>
    /// クラスにシングルトン機能を提供するベースクラス。
    /// </summary>
    /// <typeparam name="T">継承先の型</typeparam>
    public abstract class Singleton<T> : IDisposable where T : class, IDisposable, new()
    {
        private static int _checker = 0;
        protected Singleton() {
            if (_checker++ != 0)
            {
                throw new OverflowException();
            }
        }
        static T? _instance = null;
        public static void Create() { _instance = new T(); }
        public static void Release() { _instance?.Dispose(); _checker--; _instance = null; }
        public abstract void Dispose();

        public static T Instance { get => _instance!; }
    }

}
