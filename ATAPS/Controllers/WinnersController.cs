using System.Data;
using System.Data.Entity;
using ATAPS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using ATAPS.Models.DisplayObject;
using ATAPS.Helpers;

namespace ATAPS.Controllers
{
    public class WinnersController : Controller
    {
        private RFIDDBEntities db = new RFIDDBEntities();

        // GET: Winners
        public ActionResult Index()
        {
            int filter = int.Parse(ConfigurationManager.AppSettings["ActiveEvent"]);
            ViewBag.FilterID = filter;
            return View();
        }

        // GET: Winners/Admin
        public ActionResult Admin()
        {
            return View();
        }

        // GET: Winners/Display
        public ActionResult Display()
        {
            return View();
        }

        // GET: Winners/Presenter
        public ActionResult Presenter()
        {
            return View();
        }

        // GET: Winners/Speaker
        public ActionResult Speaker()
        {
            return View();
        }

        // GET: Winners/Prev
        public ActionResult Prev()
        {
            // init result
            IDictionary<string, string> data = new Dictionary<string, string>();

            // get current queue position
            Parm parm = db.Parms.Where(x => x.ParmName == "CurrWinnerQueuePos").First();
            int currQueuePos = int.Parse(parm.ParmValue);

            // find the previous valid queue position
            List<Attendee> attendees = new List<Attendee>();
            attendees = db.Attendees.Where(x => x.WinnerQueueOrder <= currQueuePos).OrderByDescending(x => x.WinnerQueueOrder).ToList();

            // update the queue position
            if (attendees.Count >= 2)
            {
                parm.ParmValue = attendees[1].WinnerQueueOrder.ToString();
                db.SaveChanges();
                data["status"] = "success";
            }
            else
            {
                data["status"] = "failure";
            }

            // return json result
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        
        // GET: Winners/Next
        public ActionResult Next()
        {
            // init result
            IDictionary<string, string> data = new Dictionary<string, string>();

            // get current queue position
            Parm parm = db.Parms.Where(x => x.ParmName == "CurrWinnerQueuePos").First();
            int currQueuePos = int.Parse(parm.ParmValue);

            // find the next valid queue position
            List<Attendee> attendees = new List<Attendee>();
            attendees = db.Attendees.Where(x => x.WinnerQueueOrder >= currQueuePos).OrderBy(x => x.WinnerQueueOrder).ToList();

            // update the queue position
            if (attendees.Count >= 2)
            {
                parm.ParmValue = attendees[1].WinnerQueueOrder.ToString();
                db.SaveChanges();
                data["status"] = "success";
            }
            else
            {
                data["status"] = "failure";
            }

            // return json result
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: Winners/Reset
        public ActionResult Reset()
        {
            // reset the queue position
            Parm parm = db.Parms.Where(x => x.ParmName == "CurrWinnerQueuePos").First();
            parm.ParmValue = "0";
            db.SaveChanges();

            // return json result
            IDictionary<string, string> data = new Dictionary<string, string>();
            data["status"] = "success";
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: Winners/WinnersUpdate
        public ActionResult WinnersUpdate()
        {
            // get current queue position
            Parm parm = db.Parms.Where(x => x.ParmName == "CurrWinnerQueuePos").First();
            int currQueuePos = int.Parse (parm.ParmValue);

            // grab a record from the database
            List<Attendee> attendees = new List<Attendee>();
            attendees = db.Attendees.Where(x => x.WinnerQueueOrder >= currQueuePos).OrderBy(x => x.WinnerQueueOrder).ToList();

            // store it in a result array
            IDictionary<string, string> data = new Dictionary<string, string>();
            if (attendees.Count >= 1)
            {
                data["curr_id"] = attendees[0].ID.ToString();
                data["curr_name"] = attendees[0].FirstName + " " + attendees[0].LastName;
                data["curr_phonetic"] = attendees[0].PhoneticFirst + " " + attendees[0].PhoneticLast;
                data["curr_preferred"] = attendees[0].PreferredFirst + " " + attendees[0].PreferredLast;
                data["curr_picture"] = attendees[0].Filename;
                if (attendees.Count >= 2)
                {
                    data["next_id"] = attendees[1].ID.ToString();
                    data["next_name"] = attendees[1].FirstName + " " + attendees[1].LastName;
                    data["next_phonetic"] = attendees[1].PhoneticFirst + " " + attendees[1].PhoneticLast;
                    data["next_preferred"] = attendees[1].PreferredFirst + " " + attendees[1].PreferredLast;
                    data["next_picture"] = attendees[1].Filename;
                }
            }

            // see if are at the start or end of the list
            attendees = db.Attendees.OrderBy(x => x.WinnerQueueOrder).ToList();
            data["can_click_prev"] = (currQueuePos <= attendees[0].WinnerQueueOrder) ? "false" : "true";
            data["can_click_next"] = (currQueuePos >= attendees[attendees.Count - 1].WinnerQueueOrder) ? "false" : "true";

            // json encode and return it
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}