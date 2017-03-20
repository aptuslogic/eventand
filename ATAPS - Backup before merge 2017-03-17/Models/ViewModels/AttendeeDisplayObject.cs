using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATAPS.Models.DisplayObject
{
    [Serializable]
    public class AttendeeDisplayObject
    {
        public Attendee Attendee { get; set; }
        public AttendeeType Type { get; set; }
        public List<AttendeeLastCheck> Checks { get; set; }
        public List<CheckinDisplayObject> CheckDOList { get; set; }
    }
}