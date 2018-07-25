using System;
using System.Collections.Generic;
using System.Text;

namespace TokyoTransport.Helper
{
    public static class Converter
    {
        public static float KelvinToCelsius(float input)
        {
            return (float)Math.Round(input - 273.15, 2);
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}
