using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EInvoice.Core.Domain;
using FX.Utils.MvcPaging;

namespace EInvoice.CAdmin.Models
{
    public class StaffModel
    {
        public string fullname { get; set; }
        public string division { get; set; }
        public string account { get; set;}
        public IPagedList<Staff> PageListStaff;
    }
}