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
    public class EventRecordsController : Controller
    {
        private RFIDDBEntities db = new RFIDDBEntities();

        // GET: EventRecords
        public ActionResult Index(int? filter)
        {
            if(filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            
            List<EventDisplayObject> vList = new List<EventDisplayObject>();

            vList = DBHelper.GetAllEventsWithDataByClientID(filter);
            Client client = db.Clients.Find(filter);
            ViewBag.ClientName = client.ClientName;
            
            return View(vList);
        }

        // GET: EventRecords/Details/5
        public ActionResult Details(int? id, int? filter)
        {
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //EventRecord eventRecord = db.EventRecords.Find(id);
            EventDisplayObject eventDO = new EventDisplayObject();
            eventDO.Event = db.EventRecords.Where(o => o.ID == id).FirstOrDefault();
            eventDO.EventDates = db.EventDates.Where(o => o.EventRecordsID == id).ToList();
            eventDO.Client = db.Clients.Where(o => o.ID == eventDO.Event.ClientID).FirstOrDefault();
            if (eventDO == null || eventDO.Event == null)
            {
                return HttpNotFound();
            }
            return View(eventDO);
        }

        // GET: EventRecords/Create
        public ActionResult Create(int? filter)
        {
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            RFIDDBEntities db = new RFIDDBEntities();
            ViewBag.PossibleClients = db.Clients.Where(o => o.ID == filter).ToList();
            EventRecord eventRec = new EventRecord();
            eventRec.EventCode = "Invalid Client ID";
            if(ViewBag.PossibleClients.Count > 0)
            {
                eventRec.EventCode = ATAPS_Pile.GenerateEventShortcode(ViewBag.PossibleClients[0].ClientName);
            }

            return View(eventRec);
        }

        // POST: EventRecords/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,EventName,EventCode,EventDesc,ClientID")] EventRecord eventRecord, int? filter)
        {
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            //RFIDDBEntities db = new RFIDDBEntities();
            if (ModelState.IsValid && eventRecord.ClientID != null)
            {
                db.EventRecords.Add(eventRecord);
                db.SaveChanges();
                return RedirectToAction("Index", new { filter=filter });
            }

            ViewBag.PossibleClients = db.Clients.Where(o => o.ID == filter).ToList();
            return View(eventRecord);
        }

        // GET: EventRecords/Edit/5
        public ActionResult Edit(int? id, int? filter)
        {
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventRecord eventRecord = db.EventRecords.Find(id);
            ViewBag.PossibleClients = db.Clients;
            if (eventRecord == null)
            {
                return HttpNotFound();
            }

            return View(eventRecord);
        }

        // POST: EventRecords/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,EventName,EventCode,EventDesc,ClientID")] EventRecord eventRecord, int? filter)
        {
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            if (ModelState.IsValid && eventRecord.ClientID != null)
            {
                db.Entry(eventRecord).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(eventRecord);
        }

        // GET: EventRecords/Delete/5
        public ActionResult Delete(int? id, int? filter)
        {
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventRecord eventRecord = db.EventRecords.Find(id);
            if (eventRecord == null)
            {
                return HttpNotFound();
            }
            return View(eventRecord);
        }

        // POST: EventRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int? filter)
        {
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            EventRecord eventRecord = db.EventRecords.Find(id);
            db.EventRecords.Remove(eventRecord);
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
