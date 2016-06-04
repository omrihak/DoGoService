using System;
using System.Linq;
using System.Collections.Generic;
using Google.Maps;
using System.Web;
using DoGoService.Paths.Models;
using System.Runtime.Serialization;

namespace DoGoService.Paths
{
    [DataContract]
    public class WalkerPath
    {
        [DataMember]
        public Queue<PathNode> Path { get; set; }
        [DataMember]
        public TimeSpan StartTime { get; set; }
    }
}