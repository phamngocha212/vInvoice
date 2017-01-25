using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EInvoice.Core.Domain;
using FX.Utils.MvcPaging;
namespace EInvoice.CAdmin.Models
{
    public class EInvoiceIndexModel
    {
        public string Pattern { get; set; }
        int _typeInvoice = -1;
        public int typeInvoice { get { return _typeInvoice; } set { _typeInvoice = value; } }
        int _status = -1;
        public int Status { get { return _status; } set { _status = value; } }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Serial { get; set; }
        public decimal? InvNo { get; set; }
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
        string _CodeTax;
        public string CodeTax
        {
            get
            {
                if (!string.IsNullOrEmpty(_CodeTax)) return _CodeTax.Trim();
                return "";
            }
            set { _CodeTax = value; }
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
        public IList<string> lstpattern;
        public IList<string> lstserial;
        public IPagedList<IInvoice> PageListINV;
        int _SignPlugin = 0;

        public int SignPlugin
        {
            get { return _SignPlugin; }
            set { _SignPlugin = value; }
        }

    }
}