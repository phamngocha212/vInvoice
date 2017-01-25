using EInvoice.Core.Domain;
using FX.Utils.MvcPaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EInvoice.CAdmin.Models
{
    public class TransactionIndexModels
    {
        private TranSactionStatus _status = TranSactionStatus.Null;
        public string code { get; set; }
        public string keyword { get; set; }
        public int comID { get; set; }

        public TranSactionStatus status
        {
            get { return _status; }
            set { _status = value; }
        }
        public IPagedList<Transaction> PagedListTransaction { get; set; }
    }
}