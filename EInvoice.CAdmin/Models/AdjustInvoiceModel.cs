/*
 * 28/11/2014 update by duyetnv on: add PageListAdjustSearch, remove PageListadjustInvoice, PageListInvoice, PageListAdjustInv
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EInvoice.Core.Domain;
using FX.Utils.MvcPaging;
using System.Web.Mvc;
using EInvoice.Core.IService;
namespace EInvoice.CAdmin.Models
{
    public class AdjustInvoiceModel
    {
        public string pattern { get; set; }
        public string Serial { get; set; }
        //public IPagedList<adjustInvoice> PageListadjustInvoice;
        //public IPagedList<IInvoice> PageListInvoice;
        //public IPagedList<AdjustInv> PageListAdjustInv;
        public IPagedList<AjustSearchModel> PageListAdjustSearch;
        public SelectList lstPattern { get; set; }
        public SelectList lstSerial { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public decimal? InvNo { get; set; }
        public int ComId { get; set; }
        private string _CodeTax;
        public string CodeTax
        {
            get
            {
                if (!string.IsNullOrEmpty(_CodeTax)) return _CodeTax.Trim();
                return "";
            }
            set { _CodeTax = value; }
        }
        private string _nameCus;
        public string nameCus
        {
            get
            {
                if (!string.IsNullOrEmpty(_nameCus)) return _nameCus.Trim();
                return "";
            }
            set { _nameCus = value; }

        }
        private string _code;
        public string code
        {
            get
            {
                if (!string.IsNullOrEmpty(_code)) return _code.Trim();
                return "";
            }
            set { _code = value; }
        }
    }
}