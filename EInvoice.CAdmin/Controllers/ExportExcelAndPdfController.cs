using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using log4net;
using EInvoice.Core.IService;
using EInvoice.Core.Domain;
using FX.Core;
using EInvoice.Core;
using FX.Utils.MVCMessage;
using IdentityManagement.Authorization;
using EInvoice.Core.ServiceImp;
using EInvoice.CAdmin.Models;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.Xml;
using System.Xml.Xsl;
using FX.Context;
namespace EInvoice.CAdmin.Controllers
{
    public class ExportExcelAndPdfController : BaseController
    {       
        private static readonly ILog log = LogManager.GetLogger(typeof(ExportExcelAndPdfController));
     
        #region "Thống kê chi tiết hóa đơn"
        [RBACAuthorize(Permissions = "DownloadQuickreportPrince")]
        public ActionResult Quickreport(ReportsDetailModel model)
        {
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IPublishInvoiceService PubInvSrv = IoC.Resolve<IPublishInvoiceService>();
            //if (model == null) model = new ReportsDetailModel();
            IList<string> lstPattern = PubInvSrv.LstByPattern(currentCom.id, 2);            
            if (lstPattern.Count == 0)
            {
                Messages.AddErrorFlashMessage("Thông báo phát hành chưa được chấp nhận hoặc chưa có hóa đơn.");
                return Redirect("/");
            }
            if (lstPattern.Count > 0)
            {
                if (String.IsNullOrEmpty(model.Pattern))
                {
                    model.Pattern = lstPattern[0];
                }
            }            
            List<string> lstSerial = (from p in PubInvSrv.Query where p.ComId == currentCom.id && p.InvPattern == model.Pattern && (p.Status > 1) select p.InvSerial).Distinct().ToList<string>();
            model.lstPattern = new SelectList(lstPattern);
            model.lstSerial = new SelectList(lstSerial);
            return View(model);
        }
        // dua ra du lieu tim kiem QuickreportPrince
        [RBACAuthorize(Permissions = "DownloadQuickreportPrince")]
        public ActionResult QuickreportPrince(ReportsDetailModel model, int? Pagesize, int? page)
        {
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int pageSize = Pagesize.HasValue ? Convert.ToInt32(Pagesize) : 10;
            int pageIndex = page.HasValue ? page.Value - 1 : 0;
            int totalRecords = 0;
            DateTime? DateFrom = null;
            DateTime? DateTo = null;
            if (!string.IsNullOrWhiteSpace(model.FromDate)) DateFrom = DateTime.ParseExact(model.FromDate, "dd/MM/yyyy", null);
            if (!string.IsNullOrWhiteSpace(model.ToDate)) DateTo = DateTime.ParseExact(model.ToDate, "dd/MM/yyyy", null);
            ReportService repSrv = new ReportService();
            string xmlString = repSrv.ReportDetail(currentCom.id, model.Pattern, model.Serial, model.Status, null, DateFrom, DateTo, pageIndex, pageSize, out totalRecords);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);
            // transform
            StringWriter sw = new StringWriter();
            XslCompiledTransform xslCT = new XslCompiledTransform();
            xslCT.Load(System.Web.HttpContext.Current.Server.MapPath("~/Views/ReportsInv/TKChiTiet.xslt"));
            xslCT.Transform(xmlDoc, null, sw);
            model.Html = sw.ToString();
            model.pageSize = pageSize;
            model.pageIndex = pageIndex;
            model.totalRecords = totalRecords;
            return View(model);
        }
        //export ra execl hay pdf
        [RBACAuthorize(Permissions = "DownloadQuickreportPrince")]
        public void DownloadQuickReportPrint(ReportsDetailModel model, string type)
        {
            if (type != "pdf" && type != "xls" && type != "xlsx")
            {
                Response.Clear();
                Response.Write(String.Format("<script type='text/javascript'>alert('Có lỗi trong quá trình tải dữ liệu! <br /> Lỗi: {0}');</script>", type));
                Response.Redirect("/Home/PotentiallyError");
                Response.End();
                Response.Flush();
            }
            try
            {
                Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                DateTime? DateFrom = null;
                DateTime? DateTo = null;
                if (!string.IsNullOrWhiteSpace(model.FromDate)) DateFrom = DateTime.ParseExact(model.FromDate, "dd/MM/yyyy", null);
                if (!string.IsNullOrWhiteSpace(model.ToDate)) DateTo = DateTime.ParseExact(model.ToDate, "dd/MM/yyyy", null);
                ReportService repSrv = new ReportService();
                int totalRecords = 0;
                string xmlString = repSrv.ReportDetail(currentCom.id, model.Pattern, model.Serial, model.Status, "1", DateFrom, DateTo, 0, 0, out totalRecords);
                if (xmlString == "Error")
                {
                    Response.Clear();
                    Response.Write("Danh sách báo cáo quá 200,000 bản ghi!");
                    Response.Redirect("/ExportExcelAndPdf/QuickreportPrince");
                    Response.End();
                    Response.Flush();
                }
                else if (type == "pdf")
                {//export pdf
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xmlString);
                    // transform
                    StringWriter sw = new StringWriter();
                    XslCompiledTransform xslCT = new XslCompiledTransform();
                    xslCT.Load(System.Web.HttpContext.Current.Server.MapPath("~/Views/ReportsInv/TKChiTiet.xslt"));
                    // add argument
                    XsltArgumentList xslArg = new XsltArgumentList();
                    xslArg.AddParam("Type", "", type);
                    xslCT.Transform(xmlDoc, xslArg, sw);
                    Response.Clear();
                    Response.AddHeader("content-disposition", "attachment;filename=ReportDetails.pdf");
                    Response.ContentType = "application/pdf";

                    StringWriter sw1 = new StringWriter();
                    sw1.WriteLine("<table>");
                    sw1.WriteLine("<tr><td>1</td><td> một</td></tr>");
                    sw1.WriteLine("<tr><td>2</td><td> hai</td></tr>");
                    sw1.WriteLine("</table>");
                    StringReader sr = new StringReader(sw.ToString());
                    Document pdfDoc = new Document();
                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                    PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                    pdfDoc.Open();
                    htmlparser.Parse(sr);
                    pdfDoc.HtmlStyleClass = FX.Utils.UrlUtil.GetSiteUrl() + @"/Content/css/grid.css";
                    pdfDoc.Close();
                    Response.Write(sr.ReadToEnd());
                    Response.End();
                }
                else
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xmlString);
                    // transform
                    StringWriter sw = new StringWriter();
                    XslCompiledTransform xslCT = new XslCompiledTransform();
                    xslCT.Load(System.Web.HttpContext.Current.Server.MapPath("~/Views/ReportsInv/TKChiTiet.xslt"));
                    // add argument
                    XsltArgumentList xslArg = new XsltArgumentList();
                    xslArg.AddParam("Type", "", type);
                    xslCT.Transform(xmlDoc, xslArg, sw);
                    Response.Clear();
                    Response.AddHeader("Content-Disposition", "attachment; filename=ReportDetails.xls");
                    Response.ContentType = "application/vnd.ms-excel";
                    StringReader sr = new StringReader(sw.ToString());
                    Response.Write("<html xmlns:x=\"urn:schemas-microsoft-com:office:excel\">");
                    string style = @"<style>.text { mso-number-format:\@;} .datetime {mso-number-format:'mm\/dd\/yyyy'}</style>";
                    Response.ContentType = "application/text";
                    Response.Write(style);
                    Response.Write(sr.ReadToEnd());
                    Response.Flush();
                }
            }
            catch (Exception ex)
            {
                Response.Clear();
                Response.Write(String.Format("<script type='text/javascript'>alert('Có lỗi trong quá trình tải dữ liệu! <br /> Lỗi: {0}');</script>", ex.Message));
                Response.Redirect("/ExportExcelAndPdf/QuickreportPrince");
                Response.End();
                Response.Flush();
            }
        }

        public ActionResult PrintDetails(ReportsDetailModel model)
        {
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            DateTime? DateFrom = null;
            DateTime? DateTo = null;
            if (!string.IsNullOrWhiteSpace(model.FromDate)) DateFrom = DateTime.ParseExact(model.FromDate, "dd/MM/yyyy", null);
            if (!string.IsNullOrWhiteSpace(model.ToDate)) DateTo = DateTime.ParseExact(model.ToDate, "dd/MM/yyyy", null);
            ReportService repSrv = new ReportService();
            int totalRecords = 0;
            string xmlString = repSrv.ReportDetail(currentCom.id, model.Pattern, model.Serial, model.Status, "1", DateFrom, DateTo, 0, 0, out totalRecords);
            if (xmlString == "Error")
            {
                string htmlError = "<p>Dung lượng quá 200,000 bản ghi!</p>";
                return Json(new
                {
                    htmlError
                });
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);
            // transform
            StringWriter sw = new StringWriter();
            XslCompiledTransform xslCT = new XslCompiledTransform();
            xslCT.Load(System.Web.HttpContext.Current.Server.MapPath("~/Views/ReportsInv/TKChiTiet.xslt"));
            xslCT.Transform(xmlDoc, null, sw);
            string html = sw.ToString();
            return Json(new
            {
                html
            });
        }
        #endregion

        #region Xây dựng Bảng kê tạo lập và phát hành hóa đơn
        [RBACAuthorize(Permissions = "ReportLauch")]
        public ActionResult ReportLaunch(ReportsLaunchModel model)
        {
            try
            {
                Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                IPublishInvoiceService PubInvSrv = IoC.Resolve<IPublishInvoiceService>();
                IList<string> lstPattern = PubInvSrv.LstByPattern(currentCom.id, 2);                
                if (lstPattern.Count == 0)
                {
                    Messages.AddErrorFlashMessage("Thông báo phát hành chưa được chấp nhận hoặc chưa có hóa đơn.");
                    return Redirect("/");
                }
                if (lstPattern.Count > 0)
                {
                    if (string.IsNullOrEmpty(model.Pattern))
                    {
                        model.Pattern = lstPattern[0];
                        IInvoiceService IInvSrv = InvServiceFactory.GetService(model.Pattern, currentCom.id);
                    }
                }                
                List<string> lstSerial = (from p in PubInvSrv.Query where (p.ComId == currentCom.id) && (p.InvPattern == model.Pattern) && (p.Status > 1) select p.InvSerial).Distinct().ToList<string>();
                model.lstPattern = new SelectList(lstPattern);
                model.lstSerial = new SelectList(lstSerial);
                return View(model);
            }
            catch (Exception ex)
            {
                Messages.AddErrorFlashMessage(ex.Message);
                return RedirectToAction("Index", "Home");
            }
        }
        [RBACAuthorize(Permissions = "ReportLauchPrint")]
        public ActionResult ReportLaunchPrint(ReportsLaunchModel model, int? page, int? Pagesize)
        {
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int pageSize = Pagesize.HasValue ? Convert.ToInt32(Pagesize) : 10;
            int pageIndex = page.HasValue ? page.Value - 1 : 0;
            int totalRecords = 0;
            DateTime? publishdate = null;
            if (!string.IsNullOrWhiteSpace(model.PublishDate)) publishdate = DateTime.ParseExact(model.PublishDate, "dd/MM/yyyy", null);
            decimal no = Convert.ToDecimal(model.InvNo);
            // transform
            IReportService repSrv = currentCom.Config.Keys.Contains("IReportService") ? IoC.Resolve(Type.GetType(currentCom.Config["IReportService"])) as IReportService : new ReportService(); 
            string xmlData = repSrv.ReportLaunch(currentCom.id, model.Pattern, model.Serial, no, model.CreateBy, model.PublishBy, publishdate, null, pageIndex, pageSize, out totalRecords);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlData);
            StringWriter sw = new StringWriter();
            XslCompiledTransform xslCT = new XslCompiledTransform();
            xslCT.Load(System.Web.HttpContext.Current.Server.MapPath("~/Views/ReportsInv/TKTao_PH.xslt"));
            xslCT.Transform(xmlDoc, null, sw);
            model.Html = sw.ToString();
            model.pageSize = pageSize;
            model.pageIndex = pageIndex;
            model.totalRecords = totalRecords;
            return View(model);
        }
        [RBACAuthorize(Permissions = "DownloadReportLauch")]
        public void DownloadReportLauch(ReportsLaunchModel model, string type)
        {
            if (type != "pdf" && type != "xls" && type != "xlsx")
            {
                Response.Clear();
                Response.Write(String.Format("<script type='text/javascript'>alert('Có lỗi trong quá trình tải dữ liệu! <br /> Lỗi: {0}');</script>", type));
                Response.Redirect("/Home/PotentiallyError");
                Response.End();
                Response.Flush();
            }
            try
            {
                Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                DateTime? publishdate = null;
                if (!string.IsNullOrWhiteSpace(model.PublishDate)) publishdate = DateTime.ParseExact(model.PublishDate, "dd/MM/yyyy", null);
                decimal no = Convert.ToDecimal(model.InvNo);
                // transform
                IReportService repSrv = currentCom.Config.Keys.Contains("IReportService") ? IoC.Resolve(Type.GetType(currentCom.Config["IReportService"])) as IReportService : new ReportService(); 
                int totalRecords = 0;
                string xmlData = repSrv.ReportLaunch(currentCom.id, model.Pattern, model.Serial, no, model.CreateBy, model.PublishBy, publishdate, "1", 0, 0, out totalRecords);
                if (xmlData == "Error")
                {
                    Response.Write("Báo cáo quá 200,000 bản ghi!");
                    Response.Flush();
                }
                else if (type == "pdf") // in pdf
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xmlData);
                    StringWriter sw = new StringWriter();
                    XslCompiledTransform xslCT = new XslCompiledTransform();
                    xslCT.Load(System.Web.HttpContext.Current.Server.MapPath("~/Views/ReportsInv/TKTao_PH.xslt"));
                    // add argument
                    XsltArgumentList xslArg = new XsltArgumentList();
                    xslArg.AddParam("Type", "", type);
                    xslCT.Transform(xmlDoc, xslArg, sw);
                    string html = sw.ToString();
                    Response.Clear();
                    Response.AddHeader("Content-Disposition", "attachment; filename=ReportLaunch.pdf");
                    Response.ContentType = "application/pdf";
                    StringReader sr = new StringReader(sw.ToString());
                    Document pdfDoc = new Document();
                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                    PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                    pdfDoc.Open();
                    htmlparser.Parse(sr);
                    pdfDoc.Close();
                    Response.Write(sr.ReadToEnd());
                    Response.Flush();
                }
                else // in excel
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xmlData);
                    StringWriter sw = new StringWriter();
                    XslCompiledTransform xslCT = new XslCompiledTransform();
                    xslCT.Load(System.Web.HttpContext.Current.Server.MapPath("~/Views/ReportsInv/TKTao_PH.xslt"));
                    // add argument
                    XsltArgumentList xslArg = new XsltArgumentList();
                    xslArg.AddParam("Type", "", type);
                    xslCT.Transform(xmlDoc, xslArg, sw);
                    string html = sw.ToString();
                    Response.Clear();
                    Response.AddHeader("Content-Disposition", "attachment; filename=ReportLaunch.xls");
                    Response.ContentType = "application/vnd.ms-excel";
                    StringReader sr = new StringReader(sw.ToString());
                    Response.Write("<html xmlns:x=\"urn:schemas-microsoft-com:office:excel\">");
                    string style = @"<style>.text { mso-number-format:\@;} .datetime {mso-number-format:'mm\/dd\/yyyy'}</style>";
                    Response.ContentType = "application/text";
                    Response.Write(style);
                    Response.Write(sr.ReadToEnd());
                    Response.Flush();
                }
            }
            catch (Exception ex)
            {
                Response.Clear();
                Response.Write(String.Format("<script type='text/javascript'>alert('Có lỗi trong quá trình tải dữ liệu! <br /> Lỗi: {0}');</script>", ex.Message));
                Response.Redirect("/ExportExcelAndPdf/ReportLaunchPrint");
                Response.End();
                Response.Flush();
            }
        }

        public ActionResult PrintLaunch(ReportsLaunchModel model)
        {
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            DateTime? publishdate = null;
            if (!string.IsNullOrWhiteSpace(model.PublishDate)) publishdate = DateTime.ParseExact(model.PublishDate, "dd/MM/yyyy", null);
            decimal no = Convert.ToDecimal(model.InvNo);
            // transform
            IReportService repSrv = currentCom.Config.Keys.Contains("IReportService") ? IoC.Resolve(Type.GetType(currentCom.Config["IReportService"])) as IReportService : new ReportService(); 
            int totalRecords = 0;
            string xmlData = repSrv.ReportLaunch(currentCom.id, model.Pattern, model.Serial, no, model.CreateBy, model.PublishBy, publishdate, "1", 0, 0, out totalRecords);
            if (xmlData == "Error")
            {
                string htmlError = "<p>Dung lượng quá 200,000 bản ghi!</p>";
                return Json(new
                {
                    htmlError
                });
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlData);
            StringWriter sw = new StringWriter();
            XslCompiledTransform xslCT = new XslCompiledTransform();
            xslCT.Load(System.Web.HttpContext.Current.Server.MapPath("~/Views/ReportsInv/TKTao_PH.xslt"));
            xslCT.Transform(xmlDoc, null, sw);
            string html = sw.ToString();
            return Json(new
            {
                html
            });
        }
        #endregion       
    }
}