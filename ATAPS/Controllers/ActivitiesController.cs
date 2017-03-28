using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ATAPS.Models;
using ATAPS.Models.DisplayObject;
using ATAPS.Helpers;

namespace ATAPS.Controllers
{
    public class ActivitiesController : Controller
    {
        private RFIDDBEntities db = new RFIDDBEntities();

        // GET: Activities
        public ActionResult Index(int? filter)
        {
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            List<ActivityDisplayObject> doList = DBHelper.GetAllActivitiesWithDataByAgendaID(filter);
            List<ActivityType> typeList = db.ActivityTypes.ToList();
            ViewBag.TypeList = typeList;
            return View(doList);
        }

        // GET: Activities/Details/5
        public ActionResult Details(int? id, int? filter)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            Activity activity = db.Activities.Where(o => o.ID == id).FirstOrDefault();
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        // GET: Activities/Create
        public ActionResult Create(int? filter)
        {
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            List<ActivityType> typeList = db.ActivityTypes.ToList();
            ViewBag.TypeList = typeList;
            return View();
        }

        // POST: Activities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ActivityName,AgendaID")] Activity activity, int? filter)
        {
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            activity.AgendaID = filter ?? default(int);
            if (ModelState.IsValid)
            {
                db.Activities.Add(activity);
                db.SaveChanges();
                return RedirectToAction("Index", new { filter = ViewBag.FilterID });
            }

            return View(activity);
        }

        // GET: Activities/Edit/5
        public ActionResult Edit(int? id, int? filter)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            List<ActivityType> typeList = db.ActivityTypes.ToList();
            ViewBag.TypeList = typeList;
            Activity activity = db.Activities.Where(o => o.ID == id).FirstOrDefault();
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        // POST: Activities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ActivityName,AgendaID")] Activity activity, int? filter)
        {
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            if (ModelState.IsValid)
            {
                db.Entry(activity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(activity);
        }

        // GET: Activities/Delete/5
        public ActionResult Delete(int? id, int? filter)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            Activity activity = db.Activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        // POST: Activities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int? filter)
        {
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            Activity activity = db.Activities.Where(o => o.ID == id).FirstOrDefault();
            db.Activities.Remove(activity);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Activities/UploadWaiver
        public ActionResult UploadWaiver(int id)
        {
            // get activity info
            Activity activity = db.Activities.Where(o => o.ID == id).FirstOrDefault();
            ViewBag.ActivityName = activity.ActivityName;
            ViewBag.ActivityID = id;

            // display the view
            return (View());
        }

        // POST: Activities/UploadWaiver
        [HttpPost]
        public ActionResult UploadWaiver()
        {
            // process incoming waiver
            if (Request.Files.Count > 0 && Request.Files["Document"].FileName != "")
            {
                // set local and web path of uploaded photo
                string local_path = Server.MapPath("~") + "\\Content\\activity_waivers\\";
                string web_path = "/Content/activity_waivers/";
                string prefix = string.Format(@"{0}", DateTime.Now.Ticks);
                string suffix = ".pdf";//ega grab suffix from uploaded filename
                string local_fname = local_path + prefix + suffix;
                string url = web_path + prefix + suffix;

                // save the uploaded file
                var uploadedFile = Request.Files["Document"];
                System.IO.File.Delete(local_fname);
                uploadedFile.SaveAs(local_fname);

                // save this in the database
                int activityId = Int32.Parse (Request["id"]);
                string parmName = "ActivityWaiver-" + activityId;
                string parmValue = url + "," + Request["Label"];
                Parm parm = new Parm ();
                parm.ParmName = parmName;
                parm.ParmValue = parmValue;
                db.Parms.Add(parm);
                db.SaveChanges();
            }

            // redirect back to waiver list
            return (RedirectToAction("Waivers/" + Request["id"]));
        }

        // GET: Activities/Waivers/5
        public ActionResult Waivers(int id)
        {
            // get activity info
            Activity activity = db.Activities.Where(o => o.ID == id).FirstOrDefault();
            ViewBag.ActivityName = activity.ActivityName;
            ViewBag.ActivityID = id;

            // get list of waivers for this activity
            List<Parm> parms = new List<Parm>();
            string name = "ActivityWaiver-" + id;
            parms = db.Parms.Where(o => o.ParmName == name).ToList();

            // return the list
            return (View(parms));
        }

        public ActionResult DeleteWaiver(int id)
        {
            // delete the waiver
            Parm parm = db.Parms.Where(o => o.ID == id).FirstOrDefault();
            db.Parms.Remove(parm);
            db.SaveChanges();

            // return to editing this waiver
            int activityId = Int32.Parse (Request["activity"]);
            return (RedirectToAction("Waivers/" + activityId));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
