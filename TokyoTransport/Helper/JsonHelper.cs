using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TokyoTransport.Helper
{
    public class JsonHelper
    {
        static HttpClient httpClient;
        public static async Task<string> GetJsonString(string url)
        {
            string response;
            using (httpClient = new HttpClient())
            {
                response = await httpClient.GetStringAsync(new Uri(url));
            }
            return response;
        }
        static readonly string _baseURL = "https://api-tokyochallenge.odpt.org";
        static readonly Dictionary<string, string> _paths = new Dictionary<string, string>()
        {
            { "GetRailwayStationInfo","/api/v4/odpt:Station" },
            { "GetRailwayFare","/api/v4/odpt:RailwayFare" },

        };
        public static string ComposeURL(string requestPath)
        {
            return _baseURL + _paths[requestPath];
        }
    }
}
