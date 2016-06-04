using Google.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoGoService.Paths.Enums
{
    public enum NodeAction
    {
        Pickup = 1,
        Return,
        Walk,
        Wait,
        Start
    }
}