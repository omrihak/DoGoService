using System;
using System.Linq;
using System.Collections.Generic;
using Google.Maps;
using System.Web;
using DoGoService.Paths.Enums;

namespace DoGoService.Paths.Models
{
    public class PathNode
    {
        public string Waypoint { get; set; }
        public int Duration { get; set; }
        public NodeAction Type { get; set; }

        public PathNode(int duration, NodeAction type) : this(string.Empty, duration, type)
        {}

        public PathNode(string waypoint, int duration, NodeAction type)
        {
            Waypoint = waypoint;
            Duration = duration;
            Type = type;
        }
    }
}