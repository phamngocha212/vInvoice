using EInvoice.Core;
using EInvoice.Core.Domain;
using EInvoice.Core.Launching;
using FX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EInvoice.CAdmin.Api.Controllers
{
    [APIAuthenticate(Permissions = "Release_invInList")]
    [RoutePrefix("api/certifyinv")]
    public class CertifyInvController : ApiController
    {
        [Route("certify")]
        [HttpPost]
        public string Certify(string pattern, IInvoice[] invoices, Company currentCompany, bool resend = false)
        {
            string ErrorMessages = "";
            RegisterTemp temp = InvServiceFactory.GetRegister(pattern, currentCompany.id);
            ICertifyProvider _CertifyProvider = IoC.Resolve(Type.GetType(temp.ICertifyProvider)) as ICertifyProvider;

            /// Xac thuc hoa don
            string errCertify = _CertifyProvider.Certify(pattern, invoices, currentCompany);
            if (!string.IsNullOrWhiteSpace(errCertify))
                ErrorMessages = errCertify;
            return "value";
        }

        [Route("certifycancel")]
        [HttpPost]
        public string CertifyCancel(string pattern, IInvoice[] invoices, Company currentCompany, bool resend = false)
        {
            string ErrorMessages = "";
            RegisterTemp temp = InvServiceFactory.GetRegister(pattern, currentCompany.id);
            ICertifyProvider _CertifyProvider = IoC.Resolve(Type.GetType(temp.ICertifyProvider)) as ICertifyProvider;

            /// Xac thuc hoa don
            string errCertify = _CertifyProvider.CertifyCancel(pattern, invoices, currentCompany);
            if (!string.IsNullOrWhiteSpace(errCertify))
                ErrorMessages = errCertify;
            return "value";
        }

        [Route("certifyadjust")]
        [HttpPost]
        public string CertifyAdjust(string pattern, IInvoice[] invoices, Company currentCompany, bool resend = false)
        {
            string ErrorMessages = "";
            RegisterTemp temp = InvServiceFactory.GetRegister(pattern, currentCompany.id);
            ICertifyProvider _CertifyProvider = IoC.Resolve(Type.GetType(temp.ICertifyProvider)) as ICertifyProvider;

            /// Xac thuc hoa don
            string errCertify = _CertifyProvider.CertifyAdjust(pattern, invoices, currentCompany);
            if (!string.IsNullOrWhiteSpace(errCertify))
                ErrorMessages = errCertify;
            return "value";
        }

        [Route("certifyreplace")]
        [HttpPost]
        public string CertifyReplace(string pattern, IList<IInvoice> invoices, Company currentCompany, bool resend = false)
        {
            string ErrorMessages = "";
            RegisterTemp temp = InvServiceFactory.GetRegister(pattern, currentCompany.id);
            ICertifyProvider _CertifyProvider = IoC.Resolve(Type.GetType(temp.ICertifyProvider)) as ICertifyProvider;

            /// Xac thuc hoa don
            string errCertify = _CertifyProvider.CertifyReplace(pattern, invoices, currentCompany);
            if (!string.IsNullOrWhiteSpace(errCertify))
                ErrorMessages = errCertify;
            return "value";
        }
    }
}