using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EInvoice.CAdmin.Models;
using FX.Utils.MvcPaging;
using EInvoice.Core.Domain;
using System.Web.Mvc;

namespace EInvoice.CAdmin.Models
{
    public class CusSignIndexModel
    {
        public string Pattern{get;set;}
        public int? SignStatus{get;set;}
        public int defautPagesize { get; set; }
        public string FromDate{get;set;}
        public string ToDate{get;set;}
        public string Serial{get;set;}
        public string InvNo { get; set; }
        public SelectList lstpattern;
        public SelectList lstserial;
        public IPagedList<IInvoice> PageListCusSign;
    }
}