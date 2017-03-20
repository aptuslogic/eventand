using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATAPS.Models.DisplayObject
{
    public class ActivityDisplayObject
    {
        public Activity Activity { get; set; }
        public Agenda Agenda { get; set; }
        public ActivityDetail ActivityDetail { get; set; }
        public ActivityType ActivityType { get; set; }
        public List<Attendee> AttendeeList { get; set; }
        public List<AttendeeLastCheck> CurrentCheckins { get; set; }
        public int CheckedInCount { get; set; }
    }
}