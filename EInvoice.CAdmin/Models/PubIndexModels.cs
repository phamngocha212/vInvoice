using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EInvoice.Core.Domain;
using FX.Utils.MvcPaging;

namespace EInvoice.CAdmin.Models
{
    public class PubIndexModels
    {
        public string status{get;set;}
        public string fromdate{get;set;}
        public string todate{get;set;}
        public IPagedList<Publish> PageListPUB;
    }

    public class RegIndexModel
    {
        public int comId { get; set; }        
        public string NameCompany { get; set; }
        public string back { get; set; }        
        public IPagedList<RegisterTemp> PagedlistReg;
    }    
}