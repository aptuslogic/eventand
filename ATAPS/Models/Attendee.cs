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
    
    public partial class Attendee
    {
        public int ID { get; set; }
        public Nullable<int> ParticipantID { get; set; }
        public Nullable<int> PrimaryID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string ParticipantType { get; set; }
        public string RSVPStatus { get; set; }
        public bool IsPrimary { get; set; }
        public byte[] AttPicture { get; set; }
        public string Filename { get; set; }
        public string RfID { get; set; }
        public string PhoneticFirst { get; set; }
        public string PhoneticLast { get; set; }
        public string PreferredFirst { get; set; }
        public string PreferredLast { get; set; }
        public string Mobile { get; set; }
        public Nullable<int> EventID { get; set; }
        public string ActivityListNames { get; set; }
        public Nullable<int> WinnerQueueOrder { get; set; }
    }
}
