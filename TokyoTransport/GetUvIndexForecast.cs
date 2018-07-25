
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Threading.Tasks;
using TokyoTransport.Helper;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace TokyoTransport
{
    public static class GetUvIndexForecast
    {
        [FunctionName("GetUvIndexForecast")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            string lat = req.Query["lat"];
            string lon = req.Query["lon"];
            string baseURL = "https://api.openweathermap.org/data/2.5/uvi/forecast?";

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            lat = lat ?? data?.lat;
            lon = lon ?? data?.lon;

            if (lat != null & lon != null)
            {
                baseURL += $"lat={lat}&lon={lon}&";
                baseURL += $"appid={await TokenHelper.GetToken("weather")}";
                string response = await JsonHelper.GetJsonString(baseURL);
                JArray jsonArr = JArray.Parse(response);
                List<dynamic> result = new List<dynamic>();
                if (jsonArr != null)
                {
                    foreach (var i in jsonArr)
                    {
                        result.Add(new
                        {
                            UvIndex = i["value"],
                            Time = Converter.UnixTimeStampToDateTime(i["date"].Value<double>()).ToString()
                        });
                    }
                }
                return new OkObjectResult(result);
            }
            else
                return new BadRequestObjectResult("Please pass parameters on the query string or in the request body");
        }
    }
}
