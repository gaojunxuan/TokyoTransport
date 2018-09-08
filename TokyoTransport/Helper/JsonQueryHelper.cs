using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TokyoTransport.Model;
using System.Linq;
using Newtonsoft.Json;

namespace TokyoTransport.Helper
{
    public class JsonQueryHelper
    {
        static List<Stations> stations = JsonConvert.DeserializeObject<List<Stations>>(File.ReadAllText("stations.json"));
        static List<Lines> lines = JsonConvert.DeserializeObject<List<Lines>>(File.ReadAllText("lines.json"));

        public static Stations QueryStationWithJpName(string jaName)
        {
            return stations.Where(s => s.ja == jaName).FirstOrDefault();
        }
        public static Lines QueryLineWithJpName(string jaName, string linecode = "")
        {
            List<Lines> result = new List<Lines>();
            if (!string.IsNullOrEmpty(linecode))
            {
                result = lines.Where(l => l.code == linecode).ToList();
                if (result.Count == 1)
                    return result.First();
            }
            result = lines.Where(l => l.ja == jaName).ToList();
            return result.FirstOrDefault();
        }
    }
}
