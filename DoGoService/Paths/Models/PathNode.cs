using System;
using System.Linq;
using System.Collections.Generic;
using Google.Maps;
using System.Web;
using DoGoService.Paths.Enums;
using System.Runtime.Serialization;

namespace DoGoService.Paths.Models
{
    [DataContract]
    public class PathNode
    {
        [DataMember]
        public int UserId { get; set; }
        [DataMember]
        public string Waypoint { get; set; }
        [DataMember]
        public int Duration { get; set; }
        [DataMember]
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