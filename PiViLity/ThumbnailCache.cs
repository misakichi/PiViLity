using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.WebUI;
using Microsoft.Data.Sqlite;
using System.Drawing.Imaging;
using System.IO;
namespace PiViLity
{
    internal class ThumbnailCache
    {
        private static ThumbnailCache _incetance = new();

        public static ThumbnailCache Instance => _incetance;
        private string _dbFilename = "";

        public void Initialize(string dbFilename)
        {
            using (SqliteConnection db = new SqliteConnection($"Filename={dbFilename}"))
            {
                db.Open();
                using (SqliteCommand cmd = db.CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS ThumbnailCache (Path TEXT PRIMARY KEY, UpdateTime INTEGER, FileSize INTEGER, Width INTEGER, Height INTEGER, Thumbnail BLOB)";
                    cmd.ExecuteNonQuery();
                }
                _dbFilename = dbFilename;
            }
        }

        public Image? GetThumbnail(string filename)
        {
            var fi = new FileInfo(filename);
            if (!fi.Exists)
                return null;

            var ThumbnailSize = Setting.AppSettings.Instance.ThumbnailSize;

            using (SqliteConnection db = new SqliteConnection($"Filename={_dbFilename}"))
            {
                db.Open();
                using (SqliteCommand cmd = db.CreateCommand())
                {
                    cmd.CommandText = "SELECT UpdateTime, FileSize, Width, Height, Thumbnail FROM ThumbnailCache WHERE Path = @Path";
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
                                return Image.FromStream(new MemoryStream(buffer));
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
            using (SqliteConnection db = new SqliteConnection($"Filename={_dbFilename}"))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    bool isExistRecord = false;
                    using (SqliteCommand cmd = db.CreateCommand())
                    {
                        cmd.CommandText = "SELECT COUNT(*) FROM ThumbnailCache WHERE Path = @Path";
                        cmd.Parameters.AddWithValue("@Path", inDbFilename);
                        cmd.Transaction = transaction;
                        var execRet = cmd.ExecuteScalar();
                        if (execRet != null)
                        {
                            isExistRecord = (long)execRet > 0;
                        }
                    }

                    using (MemoryStream ms = new MemoryStream())
                    {
                        var ext = Path.GetExtension(filename).ToLower();
                        ImageFormat? imageFormat = null;
                        if (ext == ".jpg" || ext == ".jpeg" || ext==".bmp")
                        {
                            imageFormat = ImageFormat.Jpeg;
                        }
                        else if (thumbnail.PixelFormat == PixelFormat.Format16bppArgb1555 ||
                            thumbnail.PixelFormat == PixelFormat.Format32bppArgb ||
                            thumbnail.PixelFormat == PixelFormat.Format64bppArgb)
                        {
                            imageFormat =  ImageFormat.Png;
                        }
                        else
                        {
                            imageFormat = ImageFormat.Jpeg;
                        }
                        var codec = GetImageCodecInfo(imageFormat);
                        if(codec != null)
                        {
                            using (var param0 = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)25L))
                            {
                                using (var eps = new EncoderParameters(1))
                                {
                                    eps.Param[0] = param0;

                                    try
                                    {
                                        thumbnail.Save(ms, codec, eps);
                                    }
                                    catch (Exception)
                                    {
                                        thumbnail.Save(ms, imageFormat);
                                    }
                                }
                            }
                        }
                        else
                        {
                            thumbnail.Save(ms, imageFormat);
                        }
                        using (SqliteCommand cmd = db.CreateCommand())
                        {
                            if (isExistRecord)
                            {
                                cmd.CommandText = "UPDATE ThumbnailCache SET UpdateTime = @UpdateTime, FileSize = @FileSize, Width = @Width, Height = @Height, Thumbnail = @Thumbnail WHERE Path = @Path";
                            }
                            else
                            {
                                cmd.CommandText = "INSERT INTO ThumbnailCache (Path, UpdateTime, FileSize, Width, Height, Thumbnail) VALUES (@Path, @UpdateTime, @FileSize, @Width, @Height, @Thumbnail)";
                            }
                            cmd.Parameters.AddWithValue("@Path", inDbFilename);
                            cmd.Parameters.AddWithValue("@UpdateTime", fi.LastWriteTimeUtc.ToFileTimeUtc());
                            cmd.Parameters.AddWithValue("@FileSize", fi.Length);
                            cmd.Parameters.AddWithValue("@Width", thumbnail.Size.Width);
                            cmd.Parameters.AddWithValue("@Height", thumbnail.Size.Height);
                            cmd.Parameters.AddWithValue("@Thumbnail", ms.ToArray());
                            cmd.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                }
            }
        }
    }
}
