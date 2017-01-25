using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EInvoice.CAdmin.Api
{
    public class PortalAPI : DataAPI
    {
        public string cusCode { get; set; }
        public string token { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public string pattern { get; set; }
        public string serial { get; set; }
        public string invNumber { get; set; }
        public int? invStatus { get; set; }
        public int? page { get; set; }
        public int? cussignStatus { get; set; }
        public int? payment { get; set; }
        public string invToken { get; set; }
        public string signValue { get; set; }
        public string accountName { get; set; }
    }
}