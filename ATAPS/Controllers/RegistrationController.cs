using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ATAPS.Models.DisplayObject;
using ATAPS.Models;
using ATAPS.Helpers;
using System.Configuration;
using System.Drawing;
using System.Text;
using SuperSignature;

namespace ATAPS.Controllers
{
    public class RegistrationController : Controller
    {
        private RFIDDBEntities db = new RFIDDBEntities();

        // NOTE:  This section will control automated generation of Agenda / Events for
        // The registration process.  Business logic will be in ATAPS_Pile.cs

        // GET: Registration
        public ActionResult Index()
        {
            int temp = 0; // force Git change
            return View();
        }

        // GET:  Registration/Busses
        public ActionResult Busses()
        {
            Parm checkinParm = db.Parms.Where(o => o.ParmName == "RegistrationAgendaID").FirstOrDefault();
            if(checkinParm == null)
            {
                bool good = ATAPS_Pile.CreateRegistrationAgenda();
            }

            checkinParm = db.Parms.Where(o => o.ParmName == "RegistrationAgendaID").FirstOrDefault();
            AgendaDisplayObject regAgenda = DBHelper.GetAgendaWithDataByID(int.Parse(checkinParm.ParmValue));
            return View(regAgenda);
        }

        public ActionResult BulkCheckin(string searchString, string busNum)
        {
            ViewBag.BusNumber = busNum;

            int? pageNum = 1;
            int pageSize = 25;
            ViewBag.PageSize = pageSize;
            // here we pull the query based on the sort order and direction
            List<Attendee> attendees = new List<Attendee>();

            string sortOrder = "name";
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.QueueSortParm = sortOrder == "WinnerQueueOrder" ? "wqo_desc" : "WinnerQueueOrder";
            ViewBag.PartSortParm = sortOrder == "ParticipantType" ? "ptype_desc" : "ParticipantType";
            ViewBag.RSVPSortParm = sortOrder == "RSVPStatus" ? "rsvp_desc" : "RSVPStatus";
            ViewBag.RFIDSortParm = sortOrder == "RFID" ? "rfid_desc" : "RFID";
            ViewBag.LastSort = sortOrder;
            if (sortOrder == "") { ViewBag.LastSort = "Name"; }

            // find max queue order and add one
            List<Attendee> all_attendees = new List<Attendee>();
            all_attendees = db.Attendees.OrderBy(x => x.WinnerQueueOrder).ToList();
            int? last_queue_position = all_attendees[all_attendees.Count - 1].WinnerQueueOrder;
            int next_queue_position = (last_queue_position == null) ? 1 : (int)(last_queue_position + 1);

            // get attendees matching the search string
            int eventId = int.Parse(ConfigurationManager.AppSettings["ActiveEvent"]);
            attendees = ATAPS_Pile.GetSortedAttendeesWithFilter(eventId, sortOrder, searchString, pageNum);

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

            return View(attendees);
        }

        // GET: /Registration/SignWaiver
        [HttpGet]
        public ActionResult SignWaiver()
        {
            // load in the waiver
            int waiver_id = Int32.Parse(Request["waiver"]);
            Parm parm = db.Parms.Where(x => x.ID == waiver_id).FirstOrDefault();
            int commaPos = parm.ParmValue.IndexOf(',');
            string url = parm.ParmValue.Substring(0, commaPos);
            string name = parm.ParmValue.Substring(commaPos + 1);
            ViewBag.waiver = new Waiver(parm.ID, url, name, null);

            // load in the attendee
            int id = Int32.Parse(Request["attendee"]);
            Attendee attendee = db.Attendees.Where(x => x.ID == id).FirstOrDefault();
            ViewBag.attendee = attendee;

            // show the view
            return (View());
        }

        // POST: /Registration/ReceiveGiftCard
        [HttpPost]
        public ActionResult ReceiveGiftCard()
        {
            // get attendee info
            int id = Int32.Parse(Request["id"]);
            Attendee attendee = db.Attendees.Where(x => x.ID == id).FirstOrDefault();

            // get card number
            string cardNum = Request["CardNumber"];

            // store the card in the database
            Parm parm = new Parm();
            parm.ParmName = "GiftCard-" + attendee.ID;
            parm.ParmValue = cardNum;
            db.Parms.Add(parm);
            db.SaveChanges();

            // store the signature
            string sig_fname = Server.MapPath("~") + "\\Content\\gift_card_signatures\\" + attendee.LastName + attendee.FirstName + "-GiftCardSig-" + cardNum + ".png";
            SaveSig(sig_fname);

            // show the view
            return RedirectToAction("IndivCheckin", new { id = id });
        }

        // GET: /Registration/AddGiftCard
        public ActionResult AddGiftCard ()
        {
            // get attendee info
            int id = Int32.Parse(Request["id"]);
            Attendee attendee = db.Attendees.Where(x => x.ID == id).FirstOrDefault();
            ViewBag.attendee = attendee;

            // show the view
            return (View());
        }

        // POST: /Registration/SignWaiver
        [HttpPost]
        public ActionResult ReceiveSignedWaiver ()
        {
            // get attendee
            int id = Int32.Parse(Request["id"]);
            Attendee attendee = db.Attendees.Where(x => x.ID == id).FirstOrDefault();

            // get waiver
            int waiver_id = Int32.Parse(Request["waiver"]);
            Parm parm = db.Parms.Where(x => x.ID == waiver_id).FirstOrDefault();
            int commaPos = parm.ParmValue.IndexOf(',');
            string url = parm.ParmValue.Substring(0, commaPos);
            string name = parm.ParmValue.Substring(commaPos + 1);

            // save signature
            string name_encoded = new string(name.Where(Char.IsLetter).ToArray());
            string sig_fname = Server.MapPath("~") + "Content\\activity_waivers\\" + attendee.LastName + attendee.FirstName + "-WaiverSig-" + name_encoded + ".png";
            SaveSig(sig_fname);

            // redirect back to waivers
            return RedirectToAction("IndivCheckin", new { id = id });
        }

        // GET: /Registration/IndivCheckin
        public ActionResult IndivCheckin(int id)
        {
            // pass the attendee to the view
            Attendee attendee = db.Attendees.Where(x => x.ID == id).FirstOrDefault();
            ViewBag.attendee = attendee;

            // pass list of registered gift cards
            ViewBag.giftcards = GiftCard.GetIssued(db, attendee);

            // pass a list of waivers
            List<Parm> parms = db.Parms.Where(x => x.ParmName.StartsWith("ActivityWaiver-")).ToList();
            ViewBag.waivers = new List<Waiver>();
            foreach (Parm parm in parms)
            {
                // get the activity id
                int activityId = Int32.Parse(parm.ParmName.Substring(parm.ParmName.IndexOf('-') + 1));

                // parse out the url and name
                int commaPos = parm.ParmValue.IndexOf(',');
                string url = parm.ParmValue.Substring(0, commaPos);
                string name = parm.ParmValue.Substring(commaPos + 1);

                // check for existing signature
                string name_encoded = new string (name.Where(Char.IsLetter).ToArray());
                string sig_url = "/Content/activity_waivers/" + attendee.LastName + attendee.FirstName + "-WaiverSig-" + name_encoded + ".png";
                string sig_fname = Server.MapPath("~") + "Content\\activity_waivers\\" + attendee.LastName + attendee.FirstName + "-WaiverSig-" + name_encoded + ".png";
                if (!System.IO.File.Exists (sig_fname))
                {
                    sig_url = null;
                }

                // store this waiver in the list
                ViewBag.waivers.Add(new Waiver(parm.ID, url, name, sig_url));
            }

            // display the view
            return (View());
        }

        public ActionResult BulkRfid (int busID)
        {
            return (View());
        }

        public ActionResult TableCheckin()
        {
            return View();
        }

        public ActionResult CreateBus(int filter)
        {

            ViewBag.FilterID = -1;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBus([Bind(Include = "ID,ActivityName,AgendaID")] Activity activity, int? filter)
        {
            if (filter == null) { return HttpNotFound(); }
            ViewBag.FilterID = filter;
            activity.AgendaID = filter ?? default(int);
            Parm checkinParm = db.Parms.Where(o => o.ParmName == "RegistrationAgendaID").FirstOrDefault();
            activity.AgendaID = int.Parse(checkinParm.ParmValue);
            activity.ActivityTypeID = 1;
            if (ModelState.IsValid)
            {
                db.Activities.Add(activity);
                db.SaveChanges();
                return RedirectToAction("Busses");
            }

            return View(activity);
        }

        // this function saves a Super Signature sig file
        void SaveSig (string fname)
        {
            // receive and convert incoming data
            string signData = Request["ctlSignature_data"];
            string signDataSmooth = Request["ctlSignature_data_smooth"];
            MouseSignature ctlSignature = new MouseSignature();
            byte[] arrayOfBytes = Convert.FromBase64String(signData);
            signData = Encoding.UTF8.GetString(arrayOfBytes);
            byte[] arrayOfBytesSmooth = Convert.FromBase64String(signDataSmooth);
            signDataSmooth = Encoding.UTF8.GetString(arrayOfBytesSmooth);
            ctlSignature.SignDataSmooth = signDataSmooth;

            // generate bitmap
            Bitmap bmpSign = ctlSignature.GenerateSignature(signData, "");

            // save a file
            using (FileStream fs = System.IO.File.Create(fname))
            {
                bmpSign.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        // GET: /Registration/Monitor
        public ActionResult Monitor()
        {
            // get all attendees
            List<Attendee> attendees = db.Attendees.ToList();

            // convert to monitor items
            List<MonitorItem> items = new List<MonitorItem>();
            foreach (Attendee attendee in attendees)
            {
                MonitorItem item = new MonitorItem(attendee.FirstName + " " + attendee.LastName, attendee.RfID, attendee.Filename, attendee.Mobile, attendee.ActivityListNames, "msg goes here");//ega msg
                items.Add(item);
            }
            ViewBag.MonitorItems = items;

            //ega show actual thumbnail, not filename
            //ega gift cards: show each, with card number and link to signature
            //ega waiver: show each, with name, link to doc, and link to sig
            //ega show waivers not signed
            //ega have a refresh link, make datatable preserve its settings
            //ega is there anything else they want to show here?
            //ega have form to add rfid

            return (View());
        }
    }

    public class Waiver
    {
        public int id;
        public string waiver_url;
        public string waiver_name;
        public string signature_url;

        public Waiver (int id_param, string waiver_url_param, string waiver_name_param, string signature_url_param)
        {
            id = id_param;
            waiver_url = waiver_url_param;
            waiver_name = waiver_name_param;
            signature_url = signature_url_param;
        }
    }

    public class GiftCard
    {
        public string signature_url;
        public string card_number;

        public GiftCard (string signature_url_param, string card_number_param)
        {
            signature_url = signature_url_param;
            card_number = card_number_param;
        }

        public static List<GiftCard> GetIssued (RFIDDBEntities db, Attendee attendee)
        {
            // start an empty list
            List<GiftCard> results = new List<GiftCard>();

            // get all the parms
            string name = "GiftCard-" + attendee.ID;
            List<Parm> parms = db.Parms.Where(x => x.ParmName == name).ToList();

            // convert the parms to GiftCard objects
            foreach (Parm parm in parms)
            {
                string number = parm.ParmValue;
                string sig_url = "/Content/gift_card_signatures/" + attendee.LastName + attendee.FirstName + "-GiftCardSig-" + number + ".png";
                results.Add(new GiftCard(sig_url, number));
            }

            // return the list
            return (results);
        }
    }

    public class MonitorItem
    {
        public string name;
        public string rfid;
        public string thumbnail;
        public string mobile;
        public string activity_list;
        public string msg;

        public MonitorItem (string name_param, string rfid_param, string thumbnail_param, string mobile_param, string activity_list_param, string msg_param)
        {
            name = name_param;
            rfid = rfid_param;
            thumbnail = thumbnail_param;
            mobile = mobile_param;
            activity_list = activity_list_param;
            msg = msg_param;
        }
    }
}