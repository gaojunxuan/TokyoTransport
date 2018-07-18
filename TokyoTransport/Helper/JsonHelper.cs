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
    }
}
