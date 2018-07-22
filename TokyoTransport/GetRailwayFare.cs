
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

namespace TokyoTransport
{
    public static class GetRailwayFare
    {
        [FunctionName("GetRailwayFare")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            string url = JsonHelper.ComposeURL("GetRailwayFare");
        
            string fromStation = req.Query["fromStation"];
            string fromLine = req.Query["fromLine"];
            string toStation = req.Query["toStation"];
            string toLine = req.Query["toLine"];
            string company = req.Query["company"];

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            fromStation = fromStation ?? data?.fromStation;
            fromLine = fromLine ?? data?.fromLine;
            toStation = toStation ?? data?.toStation;
            toLine = toLine ?? data?.toLine;
            company = company ?? data?.company;

            if (fromStation != null & toStation != null & fromLine != null & toStation != null & company != null)
            {
                url = $"{url}?acl:consumerKey={await TokenHelper.GetToken("tokyochallenge")}&odpt:operator={OperatorInfo.GetCompanyByName(company)}&odpt:fromStation={OperatorInfo.GetFormattedStationName(company, fromLine, fromStation)}&odpt:toStation={OperatorInfo.GetFormattedStationName(company, toLine, toStation)}";
                string response = await JsonHelper.GetJsonString(url);
                JToken jsonArr = JArray.Parse(response);
                List<dynamic> result = new List<dynamic>();
                foreach (var i in jsonArr)
                {
                    result.Add(new
                    {
                        IcCardFare = i["odpt:icCardFare"],
                        TicketFare = i["odpt:ticketFare"],
                        ChildIcCardFare = i["odpt:childIcCardFare"],
                        ChildTicketFare = i["odpt:childTicketFare"]
                    });
                }
                return new OkObjectResult(result);
            }

            return new BadRequestObjectResult("Please pass the required parameters on the query string or in the request body");
        }
    }
}
