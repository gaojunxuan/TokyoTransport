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
    public static class GetRailwayStationInfo
    {
        [FunctionName("GetRailwayStationInfo")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            string url = JsonHelper.ComposeURL("GetRailwayStationInfo");
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
                url = $"{url}?acl:consumerKey={await TokenHelper.GetToken("tokyochallenge")}&odpt:operator={OperatorInfo.GetCompanyByName(company)}";
                //url = $"{url}?acl:consumerKey={"KEY"}&odpt:operator={OperatorInfo.GetCompanyByName(company)}";
                url += $"&odpt:railway={OperatorInfo.GetFormattedLineName(company, line)}";
                if (station != null)
                {
                    url += $"&owl:sameAs={OperatorInfo.GetFormattedStationName(company,line,station)}";
                }
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
                        Latitude = i["geo:lat"],
                        Longitude = i["geo:long"]
                    });
                }
                return (ActionResult)new OkObjectResult(result);
            }
            return new BadRequestObjectResult("Please pass parameters on the query string or in the request body");
        }
    }
}
