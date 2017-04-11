using ATAPS.Models;
using ATAPS.Models.DisplayObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATAPS.Helpers
{
    public class DBHelper
    {

        public static List<EventDisplayObject> GetAllEventsWithData()
        {
            List<EventDisplayObject> retVal = new List<EventDisplayObject>();
            using (var db = new RFIDDBEntities())
            {
                try
                {
                    //var query = db.Clients.ToList();
                    var query = from b in db.EventRecords
                                orderby b.ID descending
                                select b;

                    foreach(var item in query)
                    {
                        EventDisplayObject toAdd = new EventDisplayObject();

                        toAdd.Event = item;
                        toAdd.Client = db.Clients.Find(item.ClientID);
                        var datesQuery = from b in db.EventDates
                                         where b.EventRecordsID == item.ID
                                         select b;
                        toAdd.EventDates = datesQuery.ToList();
                                         
                        retVal.Add(toAdd);
                    }
                }
                catch { }
            }
            return retVal;
        }

        internal static List<EventDisplayObject> GetAllEventsWithDataByClientID(int? filter)
        {
            List<EventDisplayObject> retVal = new List<EventDisplayObject>();
            int clientID = filter ?? default(int);
            using (var db = new RFIDDBEntities())
            {
                try
                {
                    //var query = db.Clients.ToList();
                    var query = from b in db.EventRecords
                                orderby b.ID descending
                                where b.ClientID == clientID
                                select b;

                    foreach (var item in query)
                    {
                        EventDisplayObject toAdd = new EventDisplayObject();

                        toAdd.Event = item;
                        toAdd.Client = db.Clients.Find(item.ClientID);
                        var datesQuery = from b in db.EventDates
                                         where b.EventRecordsID == item.ID
                                         select b;
                        toAdd.EventDates = datesQuery.ToList();

                        retVal.Add(toAdd);
                    }
                }
                catch { }
            }
            return retVal;
        }

        internal static List<ActivityDisplayObject> GetAllActivitiesWithDataByAgendaID(int? eID)
        {
            List<ActivityDisplayObject> retval = new List<ActivityDisplayObject>();

            if (eID != null)
            {
                using (var db = new RFIDDBEntities())
                {
                    try
                    {
                        List<Activity> aList = db.Activities.Where(o => o.AgendaID == eID).ToList();

                        foreach(var item in aList)
                        {
                            ActivityDisplayObject toAdd = new ActivityDisplayObject();

                            toAdd.Activity = item;
                            toAdd.Agenda = db.Agendas.Where(o => o.ID == item.AgendaID).FirstOrDefault();
                            toAdd.ActivityDetail = db.ActivityDetails.Where(o => o.ActivityID == item.ID).FirstOrDefault();
                            
                            retval.Add(toAdd);
                        }
                        
                    }
                    catch { }
                }
            }

            return retval;
        }

        internal static List<EventDateDisplayObject> GetAllEventDatesByEventID(int? filter)
        {
            List<EventDateDisplayObject> retVal = new List<EventDateDisplayObject>();
            using (var db = new RFIDDBEntities())
            {
                try
                {
                    //var query = db.Clients.ToList();
                    var query = from b in db.EventDates
                                orderby b.EventDate1 ascending
                                where b.EventRecordsID == filter
                                select b;

                    foreach (var item in query)
                    {
                        EventDateDisplayObject toAdd = new EventDateDisplayObject();

                        toAdd.EventDate = item;
                        toAdd.Event = db.EventRecords.Find(item.EventRecordsID);
                        toAdd.Client = db.Clients.Find(toAdd.Event.ClientID);
                        var agQuery = from b in db.Agendas
                                      where b.EventDateID == item.ID
                                      select b;
                        toAdd.Agendas = agQuery.ToList();

                        retVal.Add(toAdd);
                    }
                }
                catch { }
            }
            return retVal;
        }

        internal static List<EventDateDisplayObject> GetAllEventDates()
        {
            List<EventDateDisplayObject> retVal = new List<EventDateDisplayObject>();
            using (var db = new RFIDDBEntities())
            {
                try
                {
                    //var query = db.Clients.ToList();
                    var query = from b in db.EventDates
                                orderby b.ID descending
                                select b;

                    foreach (var item in query)
                    {
                        EventDateDisplayObject toAdd = new EventDateDisplayObject();

                        toAdd.EventDate = item;
                        toAdd.Event = db.EventRecords.Find(item.EventRecordsID);
                        toAdd.Client = db.Clients.Find(toAdd.Event.ClientID);
                        var agQuery = from b in db.Agendas
                                         where b.EventDateID == item.ID
                                         select b;
                        toAdd.Agendas = agQuery.ToList();

                        retVal.Add(toAdd);
                    }
                }
                catch { }
            }
            return retVal;
        }

        public static EventDisplayObject GetEventWithDataByID(int? eID)
        {
            EventDisplayObject retVal = new EventDisplayObject();
            if (eID != null)
            {
                using (var db = new RFIDDBEntities())
                {
                    try
                    {
                        //var query = db.Clients.ToList();
                        var query = from b in db.EventRecords
                                    where b.ID == eID
                                    select b;

                        foreach (var item in query)
                        {
                            EventDisplayObject toAdd = new EventDisplayObject();

                            toAdd.Event = item;
                            toAdd.Client = db.Clients.Find(item.ClientID);
                            var datesQuery = from b in db.EventDates
                                             where b.EventRecordsID == item.ID
                                             select b;
                            toAdd.EventDates = datesQuery.ToList();

                            retVal = toAdd;
                        }
                    }
                    catch { }
                }
            }
            return retVal;
        }

        public static AgendaDisplayObject GetAgendaWithDataByID(int? eID)
        {
            AgendaDisplayObject retVal = new AgendaDisplayObject();

            if (eID != null)
            {
                using (var db = new RFIDDBEntities())
                {
                    try
                    {

                        retVal.Agenda = db.Agendas.Where(o => o.ID == eID).FirstOrDefault();
                        retVal.EventDate = db.EventDates.Where(o => o.ID == retVal.Agenda.EventDateID).FirstOrDefault();
                        retVal.ActivityList = db.Activities.Where(o => o.AgendaID == retVal.Agenda.ID).ToList();

                    }
                    catch { }
                }
            }

            return retVal;
        }
    }
}