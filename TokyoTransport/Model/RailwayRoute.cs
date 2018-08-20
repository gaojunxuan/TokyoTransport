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
        public RailwayLine Line { get; set; }
        public RailwayFare Fare { get; set; }
        public int StationCount { get; set; }
    }
}
