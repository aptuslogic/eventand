using System.Data;
using System.Data.Entity;
using ATAPS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATAPS.Controllers
{
    public class WinnersController : Controller
    {
        private RFIDDBEntities db = new RFIDDBEntities();

        // GET: Winners
        public ActionResult Index()
        {
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
            return View();//ega return json here
        }
        
        // GET: Winners/Next
        public ActionResult Next()
        {
            return View();//ega return json here
        }

        // GET: Winners/Reset
        public ActionResult Reset()
        {
            return View();//ega return json here
        }

        // GET: Winners/WinnersUpdate
        public ActionResult WinnersUpdate(int? prevID)
        {
            // grab a record from the database
            List<Attendee> attendees = new List<Attendee>();
            //ega limit query to two 
            if (prevID == null)
            {
                attendees = db.Attendees.OrderBy(x => x.ID).ToList();
            }
            else
            {
                attendees = db.Attendees.Where(x => x.ID > prevID).OrderBy(x => x.ID).ToList();
            }

            // store it in a result array
            IDictionary<string, string> data = new Dictionary<string, string>();
            if (attendees.Count >= 1)
            {
                data["curr_id"] = attendees[0].ID.ToString();
                data["curr_name"] = attendees[0].FirstName + " " + attendees[0].LastName;
                data["curr_phonetic"] = attendees[0].PhoneticFirst + " " + attendees[0].PhoneticLast;
                data["curr_preferred"] = attendees[0].PreferredFirst + " " + attendees[0].PreferredLast;
                //data["curr_picture"] = attendees[0].AttPicture;//ega not sure the format of this field, it's not just a filename apparently
                if (attendees.Count >= 2)
                {
                    data["next_id"] = attendees[1].ID.ToString();
                    data["next_name"] = attendees[1].FirstName + " " + attendees[1].LastName;
                    data["next_phonetic"] = attendees[1].PhoneticFirst + " " + attendees[1].PhoneticLast;
                    data["next_preferred"] = attendees[1].PreferredFirst + " " + attendees[1].PreferredLast;
                    //data["next_picture"] = attendees[1].AttPicture;//ega not sure the format of this field, it's not just a filename apparently
                }
            }

            // json encode and return it
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}