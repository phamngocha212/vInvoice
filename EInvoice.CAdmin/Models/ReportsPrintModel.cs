using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EInvoice.CAdmin.Models
{
    public class ReportsPrintModel
    {
        public string Html { get; set; }
        public ReportsPrintModel(string html) {
            Html = html;
        }
    }
}