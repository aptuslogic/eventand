//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ATAPS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AttendeeLastCheck
    {
        public int ID { get; set; }
        public Nullable<int> AgendaID { get; set; }
        public Nullable<System.DateTime> LastUpdate { get; set; }
        public Nullable<int> LastAgenda { get; set; }
        public Nullable<int> LastActivity { get; set; }
        public int AttendeeID { get; set; }
        public string CheckDir { get; set; }
    }
}
