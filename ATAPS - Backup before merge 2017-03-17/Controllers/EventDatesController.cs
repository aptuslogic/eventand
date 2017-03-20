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
    public class EventDatesController : Controller
    {
        private RFIDDBEntities db = new RFIDDBEntities();

        // GET: EventDates
        public ActionResult Index(int? filter)
        {
            ViewBag.FilterID = filter;
            List<EventDateDisplayObject> datesDO = new List<EventDateDisplayObject>();
            if (filter == null || filter == 0)
            {
                //List<EventDate> dateList = db.EventDates.ToList();
                return HttpNotFound();
            }

            if (filter > 0)
            {
                datesDO = DBHelper.GetAllEventDatesByEventID(filter);
            }
            ViewBag.AttendeeCount = db.Attendees.Where(o => o.EventID == filter).Count();
            EventRecord eventRec = db.EventRecords.Find(filter);
            ViewBag.EventName = eventRec.EventName;
            ViewBag.EventID = eventRec.ID;
            return View(datesDO);

        }

        // GET: EventDates/Details/5
        public ActionResult Details(int? id, int? filter)
        {
            if (id == null || filter == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventDate eventDateList = db.EventDates.Where(o => o.ID == id).FirstOrDefault();
            //EventDate eventDate = db.EventDates.Find(id);
           
            EventDateDisplayObject edDO = new EventDateDisplayObject();
            edDO.Agendas = db.Agendas.Where(o => o.EventDateID == id).ToList();
            edDO.EventDate = eventDateList;

            ViewBag.FilterID = filter;

            return View(edDO);
        }

        // GET: EventDates/Create
        public ActionResult Create(int? filter)
        {
            ViewBag.FilterID = filter;
            if (filter == null) { return HttpNotFound(); }
            //ViewBag.PossibleEvents = db.EventRecords;
            ViewBag.Event = DBHelper.GetEventWithDataByID(filter);
            ViewBag.FilterID = filter;
            return View();
        }

        // POST: EventDates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,EventDate1,EventRecordsID")] EventDate eventDate, int? filter)
        {
            ViewBag.FilterID = -1;
            if (filter == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                ViewBag.FilterID = filter;
                eventDate.EventRecordsID = filter ?? default(int);
                if (ModelState.IsValid)
                {
                    db.EventDates.Add(eventDate);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { filter = filter });
                }
            }
            return View(eventDate);
        }

        // GET: EventDates/Edit/5
        public ActionResult Edit(int? id, int? filter)
        {
            List<EventDate> eventDateList = new List<EventDate>();
            if (filter == null)
            {
                return HttpNotFound();
            }
            else
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                eventDateList = db.EventDates.Where(o => o.ID == id).ToList();
                //EventDate eventDate = db.EventDates.Find(id);
                if (eventDateList.Count() == 0)
                {
                    return HttpNotFound();
                }
            }
            ViewBag.FilterID = filter;
            return View(eventDateList[0]);
        }

        // POST: EventDates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,EventDate1,EventRecordsID")] EventDate eventDate, int? filter)
        {
            if (filter == null)
            {
                return HttpNotFound();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    db.Entry(eventDate).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", new { filter = filter });
                }
            }
            ViewBag.FilterID = filter;
            return View(eventDate);
        }

        // GET: EventDates/Delete/5
        public ActionResult Delete(int? id, int? filter)
        {
            if (id == null || filter == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventDate eventDateList = db.EventDates.Where(o => o.ID == id).FirstOrDefault();
            //EventDate eventDate = db.EventDates.Find(id);

            EventDateDisplayObject edDO = new EventDateDisplayObject();
            edDO.Agendas = db.Agendas.Where(o => o.EventDateID == id).ToList();
            edDO.EventDate = eventDateList;

            ViewBag.FilterID = filter;

            return View(edDO);

            //if (id == null || filter == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //int getID = id ?? default(int);
            //EventDate eventDate = db.EventDates.Find(getID);
            //if (eventDate == null)
            //{
            //    return HttpNotFound();
            //}
            //ViewBag.FilterID = filter;
            //return View(eventDate);
        }

        // POST: EventDates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int? filter)
        {
            if (filter == null)
            {

            }
            else
            {
                EventDate eventDate = db.EventDates.Where(o => o.ID == id).FirstOrDefault();
                db.EventDates.Remove(eventDate);
                db.SaveChanges();
            }
            ViewBag.FilterID = filter;
            return RedirectToAction("Index", new { filter = filter });
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
