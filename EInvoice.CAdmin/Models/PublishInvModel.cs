using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FX.Utils.MvcPaging;
using EInvoice.Core.Domain;

namespace EInvoice.CAdmin.Models
{
    public class PublishInvModel
    {
        public string Pattern { get; set; }
        public string Serial { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public IList<string> lstpattern;
        public IList<string> lstserial;
        public IPagedList<PublishInvoice> PageListPubInv;
    }
}