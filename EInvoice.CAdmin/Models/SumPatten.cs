using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EInvoice.CAdmin.Models
{
    public class SumPatten
    {
        public int TemId { get; set; }
        public string InvPattern { get; set; }
        public string InvSerial { get; set; }
        public decimal SumTP { get; set; }
        public decimal SumUH { get; set; }
        public decimal SumU { get; set; }
        public decimal SumH { get; set; }
        public decimal SumC { get; set; }
    }
}