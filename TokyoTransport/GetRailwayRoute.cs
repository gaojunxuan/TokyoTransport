
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using TokyoTransport.Helper;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TokyoTransport.Model;

namespace TokyoTransport
{
    public static class GetRailwayRoute
    {
        [FunctionName("GetRailwayRoute")]
        public static async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");


            string from = req.Query["from"];
            string to = req.Query["to"];

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            from = from ?? data?.from;
            to = to ?? data?.to;

            if (from != null & to != null)
            {
                string response = await RequestHelper.GetJsonString($"https://api.trip2.jp/ex/tokyo/v1.0/json?src={from}&dst={to}&key=kevingao");
                JObject jsonObj = JObject.Parse(response);
                var jsonArr = jsonObj["ways"].AsJEnumerable();
                List<RailwayRoute> result = new List<RailwayRoute>();
                foreach (var j in jsonArr)
                {
                    result.Add(new RailwayRoute()
                    {
                        From = new RailwayStation()
                        {
                            JpName = j["src_station"]["station_name"].ToString(),
                            HiraganaName = j["src_station"]["station_name_hira"].ToString(),
                            SimpCnName = j["src_station"]["station_name_zh_CN"].ToString(),
                            TradCnName = j["src_station"]["station_name_zh_TW"].ToString(),
                            EnName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(j["src_station"]["station_name_roma"].ToString()),
                            StationNumber = j["src_station"]["station_number"].ToString(),
                        },
                        To = new RailwayStation()
                        {
                            JpName = j["dst_station"]["station_name"].ToString(),
                            HiraganaName = j["dst_station"]["station_name_hira"].ToString(),
                            SimpCnName = j["dst_station"]["station_name_zh_CN"].ToString(),
                            TradCnName = j["dst_station"]["station_name_zh_TW"].ToString(),
                            EnName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(j["dst_station"]["station_name_roma"].ToString()),
                            StationNumber = j["dst_station"]["station_number"].ToString(),
                        },
                        Line = new RailwayLine()
                        {
                            JpName = j["line"]["line_name"].ToString(),
                            LineNumber = j["line"]["line_prefix"].ToString()
                        },
                        StationCount = (int)j["line"]["station_cnt"],
                        Time = (int)j["min"],
                    });
                }
                return new OkObjectResult(result);
            }

            return new BadRequestObjectResult("Please pass the required parameters on the query string or in the request body");
        }
    }
}
