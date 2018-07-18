using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using TokyoTransport.Helper;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace TokyoTransport
{
    public static class GetStationInfo
    {
        [FunctionName("GetStationInfo")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");
            string url = "https://api-tokyochallenge.odpt.org/api/v4/odpt:Station";
            string company = req.Query["company"];
            string line = req.Query["line"];
            string station = req.Query["station"];
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            company = company ?? data?.company;
            line = line ?? data?.line;
            station = station ?? data?.station;

            if (company != null && line != null)
            {
                url = $"{url}?acl:consumerKey={await TokenHelper.GetToken("tokyochallenge")}&odpt:operator={RailwayCompany.GetJapaneseCompanyName(company)}";
                //url = $"{url}?acl:consumerKey={"replace with your key while debugging"}&odpt:operator={RailwayCompany.GetCompanyByName(company)}";
                url += $"&odpt:railway={RailwayCompany.GetFormattedLineName(company, line)}";
                string response = await JsonHelper.GetJsonString(url);
                JToken jsonArr = JArray.Parse(response);
                List<dynamic> result = new List<dynamic>();
                foreach (var i in jsonArr)
                {
                    result.Add(new
                    {
                        StationName = i["dc:title"]?.ToString(),
                        StationCode = i["odpt:stationCode"]?.ToString(),
                        English = i["odpt:stationTitle"]?["en"].ToString(),
                        Longitude = i["geo:long"],
                        Latitude = i["geo:lat"]
                    });
                }
                if (station != null)
                {
                    result = result.Where(x => x.English == station).ToList();
                }
                return (ActionResult)new OkObjectResult(result);
            }
            return new BadRequestObjectResult("Please pass parameters on the query string or in the request body");
        }
    }
}
