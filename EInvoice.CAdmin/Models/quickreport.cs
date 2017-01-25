using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EInvoice.CAdmin.Models
{
    public class quickreport
    {
        public string invno { get; set; }
        public string namecus { get; set; }
        public string Pattern { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Serial { get; set; }
        public int? Status { get; set; }
    }
}