using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ATAPS.Models.DisplayObject;
using ATAPS.Helpers;
using ATAPS.Models;

namespace ATAPS.Controllers
{
    public class HostController : Controller
    {
        private RFIDDBEntities db = new RFIDDBEntities();

        // GET: Host
        public ActionResult Index()
        {
            int accessEventID = int.Parse(ConfigurationManager.AppSettings["ActiveEvent"]);
            EventDisplayObject eventDO = new EventDisplayObject();
            eventDO.Event = db.EventRecords.Where(o => o.ID == accessEventID).FirstOrDefault();
            eventDO.EventDates = db.EventDates.Where(o => o.EventRecordsID == accessEventID).ToList();
            eventDO.Client = db.Clients.Where(o => o.ID == eventDO.Event.ClientID).FirstOrDefault();

            // Holds links to various event bits
            return View(eventDO);
        }

        public ActionResult Checkins()
        {
            // A page for seeing what EventDay and Agendas to select
            int accessEventID = int.Parse(ConfigurationManager.AppSettings["ActiveEvent"]);
            //List<AgendaDisplayObject> retVal = new List<AgendaDisplayObject>();
            List<EventDateDisplayObject> retVal = new List<EventDateDisplayObject>();
            List<EventDate> dList = db.EventDates.Where(o => o.EventRecordsID == accessEventID).ToList();

            foreach (EventDate eDate in dList)
            {
                EventDateDisplayObject toAdd = new EventDateDisplayObject();
                toAdd.EventDate = eDate;
                toAdd.Agendas = db.Agendas.Where(o => o.EventDateID == eDate.ID).ToList();
                toAdd.Event = db.EventRecords.Where(o => o.ID == eDate.EventRecordsID).FirstOrDefault();
                toAdd.Client = db.Clients.Where(o => o.ID == toAdd.Event.ClientID).FirstOrDefault();
                toAdd.AgendaFull = new List<AgendaDisplayObject>();
                foreach (Agenda agenda in toAdd.Agendas)
                {
                    AgendaDisplayObject nowAdd = new AgendaDisplayObject();
                    nowAdd.Agenda = agenda;
                    nowAdd.ActivityList = new List<Activity>();
                    nowAdd.ActivityList = db.Activities.Where(o => o.AgendaID == agenda.ID).ToList();
                    toAdd.AgendaFull.Add(nowAdd);
                }
                retVal.Add(toAdd);
            }

            return View(retVal);
        }

        // Select an Agenda w/ SubActivities (This will be the JS-enabled "worksheet")
        // Posting Checkins will be done through API
        public ActionResult RFIDTool(int? filter)
        {
            if (filter == null || filter == 0)
            {
                //List<EventDate> dateList = db.EventDates.ToList();
                return HttpNotFound();
            }
            int accessEventID = int.Parse(ConfigurationManager.AppSettings["ActiveEvent"]);
            ViewBag.EventID = accessEventID;
            ViewBag.EventDateID = filter;
            return View();
        }


        // Attendee List (this will serve as an analogue to the Admin/Attendees page
        public ActionResult Attendees(string sortOrder, string searchString, int? pageNum)
        {
            int accessEventID = int.Parse(ConfigurationManager.AppSettings["ActiveEvent"]);
            ViewBag.EventID = accessEventID;
            int pageSize = 25;
            ViewBag.PageSize = pageSize;
            // here we pull the query based on the sort order and direction
            List<Attendee> attendees = new List<Attendee>();

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PartSortParm = sortOrder == "ParticipantType" ? "ptype_desc" : "ParticipantType";
            ViewBag.RSVPSortParm = sortOrder == "RSVPStatus" ? "rsvp_desc" : "RSVPStatus";
            ViewBag.RFIDSortParm = sortOrder == "RFID" ? "rfid_desc" : "RFID";
            ViewBag.LastSort = sortOrder;
            if (sortOrder == "") { ViewBag.LastSort = "Name"; }

            attendees = ATAPS_Pile.GetSortedAttendeesWithFilter(accessEventID, sortOrder, searchString, pageNum);
            #region refactored in ATAPS_Pile

            //attendees = db.Attendees.Where(o => o.EventID == filter).ToList();

            //if (!String.IsNullOrEmpty(searchString))
            //{
            //    attendees = attendees.Where(s => s.LastName.ToLower().Contains(searchString.ToLower())
            //                           || s.FirstName.ToLower().Contains(searchString.ToLower())
            //                           || s.RfID.ToLower().Contains(searchString.ToLower())).ToList();
            //}

            //ViewBag.CurSearch = searchString;

            //switch (sortOrder)
            //{
            //    case "name_desc":
            //        attendees = attendees.OrderByDescending(s => s.LastName).ToList();
            //        break;
            //    case "ParticipantType":
            //        attendees = attendees.OrderBy(s => s.ParticipantType).ToList();
            //        break;
            //    case "ptype_desc":
            //        attendees = attendees.OrderByDescending(s => s.ParticipantType).ToList();
            //        break;
            //    case "RSVPStatus":
            //        attendees = attendees.OrderBy(s => s.RSVPStatus).ToList();
            //        break;
            //    case "rsvp_desc":
            //        attendees = attendees.OrderByDescending(s => s.RSVPStatus).ToList();
            //        break;
            //    case "RFID":
            //        attendees = attendees.OrderBy(s => s.RfID).ToList();
            //        break;
            //    case "rfid_desc":
            //        attendees = attendees.OrderByDescending(s => s.RfID).ToList();
            //        break;
            //    default:
            //        attendees = attendees.OrderBy(s => s.LastName).ToList();
            //        break;
            //}

            //if (pageNum == null)
            //{
            //    ViewBag.PageNum = 1;
            //    pageNum = 1;
            //}
            //else
            //{
            //    ViewBag.PageNum = pageNum;
            //}
            //decimal pages = 1+(attendees.Count() / pageSize);
            //ViewBag.NumPages = Math.Ceiling(pages);
            //if (pageNum * pageSize > attendees.Count())
            //{
            //    int modCheck = attendees.Count() % pageSize; // get remainder
            //    int lastPage = (int)Math.Ceiling(pages); // last page
            //    ViewBag.PageNum = lastPage;
            //    attendees = attendees.Skip((lastPage - 1) * pageSize).Take(modCheck).ToList();
            //}
            //else
            //{
            //    int startAt = (pageNum ?? default(int)) - 1;
            //    attendees = attendees.Skip(startAt * pageSize).Take(pageSize).ToList();
            //}
            #endregion

            if (pageNum == null)
            {
                pageNum = 1;
            }
            decimal pages = 1 + (attendees.Count() / pageSize);
            if (pageNum * pageSize > attendees.Count())
            {
                int modCheck = attendees.Count() % pageSize; // get remainder
                int lastPage = (int)Math.Ceiling(pages); // last page
                attendees = attendees.Skip((lastPage - 1) * pageSize).Take(modCheck).ToList();
            }
            else
            {
                int startAt = (pageNum ?? default(int)) - 1;
                attendees = attendees.Skip(startAt * pageSize).Take(pageSize).ToList();
            }
            // finally populate pagination tags


            //decimal pages = 1 + (attendees.Count() / pageSize);
            int upperLim = (int)pages;
            int lowerLim = 1;
            int curPage = pageNum ?? default(int);
            ViewBag.CurPage = curPage;
            ViewBag.Pages = upperLim;
            ViewBag.LowerLim = lowerLim;
            ViewBag.UpperLim = upperLim;

            List<int> pageList = ATAPS_Pile.GetAttendeePageList(lowerLim, upperLim, curPage, attendees.Count());

            ViewBag.PageList = pageList;

            EventRecord eRec = db.EventRecords.Find(accessEventID);
            ViewBag.EventName = eRec.EventName;
            return View(attendees);
        }

        public ActionResult AttendeeDetails(int id)
        {
            AttendeeDisplayObject retVal = new AttendeeDisplayObject();
            retVal.Attendee = db.Attendees.Where(o => o.ID == id).FirstOrDefault();
            retVal.Checks = db.AttendeeLastChecks.Where(o => o.AttendeeID == id).ToList().OrderByDescending(o => o.LastUpdate).ToList();
            retVal.Type = db.AttendeeTypes.Where(o => o.AttendeeType1 == retVal.Attendee.ParticipantType).FirstOrDefault();
            retVal.CheckDOList = new List<CheckinDisplayObject>();

            foreach (AttendeeLastCheck check in retVal.Checks)
            {
                CheckinDisplayObject toAdd = new CheckinDisplayObject();

                toAdd.Agenda = db.Agendas.Where(o => o.ID == check.AgendaID).FirstOrDefault();
                toAdd.Activity = db.Activities.Where(o => o.ID == check.LastActivity).FirstOrDefault();
                if (toAdd.Activity == null)
                {
                    toAdd.Activity = new Activity();
                    toAdd.ActivityDetail = new ActivityDetail();
                    toAdd.ActivityType = new ActivityType();
                    toAdd.ActivityType.Name = "";
                    toAdd.Activity.ActivityName = "";
                }
                else
                {
                    toAdd.ActivityType = db.ActivityTypes.Where(o => o.ID == toAdd.Activity.ActivityTypeID).FirstOrDefault();
                    toAdd.ActivityDetail = db.ActivityDetails.Where(o => o.ActivityID == check.ID).FirstOrDefault();
                }
                toAdd.Check = check;

                retVal.CheckDOList.Add(toAdd);
            }

            return View(retVal);
        }



        // See all EventDates associated with the Event


    }
}