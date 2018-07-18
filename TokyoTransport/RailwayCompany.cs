using System;
using System.Collections.Generic;
using System.Text;

namespace TokyoTransport
{
    public class RailwayCompany
    {
        private static readonly Dictionary<string, string> _companies = new Dictionary<string, string>()
        {
            { "JR", "JR-East" },
            { "Keikyu", "Keikyu" },
            { "Keio", "Keio" },
            { "Keisei", "Keisei" },
            { "Odakyu", "Odakyu" },
            { "Seibu", "Seibu" },
            { "Tobu", "Tobu" },
            { "Toei", "Toei" },
            { "TokyoMetro", "TokyoMetro" },
            { "Tokyu", "Tokyu" },
            { "TWR", "TWR" },
            { "Yurikamome", "Yurikamome" },
        };
        private static readonly Dictionary<string, string> _companiesJapanese = new Dictionary<string, string>()
        {
            { "JR", "JR東日本" },
            { "Keikyu", "京急電鉄" },
            { "Keio", "京王電鉄" },
            { "Keisei", "京成電鉄" },
            { "Odakyu", "小田急電鉄" },
            { "Seibu", "西武鉄道" },
            { "Tobu", "東武鉄道" },
            { "Toei", "都営地下鉄" },
            { "TokyoMetro", "東京メトロ" },
            { "Tokyu", "東急電鉄" },
            { "TWR", "東京臨海高速鉄道" },
            { "Yurikamome", "ゆりかもめ" },
        };
        public static string GetCompanyByName(string name)
        {
            return string.Format("odpt.Operator:{0}",_companies[name]);
        }
        public static string GetJapaneseCompanyName(string name)
        {
            return _companiesJapanese[name];
        }
        public static string GetFormattedLineName(string company, string lineName)
        {
            return string.Format("odpt.Railway:{0}.{1}", _companies[company], lineName);
        }
        public static string GetFormattedStationName(string company,string lineName,string staName)
        {
            return string.Format("odpt.Station:{0}.{1}.{2}", _companies[company], lineName, staName);
        }
    }
}
