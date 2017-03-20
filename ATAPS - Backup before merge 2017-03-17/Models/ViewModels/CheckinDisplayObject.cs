using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATAPS.Models.DisplayObject
{
    public class CheckinDisplayObject
    {
        public Attendee Attendee { get; set; }
        public Agenda Agenda { get; set; }
        public Activity Activity { get; set; }
        public ActivityDetail ActivityDetail { get; set; }
        public ActivityType ActivityType { get; set; }
        public AttendeeLastCheck Check { get; set; }
    }
}