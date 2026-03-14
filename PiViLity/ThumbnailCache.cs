using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic;
using PiViLityPlugin;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing.Imaging;

namespace PiViLity
{
    /// <summary>
    /// サムネイル画像のキャッシュ
    /// アルファ付はRgbのJpegとAlphaのPngを組み合わせて保存する
    /// </summary>
    internal class ThumbnailCache : Singleton<ThumbnailCache>
    {

        private const string _dbName = "PVilityIconThumbnailCache.db";

        private string _dbFilename = "";
        private SqliteConnection? _appDb;

        const string MemoryName = "PiVilityIconDbRef";
        const string MemoryMutexName = "PiVilityIconDbRefMutex";

        //共有メモリ排他アクセス用
        private Mutex? _mmfDbMuex = new(false, MemoryMutexName);

        /// <summary>
        /// DBを参照している数を管理するための共有メモリ
        /// </summary>
        private System.IO.MemoryMappedFiles.MemoryMappedFile? _mmfDbRef = System.IO.MemoryMappedFiles.MemoryMappedFile.CreateOrOpen(MemoryName, 4, System.IO.MemoryMappedFiles.MemoryMappedFileAccess.ReadWrite);

        public override void Dispose()
        {
            _appDb?.Dispose();
            _appDb = null;
            _mmfDbRef?.Dispose();
            _mmfDbRef = null;
            _mmfDbMuex?.Dispose();
            _mmfDbMuex = null;
        }
        public void Initialize(string dbFilename)
        {
            _Initialize(dbFilename);
        }
        public void Terminate()
        {
            SaveDb(_dbFilename);
            _terminate();
        }

        private SqliteConnection openDb()
        {
            var db = new SqliteConnection($"Data Source=File:{_dbName}?Mode=Memory&Cache=Shared");
            db.Open();
            return db;
        }

        //トランザクションの別スレッド実行管理
        System.Threading.Thread? _transactionThread = null;
        System.Threading.AutoResetEvent _transactionEvent = new(false);
        System.Threading.Mutex? _transactionCommandMutex = new(false);
        Queue<Action<SqliteConnection,SqliteCommand>> _transactionCommandQueue = new();
        bool _transactionExit = false;

        /// <summary>
        /// トランザクションスレッド
        /// </summary>
        void TransactionoProc()
        {
            using var db = openDb();
            while (!_transactionExit)
            {

                //コマンドがあれば処理する
                _transactionEvent.WaitOne(1000);


                while (true)
                {
                    //キューがあれば取得する
                    Action<SqliteConnection,SqliteCommand>? cmd = null;
                    _transactionCommandMutex?.WaitOne();
                    if (_transactionCommandQueue.Count > 0)
                    {
                        cmd = _transactionCommandQueue.Dequeue();
                    }
                    _transactionCommandMutex?.ReleaseMutex();

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
            _transactionCommandMutex?.WaitOne();
            _transactionCommandQueue.Enqueue(cmd);
            _transactionCommandMutex?.ReleaseMutex();
            _transactionEvent.Set();
        }

        private void _Initialize(string dbFilename)
        {
            if (_appDb != null)
                return;


            _appDb = openDb();
            using (SqliteConnection db = openDb())
            {
                //mutexで扱うのは共有メモリやDBの変更が衝突しないため
                if (_mmfDbMuex != null)
                {
                    _mmfDbMuex.WaitOne();
                    using (var view = _mmfDbRef?.CreateViewAccessor(0, 4, System.IO.MemoryMappedFiles.MemoryMappedFileAccess.ReadWrite))
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
                    _mmfDbMuex.ReleaseMutex();
                }

                //初期ではテーブルがない
                using (SqliteCommand cmd = db.CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS ThumbnailCache (Path TEXT PRIMARY KEY, UpdateTime INTEGER, FileSize INTEGER, Width INTEGER, Height INTEGER, Thumbnail BLOB, ThumbnailSub BLOB)";
                    cmd.ExecuteNonQuery();
                }
                _dbFilename = dbFilename;
            }

            _transactionThread = new(TransactionoProc);
            _transactionThread.IsBackground = true;
            _transactionThread.Start();
        }
        private void _terminate()
        {
            //終了前にトランザクションを完全に終えないと破綻するのでスレッドに終了伝えて待っている
            _transactionExit = true;
            _transactionEvent.Set();
            _transactionThread?.Join();
            _transactionThread = null;

            if (_appDb != null)
            {
                _appDb.Close();
                _appDb.Dispose();
                _appDb = null;
            }

            //複数起動時のDB共有用管理破棄
            if (_mmfDbMuex != null)
            {
                _mmfDbMuex.WaitOne();
                using (var view = _mmfDbRef?.CreateViewAccessor(0, 4, System.IO.MemoryMappedFiles.MemoryMappedFileAccess.ReadWrite))
                {
                    int dbRefCount = 0;
                    view?.Read<int>(0, out dbRefCount);
                    if (dbRefCount > 0)
                    {
                        view?.Write(0, dbRefCount - 1);
                    }
                }
                _mmfDbMuex.ReleaseMutex();
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

            var ThumbnailSize = PiViLityCore.Option.ThumbnailSettings.Instance.ThumbnailSize;

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

        /// <summary>
        /// サムネイルの設定（保存）
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="thumbnail"></param>
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
                    //使用頻度の高い32bitに限りRGBとAを分解してjpegを活用している。
                    //todo:Aもjpegにできないか？
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

                //イメージをフォーマットに応じてセムネイル用にエンコード
                //基本はjpeg、アルファがあるとpng
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
            }
        }
    }
}
