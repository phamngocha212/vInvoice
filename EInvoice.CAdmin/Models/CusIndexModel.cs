using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FX.Utils.MvcPaging;
using EInvoice.Core.Domain;

namespace EInvoice.CAdmin.Models
{
    public class CusIndexModel
    {
        public string name { get; set; }
        public string code { get; set; }
        public IPagedList<Customer> PagedListCUS { get; set; }
    }
}