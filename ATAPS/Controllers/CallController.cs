using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using ATAPS.Models;
using ATAPS.Models.DisplayObject;
using ATAPS.Helpers;
using System.Configuration;
using System.Dynamic;

namespace ATAPS.Controllers
{
    public class CallController : ApiController
    {
        private RFIDDBEntities db = new RFIDDBEntities();

        // GET: api/Call
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Call/5
        public string Get(int id)
        {
            
            return "value is: " + id;
        }

        // GET:  api/Call/GetTest?id=5
        [Route("api/Call/GetTest")]
        [HttpGet]
        public string GetTest(int id)
        {

            return "value is: " + id;
        }

        [Route("api/Call/GetAgendasByEventDateID")]
        [HttpGet]
        public IHttpActionResult GetAgendasByEventDateID(int filter)
        {
            List<Agenda> retVal = new List<Agenda>();

            retVal = db.Agendas.Where(o => o.EventDateID == filter).ToList();

            return Ok(retVal);
        }


        [Route("api/Call/GetActivitiesByAgendaID")]
        [HttpGet]
        public IHttpActionResult GetActivitiesByAgendaID(int filter)
        {
            List<ActivityDisplayObject> retVal = new List<ActivityDisplayObject>();

            List<Activity> aList = db.Activities.Where(o => o.AgendaID == filter).ToList();
            foreach(Activity act in aList)
            {
                ActivityDisplayObject toAdd = new ActivityDisplayObject();
                toAdd.Activity = act;
                toAdd.ActivityDetail = db.ActivityDetails.Where(o => o.ActivityID == act.ID).FirstOrDefault();
                toAdd.Agenda = db.Agendas.Where(o => o.ID == filter).FirstOrDefault();
                toAdd.ActivityType = db.ActivityTypes.Where(o => o.ID == toAdd.Activity.ActivityTypeID).FirstOrDefault();
                retVal.Add(toAdd);
            }

            return Ok(retVal);
        }


        [Route("api/Call/GetActivityByID")]
        [HttpGet]
        public IHttpActionResult GetActivityByID(int filter)
        {
            ActivityDisplayObject retVal = new ActivityDisplayObject();

            if (filter > 0)
            {
                retVal.Activity = db.Activities.Where(o => o.ID == filter).FirstOrDefault();
                retVal.ActivityDetail = db.ActivityDetails.Where(o => o.ActivityID == filter).FirstOrDefault();
                retVal.ActivityType = db.ActivityTypes.Where(o => o.ID == retVal.Activity.ActivityTypeID).FirstOrDefault();

                int accessEventID = int.Parse(ConfigurationManager.AppSettings["ActiveEvent"]);
                retVal.CheckedInCount = ATAPS_Pile.GetActivityAttendeeCurrentCountByActivityID(retVal.Activity.ID, retVal.Activity.AgendaID);
                
            }
            return Ok(retVal);
        }

        [Route("api/Call/TagAttendeeInByID")]
        [HttpGet]
        public IHttpActionResult TagAttendeeInByID(string rfid, int agendaID, int? activityID, string dir)
        {
            Attendee retVal = new Attendee();
            retVal.LastName = "None Found";

            int actID = -1;
            if (activityID > 0) { actID = activityID ?? default(int); }

            // find attendee by RFID
            Attendee found = db.Attendees.Where(o => o.RfID == rfid.ToString()).FirstOrDefault();
            dynamic retJSON = new ExpandoObject();
            retJSON.Attendee = retVal;
            retJSON.DuplicateCheckin = false;
            // if found, insert a TagIn for this Attendee and return the object
            if (found != null)
            {
                retVal = found;
                // check to see if this is a duplicate check
                AttendeeLastCheck confirm = db.AttendeeLastChecks.Where(o => o.AttendeeID == found.ID).OrderByDescending(o => o.LastUpdate).FirstOrDefault();
                if (confirm != null)
                {
                    if (confirm.LastAgenda == agendaID && confirm.LastActivity == actID && confirm.CheckDir == dir)
                    {
                        // this is a duplicate
                        retJSON.DuplicateCheckin = true;
                    }
                }

                if(retJSON.DuplicateCheckin == false)
                {
                    AttendeeLastCheck tagIn = new AttendeeLastCheck();
                    tagIn.AgendaID = agendaID;
                    tagIn.AttendeeID = found.ID;
                    tagIn.LastActivity = activityID;
                    tagIn.LastAgenda = agendaID;
                    tagIn.LastUpdate = DateTime.Now;
                    tagIn.CheckDir = dir;
                    db.AttendeeLastChecks.Add(tagIn);
                    db.SaveChanges();
                }
                retJSON.Attendee = found;
            }

            retJSON.CheckedInCount = ATAPS_Pile.GetActivityAttendeeCurrentCountByActivityID(actID, agendaID);
            return Ok(retJSON);
        }

        [HttpGet, ActionName("GetAttendeesFiltered")]
        public IHttpActionResult GetAttendeesFiltered(int id, string sortOrder, string searchString, int? pageNum)
        {
            FilteredAttendeeListWithPages retVal = new FilteredAttendeeListWithPages();
            List<Attendee> attendees = new List<Attendee>();
            int pageSize = 25;
            
            attendees = ATAPS_Pile.GetSortedAttendeesWithFilter(id, sortOrder, searchString, pageNum);
            
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
            retVal.CurPage = curPage;
            retVal.TotalPages = upperLim;
            retVal.LowerLim = lowerLim;
            retVal.UpperLim = upperLim;

            retVal.Pages = ATAPS_Pile.GetAttendeePageList(lowerLim, upperLim, curPage, attendees.Count());

            retVal.Attendees = attendees;
            return Ok(retVal);
        }

        // POST: api/Call
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Call/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Call/5
        public void Delete(int id)
        {
        }
    }
}
