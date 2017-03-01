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
            Attendee retval = new Attendee();

            // reference @Model.LastName and @ViewBag.parm in the view to see these values
            retval.LastName = "some dude";
            ViewBag.parm = "some text";

            return View(retval);
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

        // GET: Winners/DisplayUpdate
        public ActionResult DisplayUpdate(int? prevID, int? filter)
        {
            //ega pass in an id param and do where clause

            // grab a record from the database
            Attendee attendee = db.Attendees.OrderBy (x => x.ID).FirstOrDefault();

            // store it in a result array
            IDictionary<string, string> data = new Dictionary<string, string>();
            data["name"] = attendee.FirstName + " " + attendee.LastName;
            data["id"] = attendee.ID.ToString ();

            // json encode and return it
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: Winners/PresenterUpdate
        public ActionResult PresenterUpdate()
        {
            return View();//ega return json here
        }

        // GET: Winners/SpeakerUpdate
        public ActionResult SpeakerUpdate()
        {
            return View();//ega return json here
        }
    }
}