//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoGoService.DataObjects
{
    using System;
    using System.Collections.Generic;
    
    public partial class Trip
    {
        public int id { get; set; }
        public int dogOwnerId { get; set; }
        public int dogWalkerId { get; set; }
        public System.DateTime startOfWalking { get; set; }
        public System.DateTime endOfWalking { get; set; }
        public bool isPaid { get; set; }
    }
}
