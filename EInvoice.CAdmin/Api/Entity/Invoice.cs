using EInvoice.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EInvoice.CAdmin.Api
{
    public class ApiPublish
    {
        public string xmlData { get; set; }        
        public string pattern { get; set; }
        public string serial { get; set; }
        public string certSerial { get; set; }
        public string certString { get; set; }
        public string cusCode { get; set; }
        public int convert { get; set; }
        public int[] invIDs { get; set; }
    }

    public class HisApi
    {
        public string username { get; set; }
        public string password { get; set; }
        public string xmlData { get; set; }
    }

    public class SendInv
    {
        public string Serial { get; set; }
        public string Pattern { get; set; }
        public string CusCode { get; set; }
        public Decimal No { get; set; }
        public string MonthInv { get; set; }
        public int ComID { get; set; }
    }
    public class Invoice
    {
        public int comID { get; set; }
        public string pattern { get; set; }
        public string serial { get; set; }
        public int year { get; set; }
        public int month { get; set; }
        public int quarter { get; set; }
        public int currentQuarter { get; set; }
    }

    public class ListInvoice
    {
        public string lsFkey { get; set; }
        public string lstInvToken { get; set; }
        public bool isEmail { get; set; }
    }

    public class RemoteInvoice
    {
        public string InvPattern { get; set; }
        public string InvSerial { get; set; }
        public string InvData { get; set; }
        public string CertBase64String { get; set; }
    }

    public class JsonLaunch
    {
        public string InvPattern { get; set; }
        public string InvSerial { get; set; }
        public string keys { get; set; }
        public string signeds { get; set; }
        public string CertBase64String { get; set; }
    }

    public class RemoteAdjustInvoice
    {
        public int InvType { get; set; }
        public string OriPattern { get; set; }
        public string OriSerial { get; set; }
        public decimal OriNo { get; set; }        
        public string InvData { get; set; }
        public string CertBase64String { get; set; }
    }    

    public class DataUpdate
    {
        public decimal No { get; set; }
        public string Serial { get; set; }
        public string Data { get; set; }
    }
}