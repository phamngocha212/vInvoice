using FX.Utils.MvcPaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EInvoice.CAdmin.Models
{
    public class ReportEinv_Adjust
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string adjustReport { get; set; }
        public string replateReport { get; set; }
        public IPagedList<RecordAdjust> PageListRecordAdjust;
        public int CountInv { get; set; }
        public string username { get; set; }
    }
    public class RecordAdjust
    {
        public int stt { get; set; }
        //nguoi huy hoa don huy
        public string username { get; set; }
        //mau so,ky hieu,So HD,ngay ky, so tien, 
        public string patternOlder { get; set; }
        public string serialOlder { get; set; }
        public string noOlder { get; set; }
        public DateTime publishDateOlder { get; set; }
        public decimal totalMoneyOlder { get; set; }
        //mau so,ky hieu ,so hd,ngay ky,so tien 
        public string patternNew { get; set; }
        public string serialNew { get; set; }
        public string noNew { get; set; }
        public DateTime publishDateNew { get; set; }
        public decimal totalMoneyNew { get; set; }

        //ten kh,dia chi, ma,ma so thue neu co
        public string cusnameNew { get; set; }
        public string addressCusNew { get; set; }
        public string cuscodeNew { get; set; }
        public string cusTaxcode { get; set; }
        //status(thay the, sua doi) 
        public int status { get; set; }
        //date_process
        public string proccessdate { get; set; }
       
    }
}