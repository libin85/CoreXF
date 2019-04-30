
using SQLite;
using Xamarin.Forms;
using System;

namespace CoreXF
{
    public abstract class DB
    {
        public static SQLiteConnection Connection { get; set; }

        public static string GetDbFolder()
        {
            string path = null;
            if (Device.RuntimePlatform == Device.UWP)
            {
                path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            }
            else
            {
                path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            return path;
        }

        public static void Close()
        {
            if(Connection != null)
            {
                Connection.Close();
                Connection = null;
            }
        }
    }
}
