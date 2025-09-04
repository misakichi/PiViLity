using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Identity.Core;
using Windows.UI.WebUI;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
namespace PiViLity
{
    /// <summary>
    /// サムネイル画像のキャッシュ
    /// アルファ付はRgbのJpegとAlphaのPngを組み合わせて保存する
    /// </summary>
    internal class ThumbnailCache : IDisposable
    {

        private static ThumbnailCache _incetance = new();
        private const string DbName = "PVilityIconThumbnailCache.db";

        public static ThumbnailCache Instance => _incetance;
        private string _dbFilename = "";
        private SqliteConnection? appDb;

        const string memoryName = "PiVilityIconDbRef";
        const string mutexName = "PiVilityIconDbRefMutex";

        private static Mutex? mutex = new(false, mutexName);
        private static System.IO.MemoryMappedFiles.MemoryMappedFile? mmfDbRef = System.IO.MemoryMappedFiles.MemoryMappedFile.CreateOrOpen(memoryName, 4, System.IO.MemoryMappedFiles.MemoryMappedFileAccess.ReadWrite);

        public void Dispose()
        {
            appDb?.Dispose();
            appDb = null;
            mmfDbRef?.Dispose();
            mmfDbRef = null;
            mutex?.Dispose();
            mutex = null;
        }
        public static void Initialize(string dbFilename)
        {
            _incetance._Initialize(dbFilename);
        }
        public static void Terminate()
        {
            _incetance.SaveDb(_incetance._dbFilename);
            _incetance._terminate();
            _incetance.Dispose();
        }
        private SqliteConnection openDb()
        {
            var db = new SqliteConnection($"Data Source=File:{DbName}?Mode=Memory&Cache=Shared");
            db.Open();
            return db;
        }

        System.Threading.Thread? transactionThread = null;
        System.Threading.AutoResetEvent transactionEvent = new(false);
        System.Threading.Mutex? transactionCommandMutex = new(false);
        Queue<Action<SqliteConnection,SqliteCommand>> transactionCommandQueue = new();
        bool transactionExit = false;
        void TransactionoProc()
        {
            using var db = openDb();
            while (!transactionExit)
            {

                //コマンドがあれば処理する
                transactionEvent.WaitOne(1000);


                while (true)
                {
                    //キューがあれば取得する
                    Action<SqliteConnection,SqliteCommand>? cmd = null;
                    transactionCommandMutex?.WaitOne();
                    if (transactionCommandQueue.Count > 0)
                    {
                        cmd = transactionCommandQueue.Dequeue();
                    }
                    transactionCommandMutex?.ReleaseMutex();

                    if (cmd == null)
                        break;

                    using (var sqlCmd = db.CreateCommand())
                        cmd.Invoke(db, sqlCmd);
                }
                Thread.Sleep(200);

            }
        }

        /// <summary>
        /// トランザクションスレッドへのコマンドアクション追加
        /// </summary>
        /// <param name="cmd">トランザクションスレッドによる実行を行う処理</param>
        void AddTransactionCommand(Action<SqliteConnection,SqliteCommand> cmd)
        {
            transactionCommandMutex?.WaitOne();
            transactionCommandQueue.Enqueue(cmd);
            transactionCommandMutex?.ReleaseMutex();
            transactionEvent.Set();
        }

        private void _Initialize(string dbFilename)
        {
            if (appDb != null)
                return;


            appDb = openDb();
            using (SqliteConnection db = openDb())
            {
                if (mutex != null)
                {
                    mutex.WaitOne();
                    using (var view = mmfDbRef?.CreateViewAccessor(0, 4, System.IO.MemoryMappedFiles.MemoryMappedFileAccess.ReadWrite))
                    {
                        int dbRefCount = 0;
                        view?.Read<int>(0, out dbRefCount);
                        if (dbRefCount == 0)
                        {
                            using (SqliteConnection backup = new SqliteConnection($"Filename={dbFilename}"))
                            {
                                backup.Open();
                                backup.BackupDatabase(db);
                            }
                            view?.Write(0, dbRefCount + 1);
                        }
                    }
                    mutex.ReleaseMutex();
                }

                using (SqliteCommand cmd = db.CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS ThumbnailCache (Path TEXT PRIMARY KEY, UpdateTime INTEGER, FileSize INTEGER, Width INTEGER, Height INTEGER, Thumbnail BLOB, ThumbnailSub BLOB)";
                    cmd.ExecuteNonQuery();
                }
                _dbFilename = dbFilename;
            }

            transactionThread = new(TransactionoProc);
            transactionThread.IsBackground = true;
            transactionThread.Start();
        }
        private void _terminate()
        {
            transactionExit = true;
            transactionEvent.Set();
            transactionThread?.Join();
            transactionThread = null;
            if (appDb != null)
            {
                appDb.Close();
                appDb.Dispose();
                appDb = null;
            }
            if (mutex != null)
            {
                mutex.WaitOne();
                using (var view = mmfDbRef?.CreateViewAccessor(0, 4, System.IO.MemoryMappedFiles.MemoryMappedFileAccess.ReadWrite))
                {
                    int dbRefCount = 0;
                    view?.Read<int>(0, out dbRefCount);
                    if (dbRefCount > 0)
                    {
                        view?.Write(0, dbRefCount - 1);
                    }
                }
                mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// データベースをファイルに書き出す
        /// </summary>
        /// <param name="dbFilename"></param>
        public void SaveDb(string dbFilename)
        {
            using (SqliteConnection db = openDb())
            {
                using (SqliteConnection backup = new SqliteConnection($"Filename={dbFilename}"))
                {
                    backup.Open();
                    db.BackupDatabase(backup);
                }
            }
        }

        /// <summary>
        /// データベースに保存されているサムネイルを取得する
        /// </summary>
        /// <param name="filename">ファイルパス</param>
        /// <returns></returns>
        public Image? GetThumbnail(string filename)
        {
            var fi = new FileInfo(filename);
            if (!fi.Exists)
                return null;

            var ThumbnailSize = PiViLityCore.Option.ShellSettings.Instance.ThumbnailSize;

            byte[]? buffer = null;
            byte[]? subBuffer = null ;

            using (SqliteConnection db = openDb())
            {
                using (SqliteCommand cmd = db.CreateCommand())
                {
                    cmd.CommandText = "SELECT UpdateTime, FileSize, Width, Height, Thumbnail, ThumbnailSub FROM ThumbnailCache WHERE Path = @Path";
                    cmd.Parameters.AddWithValue("@Path", filename.ToLower());
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var updateTime = reader.GetInt64(0);
                            var fileSize = reader.GetInt64(1);
                            var width = reader.GetInt32(2);
                            var height = reader.GetInt32(3);
                            if (updateTime != fi.LastWriteTimeUtc.ToFileTimeUtc() || fileSize != fi.Length || ThumbnailSize.Width != width || ThumbnailSize.Height != height)
                            {
                                reader.Close();
                                AddTransactionCommand((db,cmd) =>
                                {
                                    cmd.CommandText = "DELETE FROM ThumbnailCache WHERE Path = @Path";
                                    cmd.Parameters.AddWithValue("@Path", filename.ToLower());
                                    cmd.ExecuteNonQuery();
                                }
                                );
                            }
                            else
                            {
                                buffer = reader.GetFieldValue<byte[]>(4);
                                subBuffer = reader.GetFieldValue<byte[]>(5);
                            }
                        }
                    }
                }
            }

            if (buffer?.Length > 0)
            {
                if (subBuffer?.Length > 0)
                {
                    using (var ms = new MemoryStream(buffer))
                    using (var msa = new MemoryStream(subBuffer))
                    {
                        using (var rgb = Image.FromStream(ms))
                        using (var alpha = Image.FromStream(msa))
                        {
                            if (rgb is Bitmap rgbBmp && alpha is Bitmap alphaBmp)
                            {
                                Bitmap? argb = null;
                                PiVilityNative.BitmapUtil.RgbAndAlphaCombineToArgb((Bitmap)rgb, (Bitmap)alpha, ref argb);
                                if (argb != null)
                                {
                                    return argb;
                                }
                            }
                        }
                    }
                }
                using (var ms = new MemoryStream(buffer))
                    return Image.FromStream(ms);
            }

            return null;
        }


        public void SetThumbnail(string filename, Image thumbnail)
        {
            ImageCodecInfo? GetImageCodecInfo(ImageFormat imageFormat)
            {
                ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
                foreach (var encoder in encoders)
                {
                    if (encoder.FormatID == imageFormat.Guid)
                    {
                        return encoder;
                    }
                }
                return null;
            }
            var fi = new FileInfo(filename);
            if (!fi.Exists)
                return;
            var inDbFilename = filename.ToLower();
            using (MemoryStream ms = new MemoryStream())
            using (MemoryStream msa = new MemoryStream())
            {
                var ext = Path.GetExtension(filename).ToLower();
                ImageFormat? imageFormat = ImageFormat.Jpeg;
                ImageFormat? imageFormatA = null;
                Image? rgbImage = null;
                Image? alphaImage = null;
                if (ext == ".jpg" || ext == ".jpeg" || ext == ".bmp")
                {
                    imageFormat = ImageFormat.Jpeg;
                }
                else if (thumbnail.PixelFormat == PixelFormat.Format32bppArgb)
                {
                    Bitmap? srcBmp = null;
                    if (thumbnail is Bitmap bmp)
                    {
                        srcBmp = bmp;
                    }
                    else
                    {
                        srcBmp = new Bitmap(thumbnail);
                    }
                    Bitmap? rgb = null;
                    Bitmap? alpha = null;
                    PiVilityNative.BitmapUtil.BitmapDivineRgbAndAlpha(srcBmp, ref rgb, ref alpha);
                    if (rgb != null && alpha != null)
                    {
                        rgbImage = rgb;
                        alphaImage = alpha;
                        imageFormat = ImageFormat.Jpeg;
                        imageFormatA = ImageFormat.Png;
                    }
                    if (srcBmp != thumbnail)
                    {
                        srcBmp.Dispose();
                    }

                }
                else if (thumbnail.PixelFormat == PixelFormat.Format16bppArgb1555 || thumbnail.PixelFormat == PixelFormat.Format64bppArgb)
                {
                    imageFormat = ImageFormat.Png;
                }
                else
                {
                    imageFormat = ImageFormat.Jpeg;
                }

                void SaveImage(Image img, MemoryStream ms, ImageFormat format)
                {
                    var codec = GetImageCodecInfo(format);
                    if (codec != null)
                    {
                        using (var param0 = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)25L))
                        {
                            using (var eps = new EncoderParameters(1))
                            {
                                eps.Param[0] = param0;
                                try
                                {
                                    img.Save(ms, codec, eps);
                                }
                                catch (Exception)
                                {
                                    img.Save(ms, format);
                                }
                            }
                        }
                    }
                    else
                    {
                        img.Save(ms, format);
                    }
                }

                //サムネイルを保存
                if (rgbImage != null && alphaImage != null && imageFormatA != null)
                {
                    SaveImage(rgbImage, ms, imageFormat);
                    SaveImage(alphaImage, msa, imageFormatA);
                }
                else
                {
                    SaveImage(thumbnail, ms, imageFormat);
                }
#if true
                var width = thumbnail.Size.Width;
                var height = thumbnail.Size.Height;
                AddTransactionCommand((db, cmd) =>
                {
                    SqliteTransaction? transaction = null;
                    {
                        using (SqliteCommand cmdCheck = db.CreateCommand())
                        {
                            cmdCheck.CommandText = "SELECT COUNT(*) FROM ThumbnailCache WHERE Path = @Path";
                            cmdCheck.Parameters.AddWithValue("@Path", inDbFilename);
                            var execRet = cmdCheck.ExecuteScalar();
                            bool isExistRecord = ((long?)execRet ?? 0) > 0;
                            if (isExistRecord)
                            {
                                cmd.CommandText = "UPDATE ThumbnailCache SET UpdateTime = @UpdateTime, FileSize = @FileSize, Width = @Width, Height = @Height, Thumbnail = @Thumbnail, ThumbnailSub=@ThumbnailSub WHERE Path = @Path";
                            }
                            else
                            {
                                cmd.CommandText = "INSERT INTO ThumbnailCache (Path, UpdateTime, FileSize, Width, Height, Thumbnail, ThumbnailSub) VALUES (@Path, @UpdateTime, @FileSize, @Width, @Height, @Thumbnail, @ThumbnailSub)";
                            }
                            transaction = db.BeginTransaction();
                            cmd.Parameters.AddWithValue("@Path", inDbFilename);
                            cmd.Parameters.AddWithValue("@UpdateTime", fi.LastWriteTimeUtc.ToFileTimeUtc());
                            cmd.Parameters.AddWithValue("@FileSize", fi.Length);
                            cmd.Parameters.AddWithValue("@Width", width);
                            cmd.Parameters.AddWithValue("@Height", height);
                            cmd.Parameters.AddWithValue("@Thumbnail", ms.ToArray());
                            cmd.Parameters.AddWithValue("@ThumbnailSub", msa.ToArray());
                            cmd.Transaction = transaction;
                            cmd.ExecuteNonQuery();
                        }
                        transaction?.Commit();
                    }
                }
                );
#else
                using (SqliteConnection db = openDb())
                {
                    SqliteTransaction? transaction = null;
                    {
                        using (SqliteCommand cmdCheck = db.CreateCommand())
                        using (SqliteCommand cmd = db.CreateCommand())
                        {
                            cmdCheck.CommandText = "SELECT COUNT(*) FROM ThumbnailCache WHERE Path = @Path";
                            cmdCheck.Parameters.AddWithValue("@Path", inDbFilename);
                            var execRet = cmdCheck.ExecuteScalar();
                            bool isExistRecord = ((long?)execRet ?? 0) > 0;
                            if (isExistRecord)
                            {
                                cmd.CommandText = "UPDATE ThumbnailCache SET UpdateTime = @UpdateTime, FileSize = @FileSize, Width = @Width, Height = @Height, Thumbnail = @Thumbnail, ThumbnailSub=@ThumbnailSub WHERE Path = @Path";
                            }
                            else
                            {
                                cmd.CommandText = "INSERT INTO ThumbnailCache (Path, UpdateTime, FileSize, Width, Height, Thumbnail, ThumbnailSub) VALUES (@Path, @UpdateTime, @FileSize, @Width, @Height, @Thumbnail, @ThumbnailSub)";
                            }
                            transaction = db.BeginTransaction();
                            cmd.Parameters.AddWithValue("@Path", inDbFilename);
                            cmd.Parameters.AddWithValue("@UpdateTime", fi.LastWriteTimeUtc.ToFileTimeUtc());
                            cmd.Parameters.AddWithValue("@FileSize", fi.Length);
                            cmd.Parameters.AddWithValue("@Width", thumbnail.Size.Width);
                            cmd.Parameters.AddWithValue("@Height", thumbnail.Size.Height);
                            cmd.Parameters.AddWithValue("@Thumbnail", ms.ToArray());
                            cmd.Parameters.AddWithValue("@ThumbnailSub", msa.ToArray());
                            cmd.Transaction = transaction;
                            cmd.ExecuteNonQuery();
                        }
                        transaction?.Commit();
                    }
                }
#endif
            }
        }
    }
}
