using System;
using System.Linq;
using System.Collections.Generic;
using Google.Maps;
using System.Web;
using DoGoService.Paths.Models;

namespace DoGoService.Paths
{
    public class WalkerPath
    {
        public Queue<PathNode> Path { get; set; }
        public TimeSpan StartTime { get; set; }
    }
}