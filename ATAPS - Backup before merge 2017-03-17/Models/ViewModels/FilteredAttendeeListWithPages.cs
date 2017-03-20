using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATAPS.Models.DisplayObject
{
    public class FilteredAttendeeListWithPages
    {
        public List<Attendee> Attendees { get; set; }
        public List<int> Pages { get; set; }
        public int TotalPages { get; set; }
        public int CurPage { get; set; }
        public int LowerLim { get; set; }
        public int UpperLim { get; set; }
        public string SearchString { get; set; }
    }
}