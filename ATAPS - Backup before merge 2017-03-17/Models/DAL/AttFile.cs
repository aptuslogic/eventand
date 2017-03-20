using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OfficeOpenXml;
using System.IO;
using System.Web;

namespace ATAPS.Models.DAL
{
    public class AttFile
    {
        public int Id { get; set; }
        
        public string FileName { get; set; }
        
        public string ContentType { get; set; }

        public byte[] Content { get; set; }

        public string FileType { get; set; }
        
        public int EventID { get; set; }

        internal void SetExcelContent(ExcelPackage excelFile)
        {
            ContentType = "application/octet-stream";
            Content = excelFile.GetAsByteArray();
        }
    }
}