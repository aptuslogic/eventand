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
    public class AgendaTemplatesController : Controller
    {
        private RFIDDBEntities db = new RFIDDBEntities();

        // GET: AgendaTemplates
        public ActionResult Index()
        {
            return View(db.AgendaTemplates.ToList());
        }

        // GET: AgendaTemplates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AgendaTemplate agendaTemplate = db.AgendaTemplates.Find(id);
            if (agendaTemplate == null)
            {
                return HttpNotFound();
            }
            return View(agendaTemplate);
        }

        // GET: AgendaTemplates/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AgendaTemplates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,ActivityType,Description")] AgendaTemplate agendaTemplate)
        {
            if (ModelState.IsValid)
            {
                db.AgendaTemplates.Add(agendaTemplate);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(agendaTemplate);
        }

        // GET: AgendaTemplates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AgendaTemplate agendaTemplate = db.AgendaTemplates.Find(id);
            if (agendaTemplate == null)
            {
                return HttpNotFound();
            }
            return View(agendaTemplate);
        }

        // POST: AgendaTemplates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,ActivityType,Description")] AgendaTemplate agendaTemplate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(agendaTemplate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(agendaTemplate);
        }

        // GET: AgendaTemplates/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AgendaTemplate agendaTemplate = db.AgendaTemplates.Find(id);
            if (agendaTemplate == null)
            {
                return HttpNotFound();
            }
            return View(agendaTemplate);
        }

        // POST: AgendaTemplates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AgendaTemplate agendaTemplate = db.AgendaTemplates.Find(id);
            db.AgendaTemplates.Remove(agendaTemplate);
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
