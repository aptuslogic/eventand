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
    
    public partial class EventRecord
    {
        public int ID { get; set; }
        public string EventName { get; set; }
        public string EventCode { get; set; }
        public string EventDesc { get; set; }
        public Nullable<int> ClientID { get; set; }
    }
}