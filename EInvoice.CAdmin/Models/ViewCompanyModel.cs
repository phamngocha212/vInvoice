using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EInvoice.CAdmin.Models
{
    public class ViewCompanyModel
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string BankAccountName { get; set; }
        public string BankName { get; set; }
        public string BankNumber { get; set; }
        public string ContactPerson { get; set; }
        public string Descriptions { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string Phone { get; set; }
        public string RepresentPerson { get; set; }
        public string TaxAuthorityCode { get; set; }
        public string TaxName { get; set; }
        public string AccountName { get; set; }
        public string SignatureImage { get; set; }
    }
}