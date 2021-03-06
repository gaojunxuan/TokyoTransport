﻿using System;
using System.Collections.Generic;
using System.Text;
using TokyoTransport.Model;

namespace TokyoTransport
{
    public class OperatorInfo
    {
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
            { "Haneda", "羽田空港"}
        };
        private static readonly Dictionary<string, string> _companiesChinese = new Dictionary<string, string>()
        {
            { "JR", "JR 东日本" },
            { "Keikyu", "京急电铁" },
            { "Keio", "京王电铁" },
            { "Keisei", "京成电铁" },
            { "Odakyu", "小田急电铁" },
            { "Seibu", "西武铁道" },
            { "Tobu", "東武铁道" },
            { "Toei", "都营地下铁" },
            { "TokyoMetro", "东京 Metro" },
            { "Tokyu", "东急电铁" },
            { "TWR", "临海线" },
            { "Yurikamome", "百合鸥线" },
            { "Haneda", "羽田空港"}
        };
        private static readonly Dictionary<string, string> _airports = new Dictionary<string, string>()
        {
            { "Haneda", "HND-TIAT" }
        };
        public static string GetCompanyByName(string name)
        {
            return string.Format("odpt.Operator:{0}",name);
        }
        public static string GetJapaneseCompanyName(string name)
        {
            return _companiesJapanese[name];
        }
        public static string GetFormattedLineName(string company, string lineName)
        {
            return string.Format("odpt.Railway:{0}.{1}", company, lineName);
        }
        public static string GetFormattedStationName(string company,string lineName,string staName)
        {
            return string.Format("odpt.Station:{0}.{1}.{2}", company, lineName, staName);
        }
        public static string GetAirportCode(string name)
        {
            return string.Format("odpt.Operator:{0}", _airports[name]);
        }
    }
}
