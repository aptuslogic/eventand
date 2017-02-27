using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATAPS.Models.DisplayObject
{
    [Serializable]
    public class EventDisplayObject
    {
        public EventRecord Event { get; set; }
        public Client Client { get; set; }
        public List<EventDate> EventDates { get; set; }
    }
}