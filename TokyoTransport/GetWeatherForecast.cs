
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
    public static class GetWeatherForecast
    {
        [FunctionName("GetWeatherForecast")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            string lat = req.Query["lat"];
            string lon = req.Query["lon"];
            string baseURL = "https://api.openweathermap.org/data/2.5/forecast?";

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            lat = lat ?? data?.lat;
            lon = lon ?? data?.lon;

            if (lat != null & lon != null)
                baseURL += $"lat={lat}&lon={lon}&";
            else
                baseURL += "q=Tokyo,jp&";
            baseURL += $"appid={await TokenHelper.GetToken("weather")}";

            string response = await JsonHelper.GetJsonString(baseURL);
            JObject jobj = JObject.Parse(response);
            JArray jsonArr = (JArray)jobj["list"];
            List<dynamic> result = new List<dynamic>();
            if (jsonArr != null)
            {
                foreach (var i in jsonArr)
                {
                    result.Add(new
                    {
                        Temp = Converter.KelvinToCelsius(i["main"]["temp"].Value<float>()),
                        MinTemp = Converter.KelvinToCelsius(i["main"]["temp_min"].Value<float>()),
                        MaxTemp = Converter.KelvinToCelsius(i["main"]["temp_max"].Value<float>()),
                        Pressure = i["main"]["pressure"],
                        Humidity = i["main"]["humidity"],
                        Weather = i["weather"].First["main"],
                        WindSpeed = i["wind"]?["speed"],
                        WindDirection = i["wind"]?["deg"],
                        Cloud = i["clouds"]?["all"],
                        Rainfall = i["rain"]?["3h"],
                        Snow = i["snow"]?["3h"],
                        Time = i["dt_txt"],
                    });
                }
            }
            return new OkObjectResult(result);
        }
    }
}
