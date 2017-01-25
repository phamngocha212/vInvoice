using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using EInvoice.Core.Domain;
using EInvoice.Core.IService;
using FX.Core;
using EInvoice.Core;
using IdentityManagement.Authorization;
using IdentityManagement.WebProviders;
using IdentityManagement.Domain;
using System.Web.Security;
using log4net;
using FX.Utils.MVCMessage;
using FX.Utils.MvcPaging;
using System.Xml;
using System.IO;
using System.Xml.Xsl;
using System.Text;
namespace EInvoice.CAdmin.Controllers
{
    [HandleError]
    public class InvoiceTemplateController : ShareController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(InvoiceTemplateController));

        public ActionResult GetXSLT(string pattern)
        {
            Company currentComp = ((EInvoiceContext)FX.Context.FXContext.Current).CurrentCompany;
            byte[] xsltData;
            InvTemplate temp = new InvTemplate();

            temp = InvServiceFactory.GetTemplateByPattern(pattern, currentComp.id);
            xsltData = System.Text.Encoding.UTF8.GetBytes(temp.XsltFile);
            return File(xsltData, "text/xsl");
        }

        public ActionResult GetSchema(string pattern, int comid)
        {
            return View();
        }

        public ActionResult GetXML(string pattern, int comid)
        {
            return View();
        }
        public ActionResult GetXSLTbyPattern(string pattern)
        {
            Company currentComp = ((EInvoiceContext)FX.Context.FXContext.Current).CurrentCompany;
            RegisterTemp temp = InvServiceFactory.GetRegister(pattern, currentComp.id);
            InvTemplate invTemp = InvServiceFactory.GetTemplateByPattern(pattern, currentComp.id);
            string xslt = invTemp.XsltFile;
            string tmp = "<style type=\"text/css\">";
            StringBuilder sb = new StringBuilder();
            if (!xslt.Contains(tmp))
                tmp = "<style type=\"text/css\" rel=\"stylesheet\">";
            if (xslt.Contains(tmp))
            {
                string head = xslt.Substring(0, xslt.IndexOf(tmp) + tmp.Length);
                string foot = xslt.Substring(xslt.IndexOf("</style>"));
                if (!string.IsNullOrWhiteSpace(temp.CssData))
                    sb.AppendFormat("{0}{1}{2}{3}{4}", head, temp.CssData, temp.CssLogo, temp.CssBackgr, foot);
                else
                    sb.AppendFormat("{0}{1}{2}{3}{4}", head, invTemp.CssData, invTemp.CssLogo, invTemp.CssBackgr, foot);
            }
            //InvTemplate temp = src.GetByName(tempname);
            byte[] xsltData = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            return File(xsltData, "text/xsl");
        }

        public ActionResult GetXSLTbyTempName(string tempname)
        {
            IInvTemplateService src = IoC.Resolve<IInvTemplateService>();
            InvTemplate temp = src.GetByName(tempname);
            byte[] xsltData = null;
            if (temp.IsPub)
            {
                string xslt = temp.XsltFile;
                string tmp = "<style type=\"text/css\">";
                StringBuilder sb = new StringBuilder();
                if (!xslt.Contains(tmp))
                    tmp = "<style type=\"text/css\" rel=\"stylesheet\">";
                if (xslt.Contains(tmp))
                {
                    string head = xslt.Substring(0, xslt.IndexOf(tmp) + tmp.Length);
                    string foot = xslt.Substring(xslt.IndexOf("</style>"));
                    sb.AppendFormat("{0}{1}{2}{3}{4}", head, temp.CssData, temp.CssLogo, temp.CssBackgr, foot);
                }
                //InvTemplate temp = src.GetByName(tempname);
                xsltData = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            }
            else xsltData = System.Text.Encoding.UTF8.GetBytes(temp.XsltFile);
            return File(xsltData, "text/xsl");
        }

        [RBACAuthorize(Roles = "Admin")]
        public ActionResult ResetTem()
        {
            try
            {
                InvServiceFactory.Initial();
                ViewData["MessageResetTem"] = "Reset mau thanh cong.";
                return View();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                ViewData["MessageResetTem"] = "Reset mau khong thanh cong.";
                return View();
            }
        }
    }
}
