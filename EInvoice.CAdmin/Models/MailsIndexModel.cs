using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EInvoice.Core.Domain;
using FX.Utils.MvcPaging;

namespace EInvoice.CAdmin.Models
{
    public class MailsIndexModel
    {
        public string Subject { get; set; }
        int _status = -1;
        public int Status { get { return _status; } set { _status = value; } }
        public string FromSendedDate { get; set; }
        public string ToSendedDate { get; set; }
        public string EmailTo { get; set; }
        public IPagedList<SendMail> PageListMail;
    }
}