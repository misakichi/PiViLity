using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.WebUI;
using Microsoft.Data.Sqlite;
using System.Drawing.Imaging;
namespace PiViLity
{
    internal class ThumbnailCache
    {
        private static ThumbnailCache _incetance = new();

        public static ThumbnailCache Instance => _incetance;
        private string _dbFilename = "";

        public Size ThumbnailSize { get; set; } = new Size(128, 128);

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
            var fi = new FileInfo(filename);
            if (!fi.Exists)
                return;
            using (SqliteConnection db = new SqliteConnection($"Filename={_dbFilename}"))
            {
                db.Open();
                using (SqliteCommand cmd = db.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO ThumbnailCache (Path, UpdateTime, FileSize, Width, Height, Thumbnail) VALUES (@Path, @UpdateTime, @FileSize, @Width, @Height, @Thumbnail)";
                    cmd.Parameters.AddWithValue("@Path", filename.ToLower());
                    cmd.Parameters.AddWithValue("@UpdateTime", fi.LastWriteTimeUtc.ToFileTimeUtc());
                    cmd.Parameters.AddWithValue("@FileSize", fi.Length);
                    cmd.Parameters.AddWithValue("@Width", ThumbnailSize.Width);
                    cmd.Parameters.AddWithValue("@Height", ThumbnailSize.Height);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        if (thumbnail.PixelFormat == PixelFormat.Format16bppArgb1555 ||
                            thumbnail.PixelFormat == PixelFormat.Format32bppArgb ||
                            thumbnail.PixelFormat == PixelFormat.Format64bppArgb)
                        {
                            thumbnail.Save(ms,ImageFormat.Png);
                        }
                        else
                        {
                            thumbnail.Save(ms, ImageFormat.Jpeg);
                        }
                        cmd.Parameters.AddWithValue("@Thumbnail", ms.ToArray());
                    }
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
