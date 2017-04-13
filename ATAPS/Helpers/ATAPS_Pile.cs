using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ATAPS.Models;
using OfficeOpenXml;
using System.Configuration;

namespace ATAPS.Helpers
{
    public class ATAPS_Pile
    {

        public static string GenerateEventShortcode(string clientName)
        {
            string retVal = "";

            string[] parsed = clientName.Split(' ');
            foreach (string word in parsed)
            {
                retVal += word.Substring(0, 1);
            }

            retVal += "-";
            retVal += DateTime.Now.Year.ToString();

            return retVal;
        }

        public static List<Attendee> GetSortedAttendeesWithFilter(int? filter, string sortOrder, string searchString, int? pageNum)
        {
            RFIDDBEntities db = new RFIDDBEntities();
            List<Attendee> retVal = new List<Attendee>();

            int pageSize = 25;
            // here we pull the query based on the sort order and direction
            List<Attendee> attendees = new List<Attendee>();
            attendees = db.Attendees.Where(o => o.EventID == filter).ToList();

            if (!String.IsNullOrEmpty(searchString))
            {
                attendees = attendees.Where(s => s.LastName.ToLower().Contains(searchString.ToLower())
                                       || s.FirstName.ToLower().Contains(searchString.ToLower())
                                       || s.RfID.ToLower().Contains(searchString.ToLower())).ToList();
            }

            switch (sortOrder)
            {
                case "name_desc":
                    attendees = attendees.OrderByDescending(s => s.LastName).ToList();
                    break;
                case "WinnerQueueOrder":
                    attendees = attendees.OrderByDescending(s => s.WinnerQueueOrder.HasValue).ThenBy(s => s.WinnerQueueOrder).ToList();
                    break;
                case "wqo_desc":
                    attendees = attendees.OrderByDescending(s => s.WinnerQueueOrder.HasValue).ThenByDescending(s => s.WinnerQueueOrder).ToList();
                    break;
                case "ParticipantType":
                    attendees = attendees.OrderBy(s => s.ParticipantType).ToList();
                    break;
                case "ptype_desc":
                    attendees = attendees.OrderByDescending(s => s.ParticipantType).ToList();
                    break;
                case "RSVPStatus":
                    attendees = attendees.OrderBy(s => s.RSVPStatus).ToList();
                    break;
                case "rsvp_desc":
                    attendees = attendees.OrderByDescending(s => s.RSVPStatus).ToList();
                    break;
                case "RFID":
                    attendees = attendees.OrderBy(s => s.RfID).ToList();
                    break;
                case "rfid_desc":
                    attendees = attendees.OrderByDescending(s => s.RfID).ToList();
                    break;
                default:
                    attendees = attendees.OrderBy(s => s.LastName).ToList();
                    break;
            }



            retVal = attendees;
            return retVal;
        }

        internal static bool CreateRegistrationAgenda()
        {
            // no Registration Agenda has yet been created for this Event

            RFIDDBEntities db = new RFIDDBEntities();
            int accessEventID = int.Parse(ConfigurationManager.AppSettings["ActiveEvent"]);
            Parm checkinParm = new Parm();

            List<EventDate> eDates = db.EventDates.Where(o => o.EventRecordsID == accessEventID).OrderBy(o => o.EventDate1).ToList();

            checkinParm.ParmName = "RegistrationAgendaID";
            Agenda regAg = new Agenda();
            regAg.AgendaName = "Registration";
            regAg.AgendaOrder = 0;
            regAg.AgendaType = 1;
            regAg.EventDateID = eDates[0].ID;

            db.Agendas.Add(regAg);
            db.SaveChanges();

            checkinParm.ParmValue = regAg.ID.ToString();
            db.Parms.Add(checkinParm);
            db.SaveChanges();

            return true;
        }

        internal static int GetActivityAttendeeCurrentCountByActivityID(int actID, int agnID)
        {
            List<AttendeeLastCheck> latest = GetActivityAttendeeCurrentByActivityID(actID, agnID);
            int retVal = latest.Count();
            return retVal;
        }

        public static List<Attendee> GetTotalAgendaAttendanceByID(int agnID)
        {
            RFIDDBEntities db = new RFIDDBEntities();
            List<AttendeeLastCheck> checks = db.AttendeeLastChecks.Where(o => o.AgendaID == agnID).ToList();

            List<Attendee> retVal = db.Attendees.Where(p => checks.Any(p2 => p2.AttendeeID == p.ID)).ToList();

            return retVal;
        }

        public static List<AttendeeLastCheck> GetActivityAttendeeCurrentByActivityID(int actID, int agnID)
        {
            RFIDDBEntities db = new RFIDDBEntities();
            int retVal = 0;

            // NOTE:  I know there's a cleaner way to pull this list without having to loop through it, but I'm coming up with blank.  We should take another look at this when
            // we have time to ponder the problem.  I know this is close, but should be possible to do without the loop.
            // UPDATE:  I added a LINQ query that seems to be right, but still have to loop through the var to get the count.
            List<AttendeeLastCheck> checks = db.AttendeeLastChecks.Where(o => o.LastActivity == actID).OrderBy(o => o.AttendeeID).ThenByDescending(o => o.LastUpdate).ToList();

            //var res = from element in checks
            //          group element by element.AttendeeID
            //  into groups
            //          select groups.OrderBy(p => p.LastUpdate).First();

            //int temp = 0;

            //foreach(var item in res)
            //{
            //    if(item.CheckDir == "In") { temp++; }
            //}

            List<AttendeeLastCheck> latest = new List<AttendeeLastCheck>();
            int lastID = -1;
            foreach (AttendeeLastCheck aCh in checks)
            {
                if (aCh.AttendeeID != lastID)
                {
                    lastID = aCh.AttendeeID;
                    if (aCh.CheckDir == "In")
                    {
                        latest.Add(aCh);
                    }
                }
            }

            return latest;
        }

        public static List<int> GetAttendeePageList(int lowerLim, int upperLim, int curPage, int attCount)
        {
            List<int> retVal = new List<int>();

            // finally populate pagination tags

            List<int> pageList = new List<int>();

            int startAtPg = curPage - 2;
            int endAt = curPage + 3;
            if (curPage < 3) { endAt += 1; }
            if (curPage < 2) { endAt += 1; }
            if (curPage + 2 > upperLim) { startAtPg -= 1; }
            if (curPage + 1 > upperLim) { startAtPg -= 1; }

            for (int i = startAtPg; i < endAt; i++)
            {
                if (i >= lowerLim && i <= upperLim)
                {
                    pageList.Add(i);
                }
            }

            return pageList;
        }

        internal static List<Attendee> ParseAttendeeXLS(HttpPostedFileBase file, int EventID)
        {
            List<Attendee> retVal = new List<Attendee>();

            ExcelPackage package = new ExcelPackage(file.InputStream);

            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

            for (var rowNumber = 2; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
            {
                try // wrapping this in a try / catch in case there are any "null" rows in the excel doc.
                {
                    var row = workSheet.Cells[rowNumber, 1, rowNumber, workSheet.Dimension.End.Column];
                    Attendee toAdd = new Attendee();
                    // put the population logic here
                    toAdd.LastName = (workSheet.Cells[rowNumber, 1].Value != null) ? workSheet.Cells[rowNumber, 1].Value.ToString() : "";
                    toAdd.FirstName = (workSheet.Cells[rowNumber, 2].Value != null) ? workSheet.Cells[rowNumber, 2].Value.ToString() : "";
                    toAdd.ParticipantID = int.Parse((workSheet.Cells[rowNumber, 3].Value != null) ? workSheet.Cells[rowNumber, 3].Value.ToString() : "");
                    toAdd.PrimaryID = int.Parse((workSheet.Cells[rowNumber, 4].Value != null) ? workSheet.Cells[rowNumber, 4].Value.ToString() : "");
                    toAdd.Email = (workSheet.Cells[rowNumber, 5].Value != null) ? workSheet.Cells[rowNumber, 5].Value.ToString() : "";
                    toAdd.ParticipantType = (workSheet.Cells[rowNumber, 6].Value != null) ? workSheet.Cells[rowNumber, 6].Value.ToString() : "";
                    toAdd.RSVPStatus = (workSheet.Cells[rowNumber, 7].Value != null) ? workSheet.Cells[rowNumber, 7].Value.ToString() : "";
                    toAdd.IsPrimary = false;
                    if (int.Parse((workSheet.Cells[rowNumber, 8].Value != null) ? workSheet.Cells[rowNumber, 8].Value.ToString() : "") == 1) { toAdd.IsPrimary = true; };
                    toAdd.RfID = (workSheet.Cells[rowNumber, 9].Value != null) ? workSheet.Cells[rowNumber, 9].Value.ToString() : "";
                    toAdd.PhoneticFirst = (workSheet.Cells[rowNumber, 10].Value != null) ? workSheet.Cells[rowNumber, 10].Value.ToString() : "";
                    toAdd.PhoneticLast = (workSheet.Cells[rowNumber, 11].Value != null) ? workSheet.Cells[rowNumber, 11].Value.ToString() : "";
                    toAdd.PreferredFirst = (workSheet.Cells[rowNumber, 12].Value != null) ? workSheet.Cells[rowNumber, 12].Value.ToString() : "";
                    toAdd.PreferredLast = (workSheet.Cells[rowNumber, 13].Value != null) ? workSheet.Cells[rowNumber, 13].Value.ToString() : "";
                    toAdd.Mobile = (workSheet.Cells[rowNumber, 14].Value != null) ? workSheet.Cells[rowNumber, 14].Value.ToString() : "";
                    toAdd.Title = (workSheet.Cells[rowNumber, 15].Value != null) ? workSheet.Cells[rowNumber, 15].Value.ToString() : "";
                    toAdd.Department = (workSheet.Cells[rowNumber, 16].Value != null) ? workSheet.Cells[rowNumber, 16].Value.ToString() : "";
                    toAdd.ActivityListNames = (workSheet.Cells[rowNumber, 17].Value != null) ? workSheet.Cells[rowNumber, 17].Value.ToString() : "";
                    toAdd.EventID = EventID;

                    retVal.Add(toAdd);
                }
                catch { }
            }
            return retVal;

        }
    }
}