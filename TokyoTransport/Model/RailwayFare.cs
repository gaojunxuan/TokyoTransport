using System;
using System.Collections.Generic;
using System.Text;

namespace TokyoTransport.Model
{
    public class RailwayFare
    {
        public int IcCardFare { get; set; }
        public int TicketFare { get; set; }
        public int ChildIcCardFare { get; set; }
        public int ChildTicketFare { get; set; }
    }
}
