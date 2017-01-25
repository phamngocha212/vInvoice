using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FX.Utils.MvcPaging;
using EInvoice.Core.Domain;
namespace EInvoice.CAdmin.Models
{
    public class InvCancelModel
    {
        public string creater { get; set; }
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
        public IPagedList<InvCancel> PageListIC { get; set; }
    }    
}