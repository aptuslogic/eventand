﻿using System.Data;
using System.Data.Entity;
using ATAPS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using ATAPS.Models.DisplayObject;
using ATAPS.Helpers;

namespace ATAPS.Controllers
{
    public class WinnersController : Controller
    {
        private RFIDDBEntities db = new RFIDDBEntities();

        // GET: Winners
        public ActionResult Index()
        {
            int filter = int.Parse(ConfigurationManager.AppSettings["ActiveEvent"]);
            ViewBag.FilterID = filter;
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
            return View();
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

        // GET: Winners/Prev
        public ActionResult Prev()
        {
            // init result
            IDictionary<string, string> data = new Dictionary<string, string>();

            // get current queue position
            Parm parm = db.Parms.Where(x => x.ParmName == "CurrWinnerQueuePos").First();
            int currQueuePos = int.Parse(parm.ParmValue);

            // find the previous valid queue position
            List<Attendee> attendees = new List<Attendee>();
            attendees = db.Attendees.Where(x => x.WinnerQueueOrder <= currQueuePos).OrderByDescending(x => x.WinnerQueueOrder).ToList();

            // update the queue position
            if (attendees.Count >= 2)
            {
                parm.ParmValue = attendees[1].WinnerQueueOrder.ToString();
                db.SaveChanges();
                data["status"] = "success";
            }
            else
            {
                data["status"] = "failure";
            }

            // return json result
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        
        // GET: Winners/Next
        public ActionResult Next()
        {
            // init result
            IDictionary<string, string> data = new Dictionary<string, string>();

            // get current queue position
            Parm parm = db.Parms.Where(x => x.ParmName == "CurrWinnerQueuePos").First();
            int currQueuePos = int.Parse(parm.ParmValue);

            // find the next valid queue position
            List<Attendee> attendees = new List<Attendee>();
            attendees = db.Attendees.Where(x => x.WinnerQueueOrder >= currQueuePos).OrderBy(x => x.WinnerQueueOrder).ToList();

            // update the queue position
            if (attendees.Count >= 2)
            {
                parm.ParmValue = attendees[1].WinnerQueueOrder.ToString();
                db.SaveChanges();
                data["status"] = "success";
            }
            else
            {
                parm.ParmValue = "9999";
                db.SaveChanges();
                data["status"] = "failure";
            }

            // return json result
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: Winners/Reset
        public ActionResult Reset()
        {
            // reset the queue position
            Parm parm = db.Parms.Where(x => x.ParmName == "CurrWinnerQueuePos").First();
            parm.ParmValue = "0";
            db.SaveChanges();

            // return json result
            IDictionary<string, string> data = new Dictionary<string, string>();
            data["status"] = "success";
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // Get the current parade status (true = parade is running, false = parade is paused)
        private bool GetParadeStatus ()
        {
            List<Parm> parms = new List<Parm>();
            parms = db.Parms.Where(x => x.ParmName == "ParadePaused").ToList();
            if (parms.Count == 0 || parms[0].ParmValue == "false")
            {
                return (true);
            }
            else
            {
                return (false);
            }
        }

        // Set the current parade status (true = parade is running, false = parade is paused)
        private void SetParadeStatus (bool status)
        {
            List<Parm> parms = new List<Parm>();
            parms = db.Parms.Where(x => x.ParmName == "ParadePaused").ToList();
            if (parms.Count > 0)
            {
                parms[0].ParmValue = status ? "false" : "true";
                db.SaveChanges();
            }
            else
            {
                Parm parm = new Parm ();
                parm.ParmName = "ParadePaused";
                parm.ParmValue = status ? "false" : "true";
                db.Parms.Add(parm);
                db.SaveChanges();
            }
        }

        // GET: Winners/PauseResume
        public ActionResult PauseResume()
        {
            // toggle the parade status
            bool status = this.GetParadeStatus();
            this.SetParadeStatus(!status);

            // return success
            IDictionary<string, string> data = new Dictionary<string, string>();
            data["status"] = "success";
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: Winners/WinnersUpdate
        public ActionResult WinnersUpdate()
        {
            // get current queue position
            Parm parm = db.Parms.Where(x => x.ParmName == "CurrWinnerQueuePos").First();
            int currQueuePos = int.Parse (parm.ParmValue);
            IDictionary<string, string> data = new Dictionary<string, string>();
            if (!(this.GetParadeStatus()) || currQueuePos == 9999)
            {
                data["can_click_prev"] = "false";
                data["can_click_next"] = "false";
                data["can_click_reset"] = "true";
                data["status"] = "failure";
                data["parade_status"] = this.GetParadeStatus() ? "true" : "false";
            }
            else
            {
                // grab a record from the database
                List<Attendee> attendees = new List<Attendee>();
                attendees = db.Attendees.Where(x => x.WinnerQueueOrder >= currQueuePos).OrderBy(x => x.WinnerQueueOrder).ToList();
                if (attendees.Count == 0)
                {
                    data["can_click_prev"] = "false";
                    data["can_click_next"] = "false";
                    data["can_click_reset"] = "true";
                    data["status"] = "failure";
                    data["parade_status"] = this.GetParadeStatus() ? "true" : "false";
                }
                else
                {
                    int? wqo = attendees[0].WinnerQueueOrder;

                    // store it in a result array
                    if (attendees.Count >= 1)
                    {
                        data["curr_id"] = attendees[0].ID.ToString();
                        data["curr_name"] = attendees[0].FirstName + " " + attendees[0].LastName;
                        data["curr_phonetic"] = attendees[0].PhoneticFirst + " " + attendees[0].PhoneticLast;
                        data["curr_preferred"] = attendees[0].PreferredFirst + " " + attendees[0].PreferredLast;
                        data["curr_picture"] = attendees[0].Filename;
                        data["curr_title"] = attendees[0].Title;
                        data["curr_dept"] = attendees[0].Department;
                        if (attendees.Count >= 2)
                        {
                            data["next_id"] = attendees[1].ID.ToString();
                            data["next_name"] = attendees[1].FirstName + " " + attendees[1].LastName;
                            data["next_phonetic"] = attendees[1].PhoneticFirst + " " + attendees[1].PhoneticLast;
                            data["next_preferred"] = attendees[1].PreferredFirst + " " + attendees[1].PreferredLast;
                            data["next_picture"] = attendees[1].Filename;
                            data["next_title"] = attendees[1].Title;
                            data["next_dept"] = attendees[1].Department;
                        }
                    }

                    // see if are at the start or end of the list
                    attendees = db.Attendees.Where(x => x.WinnerQueueOrder != null).OrderBy(x => x.WinnerQueueOrder).ToList();
                    if (attendees.Count > 0)
                    {
                        data["can_click_reset"] = (currQueuePos <= attendees[0].WinnerQueueOrder) ? "false" : "true";
                        data["can_click_prev"] = (currQueuePos <= attendees[0].WinnerQueueOrder) ? "false" : "true";
                        data["can_click_next"] = (currQueuePos >= attendees[attendees.Count - 1].WinnerQueueOrder) ? "false" : "true";
                    }
                    else
                    {
                        data["can_click_reset"] = "false";
                        data["can_click_prev"] = "false";
                        data["can_click_next"] = "false";
                    }

                    // return current queue position
                    data["curr_queue_pos"] = wqo.ToString();

                    // return success
                    data["status"] = "success";

                    // return parade status
                    data["parade_status"] = this.GetParadeStatus() ? "true" : "false";
                }
            }

            // json encode and return it
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: Winners/ResetOrder
        public ActionResult ResetOrder()
        {
            // read all attendees and reset their queue order
            List<Attendee> attendees = new List<Attendee>();
            attendees = db.Attendees.ToList();
            foreach (Attendee attendee in attendees)
            {
                attendee.WinnerQueueOrder = null;
            }

            // save changes
            db.SaveChanges();

            // redirect back to admin page
            return RedirectToAction("Admin", new {});
        }
    }
}