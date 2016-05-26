using System;
using System.Linq;
using System.Collections.Generic;
using Google.Maps;
using System.Web;

namespace DoGoService.Paths
{
    public class WaypointLink
    {
        public string SourceWaypoint { get; set; }
        public string DestWaypoint { get; set; }
        public int Duration { get; set; }
    }
}