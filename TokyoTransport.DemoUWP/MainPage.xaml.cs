using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TokyoTransport.DemoUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void RunBtn_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(CompanyTbx.Text)&&!string.IsNullOrEmpty(LineTbx.Text))
            {
                Uri requestUri = new Uri($"https://tokyotransport.azurewebsites.net/api/GetStationInfo?code=oPMLjl5vBxUgd0nxlFMlfIg7yqHnxojyjMV8Exx0LWGa383toxWqew==&company={CompanyTbx.Text}&line={LineTbx.Text}");
                HttpClient client = new HttpClient();
                var response = await client.GetAsync(requestUri);
                if(response.IsSuccessStatusCode)
                {
                    var stations = new List<MapElement>();
                    string content = await response.Content.ReadAsStringAsync();
                    JArray array = JArray.Parse(content);
                    foreach(var i in array)
                    {
                        var longitude = i["longitude"].ToString();
                        var latitude = i["latitude"].ToString();
                        BasicGeoposition snPosition = new BasicGeoposition { Latitude = double.Parse(latitude), Longitude = double.Parse(longitude) };
                        Geopoint snPoint = new Geopoint(snPosition);
                        var spaceNeedleIcon = new MapIcon
                        {
                            Location = snPoint,
                            NormalizedAnchorPoint = new Point(0.5, 1.0),
                            ZIndex = 0,
                            Title = i["stationName"].ToString()
                        };
                        stations.Add(spaceNeedleIcon);
                    }
                    var stationsLayer = new MapElementsLayer()
                    {
                        ZIndex = 1,
                        MapElements = stations
                    };
                    MapCtl.Layers.Add(stationsLayer);
                }
            }
        }
    }
}
