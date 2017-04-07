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
    public class AttendeeTypesController : Controller
    {
        private RFIDDBEntities db = new RFIDDBEntities();

        // GET: AttendeeTypes
        public ActionResult Index()
        {
            return View(db.AttendeeTypes.ToList());
        }

        // GET: AttendeeTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AttendeeType attendeeType = db.AttendeeTypes.Find(id);
            if (attendeeType == null)
            {
                return HttpNotFound();
            }
            return View(attendeeType);
        }

        // GET: AttendeeTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AttendeeTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,EventID,AttendeeType1,Name,IsGeneral")] AttendeeType attendeeType)
        {
            if (ModelState.IsValid)
            {
                db.AttendeeTypes.Add(attendeeType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(attendeeType);
        }

        // GET: AttendeeTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AttendeeType attendeeType = db.AttendeeTypes.Find(id);
            if (attendeeType == null)
            {
                return HttpNotFound();
            }
            return View(attendeeType);
        }

        // POST: AttendeeTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,EventID,AttendeeType1,Name,IsGeneral")] AttendeeType attendeeType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(attendeeType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(attendeeType);
        }

        // GET: AttendeeTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AttendeeType attendeeType = db.AttendeeTypes.Find(id);
            if (attendeeType == null)
            {
                return HttpNotFound();
            }
            return View(attendeeType);
        }

        // POST: AttendeeTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AttendeeType attendeeType = db.AttendeeTypes.Find(id);
            db.AttendeeTypes.Remove(attendeeType);
            db.SaveChanges();
            return RedirectToAction("Index");
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
