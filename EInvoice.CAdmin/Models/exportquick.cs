using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EInvoice.CAdmin.Models
{
    public class exportquick
    {
        public int stt { get; set; }
        public string Pattern { get; set; }
        public string Serial { get; set; }
        public string invno { get; set; }
        public string PublishDate { get; set; }
        public string Customer { get; set; }
        public string Status { get; set; }
    }
}