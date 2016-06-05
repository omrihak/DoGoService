using Parse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoGoService.Models
{
    public class DogOwner
    {
        public int userId { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public bool isComfortable6To8 { get; set; }
        public bool isComfortable8To10 { get; set; }
        public bool isComfortable10To12 { get; set; }
        public bool isComfortable12To14 { get; set; }
        public bool isComfortable14To16 { get; set; }
        public bool isComfortable16To18 { get; set; }
        public bool isComfortable18To20 { get; set; }
        public bool isComfortable20To22 { get; set; }
    }
}