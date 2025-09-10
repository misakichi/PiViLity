using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Plugin
{
    public interface IReader : IDisposable
    {
        /// <summary>
        /// このプラグインがサポートする拡張子を返します。
        /// </summary>
        /// <returns></returns>
        public List<string> GetSupportedExtensions();

        /// <summary>
        /// このプラグインが指定したファイルをサポートするかどうかを返します。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool IsSupported();

        /// <summary>
        /// 画像ファイルのパスを設定します。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool SetFilePath(string filePath);

    }
}
