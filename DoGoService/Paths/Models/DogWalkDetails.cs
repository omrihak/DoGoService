using System;
using System.Linq;
using System.Collections.Generic;
using Google.Maps;
using System.Web;

namespace DoGoService.Paths
{

    public class DogWalkDetails
    {
        public int UserId { get; set; }
        public int TimeOfWalk { get; set; }
        public string Address { get; set; }
        public TimeSpan EarliestPickup { get; set; }
        public TimeSpan LatestPickup { get; set; }
    }

}