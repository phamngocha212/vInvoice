using FX.Utils.MvcPaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EInvoice.CAdmin.Models
{
    public class ReportEinv_Cancel
    {
       
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public IPagedList<RecordInvCancel> PageListRecordInvCancel;
        public int CountInv { get; set; }
        public string username { get; set; }
    }
    public class RecordInvCancel
    {
        public int stt { get; set; }
        public string Username { get; set; }
        public string pattern { get; set; }
        public string serial { get; set; }
        public string no { get; set; }
        public DateTime publishDate { get; set; }
        public decimal totalAmount { get; set; }

        public string cusName { get; set; }
        public string addressCus { get; set; }
        public string cusCode { get; set; }
        public string cusTaxCode { get; set; }
        public string DayCancelInv { get; set; }
    }
}