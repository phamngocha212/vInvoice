using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FX.Utils.MvcPaging;
using EInvoice.Core.Domain;

namespace EInvoice.CAdmin.Models
{
    public class DecIndexModels
    {
        public string status { get; set; }
        public IPagedList<Decision> PageListDEC;
    }
}