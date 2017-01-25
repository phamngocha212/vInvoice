using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EInvoice.Core.Domain;
using FX.Utils.MvcPaging;

namespace EInvoice.CAdmin.Models
{
    public class TaxAuthorityModel
    {
        public string name { get; set; }
        public IPagedList<TaxAuthority> ListTaxAuthority { get; set; }
    }
}