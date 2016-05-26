using System;
using System.Linq;
using System.Collections.Generic;
using Google.Maps;
using System.Web;

namespace DoGoService.Paths.Models
{
    public class DogoWaypoint : ICloneable
    {
        public string Address { get; set; }
        public bool IsReturnWaypoint { get; set; }
        public bool IsPassed { get; set; }
        public TimeSpan TimeOfFirstWaypointPass { get; set; }

        public object Clone()
        {
            var dogoWaypoint = new DogoWaypoint();
            dogoWaypoint.Address = Address;
            dogoWaypoint.IsReturnWaypoint = IsReturnWaypoint;
            dogoWaypoint.IsPassed = IsPassed;
            dogoWaypoint.TimeOfFirstWaypointPass = TimeOfFirstWaypointPass;

            return dogoWaypoint;
        }
    }
}