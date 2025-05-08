using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.WebUI;
using Microsoft.Data.Sqlite;
using System.Drawing.Imaging;
using System.IO;
using System.Data.Common;
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

        private static Mutex? mutex = new(false,mutexName);
        private static System.IO.MemoryMappedFiles.MemoryMappedFile? mmfDbRef= System.IO.MemoryMappedFiles.MemoryMappedFile.CreateOrOpen(memoryName, 4, System.IO.MemoryMappedFiles.MemoryMappedFileAccess.ReadWrite);

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
            _incetance.Dispose();
        }
        private SqliteConnection openDb()
        {
            var db = new SqliteConnection($"Data Source=File:{DbName}?Mode=Memory&Cache=Shared");
            db.Open();
            return db;
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
        }
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

        public Image? GetThumbnail(string filename)
        {
            var fi = new FileInfo(filename);
            if (!fi.Exists)
                return null;

            var ThumbnailSize = PiViLityCore.Option.ShellSettings.Instance.ThumbnailSize;

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
                            if(updateTime != fi.LastWriteTimeUtc.ToFileTimeUtc() || fileSize != fi.Length || ThumbnailSize.Width!=width || ThumbnailSize.Height!=height)
                            {
                                reader.Close();
                                cmd.CommandText = "DELETE FROM ThumbnailCache WHERE Path = @Path";
                                cmd.ExecuteNonQuery();
                            }
                            else
                            {
                                byte[] buffer = reader.GetFieldValue<byte[]>(4);
                                byte[] subBuffer = reader.GetFieldValue<byte[]>(5);
                                if(subBuffer?.Length > 0)
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
                                using(var ms = new MemoryStream(buffer))
                                    return Image.FromStream(ms);
                            }
                        }
                    }
                }
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
            using (SqliteConnection db = openDb())
            {
                using (var transaction = db.BeginTransaction())
                {
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
                        if (rgbImage != null && alphaImage != null && imageFormatA!=null)
                        {
                            SaveImage(rgbImage, ms, imageFormat);
                            SaveImage(alphaImage, msa, imageFormatA);
                        }
                        else
                        {
                            SaveImage(thumbnail, ms, imageFormat);
                        }

                        using (SqliteCommand cmdCheck = db.CreateCommand())
                        using (SqliteCommand cmd = db.CreateCommand())
                        {
                            cmdCheck.CommandText = "SELECT COUNT(*) FROM ThumbnailCache WHERE Path = @Path";
                            cmdCheck.Parameters.AddWithValue("@Path", inDbFilename);
                            cmdCheck.Transaction = transaction;
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
                        transaction.Commit();
                    }
                }
            }
        }
    }
}
