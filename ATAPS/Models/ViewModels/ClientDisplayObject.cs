using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATAPS.Models.DisplayObject
{
    public class ClientDisplayObject
    {
        public Client Client { get; set; }
        public List<EventRecord> Events { get; set; }
    }
}