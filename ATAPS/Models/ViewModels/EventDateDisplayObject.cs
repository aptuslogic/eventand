using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATAPS.Models.DisplayObject
{
    public class EventDateDisplayObject
    {
        public EventDate EventDate { get; set; }
        public virtual EventRecord Event { get; set; }
        public List<Agenda> Agendas { get; set; }
        public List<AgendaDisplayObject> AgendaFull { get; set;  }
        public Client Client { get; set; }

    }
}