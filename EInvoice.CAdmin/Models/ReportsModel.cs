using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EInvoice.CAdmin.Models
{
    public class ReportsModel
    {
        public int month { get; set; }
        public int year { get; set;}
        public SelectList lstPattern { get; set; }
        public SelectList lstMonth { get; set; }
        public SelectList lstYear { get; set; }
    }

    public class ReportsDetailModel {
        public string Pattern { get; set; }
        public string Serial { get; set; }
        public int Status { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public SelectList lstPattern { get; set; }
        public SelectList lstSerial { get; set; }
        public string Html { get; set; }
        public int pageSize { get; set; }
        public int pageIndex { get; set; }
        public int totalRecords { get; set; }
    }

    public class ReportsLaunchModel {
        public string Pattern { get; set; }
        public string Serial { get; set; }
        public string InvNo { get; set; }
        public string CreateBy { get; set; }
        public string PublishBy { get; set; }
        public string PublishDate { get; set; }
        public SelectList lstPattern { get; set; }
        public SelectList lstSerial { get; set; }
        public string Html { get; set; }
        public int pageSize { get; set; }
        public int pageIndex { get; set; }
        public int totalRecords { get; set; }
    }
}