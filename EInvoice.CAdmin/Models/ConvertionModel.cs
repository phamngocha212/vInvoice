using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EInvoice.Core.Domain;
using FX.Utils.MvcPaging;

namespace EInvoice.CAdmin.Models
{
    public class ConvertionModel
    {
        public string Pattern { get; set; }
        public string Serial { get; set; }
        public decimal? InvNo { get; set; }
        public int Converted { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
		        string _cusName;
        string _cuscode;
        public string cuscode
        {
            get
            {
                if (!string.IsNullOrEmpty(_cuscode)) return _cuscode.Trim();
                return "";
            }
            set { _cuscode = value; }
        }
		 public string cusName
        {
            get
            {
                if (!string.IsNullOrEmpty(_cusName)) return _cusName.Trim();
                return "";
            }
            set { _cusName = value; }
        }
        public SelectList PatternList { get; set; }
        public SelectList SerialList { get; set; }
        public IPagedList<IInvoice> PageListINV;
    }
}