using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TokyoTransport.Model
{
    public class RailwayRoute
    {
        public RailwayStation From { get; set; }
        public RailwayStation To { get; set; }
        public int Time { get; set; }
        [JsonIgnore]
        public RailwayLine FromLine { get; set; }
        [JsonIgnore]
        public RailwayLine ToLine { get; set; }
        public RailwayLine Line { get; set; }
        public string Company { get; set; }
        public int StationCount { get; set; }
    }
}
