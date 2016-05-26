using System;
using System.Linq;
using System.Collections.Generic;
using Google.Maps;
using System.Web;

namespace DoGoService.Paths.Models
{
    public class WalkerPathLink : ICloneable
    {
        public DogoWaypoint SourceWaypoint { get; set; }
        public DogoWaypoint DestWaypoint { get; set; }
        public int Duration { get; set; }
        public int RealDuration { get; set; }

        public object Clone()
        {
            var pathLink = new WalkerPathLink();
            pathLink.SourceWaypoint = SourceWaypoint;
            pathLink.DestWaypoint = DestWaypoint;
            pathLink.Duration = Duration;
            pathLink.RealDuration = RealDuration;
            return pathLink;
        }
    }
}