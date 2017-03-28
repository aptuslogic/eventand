using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ATAPS.Models.DisplayObject;
using ATAPS.Models;
using ATAPS.Helpers;
using System.Configuration;

namespace ATAPS.Controllers
{
    public class RegistrationController : Controller
    {
        private RFIDDBEntities db = new RFIDDBEntities();

        // NOTE:  This section will control automated generation of Agenda / Events for
        // The registration process.  Business logic will be in ATAPS_Pile.cs

        // GET: Registration
        public ActionResult Index()
        {
            int temp = 0; // force Git change
            return View();
        }

        // GET:  Registration/Busses
        public ActionResult Busses()
        {
            Parm checkinParm = db.Parms.Where(o => o.ParmName == "RegistrationAgendaID").FirstOrDefault();
            if(checkinParm == null)
            {
                bool good = ATAPS_Pile.CreateRegistrationAgenda();
            }

            checkinParm = db.Parms.Where(o => o.ParmName == "RegistrationAgendaID").FirstOrDefault();
            AgendaDisplayObject regAgenda = DBHelper.GetAgendaWithDataByID(int.Parse(checkinParm.ParmValue));
            return View(regAgenda);
        }

        public ActionResult BulkCheckin(string searchString, string busNum)
        {
            ViewBag.BusNumber = busNum;

            int? pageNum = 1;
            int pageSize = 25;
            ViewBag.PageSize = pageSize;
            // here we pull the query based on the sort order and direction
            List<Attendee> attendees = new List<Attendee>();

            string sortOrder = "name";
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.QueueSortParm = sortOrder == "WinnerQueueOrder" ? "wqo_desc" : "WinnerQueueOrder";
            ViewBag.PartSortParm = sortOrder == "ParticipantType" ? "ptype_desc" : "ParticipantType";
            ViewBag.RSVPSortParm = sortOrder == "RSVPStatus" ? "rsvp_desc" : "RSVPStatus";
            ViewBag.RFIDSortParm = sortOrder == "RFID" ? "rfid_desc" : "RFID";
            ViewBag.LastSort = sortOrder;
            if (sortOrder == "") { ViewBag.LastSort = "Name"; }

            // find max queue order and add one
            List<Attendee> all_attendees = new List<Attendee>();
            all_attendees = db.Attendees.OrderBy(x => x.WinnerQueueOrder).ToList();
            int? last_queue_position = all_attendees[all_attendees.Count - 1].WinnerQueueOrder;
            int next_queue_position = (last_queue_position == null) ? 1 : (int)(last_queue_position + 1);

            // get attendees matching the search string
            int eventId = int.Parse(ConfigurationManager.AppSettings["ActiveEvent"]);
            attendees = ATAPS_Pile.GetSortedAttendeesWithFilter(eventId, sortOrder, searchString, pageNum);

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

            return View(attendees);
        }

        public ActionResult IndivCheckin(int attendeeId)
        {
            //ega pass in attendee id
            return (View());
        }

        [HttpPost]
        public ActionResult IndivCheckin()
        {
            //ega create a "checkin" as if attendee had just checked in to an activity
            return (View());//ega send them back to bulk checkin
        }

        public ActionResult BulkRfid (int busID)
        {
            //ega populate list of attendees in order of check in
            return (View());
        }

        public ActionResult TableCheckin()
        {
            return View();
        }

        public ActionResult CreateBus(int filter)
        {

            ViewBag.FilterID = -1;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBus([Bind(Include = "ID,ActivityName,AgendaID")] Activity activity, int? filter)
        {
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            activity.AgendaID = filter ?? default(int);
            Parm checkinParm = db.Parms.Where(o => o.ParmName == "RegistrationAgendaID").FirstOrDefault();
            activity.AgendaID = int.Parse(checkinParm.ParmValue);
            activity.ActivityTypeID = 1;
            if (ModelState.IsValid)
            {
                db.Activities.Add(activity);
                db.SaveChanges();
                return RedirectToAction("Busses");
            }

            return View(activity);
        }
    }
}