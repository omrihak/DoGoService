using System;
using System.Linq;
using System.Collections.Generic;
using Google.Maps;
using System.Web;

namespace DoGoService.Paths
{
    public class WalkRequest
    {
        public List<DogWalk> DogWalks { get; set; }
        public int OwnerId { get; set; }
    }
}