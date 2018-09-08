using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokyoTransport.Helper;

namespace TokyoTransport.Model
{
    public class GroupedRailwayRoute
    {
        public GroupedRailwayRoute(IGrouping<string, RailwayRoute> g)
        {
            if (g == null)
                throw new ArgumentNullException("g");
            Key = g.Key;
            Routes = g.ToList();
        }
        [JsonIgnore]
        public string Key { get; private set; }
        public RailwayFare Fare { get; set; }
        public List<RailwayRoute> Routes { get; set; }
        public async Task GetFare()
        {
            string url;
            if (Key == "JR-East" & Routes != null & Routes.Count != 0)
            {
                url = $"https://tokyofare.azurewebsites.net/jr?from={Routes.First().From.Name}&to={Routes.Last().To.Name}";
                string response = await RequestHelper.GetJsonString(url);
                JToken jsonObj = JObject.Parse(response);
                List<dynamic> result = new List<dynamic>();
                result.Add(new RailwayFare()
                {
                    IcCardFare = (int)jsonObj["icCardFare"],
                    TicketFare = (int)jsonObj["ticketFare"],
                    ChildIcCardFare = (int)jsonObj["childIcCardFare"],
                    ChildTicketFare = (int)jsonObj["childTicketFare"]
                });
                Fare = result.FirstOrDefault();
            }
            else if(Key == "TsukubaExpress" & Routes != null & Routes.Count != 0)
            {
                url = $"https://tokyofare.azurewebsites.net/tsukubaexpress?from={Routes.First().From.Name}&to={Routes.Last().To.Name}";
                string response = await RequestHelper.GetJsonString(url);
                JToken jsonObj = JObject.Parse(response);
                List<dynamic> result = new List<dynamic>();
                result.Add(new RailwayFare()
                {
                    IcCardFare = (int)jsonObj["icCardFare"],
                    TicketFare = (int)jsonObj["ticketFare"],
                    ChildIcCardFare = (int)jsonObj["childIcCardFare"],
                    ChildTicketFare = (int)jsonObj["childTicketFare"]
                });
                Fare = result.FirstOrDefault();
            }
            else if (Routes != null & Routes.Count != 0)
            {
                url = $"{RequestHelper.ComposeURL("GetRailwayFare")}?acl:consumerKey={await TokenHelper.GetToken("tokyochallenge")}&odpt:operator={OperatorInfo.GetCompanyByName(Key)}&odpt:fromStation={OperatorInfo.GetFormattedStationName(Key, Routes.First().Line.Name, Routes.First().From.Name)}&odpt:toStation={OperatorInfo.GetFormattedStationName(Key, Routes.Last().Line.Name, Routes.Last().To.Name)}";
                string response = await RequestHelper.GetJsonString(url);
                if (!string.IsNullOrEmpty(response))
                {
                    JToken jsonArr = JArray.Parse(response);
                    List<dynamic> result = new List<dynamic>();
                    foreach (var i in jsonArr)
                    {
                        result.Add(new RailwayFare()
                        {
                            IcCardFare = (int)i["odpt:icCardFare"],
                            TicketFare = (int)i["odpt:ticketFare"],
                            ChildIcCardFare = (int)i["odpt:childIcCardFare"],
                            ChildTicketFare = (int)i["odpt:childTicketFare"]
                        });
                    }
                    Fare = result.FirstOrDefault();
                }
            }
        }
    }
}
