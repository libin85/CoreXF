
using SQLite;
using System;
using System.IO;

namespace CoreXF
{
    public class SystemDB
    {
        public const string DbFileName = "system.sql";

        public static SQLiteConnection Connection { get; set; }


        public static void Initialize()
        {

            string path = Path.Combine(DB.GetDbFolder(), DbFileName);

            Connection = new SQLiteConnection(path);
            Connection.CreateTable<HttpCacheModel>();
            Connection.CreateTable<HttpQueueItemModel>();
            Connection.CreateTable<HttpSharedDataItemModel>();
            
        }

        public static void Close()
        {
            Connection?.Close();
            Connection = null;
        }
    }
}
