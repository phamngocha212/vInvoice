using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EInvoice.CAdmin.Models
{
    public class LaunchModel
    {
        public string Pattern { get;set;}
        public string Serial { get;set;}
        public string FromDate {get;set;}
        public string ToDate { get; set; }
        public SelectList Listpattern{get;set;}
        public SelectList Listserial{get;set;}
    }

    public class UploadModel
    {
        public int TypeTrans { get; set; }
        public string TypeLabel { get; set; }
        public string Pattern { get; set; }
        public string Serial { get; set; }
		public string ReasonDel { get; set; }
        public SelectList Listpattern { get; set; }
        public SelectList Listserial { get; set; }
        public List<int> Months { get; set; }
        public List<int> Years { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }

    public class TransactionName
    {
        public static string NameByType(int tranType)
        {
            switch (tranType)
            {
                case 0:
                    return "UPLOAD DỮ LIỆU HÓA ĐƠN";
                case 1:
                    return "UPLOAD DỮ LIỆU KHÁCH HÀNG";
                case 2:
                    return "UPLOAD DỮ LIỆU HỦY HÓA ĐƠN";
                case 3:
                    return "UPLOAD PHÁT HÀNH LẠI HÓA ĐƠN";
                default:
                    return "UPLOAD DỮ LIỆU";
            }
        }
    }
}