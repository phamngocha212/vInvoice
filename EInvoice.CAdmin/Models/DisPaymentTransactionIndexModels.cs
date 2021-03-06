﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EInvoice.Core.Domain;
using FX.Utils.MvcPaging;
namespace EInvoice.CAdmin.Models
{
    public class DisPaymentTransactionIndexModels
    {
        private PaymentTransactionStatus _status = PaymentTransactionStatus.Null;
        public string comID{ get; set; }
        public string key{ get; set; }
        public PaymentTransactionStatus status
        {
            get { return _status; }
            set { _status = value; }
        }
        public IPagedList<PaymentTransaction> PagedListTransaction { get; set; }
    }
}