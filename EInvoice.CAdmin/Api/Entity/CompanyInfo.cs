using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EInvoice.CAdmin.Api.Entity
{
    public class KeystoresInfo
    {
        public string CertSerial { get; set; }
        public string CertData { get; set; }
        public string PassWord { get; set; }
    }

    public class RegisterTempApi
    {
        public string InvPattern { get; set; }
    }

    public class UserdataInfo
    {
        public string username { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public bool IsApproved { get; set; }
        public bool ChangePass { get; set; }
    }

    public class RegisterData
    {
        public string InvPattern { get; set; }
        public string Name { get; set; }
        public string CssData { get; set; }
        public string CssLogo { get; set; }
        public string CssBackgr { get; set; }
        public string TemplateName { get; set; }
        public string XmlFile { get; set; }
        public string XsltFile { get; set; }
        public string ServiceType { get; set; }
        public string InvoiceType { get; set; }
        public string InvoiceView { get; set; }
        public string IViewer { get; set; }
        public string IGenerator { get; set; }
        public string NameInvoice { get; set; }
    }
}