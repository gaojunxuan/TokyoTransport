
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
using System.Text.RegularExpressions;

namespace TokyoTransport
{
    public static class GetFlightInfoArrival
    {
        [FunctionName("GetFlightInfoArrival")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            Regex regex = new Regex(@"odpt\.\S+\.");
            string flightNumber = req.Query["flightNumber"];
            string airport = req.Query["airport"];
            string from = req.Query["from"];
            string url = RequestHelper.ComposeURL("GetFlightInfoArrival");

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            flightNumber = flightNumber ?? data?.flightNumber;
            airport = airport ?? data?.airport;
            from = from ?? data?.from;

            if (airport != null)
            {
                url = $"{url}?acl:consumerKey={await TokenHelper.GetToken("tokyochallenge")}&odpt:operator={OperatorInfo.GetAirportCode(airport)}";
                if (flightNumber != null)
                {
                    url += $"&odpt:flightNumbers={flightNumber}";
                }
                if (from != null)
                {
                    url += $"&odpt:departureAirport=odpt.Airport:{from}";
                }
                string response = await RequestHelper.GetJsonString(url);
                JArray jsonArray = JArray.Parse(response);
                List<dynamic> result = new List<dynamic>();
                foreach (var i in jsonArray)
                {
                    result.Add(new
                    {
                        Terminal = i["odpt:terminal"]?.ToString().Replace("odpt.AirportTerminal:", "").Substring(4),
                        Gate = i["odpt:gate]"] != null ? regex.Replace(i["odpt:gate"].ToString(), "") : null,
                        DepartureAirport = i["odpt:departureAirport"]?.ToString().Replace("odpt.Airport:", ""),
                        DestinationAirport = i["odpt:destinationAirport"]?.ToString().Replace("odpt.Airport:", ""),
                        FlightNumbers = i["odpt:flightNumbers"],
                        ActualTime = i["odpt:actualTime"],
                        ScheduledTime = i["odpt:scheduledTime"],
                        EstimatedTime = i["odpt:estimatedTime"],
                        FlightStatus = i["odpt:flightStatus"]?.ToString().Replace("odpt.FlightStatus:", ""),
                        AircraftModel = i["odpt:aircraftModel"],
                        BaggageClaim = i["odpt:baggageClaim "],
                    });
                }
                return new OkObjectResult(result);
            }
            else
            {
                return new BadRequestObjectResult("Please pass airport name on the query string or in the request body");
            }
        }
    }
}
