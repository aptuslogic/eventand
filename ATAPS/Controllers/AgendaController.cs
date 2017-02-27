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
    public class AgendaController : Controller
    {
        private RFIDDBEntities db = new RFIDDBEntities();

        // GET: Agenda
        public ActionResult Index(int? filter)
        {
            if(filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;

            List<Agenda> aList = db.Agendas.Where(o => o.EventDateID == filter).ToList();
            List<AgendaDisplayObject> retList = new List<AgendaDisplayObject>();
            foreach(var item in aList)
            {
                retList.Add(DBHelper.GetAgendaWithDataByID(item.ID));
            }
            
            return View(retList);
        }

        // GET: Agenda/Details/5
        public ActionResult Details(int? id, int? filter)
        {
            if (id == null || filter == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Agenda agenda = db.Agendas.Where(o => o.ID == id).FirstOrDefault();
            if (agenda == null)
            {
                return HttpNotFound();
            }
            ViewBag.FilterID = filter;
            return View(agenda);
        }

        // GET: Agenda/Create
        public ActionResult Create(int? filter)
        {
            if(filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            ViewBag.EventDateID = filter ?? default(int);
            return View();
        }

        // POST: Agenda/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,EventDateID,AgendaName,AgendaOrder,AgendaType")] Agenda agenda, int? filter)
        {
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            int edID = filter ?? default(int);
            if (ModelState.IsValid)
            {
                agenda.EventDateID = edID;
                db.Agendas.Add(agenda);
                db.SaveChanges();
                return RedirectToAction("Index", new { filter = filter });
            }

            return View(agenda);
        }

        // GET: Agenda/Edit/5
        public ActionResult Edit(int? id, int? filter)
        {
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Agenda agenda = db.Agendas.Where(o => o.ID == id).FirstOrDefault();
            if (agenda == null)
            {
                return HttpNotFound();
            }
            return View(agenda);
        }

        // POST: Agenda/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,EventDateID,AgendaName,AgendaOrder,AgendaType")] Agenda agenda, int? filter)
        {
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            if (ModelState.IsValid)
            {
                agenda.EventDateID = filter ?? default(int);
                db.Entry(agenda).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { filter = filter });
            }
            return View(agenda);
        }

        // GET: Agenda/Delete/5
        public ActionResult Delete(int? id, int? filter)
        {
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Agenda agenda = db.Agendas.Where(o => o.ID == id).FirstOrDefault();
            if (agenda == null)
            {
                return HttpNotFound();
            }
            return View(agenda);
        }

        // POST: Agenda/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int? filter)
        {
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            Agenda agenda = db.Agendas.Where(o => o.ID == id).FirstOrDefault();
            db.Agendas.Remove(agenda);
            db.SaveChanges();
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
