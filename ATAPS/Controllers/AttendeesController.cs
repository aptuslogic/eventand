﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ATAPS.Models;
using ATAPS.Helpers;
using System.IO;
using ImageProcessor;
using System.Drawing;
using System.Drawing.Imaging;

namespace ATAPS.Controllers
{
    public class AttendeesController : Controller
    {
        private RFIDDBEntities db = new RFIDDBEntities();

        // GET: Attendees
        public ActionResult Index(int? filter, string sortOrder, string searchString, int? pageNum)
        {
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;

            int pageSize = 25;
            ViewBag.PageSize = pageSize;
            // here we pull the query based on the sort order and direction
            List<Attendee> attendees = new List<Attendee>();

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PartSortParm = sortOrder == "ParticipantType" ? "ptype_desc" : "ParticipantType";
            ViewBag.RSVPSortParm = sortOrder == "RSVPStatus" ? "rsvp_desc" : "RSVPStatus";
            ViewBag.RFIDSortParm = sortOrder == "RFID" ? "rfid_desc" : "RFID";
            ViewBag.LastSort = sortOrder;
            if (sortOrder == "") { ViewBag.LastSort = "Name"; }

            attendees = ATAPS_Pile.GetSortedAttendeesWithFilter(filter, sortOrder, searchString, pageNum);
            #region refactored in ATAPS_Pile

            //attendees = db.Attendees.Where(o => o.EventID == filter).ToList();

            //if (!String.IsNullOrEmpty(searchString))
            //{
            //    attendees = attendees.Where(s => s.LastName.ToLower().Contains(searchString.ToLower())
            //                           || s.FirstName.ToLower().Contains(searchString.ToLower())
            //                           || s.RfID.ToLower().Contains(searchString.ToLower())).ToList();
            //}

            //ViewBag.CurSearch = searchString;

            //switch (sortOrder)
            //{
            //    case "name_desc":
            //        attendees = attendees.OrderByDescending(s => s.LastName).ToList();
            //        break;
            //    case "ParticipantType":
            //        attendees = attendees.OrderBy(s => s.ParticipantType).ToList();
            //        break;
            //    case "ptype_desc":
            //        attendees = attendees.OrderByDescending(s => s.ParticipantType).ToList();
            //        break;
            //    case "RSVPStatus":
            //        attendees = attendees.OrderBy(s => s.RSVPStatus).ToList();
            //        break;
            //    case "rsvp_desc":
            //        attendees = attendees.OrderByDescending(s => s.RSVPStatus).ToList();
            //        break;
            //    case "RFID":
            //        attendees = attendees.OrderBy(s => s.RfID).ToList();
            //        break;
            //    case "rfid_desc":
            //        attendees = attendees.OrderByDescending(s => s.RfID).ToList();
            //        break;
            //    default:
            //        attendees = attendees.OrderBy(s => s.LastName).ToList();
            //        break;
            //}

            //if (pageNum == null)
            //{
            //    ViewBag.PageNum = 1;
            //    pageNum = 1;
            //}
            //else
            //{
            //    ViewBag.PageNum = pageNum;
            //}
            //decimal pages = 1+(attendees.Count() / pageSize);
            //ViewBag.NumPages = Math.Ceiling(pages);
            //if (pageNum * pageSize > attendees.Count())
            //{
            //    int modCheck = attendees.Count() % pageSize; // get remainder
            //    int lastPage = (int)Math.Ceiling(pages); // last page
            //    ViewBag.PageNum = lastPage;
            //    attendees = attendees.Skip((lastPage - 1) * pageSize).Take(modCheck).ToList();
            //}
            //else
            //{
            //    int startAt = (pageNum ?? default(int)) - 1;
            //    attendees = attendees.Skip(startAt * pageSize).Take(pageSize).ToList();
            //}
            #endregion

            if (pageNum == null)
            {
                pageNum = 1;
            }
            decimal pages = 1 + (attendees.Count() / pageSize);
            if (pageNum * pageSize > attendees.Count())
            {
                int modCheck = attendees.Count() % pageSize; // get remainder
                int lastPage = (int)Math.Ceiling(pages); // last page
                attendees = attendees.Skip((lastPage - 1) * pageSize).Take(modCheck).ToList();
            }
            else
            {
                int startAt = (pageNum ?? default(int)) - 1;
                attendees = attendees.Skip(startAt * pageSize).Take(pageSize).ToList();
            }
            // finally populate pagination tags


            //decimal pages = 1 + (attendees.Count() / pageSize);
            int upperLim = (int)pages;
            int lowerLim = 1;
            int curPage = pageNum ?? default(int);
            ViewBag.CurPage = curPage;
            ViewBag.Pages = upperLim;
            ViewBag.LowerLim = lowerLim;
            ViewBag.UpperLim = upperLim;

            List<int> pageList = ATAPS_Pile.GetAttendeePageList(lowerLim, upperLim, curPage, attendees.Count());
            
            ViewBag.PageList = pageList;

            EventRecord eRec = db.EventRecords.Find(filter);
            ViewBag.EventName = eRec.EventName;
            //ModelState.Clear();
            return View(attendees);
        }

        // GET: Attendees/Details/5
        public ActionResult Details(int? id, int? filter)
        {
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attendee attendee = db.Attendees.Where(o => o.ID == id).FirstOrDefault();
            if (attendee == null)
            {
                return HttpNotFound();
            }
            return View(attendee);
        }

        // GET: Attendees/Create
        public ActionResult Create(int? filter)
        {
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            return View();
        }

        // POST: Attendees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ParticipantID,PrimaryID,LastName,FirstName,Email,ParticipantType,RSVPStatus,IsPrimary,AttPicture,Filename,RfID,PhoneticFirst,PhoneticLast,PreferredFirst,PreferredLast,Mobile")] Attendee attendee, int? filter)
        {
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            attendee.EventID = filter;
            if (ModelState.IsValid)
            {
                db.Attendees.Add(attendee);
                db.SaveChanges();
                return RedirectToAction("Index", new { filter = filter });
            }

            return View(attendee);
        }

        // GET: Attendees/Edit/5
        public ActionResult Edit(int? id, int? filter)
        {
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attendee attendee = db.Attendees.Where(o => o.ID == id).FirstOrDefault();
            if (attendee == null)
            {
                return HttpNotFound();
            }
            return View(attendee);
        }

        private void ResizeRotateImg (string fname)
        {
            // rotate the image if needed, and save it back out to the file
            using (System.Drawing.Image myImage = System.Drawing.Image.FromFile(fname))
            {
                if (myImage.PropertyIdList.Contains(0x0112))
                {
                    int rotationValue = myImage.GetPropertyItem(0x0112).Value[0];
                    switch (rotationValue)
                    {
                        case 1: // landscape, do nothing
                            break;

                        case 8: // rotated 90 right, so de-rotate
                            myImage.RotateFlip(rotateFlipType: RotateFlipType.Rotate270FlipNone);
                            myImage.Save(fname, ImageFormat.Jpeg);//ega this assumes jpg
                            break;

                        case 3: // bottoms up
                            myImage.RotateFlip(rotateFlipType: RotateFlipType.Rotate180FlipNone);
                            myImage.Save(fname, ImageFormat.Jpeg);//ega assumes jpg
                            break;

                        case 6: // rotated 90 left
                            myImage.RotateFlip(rotateFlipType: RotateFlipType.Rotate90FlipNone);
                            myImage.Save(fname, ImageFormat.Jpeg);//ega assumes jpg
                            break;
                    }
                }
            }

            // read the image into a buffer
            byte[] photoBytes = System.IO.File.ReadAllBytes(fname);
            int quality = 80;
            ImageProcessor.Imaging.Formats.ISupportedImageFormat format = new ImageProcessor.Imaging.Formats.JpegFormat { Quality = 70 };//ega assumes jpg
            Size size = new Size(300, 0);

            using (MemoryStream inStream = new MemoryStream(photoBytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (ImageFactory imageFactory = new ImageFactory())
                    {
                        // Load, resize, set the format and quality and save an image.
                        imageFactory.Load(inStream).Resize(size).Format(format).Quality(quality).Save(outStream);
                        System.IO.File.Delete(fname);
                        using (FileStream file = new FileStream(fname, FileMode.Create, System.IO.FileAccess.Write))
                        {
                            byte[] bytes = new byte[outStream.Length];
                            outStream.Read(bytes, 0, (int)outStream.Length);
                            file.Write(bytes, 0, bytes.Length);
                            outStream.Close();
                        }
                    }
                }
            }
        }

        // POST: Attendees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ParticipantID,PrimaryID,LastName,FirstName,Email,ParticipantType,RSVPStatus,IsPrimary,Picture,AttPicture,Filename,RfID,PhoneticFirst,PhoneticLast,PreferredFirst,PreferredLast,Mobile,EventID,ActivityListNames,WinnerQueueOrder")] Attendee attendee, int? filter)
        {
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;

            if (ModelState.IsValid)
            {
                // save previous picture, if a new one is not uploaded
                attendee.Filename = Request.Params["OrigPicture"];
                
                // save an uploaded picture, if any
                if (Request.Files.Count > 0 && (Request.Files["Picture"].ContentType == "image/jpeg" || Request.Files["Picture"].ContentType == "image/png"))
                {
                    // set local and web path of uploaded photo
                    string local_path = Server.MapPath("~") + "\\Content\\attendee_photos\\";
                    string web_path = "/Content/attendee_photos/";
                    string prefix = "attendee_photo_" + attendee.ID;
                    string suffix = (Request.Files["Picture"].ContentType == "image/jpeg") ? ".jpg" : ".png";
                    string local_fname = local_path + prefix + suffix;
                    string url = web_path + prefix + suffix;

                    // save the uploaded file
                    var uploadedFile = Request.Files["Picture"];
                    System.IO.File.Delete(local_fname);
                    uploadedFile.SaveAs(local_fname);

                    // resize and rotate the image
                    this.ResizeRotateImg(local_fname);

                    // set the url in the Attendees record
                    attendee.Filename = url;
                }

                db.Entry(attendee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { filter = filter });
            }
            return View(attendee);
        }

        // GET: Attendees/Delete/5
        public ActionResult Delete(int? id, int? filter)
        {
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attendee attendee = db.Attendees.Where(o => o.ID == id).FirstOrDefault();
            if (attendee == null)
            {
                return HttpNotFound();
            }
            return View(attendee);
        }

        // POST: Attendees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int? filter)
        {
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            Attendee attendee = db.Attendees.Where(o => o.ID == id).FirstOrDefault();
            db.Attendees.Remove(attendee);
            db.SaveChanges();
            return RedirectToAction("Index", new { filter = filter });
        }

        public ActionResult Upload(int? filter)
        {
            // Returns the Attendee XLS Upload Page.  Includes a download link for the raw XLS
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            EventRecord eRec = db.EventRecords.Where(o => o.ID == filter).FirstOrDefault();
            return View(eRec);
        }

        [HttpPost, ActionName("Upload")]
        //[ValidateAntiForgeryToken]
        public ActionResult UploadConfirmed(HttpPostedFileBase file, int? filter)
        {
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            if (file.ContentLength > 0)
            {

                List<Attendee> attendeeList = ATAPS_Pile.ParseAttendeeXLS(file, filter ?? default(int));
                // save the names
                foreach (Attendee item in attendeeList)
                {
                    // try to pull the Attendee by ParticipantID and EventID
                    Attendee already = db.Attendees.Where(o => o.ParticipantID == item.ParticipantID && o.EventID == filter).FirstOrDefault();
                    if (already != null)
                    {
                        if (already.ID != 0)
                        {
                            already.ActivityListNames = item.ActivityListNames;
                            already.AttPicture = item.AttPicture;
                            already.Email = item.Email;
                            already.EventID = item.EventID;
                            already.Filename = item.Filename;
                            already.FirstName = item.FirstName;
                            already.IsPrimary = item.IsPrimary;
                            already.LastName = item.LastName;
                            already.Mobile = item.Mobile;
                            already.ParticipantID = item.ParticipantID;
                            already.ParticipantType = item.ParticipantType;
                            already.PhoneticFirst = item.PhoneticFirst;
                            already.PhoneticLast = item.PhoneticLast;
                            already.PreferredFirst = item.PreferredFirst;
                            already.PreferredLast = item.PreferredLast;
                            already.PrimaryID = item.PrimaryID;
                            already.RfID = item.RfID;
                            already.RSVPStatus = item.RSVPStatus;

                            db.Entry(already).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        db.Attendees.Add(item);
                    }

                }
                db.SaveChanges();
            }

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
