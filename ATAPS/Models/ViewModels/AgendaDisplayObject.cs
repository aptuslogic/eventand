using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATAPS.Models.DisplayObject
{
    public class AgendaDisplayObject
    {
        public Agenda Agenda { get; set; }
        public EventDate EventDate { get; set; }

        public List<Activity> ActivityList { get; set; }
        public List<ActivityDisplayObject> ActivityFullList { get; set; }
        public List<Attendee> CheckedInAttendees { get; set; }
    }
}