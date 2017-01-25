using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EInvoice.CAdmin.Api
{
    public class DataAPI
    {
        public string fkey { get; set; }
        public string pattern { get; set; }
        public string serial { get; set; }
        public decimal invNo { get; set; }
    }    
}