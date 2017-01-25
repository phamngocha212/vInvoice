using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EInvoice.Core.Domain;
using EInvoice.Core;
using EInvoice.Core.IService;
using FX.Core;
using log4net;
using EInvoice.Core.Launching;
using EInvoice.Core.Viewer;
using FX.Context;
using FX.Utils.MVCMessage;

namespace EInvoice.CAdmin.Controllers
{
    [MessagesFilter]
    public class ShareController : TypeController
    {
        ILog logtest = LogManager.GetLogger(typeof(ShareController));
        public ActionResult ajxPreview(int idInvoice, string pattern)
        {
            try
            {
                Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                IInvoiceService IInvSrv = InvServiceFactory.GetService(pattern, currentCom.id);
                logtest.Info("call: " + idInvoice + " pattern: " + pattern + " company: " + currentCom.id);
                IInvoice oInvoice = IInvSrv.Getbykey<IInvoice>(idInvoice);
                //IViewer _iViewerSrv = IoC.Resolve<IViewer>();
                IViewer _iViewerSrv = InvServiceFactory.GetViewer(pattern, currentCom.id);
                if (oInvoice.Status != InvoiceStatus.NewInv)
                {
                    IRepositoryINV _iRepoSrv = IoC.Resolve<IRepositoryINV>();
                    byte[] data = _iRepoSrv.GetData(oInvoice);
                    return Json(new {invData = _iViewerSrv.GetHtml(data), status = oInvoice.Status});
                }
                else return Json(new { invData = _iViewerSrv.GetHtml(System.Text.Encoding.UTF8.GetBytes(oInvoice.GetXMLData())), status = 0});
            }
            catch (Exception ex)
            {
                logtest.Error(ex);
                return Json(new { invData = "Có lỗi xảy ra, vui lòng thực hiện lại.", status = 0 });
            }
        }

        public ActionResult ajxPreviewTemplate(string tempName)
        {
            InvTemplate it = new InvTemplate();
            IInvTemplateService _invTempSrc = IoC.Resolve<IInvTemplateService>();
            it = _invTempSrc.GetByName(tempName);
            System.Xml.XmlDocument xdoc = new System.Xml.XmlDocument();
            xdoc.PreserveWhitespace = true;
            xdoc.LoadXml(it.XmlFile);
            System.Xml.XmlProcessingInstruction newPI;
            String PItext = "type='text/xsl' href='" + FX.Utils.UrlUtil.GetSiteUrl() + "/InvoiceTemplate/GetXSLTbyTempName?tempname=" + it.TemplateName + "'";
            newPI = xdoc.CreateProcessingInstruction("xml-stylesheet", PItext);
            xdoc.InsertBefore(newPI, xdoc.DocumentElement);
            logtest.Info("tempName: " + tempName + " href: " + PItext);

            //IViewer _iViewerSrv = IoC.Resolve<IViewer>();
            IViewer _iViewerSrv = InvServiceFactory.GetViewer(tempName);
            return Json(_iViewerSrv.GetHtml(System.Text.Encoding.UTF8.GetBytes(xdoc.OuterXml)));
        }

        protected MessageViewData Messages
        {
            get
            {
                if (!ViewData.ContainsKey("Messages"))
                {
                    throw new InvalidOperationException("Messages are not available. Did you add the MessageFilter attribute to the controller?");
                }
                return (MessageViewData)ViewData["Messages"];
            }
        }
    }
}
