using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPSAPI.Models
{
    public class Locations
    {
        public string serialNo { get; set; }
        public string deviceNo { get; set; }
        public string telNo { get; set; }
        public string imeiNo { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string updateTime { get; set; }

        public Locations() { }

    }
}