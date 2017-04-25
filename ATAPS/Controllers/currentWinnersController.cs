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
    public class currentWinnersController : Controller
    {
        private RFIDDBEntities db = new RFIDDBEntities();

        // GET: currentWinners
        public ActionResult Index()
        {
            return View(db.currentWinners.ToList());
        }

        // GET: currentWinners/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            currentWinner currentWinner = db.currentWinners.Where(o => o.winnerID == id).First();
            if (currentWinner == null)
            {
                return HttpNotFound();
            }
            return View(currentWinner);
        }

        // GET: currentWinners/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: currentWinners/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "winnerID,attendeeID")] currentWinner currentWinner)
        {
            if (ModelState.IsValid)
            {
                db.currentWinners.Add(currentWinner);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(currentWinner);
        }

        // GET: currentWinners/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            currentWinner currentWinner = db.currentWinners.Where(o => o.winnerID == id).First();
            if (currentWinner == null)
            {
                return HttpNotFound();
            }
            return View(currentWinner);
        }

        // POST: currentWinners/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "winnerID,attendeeID")] currentWinner currentWinner)
        {
            if (ModelState.IsValid)
            {
                db.Entry(currentWinner).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(currentWinner);
        }

        // GET: currentWinners/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            currentWinner currentWinner = db.currentWinners.Where(o => o.winnerID == id).First();
            if (currentWinner == null)
            {
                return HttpNotFound();
            }
            return View(currentWinner);
        }

        // POST: currentWinners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            currentWinner currentWinner = db.currentWinners.Where(o => o.winnerID == id).First();
            db.currentWinners.Remove(currentWinner);
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
