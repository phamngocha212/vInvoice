using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EInvoice.CAdmin.Models;
using EInvoice.Core.Domain;

namespace EInvoice.CAdmin.Models
{
    public class CustomerModel
    {
        //customer
        public Customer tmpCustomer { get; set; }                     
        public string SerialCert { get; set; }
        public int Cerid { get; set; }
        public string Cer { get; set; }
        public string serialcer { get; set; }
        public string OrganizationCA { get; set; }
        public string OwnCA { get; set; }
        public string ValidForm { get; set; }
        public string ValidTo { get; set; }
        public Certificate UpdateCertificate(Certificate mCertificate)
        {
            mCertificate.id = Cerid;
            mCertificate.SerialCert = SerialCert;            
            if (ValidForm == null || ValidTo == null)
            {
                mCertificate.ValidFrom = EInvoice.Core.Domain.Enumerations.MinDate;
                mCertificate.ValidTo = EInvoice.Core.Domain.Enumerations.MinDate;
            }
            else
            {
                mCertificate.ValidFrom = DateTime.ParseExact(ValidForm, "dd/MM/yyyy", null); ;
                mCertificate.ValidTo = DateTime.ParseExact(ValidTo, "dd/MM/yyyy", null); ;
            }
            return mCertificate;
        }
    }
}