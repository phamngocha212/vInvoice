using EInvoice.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EInvoice.CAdmin.Models
{
    public class PublishModel
    {
        public Publish mPublish { get; set; }
        public string PubInvoiceList { get; set; }
        public SelectList TaxList { get; set; }
        public SelectList RegTempList { get; set; }
    }
}