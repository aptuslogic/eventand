﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class RFIDDBEntities : DbContext
    {
        public RFIDDBEntities()
            : base("name=RFIDDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Activity> Activities { get; set; }
        public virtual DbSet<ActivityDetail> ActivityDetails { get; set; }
        public virtual DbSet<Agenda> Agendas { get; set; }
        public virtual DbSet<Attendee> Attendees { get; set; }
        public virtual DbSet<AttendeeType> AttendeeTypes { get; set; }
        public virtual DbSet<currentWinner> currentWinners { get; set; }
        public virtual DbSet<dbCheckIn> dbCheckIns { get; set; }
        public virtual DbSet<EventDate> EventDates { get; set; }
        public virtual DbSet<EventRecord> EventRecords { get; set; }
        public virtual DbSet<ActivityType> ActivityTypes { get; set; }
        public virtual DbSet<AgendaTemplate> AgendaTemplates { get; set; }
        public virtual DbSet<AttendeeLastCheck> AttendeeLastChecks { get; set; }
        public virtual DbSet<Parm> Parms { get; set; }
    }
}
