using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;
using SQLite.Net;
using TokyoTransport.Model;

namespace TokyoTransport.Helper
{
    public static class SqliteHelper
    {
        static SQLiteConnection _conn = new SQLiteConnection(new SQLite.Net.Platform.Generic.SQLitePlatformGeneric(),"data.sqlite");
        public static Stations QueryStationWithJpName(string jaName)
        {
            return _conn.Query<Stations>("SELECT * FROM stations WHERE ja=?", jaName).FirstOrDefault();
        }
        public static Lines QueryLineWithJpName(string jaName, string linecode="")
        {
            List<Lines> result = new List<Lines>();
            if(!string.IsNullOrEmpty(linecode))
            {
                result = _conn.Query<Lines>("SELECT * FROM lines WHERE code=?", linecode);
                if (result.Count == 1)
                    return result.First();
            }
            result = _conn.Query<Lines>("SELECT * FROM lines WHERE ja=?", jaName);
            return result.FirstOrDefault();
        }
    }
}
