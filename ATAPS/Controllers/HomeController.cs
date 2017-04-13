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
    public class HomeController : Controller
    {
        private RFIDDBEntities db = new RFIDDBEntities();

        public ActionResult Index()
        {
            int accessEventID = int.Parse(ConfigurationManager.AppSettings["ActiveEvent"]);
            EventDisplayObject eventDO = new EventDisplayObject();
            eventDO.Event = db.EventRecords.Where(o => o.ID == accessEventID).FirstOrDefault();
            eventDO.EventDates = db.EventDates.Where(o => o.EventRecordsID == accessEventID).ToList();
            eventDO.Client = db.Clients.Where(o => o.ID == eventDO.Event.ClientID).FirstOrDefault();

            return View(eventDO);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}