using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EInvoice.CAdmin.Api
{
    public class Business : DataAPI
    {
        public string xmlData { get; set; }
        public string Attachfile { get; set; }
        public int convert { get; set; }
    }
}