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
    }
}