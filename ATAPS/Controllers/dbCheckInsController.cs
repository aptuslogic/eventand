using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ATAPS.Models;

namespace ATAPS.Controllers
{
    public class dbCheckInsController : Controller
    {
        private RFIDDBEntities db = new RFIDDBEntities();

        // GET: dbCheckIns
        public ActionResult Index()
        {
            return View(db.dbCheckIns.ToList());
        }

        // GET: dbCheckIns/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            dbCheckIn dbCheckIn = db.dbCheckIns.Where(o => o.rideID == id).First();
            if (dbCheckIn == null)
            {
                return HttpNotFound();
            }
            return View(dbCheckIn);
        }

        // GET: dbCheckIns/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: dbCheckIns/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "rideID,activity,busNumber,partID,chrDirection,stopNmbr")] dbCheckIn dbCheckIn)
        {
            if (ModelState.IsValid)
            {
                db.dbCheckIns.Add(dbCheckIn);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dbCheckIn);
        }

        // GET: dbCheckIns/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            dbCheckIn dbCheckIn = db.dbCheckIns.Where(o => o.ID == id).First();
            if (dbCheckIn == null)
            {
                return HttpNotFound();
            }
            return View(dbCheckIn);
        }

        // POST: dbCheckIns/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "rideID,activity,busNumber,partID,chrDirection,stopNmbr")] dbCheckIn dbCheckIn)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dbCheckIn).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dbCheckIn);
        }

        // GET: dbCheckIns/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            dbCheckIn dbCheckIn = db.dbCheckIns.Where(o => o.partID == id).First();
            if (dbCheckIn == null)
            {
                return HttpNotFound();
            }
            return View(dbCheckIn);
        }

        // POST: dbCheckIns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            dbCheckIn dbCheckIn = db.dbCheckIns.Where(o => o.ID == id).First();
            db.dbCheckIns.Remove(dbCheckIn);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult RefreshTest()
        {
            //if(filter == null) { return HttpNotFound(); }
            //ViewBag.FilterID = filter;
            DateTime timer = DateTime.Now;
            return View(timer);
        }

        [HttpPost, ActionName("RefreshTest")]
        public ActionResult RefreshTimer()
        {
            DateTime timer = DateTime.Now;
            return View(timer);
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
