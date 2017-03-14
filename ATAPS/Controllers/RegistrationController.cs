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

        public ActionResult BulkCheckin(string busNum)
        {
            ViewBag.BusNumber = busNum;
            return View();
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