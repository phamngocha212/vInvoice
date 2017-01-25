using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EInvoice.Core.Domain;

namespace EInvoice.CAdmin.Models
{
    public class AdjustModel
    {
        public string pattern { get; set; }
        public string serial { get; set; }
        public string invNo { get; set; }
        public string type { get; set; }
        public string typeName { get; set; }
        public SelectList lstpattern { get; set; }
        public SelectList lstserial { get; set; }
        public Company currentcom { get; set; }
    }
}