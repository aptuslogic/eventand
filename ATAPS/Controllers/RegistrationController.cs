using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATAPS.Controllers
{
    public class RegistrationController : Controller
    {
        // GET: Registration
        public ActionResult Index()
        {
            return View();
        }

        // GET:  Registration/Busses
        public ActionResult Busses()
        {
            return View();
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
    }
}