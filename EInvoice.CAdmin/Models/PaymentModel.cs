using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EInvoice.Core.Domain;
using FX.Utils.MvcPaging;

namespace EInvoice.CAdmin.Models
{
    public class PaymentModel
    {
        public string Pattern { get; set; }
        public string Serial { get; set; }
        public decimal? InvNo { get; set; }
        int _paymentStatus = -1;
        public int PaymentStatus { get { return _paymentStatus; } set { _paymentStatus = value; } }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        string _nameCus;
        public string nameCus
        {
            get
            {
                if (!string.IsNullOrEmpty(_nameCus)) return _nameCus.Trim();
                return "";
            }
            set { _nameCus = value; }

        }
        string _code;
        public string code
        {
            get
            {
                if (!string.IsNullOrEmpty(_code)) return _code.Trim();
                return "";
            }
            set { _code = value; }
        }
        public SelectList PatternList { get; set; }
        public SelectList SerialList { get; set; }
        public IPagedList<IInvoice> PageListINV;
    }
}
