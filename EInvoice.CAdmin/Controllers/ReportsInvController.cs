using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FX.Core;
using EInvoice.Core.IService;
using EInvoice.Core.Domain;
using EInvoice.Core;
using EInvoice.Core.ServiceImp;
using EInvoice.CAdmin.Models;
using FX.Utils.MVCMessage;
using IdentityManagement.Authorization;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using FX.Utils.MvcPaging;
using System.Threading.Tasks;
using log4net;
using System.Configuration;
using System.Xml.Linq;
using System.Text;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Web;
using FX.Context;
using System.Globalization;

namespace EInvoice.CAdmin.Controllers
{
    public class ReportsInvController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReportsInvController));
        #region Báo cáo Tình hình sử dụng hóa đơn
        // GET: /ReportsInv/

        [RBACAuthorize(Permissions = "Report")]
        public ActionResult Reports()
        {
            IPublishInvoiceService _PubInvSrv = IoC.Resolve<IPublishInvoiceService>();

            //năm theo của từng dải được phát hành
            List<int> lstyear = new List<int>();
            int minYear = _PubInvSrv.Query.Min(r => r.StartDate.Year);
            while (minYear <= DateTime.Now.Year)
                lstyear.Add(minYear++);
            SelectList lstYearSelect = new SelectList(lstyear);
            return View(lstYearSelect);
        }
        [RBACAuthorize(Permissions = "Report")]
        public ActionResult ReportsPrint(int year, int quarter)
        {
            if (year <= 0 || quarter <= 0)
                throw new HttpRequestValidationException();
            try
            {
                string s = Resources.Einvoice.AdjReInv_ChooseSerial;
                ICompanyService _comSrv = IoC.Resolve<ICompanyService>();
                Company _currentCom = _comSrv.Getbykey(((EInvoiceContext)FXContext.Current).CurrentCompany.id);
                //IReportService repSrv = new ReportService();
                IReportService repSrv = _currentCom.Config.Keys.Contains("IReportService") ? IoC.Resolve(Type.GetType(_currentCom.Config["IReportService"])) as IReportService : new ReportService(); 
                DateTime currentdate = DateTime.Today;
                int CurrentQuarter;
                if (currentdate.Month <= 3) CurrentQuarter = 1;
                else if (currentdate.Month <= 6) CurrentQuarter = 2;
                else if (currentdate.Month <= 9) CurrentQuarter = 3;
                else CurrentQuarter = 4;
                CurrentQuarter = CurrentQuarter + 4 * (currentdate.Year - year);
                string xmlString = repSrv.Report(_currentCom.id, quarter, year, CurrentQuarter);
                //them phan header cho bao cao
                XmlDocument xmlDoc = new XmlDocument();
                if (xmlString.Contains("HSoThueDTu") == true) xmlDoc.LoadXml(xmlString);
                else xmlDoc.LoadXml(StringxmlReportUser(xmlString, _currentCom, "", quarter.ToString(), year, 0));
                // transform
                StringWriter sw = new StringWriter();
                XslCompiledTransform xslCT = new XslCompiledTransform();
                xslCT.Load(System.Web.HttpContext.Current.Server.MapPath("~/Views/ReportsInv/THSDHD.xslt"));
                xslCT.Transform(xmlDoc, null, sw);
                string html = sw.ToString();
                ReportsPrintModel repPrint = new ReportsPrintModel(html);
                ViewData[" year"] = year;
                ViewData["quarter"] = quarter;
                return View(repPrint);
            }
            //luu lai file
            catch (Exception ex)
            {
                log.Error("Error:", ex);
                Messages.AddErrorFlashMessage("Có lỗi xảy ra, vui lòng thực hiện lại!");
                return RedirectToAction("Reports");
            }

        }
        public FileResult DownloadXML(int year, int quarter)
        {
            try
            {
                Company _currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                string fileName = string.Empty;
                string Namefile = string.Empty;
                fileName = AppDomain.CurrentDomain.BaseDirectory + string.Format("Report/THSD/{0}_{1}_{2}.xml", Convert.ToInt32(quarter), year, _currentCom.id);
                Namefile = quarter + "_" + year + "_" + _currentCom.id + ".xml";
                string contentType = "text/xml";
                return File(fileName, contentType, Namefile);
            }
            catch (Exception ex)
            {
                if (!(ex is FileNotFoundException))
                    throw new Exception("Không tìm thấy file báo cáo");
                else throw ex;
            }

        }

        #region "Hàm hỗ trợ xuất báo cáo tình hình sử dụng theo tháng và quý"
        private string StringxmlReportUser(string xmldetailReport, Company currentCom, string month, string quarter, int year, int statusReport)
        {
            string fistdate = string.Empty;
            string lastdate = string.Empty;
            string kieuKy = string.Empty;
            string kyKKhai = string.Empty;
            if (statusReport == 0)//báo cáo tình hình sử dụng theo quý
            {
                bool checkConvertTime = ConvertTimeString(Convert.ToInt32(quarter), year, out fistdate, out lastdate);
                kieuKy = "Q";
                kyKKhai = quarter.ToString();
            }
            else//báo cáo tình hình sử dụng theo tháng
            {
                DateTime FirstDate = new DateTime(year, Convert.ToInt32(month), 1);
                DateTime LastDate = FirstDate.AddMonths(1).AddDays(-1);
                fistdate = FirstDate.ToString("dd/MM/yyyy");
                lastdate = LastDate.ToString("dd/MM/yyyy");
                kieuKy = "M";
                kyKKhai = month.PadLeft(2, '0');
            }
            XElement elem = XElement.Parse(xmldetailReport);
            //xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns='http://kekhaithue.gdt.gov.vn/TKhaiThue'>"
            string ReportStringXml = "<HSoThueDTu>"
            + "<HSoKhaiThue id=\"_NODE_TO_SIGN\">"
          + "<TTinChung>"
          + "	<TTinDVu>"
          + "		<maDVu>HTKK</maDVu>"
          + "		<tenDVu>HTKK</tenDVu>"
          + "	    <pbanDVu>3.2.5</pbanDVu>"
          + "		<ttinNhaCCapDVu></ttinNhaCCapDVu>"
          + "	</TTinDVu>"
          + "	<TTinTKhaiThue>"
          + "		<TKhaiThue>"
          + "			<maTKhai></maTKhai>"
          + "			<tenTKhai>Báo cáo tình hình sử dụng hóa đơn</tenTKhai>"
          + "			<moTaBMau>(Ban hành kèm theo Thông tư số 156/2013/TT-BTC ngày 06/11/2013 của Bộ Tài chính)</moTaBMau>"
          + "			<pbanTKhaiXML></pbanTKhaiXML>"
          + "			<loaiTKhai></loaiTKhai>"
          + "			<soLan>0</soLan>"
          + "			<KyKKhaiThue>"
          + "				<kieuKy>" + kieuKy + "</kieuKy>"
          + "				<kyKKhai>" + kyKKhai + "/" + year + "</kyKKhai>"
          + "				<kyKKhaiTuNgay>" + fistdate + "</kyKKhaiTuNgay>"
          + "				<kyKKhaiDenNgay>" + lastdate + "</kyKKhaiDenNgay>"
          + "				<kyKKhaiTuThang></kyKKhaiTuThang>"
          + "				<kyKKhaiDenThang></kyKKhaiDenThang>"
          + "			</KyKKhaiThue>"
          + "           <maCQTNoiNop></maCQTNoiNop>"
          + "			<tenCQTNoiNop></tenCQTNoiNop>"
          + "			<ngayLapTKhai>" + DateTime.Now.ToString("yyyy-MM-dd") + "</ngayLapTKhai>"
          + "			<GiaHan>"
          + "				<maLyDoGiaHan></maLyDoGiaHan>"
          + "				<lyDoGiaHan></lyDoGiaHan>"
          + "			</GiaHan>"
          + "			<nguoiKy>nguoi ky</nguoiKy>"
          + "			<ngayKy>" + DateTime.Now.ToString("yyyy-MM-dd") + "</ngayKy>"
          + "			<nganhNgheKD></nganhNgheKD>"
          + "		</TKhaiThue>"
          + "		<NNT>"
          + "			<mst>" + currentCom.TaxCode + "</mst>"
          + "			<tenNNT>" + currentCom.Name + "</tenNNT>"
          + "			<dchiNNT>" + currentCom.Address + "</dchiNNT>"
          + "			<phuongXa></phuongXa>"
          + "			<maHuyenNNT></maHuyenNNT>"
          + "			<tenHuyenNNT></tenHuyenNNT>"
          + "			<maTinhNNT></maTinhNNT>"
          + "			<tenTinhNNT>Hà Nội</tenTinhNNT>"
          + "			<dthoaiNNT>" + currentCom.Phone + "</dthoaiNNT>"
          + "			<faxNNT></faxNNT>"
          + "			<emailNNT></emailNNT>"
          + "		</NNT>"
          + "	</TTinTKhaiThue>"
          + "</TTinChung>"
          + elem.ToString()
          + "</HSoKhaiThue>"
          + "</HSoThueDTu>";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(ReportStringXml);
            string fileName = statusReport == 0 ? AppDomain.CurrentDomain.BaseDirectory + string.Format("Report/THSD/{0}_{1}_{2}.xml", quarter, year, currentCom.id) : AppDomain.CurrentDomain.BaseDirectory + string.Format("Report/THSD_MONTH/{0}_{1}_{2}.xml", month, year, currentCom.id);
            xmlDoc.Save(fileName);
            return ReportStringXml;
        }
        private bool ConvertTimeString(int quarter, int year, out string Fistdate, out string Lastdate)
        {
            string fistdate = string.Empty;
            string lastdate = string.Empty;
            if (quarter == 1)
            {
                fistdate = "01/01/" + year;
                lastdate = "31/03/" + year;
            }
            else if (quarter == 2)
            {
                fistdate = "01/04/" + year;
                lastdate = "30/06/" + year;
            }
            else if (quarter == 3)
            {
                fistdate = "01/07/" + year;
                lastdate = "30/09/" + year;
            }
            else if (quarter == 4)
            {
                fistdate = "01/10/" + year;
                lastdate = "31/12/" + year;
            }
            Fistdate = fistdate;
            Lastdate = lastdate;
            return true;
        }
        public void DownloadXls(string year, string month, string Quarter)
        {
            try
            {
                Company _currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                int _year = Convert.ToInt32(year);
                IReportService repSrv = _currentCom.Config.Keys.Contains("IReportService") ? IoC.Resolve(Type.GetType(_currentCom.Config["IReportService"])) as IReportService : new ReportService(); 
                string fileName = string.Empty;
                if (!string.IsNullOrEmpty(month))
                {
                    int _month = Convert.ToInt32(month);
                    fileName = AppDomain.CurrentDomain.BaseDirectory + string.Format("Report/THSD_MONTH/{0}_{1}_{2}.xml", _month, _year, _currentCom.id);
                }
                else
                {
                    int _quater = Convert.ToInt32(Quarter);
                    fileName = AppDomain.CurrentDomain.BaseDirectory + string.Format("Report/THSD/{0}_{1}_{2}.xml", _quater, _year, _currentCom.id);
                }
                XmlDocument xmlDoc = new XmlDocument();
                try
                {
                    xmlDoc.Load(fileName);
                }
                catch (Exception ex)
                {
                    if (!(ex is FileNotFoundException))
                        throw new Exception("khong tim thay bao cao da duoc ket xuat cua ky cu");
                    else throw ex;
                }
                StringWriter sw = report_User(xmlDoc.InnerXml);
                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=Report.xls");
                Response.ContentType = "application/vnd.ms-excel";
                StringReader sr = new StringReader(sw.ToString().Replace("&lt;br/&gt;", "<br />"));
                Response.Write("<html xmlns:x=\"urn:schemas-microsoft-com:office:excel\">");
                string style = @"<style>.text { mso-number-format:\@;vertical-align:top;} .textnowrap {mso-number-format:\@; text-align:general; white-space:normal; mso-spacerun:yes;AutoFitWidth:0;max-width: 72px !important;}</style>";
                Response.Write(style);
                Response.ContentType = "application/text";
                Response.Write(sr.ReadToEnd());
                Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {
                Response.Clear();
                Response.Write(String.Format("<script type='text/javascript'>alert('Có lỗi trong quá trình tải dữ liệu! <br /> Lỗi: {0}');</script>", ex.Message));
                Response.End();
                Response.Flush();
            }
        }
        #endregion
        #endregion
        #region Báo cáo tình hình sử dụng theo tháng

        [RBACAuthorize(Permissions = "Report")]
        public ActionResult ReportsUserMonth()
        {
            IPublishInvoiceService _PubInvSrv = IoC.Resolve<IPublishInvoiceService>();
            ReportsModel model = new ReportsModel();

            //năm theo của từng dải được phát hành
            List<int> lstyear = new List<int>();
            int minYear = _PubInvSrv.Query.Min(r => r.StartDate.Year);
            while (minYear <= DateTime.Now.Year)
                lstyear.Add(minYear++);

            SelectList lstYearSelect = new SelectList(lstyear);
            model.lstYear = lstYearSelect;
            List<string> lstmonth = new List<string>() { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" };
            model.lstMonth = new SelectList(lstmonth);
            return View(model);
        }
        [RBACAuthorize(Permissions = "Report")]
        public ActionResult ReportsUserMonthPrint(int year, int month)
        {
            try
            {
                if (year <= 0 || month <= 0)
                    throw new HttpRequestValidationException();
                string s = Resources.Einvoice.AdjReInv_ChooseSerial;
                ICompanyService _comSrv = IoC.Resolve<ICompanyService>();
                Company _currentCom = _comSrv.Getbykey(((EInvoiceContext)FXContext.Current).CurrentCompany.id);
                IReportService repSrv = _currentCom.Config.Keys.Contains("IReportService") ? IoC.Resolve(Type.GetType(_currentCom.Config["IReportService"])) as IReportService : new ReportService(); 
                DateTime currentdate = DateTime.Today;
                int CurrentMonth = currentdate.Month;
                CurrentMonth = CurrentMonth + 12 * (currentdate.Year - year);
                string xmlString = repSrv.ReportUserMonth(_currentCom.id, month, year, CurrentMonth);
                //them phan header cho bao cao
                XmlDocument xmlDoc = new XmlDocument();
                if (xmlString.Contains("HSoThueDTu") == true) xmlDoc.LoadXml(xmlString);
                else xmlDoc.LoadXml(StringxmlReportUser(xmlString, _currentCom, month.ToString(), "", year, 1));
                // transform
                StringWriter sw = new StringWriter();
                XslCompiledTransform xslCT = new XslCompiledTransform();
                xslCT.Load(System.Web.HttpContext.Current.Server.MapPath("~/Views/ReportsInv/THSDHD.xslt"));
                xslCT.Transform(xmlDoc, null, sw);
                string html = sw.ToString();
                ReportsPrintModel repPrint = new ReportsPrintModel(html);
                ViewData["year"] = year;
                ViewData["month"] = month;
                return View(repPrint);

            }
            catch (Exception ex)
            {
                log.Error("Error:", ex);
                Messages.AddErrorFlashMessage("Có lỗi xảy ra, vui lòng thực hiện lại!");
                return RedirectToAction("ReportsUserMonth");
            }

        }
        #endregion
        #region "Thống kê hóa đơn VAT theo quý"
        [RBACAuthorize(Permissions = "Report")]
        public ActionResult ReportsQuater()
        {
            IPublishInvoiceService _PubInvSrv = IoC.Resolve<IPublishInvoiceService>();

            //năm theo của từng dải được phát hành
            List<int> lstyear = new List<int>();
            int minYear = _PubInvSrv.Query.Min(r => r.StartDate.Year);
            while (minYear <= DateTime.Now.Year)
                lstyear.Add(minYear++);
            SelectList lstYearSelect = new SelectList(lstyear);

            return View(lstYearSelect);
        }
        [RBACAuthorize(Permissions = "Report")]
        public ActionResult ReportsQuaterPrint(int year, int quarter)
        {
            if (year <= 0 || quarter <= 0)
                throw new HttpRequestValidationException();
            try
            {
                string s = Resources.Einvoice.AdjReInv_ChooseSerial;
                ICompanyService _comSrv = IoC.Resolve<ICompanyService>();
                Company _currentCom = _comSrv.Getbykey(((EInvoiceContext)FXContext.Current).CurrentCompany.id);
                //IReportService repSrv = new ReportService();
                IReportService repSrv = _currentCom.Config.Keys.Contains("IReportService") ? IoC.Resolve(Type.GetType(_currentCom.Config["IReportService"])) as IReportService : new ReportService(); 
                DateTime currentdate = DateTime.Today;
                int CurrentQuarter;
                if (currentdate.Month <= 3) CurrentQuarter = 1;
                else if (currentdate.Month <= 6) CurrentQuarter = 2;
                else if (currentdate.Month <= 9) CurrentQuarter = 3;
                else CurrentQuarter = 4;
                CurrentQuarter = CurrentQuarter + 4 * (currentdate.Year - year);
                string path = repSrv.ReportQuarter(_currentCom.id, quarter, year, CurrentQuarter);
                //them phan header cho bao cao
                FileInfo finfo = new FileInfo(path);
                double ifileLength = (finfo.Length / 1048576);
                if (ifileLength > 65)
                {
                    Messages.AddErrorFlashMessage("Dung lượng vượt quá mức cho phép!");
                    return RedirectToAction("ReportsQuater");
                }
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(fs);
                StringWriter sw = new StringWriter();
                XslCompiledTransform xslCT = new XslCompiledTransform();
                xslCT.Load(System.Web.HttpContext.Current.Server.MapPath("~/Views/ReportsInv/BKHDDV.xslt"));
                xslCT.Transform(xmlDoc, null, sw);
                string html = sw.ToString();
                ReportsPrintModel repPrint = new ReportsPrintModel(html);
                ViewData["quarter"] = quarter;
                ViewData["year"] = year;
                fs.Close();
                return View(repPrint);
            }
            //luu lai file
            catch (Exception ex)
            {
                log.Error("Error:", ex);
                Messages.AddErrorFlashMessage("Có lỗi xảy ra, vui lòng thực hiện lại!");
                return RedirectToAction("ReportsQuater");
            }

        }
        #endregion
        #region Thống kê hóa đơn VAT theo tháng
        [RBACAuthorize(Permissions = "Report")]
        public ActionResult ReportsMonth()
        {
            IPublishInvoiceService _PubInvSrv = IoC.Resolve<IPublishInvoiceService>();
            ReportsModel model = new ReportsModel();

            //năm theo của từng dải được phát hành
            List<int> lstyear = new List<int>();
            int minYear = _PubInvSrv.Query.Min(r => r.StartDate.Year);
            while (minYear <= DateTime.Now.Year)
                lstyear.Add(minYear++);

            SelectList lstYearSelect = new SelectList(lstyear);
            model.lstYear = lstYearSelect;
            List<string> lstmonth = new List<string>() { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" };
            model.lstMonth = new SelectList(lstmonth);
            return View(model);

        }

        [RBACAuthorize(Permissions = "Report")]
        public ActionResult ReportsMonthPrint(ReportsModel model)
        {
            if (model.month <= 0 || model.year <= 0)
                throw new HttpRequestValidationException();
            try
            {
                DateTime currentdate = DateTime.Today;
                int CurrentMonth = currentdate.Month;
                CurrentMonth = CurrentMonth + 12 * (currentdate.Year - model.year);
                if (CurrentMonth < model.month)
                {
                    Messages.AddErrorFlashMessage("Tháng báo cáo cần không lớn hơn tháng hiện tại.");
                    return RedirectToAction("ReportsMonth");
                }
                // lấy dữ liệu xml
                Company _currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                IReportService repSrv = _currentCom.Config.Keys.Contains("IReportService") ? IoC.Resolve(Type.GetType(_currentCom.Config["IReportService"])) as IReportService : new ReportService(); 
                string path = repSrv.ReportMonth(_currentCom.id, model.month, model.year, CurrentMonth);
                FileInfo finfo = new FileInfo(path);
                double ifileLength = (finfo.Length / 1048576);
                if (ifileLength > 65)
                {
                    Messages.AddErrorFlashMessage("Dung lượng vượt quá mức cho phép!");
                    return RedirectToAction("ReportsMonth");
                }
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(fs);
                StringWriter sw = new StringWriter();
                XslCompiledTransform xslCT = new XslCompiledTransform();
                xslCT.Load(System.Web.HttpContext.Current.Server.MapPath("~/Views/ReportsInv/BKHDDV.xslt"));
                xslCT.Transform(xmlDoc, null, sw);
                string html = sw.ToString();
                ReportsPrintModel repPrint = new ReportsPrintModel(html);
                ViewData["Month"] = model.month;
                ViewData["Year"] = model.year;
                fs.Close();
                return View(repPrint);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                Messages.AddErrorFlashMessage("Có lỗi xảy ra, vui lòng thực hiện lại!");
                return RedirectToAction("ReportsMonth");
            }

        }

        public ActionResult DownloadReportMonth_excel(string month, string quarter, string year)
        {
            ICompanyService _comSrv = IoC.Resolve<ICompanyService>();
            Company _currentCom = _comSrv.Getbykey(((EInvoiceContext)FXContext.Current).CurrentCompany.id);
            IReportService repSrv = _currentCom.Config.Keys.Contains("IReportService") ? IoC.Resolve(Type.GetType(_currentCom.Config["IReportService"])) as IReportService : new ReportService(); 
            string fileName = string.Empty;
            if (!string.IsNullOrEmpty(quarter))
            {
                fileName = AppDomain.CurrentDomain.BaseDirectory + string.Format("Report/DETAIL_QUARTER/{0}_{1}_{2}.xml", Convert.ToInt32(quarter), year, _currentCom.id);
            }
            else if (!string.IsNullOrEmpty(month))
            {
                fileName = AppDomain.CurrentDomain.BaseDirectory + string.Format("Report/DETAIL_MONTH/{0}_{1}_{2}.xml", Convert.ToInt32(month), year, _currentCom.id);
            }
            string fileNameTemp = AppDomain.CurrentDomain.BaseDirectory + string.Format("Report/DETAIL_MONTH/MauPhuLucHangThang.xlsx");
            //khai bao mau, khoi tao mot workbook, vao sheet
            int i = 2;
            int countRecord = 1;
            int stt = 1;
            string Ngaylap = string.Empty;
            //khoi tao css dinh dang cot
            FileStream streamfile = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            using (StreamReader reader = new StreamReader(streamfile))
            {
                //doc dong du lieu 
                string tempString = reader.ReadLine();
                //doc mau hoa don
                FileInfo FileTemp = new FileInfo(fileNameTemp);
                //mau file excel
                using (ExcelPackage package = new ExcelPackage(FileTemp, true))
                {
                    ExcelWorksheet ws = package.Workbook.Worksheets[1];
                    if (!string.IsNullOrEmpty(month))
                    {
                        ws.Cells[4, 3].Value = "[1]Kỳ tính thuế:" + " Tháng " + month + " năm " + year;
                    }
                    else
                    {
                        ws.Cells[4, 3].Value = "[1]Kỳ tính thuế:" + " Quý " + quarter + " năm " + year;
                    }
                    ws.Cells[5, 4].Value = _currentCom.Name;
                    ws.Cells[6, 4].Value = _currentCom.TaxCode;
                    int row = 15;
                    //trong khi du lieu khong rong va khong phai ket thuc
                    while (!string.IsNullOrWhiteSpace(tempString) && (reader.Peek() >= 0))
                    {
                        //doc tung dong du lieu
                        tempString = reader.ReadLine();
                        if (tempString.Contains("ngayLapTKhai"))
                        {
                            string temngaylap = "<ngaylap>" + tempString + "</ngaylap>";
                            XElement item = XElement.Parse(tempString);
                            Ngaylap = item.Value;
                        } //check neu co dong ChiTietHHDV thi bat dau lay du lieu
                        else if ("<ChiTietHHDVKChiuThue>".Equals(tempString.Trim()))
                        {
                            ws.Cells[row, 2].Value = "1. Hàng hóa, dịch vụ không chịu thuế giá trị gia tăng (GTGT):";
                            ws.Cells[row, 2].Style.Font.Bold = true;
                            row++;
                            tempString = reader.ReadLine();
                            while (!string.IsNullOrWhiteSpace(tempString) && (reader.Peek() >= 0) && !"</ChiTietHHDVKChiuThue>".Equals(tempString.Trim()))
                            {
                                if (reader.EndOfStream)
                                    break;
                                importExcel(tempString, ws, row, stt);
                                row++;
                                stt++;
                                countRecord++;
                                tempString = reader.ReadLine();
                            }
                        }
                        else if (tempString.Contains("<tongDThuBRaKChiuThue>"))
                        {
                            string temXml = "<count0>" + tempString + "</count0>";
                            XElement item = XElement.Parse(temXml);
                            ws.Cells[row, 2].Value = "Tổng Cộng";
                            ws.Cells[row, 2].Style.Font.Bold = true;
                            ws.Cells[row, 7].Value = Convert.ToDecimal(item.Element("tongDThuBRaKChiuThue").Value);
                            row++;
                            countRecord++;
                        }
                        else if ("<ChiTietHHDVThue0>".Equals(tempString.Trim()))
                        {
                            stt = 1;
                            ws.Cells[row, 2].Value = "2. Hàng hoá, dịch vụ chịu thuế suất thuế GTGT 0%:";
                            ws.Cells[row, 2].Style.Font.Bold = true;
                            row++;
                            tempString = reader.ReadLine();
                            while (!string.IsNullOrWhiteSpace(tempString) && (reader.Peek() >= 0) && !"</ChiTietHHDVThue0>".Equals(tempString.Trim()))
                            {
                                if (reader.EndOfStream)
                                    break;
                                importExcel(tempString, ws, row, stt);
                                row++;
                                stt++;
                                countRecord++;
                                tempString = reader.ReadLine();
                            }
                        }
                        else if (tempString.Contains("<tongDThuBRaThue0>"))
                        {
                            string temXml = "<count0>" + tempString + "</count0>";
                            XElement item = XElement.Parse(temXml);
                            ws.Cells[row, 2].Value = "Tổng Cộng";
                            ws.Cells[row, 2].Style.Font.Bold = true;
                            ws.Cells[row, 7].Value = Convert.ToDecimal(item.Element("tongDThuBRaThue0").Value);
                            ws.Cells[row, 8].Value = Convert.ToDecimal(item.Element("tongThueBRaThue0").Value);
                            row++;
                            countRecord++;
                        }
                        else if ("<ChiTietHHDVThue5>".Equals(tempString.Trim()))
                        {
                            stt = 1;
                            ws.Cells[row, 2].Value = "3. Hàng hoá, dịch vụ chịu thuế suất thuế GTGT 5%:";
                            ws.Cells[row, 2].Style.Font.Bold = true;
                            row++;
                            tempString = reader.ReadLine();
                            while (!string.IsNullOrWhiteSpace(tempString) && (reader.Peek() >= 0) && !"</ChiTietHHDVThue5>".Equals(tempString.Trim()))
                            {
                                if (reader.EndOfStream)
                                    break;
                                importExcel(tempString, ws, row, stt);
                                row++;
                                stt++;
                                countRecord++;
                                tempString = reader.ReadLine();
                            }
                        }
                        else if (tempString.Contains("<tongDThuBRaThue5>"))
                        {
                            string temXml = "<count5>" + tempString + "</count5>";
                            XElement item = XElement.Parse(temXml);
                            ws.Cells[row, 2].Value = "Tổng Cộng";
                            ws.Cells[row, 2].Style.Font.Bold = true;
                            ws.Cells[row, 7].Value = Convert.ToDecimal(item.Element("tongDThuBRaThue5").Value);
                            ws.Cells[row, 8].Value = Convert.ToDecimal(item.Element("tongThueBRaThue5").Value);
                            row++;
                            countRecord++;
                        }
                        else if ("<ChiTietHHDVThue10>".Equals(tempString.Trim()))
                        {
                            stt = 1;
                            ws.Cells[row, 2].Value = "4. Hàng hoá, dịch vụ chịu thuế suất thuế GTGT 10%:";
                            ws.Cells[row, 2].Style.Font.Bold = true;
                            row++;
                            tempString = reader.ReadLine();
                            while (!string.IsNullOrWhiteSpace(tempString) && (reader.Peek() >= 0) && !"</ChiTietHHDVThue10>".Equals(tempString.Trim()))
                            {
                                if (reader.EndOfStream)
                                    break;
                                importExcel(tempString, ws, row, stt);
                                row++;
                                stt++;
                                countRecord++;
                                tempString = reader.ReadLine();
                            }
                        }
                        else if (tempString.Contains("<tongDThuBRaThue10>"))
                        {
                            string temXml = "<count10>" + tempString + "</count10>";
                            XElement item = XElement.Parse(temXml);
                            ws.Cells[row, 2].Value = "Tổng Cộng";
                            ws.Cells[row, 2].Style.Font.Bold = true;
                            ws.Cells[row, 7].Value = Convert.ToDecimal(item.Element("tongDThuBRaThue10").Value);
                            ws.Cells[row, 7].Style.Font.Bold = true;
                            ws.Cells[row, 8].Value = Convert.ToDecimal(item.Element("tongThueBRaThue10").Value);
                            ws.Cells[row, 8].Style.Font.Bold = true;
                            row++;
                            countRecord++;
                            deleteStyleBorderRomdom(ws, row, 9);
                        }
                        else if (tempString.Contains("<tongDThuBRa>"))
                        {
                            string TemTongxml = "<count>" + tempString + "</count>";
                            XElement item = XElement.Parse(TemTongxml);
                            row = row + 3;
                            ws.Cells[row, 2].Style.Font.Bold = true;
                            ws.Cells[row, 2].Value = "Tổng doanh thu hàng hoá, dịch vụ bán ra chịu thuế GTGT (*): ";
                            ws.Cells[row, 7].Value = Convert.ToDecimal(item.Element("tongDThuBRa").Value);
                            ws.Cells[row, 7].Style.Font.Bold = true;
                            ws.Cells[row + 1, 2].Value = "Tổng số thuế GTGT của hàng hóa, dịch vụ bán ra (**):";
                            ws.Cells[row + 1, 2].Style.Font.Bold = true;
                            ws.Cells[row + 1, 8].Value = Convert.ToDecimal(item.Element("tongThueBRa").Value);
                            ws.Cells[row + 1, 8].Style.Font.Bold = true;
                            row++;
                            countRecord++;
                        }

                    }//ket thuc while
                    ws.Cells[row + 1, 2].Value = "Tôi cam đoan số liệu khai trên là đúng và chịu trách nhiệm trước pháp luật về những số liệu đã khai./... ";
                    ws.Cells[row + 1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["B" + (row + 1) + ":" + "J" + (row + 1)].Merge = true;

                    ws.Cells[row + 5, 2].Value = " NHÂN VIÊN ĐẠI LÝ THUẾ";
                    ws.Cells[row + 5, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["B" + (row + 5) + ":" + "D" + (row + 5)].Merge = true;
                    ws.Cells[row + 5, 2].Style.Font.Bold = true;

                    ws.Cells[row + 6, 2].Value = "Họ và tên:";
                    ws.Cells[row + 6, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["B" + (row + 6) + ":" + "D" + (row + 6)].Merge = true;

                    ws.Cells[row + 7, 2].Value = "Chứng chỉ hành nghề số: :";
                    ws.Cells[row + 7, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["B" + (row + 7) + ":" + "D" + (row + 7)].Merge = true;

                    ws.Cells[row + 5, 8].Value = "Hà nội, ngày: " + Ngaylap.Substring(8, 2) + "tháng " + Ngaylap.Substring(5, 2) + "năm " + Ngaylap.Substring(0, 4);
                    ws.Cells[row + 5, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["H" + (row + 5) + ":" + "J" + (row + 5)].Merge = true;
                    ws.Cells[row + 5, 8].Style.Font.Bold = true;

                    ws.Cells[row + 6, 8].Value = "NGƯỜI NỘP THUẾ hoặc";
                    ws.Cells[row + 7, 8].Value = "ĐẠI DIỆN HỢP PHÁP CỦA NGƯỜI NỘP THUẾ ";
                    ws.Cells[row + 8, 8].Value = "Ký, ghi rõ họ tên, chức vụ và đóng dấu (nếu có)";
                    ws.Cells[row + 6, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[row + 7, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[row + 8, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["H" + (row + 8) + ":" + "J" + (row + 8)].Merge = true;

                    ws.Cells["H" + (row + 6) + ":" + "J" + (row + 6)].Merge = true;
                    ws.Cells["H" + (row + 7) + ":" + "J" + (row + 7)].Merge = true;
                    ws.Cells[row + 6, 8].Style.Font.Bold = true;
                    ws.Cells[row + 7, 8].Style.Font.Bold = true;

                    ws.Cells[row + 10, 2].Value = "Ghi Chú";
                    ws.Cells[row + 10, 2].Style.Font.Bold = true;

                    ws.Cells[row + 11, 2].Value = "(*) Tổng doanh thu hàng hóa, dịch vụ bán ra chịu thuế GTGT là tổng cộng số liệu tại cột 6 của dòng tổng của các chỉ tiêu 2, 3, 4.";
                    ws.Cells["B" + (row + 11) + ":" + "I" + (row + 11)].Merge = true;
                    ws.Cells[row + 12, 2].Value = "(**) Tổng số thuế GTGT của hàng hóa, dịch vụ bán ra là tổng cộng số liệu tại cột 7 của dòng tổng của các chỉ tiêu 2, 3, 4.";
                    ws.Cells["B" + (row + 12) + ":" + "I" + (row + 12)].Merge = true;
                    return File(package.GetAsByteArray(), "application/vnd.ms-excel", "ReportDetail.xlsx");
                }//ket thuc lay mau excel
            }//ket thuc doc file xml           
        }
        private void deleteStyleBorderRomdom(ExcelWorksheet ws, int row, int col)
        {
            //lặp cột
            for (int i = 2; i < col + 1; i++)
            {
                ws.Cells[row + 1, i, row + 1000, i].Style.Border.BorderAround(ExcelBorderStyle.None);
            }
            //lặp xóa dòng
            for (int j = row + 1; j < row + 1000; j++)
            {
                ws.Cells[j, 2, j, col].Style.Border.BorderAround(ExcelBorderStyle.None);
            }
        }
        //thuc hien import vao excel
        private void importExcel(string tempString, ExcelWorksheet ws, int row, int stt)
        {
            XElement item = XElement.Parse(tempString);
            ws.Cells[row, 2].Value = stt;
            ws.Cells[row, 3].Value = item.Element("soHDon").Value;
            ws.Cells[row, 4].Value = item.Element("ngayPHanh").Value;
            ws.Cells[row, 5].Value = item.Element("tenNMUA").Value;
            ws.Cells[row, 6].Value = item.Element("mstNMUA").Value;
            ws.Cells[row, 7].Value = Convert.ToDecimal(item.Element("dsoBanChuaThue").Value);
            ws.Cells[row, 8].Value = Convert.ToDecimal(item.Element("thueGTGT").Value);
            ws.Cells[row, 9].Value = item.Element("ghiChu").Value;
        }

        public FileResult DownloadReportMonth_xml(string month, string quarter, string year)
        {
            try
            {
                Company _currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                IReportService repSrv = _currentCom.Config.Keys.Contains("IReportService") ? IoC.Resolve(Type.GetType(_currentCom.Config["IReportService"])) as IReportService : new ReportService(); 
                string fileName = string.Empty;
                string Namefile = string.Empty;
                if (!string.IsNullOrEmpty(quarter))
                {
                    fileName = AppDomain.CurrentDomain.BaseDirectory + string.Format("Report/DETAIL_QUARTER/{0}_{1}_{2}.xml", Convert.ToInt32(quarter), year, _currentCom.id);
                    Namefile = quarter + "_" + year + "_" + _currentCom.id;
                }
                else if (!string.IsNullOrEmpty(month))
                {
                    int m = int.Parse(month);
                    int y = int.Parse(year);
                    fileName = AppDomain.CurrentDomain.BaseDirectory + string.Format("Report/DETAIL_MONTH/{0}_{1}_{2}.xml", Convert.ToInt32(month), year, _currentCom.id);
                    Namefile = month + "_" + year + "_" + _currentCom.id;
                }


                string contentType = "text/xml";
                return File(fileName, contentType, Namefile);
            }
            catch (Exception ex)
            {
                if (!(ex is FileNotFoundException))
                    throw new Exception("Không tìm thấy file báo cáo");
                else throw ex;
            }

        }
        private StringWriter ReportMonthXML(string xmlReportMonth)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlReportMonth);
            // transform
            StringWriter sw = new StringWriter();
            XslCompiledTransform xslCT = new XslCompiledTransform();
            xslCT.Load(System.Web.HttpContext.Current.Server.MapPath("~/Views/ReportsInv/BKHDDV.xslt"));
            xslCT.Transform(xmlDoc, null, sw);
            return sw;
        }

        private StringWriter report_User(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            StringWriter sw = new StringWriter();
            XslCompiledTransform xslCT = new XslCompiledTransform();
            xslCT.Load(System.Web.HttpContext.Current.Server.MapPath("~/Views/ReportsInv/THSDHD.xslt"));
            xslCT.Transform(xmlDoc, null, sw);
            return sw;
        }
        #endregion
        #region "bao cao thay the sua doi"
        //bao cao thay the
        [RBACAuthorize(Permissions = "Report_AdjustInv")]
        public ActionResult ReportAdjust(ReportEinv_Adjust model, int? page)
        {
            int defautPagesize = 10;
            int total = 0;
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            IList<RecordAdjust> lstRecordAdjust = getrecordAdjustReplace(model.FromDate, model.ToDate, model.username, BusinessLogType.Adjust);
            total = lstRecordAdjust.Count();
            model.PageListRecordAdjust = new PagedList<RecordAdjust>(lstRecordAdjust, currentPageIndex, defautPagesize, total);
            return View(model);
        }
        //bao cao thay the
        [RBACAuthorize(Permissions = "Report_ReplaceInv")]
        public ActionResult ReportReplate(ReportEinv_Adjust model, int? page)
        {
            int defautPagesize = 10;
            int total = 0;
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            IList<RecordAdjust> lstRecordAdjust = getrecordAdjustReplace(model.FromDate, model.ToDate, model.username, BusinessLogType.Replace);
            total = lstRecordAdjust.Count();
            model.PageListRecordAdjust = new PagedList<RecordAdjust>(lstRecordAdjust, currentPageIndex, defautPagesize, total);
            return View(model);
        }

        private IList<RecordAdjust> getrecordAdjustReplace(string FromDate, string ToDate, string username, BusinessLogType type)
        {
            IList<RecordAdjust> lstRecord = new List<RecordAdjust>();
            try
            {
                DateTime? DateFrom = null;
                DateTime? DateTo = null;
                IList<BusinessLog> logLst = new List<BusinessLog>();
                if (!string.IsNullOrEmpty(ToDate) && !string.IsNullOrEmpty(FromDate))
                {
                    if (!string.IsNullOrWhiteSpace(FromDate)) DateFrom = DateTime.ParseExact(FromDate, "dd/MM/yyyy", null);
                    if (!string.IsNullOrWhiteSpace(ToDate)) DateTo = DateTime.ParseExact(ToDate, "dd/MM/yyyy", null);
                    if (DateFrom.Value.AddDays(31) <= DateTo)
                    {
                        Messages.AddErrorMessage("Bạn chỉ được chọn ngày bắt đầu và ngày kết thúc trong khoảng 1 tháng!");
                        return lstRecord;
                    }
                    else
                    {
                        int comID = ((EInvoiceContext)FXContext.Current).CurrentCompany.id;
                        ILogSystemService logSrv = IoC.Resolve<ILogSystemService>();
                        logLst = (List<BusinessLog>)logSrv.getByDate(comID, username, DateFrom, DateTo, type);
                    }
                    if (logLst.Count() > 0)
                    {
                        int i = 1;
                        foreach (var loginfo in logLst)
                        {
                            XElement xe = XElement.Parse(loginfo.Data);
                            RecordAdjust item = new RecordAdjust();
                            item.stt = i;
                            item.username = loginfo.UserName;
                            //hoa don cu
                            item.patternOlder = xe.Element("OldPattern").Value;
                            item.serialOlder = xe.Element("OldSerial").Value;
                            item.noOlder = xe.Element("OldNo").Value;
                            if (!string.IsNullOrEmpty(xe.Element("OldPublishDate").Value))
                                item.publishDateOlder = DateTime.ParseExact(xe.Element("OldPublishDate").Value, "dd/MM/yyyy", new CultureInfo("en-US"));                            
                            if (!string.IsNullOrEmpty(xe.Element("OldAmount").Value)) item.totalMoneyOlder = Convert.ToDecimal(xe.Element("OldAmount").Value);
                            //hoa don moi
                            item.patternNew = xe.Element("NewPattern").Value;
                            item.serialNew = xe.Element("NewSerial").Value;
                            item.noNew = xe.Element("NewNo").Value;
                            if (!string.IsNullOrEmpty(xe.Element("NewPublishDate").Value))
                                item.publishDateNew = DateTime.ParseExact(xe.Element("NewPublishDate").Value, "dd/MM/yyyy", new CultureInfo("en-US"));                                
                            if (!string.IsNullOrEmpty(xe.Element("NewAmount").Value)) item.totalMoneyNew = Convert.ToDecimal(xe.Element("NewAmount").Value);
                            //thong tin khach hang
                            item.cusnameNew = xe.Element("NewCusName").Value;
                            item.addressCusNew = xe.Element("CusAddress").Value;
                            item.cuscodeNew = xe.Element("NewCusCode").Value;
                            item.cusTaxcode = xe.Element("NewCusTaxCode").Value;
                            //cac thong tin khac
                            item.username = loginfo.UserName;
                            item.proccessdate = loginfo.Datetime.ToString("dd/MM/yyyy");
                            item.status = Convert.ToInt32(type);
                            lstRecord.Add(item);
                            i++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return lstRecord;
        }
        public ActionResult DownloadExcel(string FromDate, string ToDate, string username, string status)
        {
            string fileNameTemp = string.Empty;

            ICompanyService _comSrv = IoC.Resolve<ICompanyService>();
            Company _currentCom = _comSrv.Getbykey(((EInvoiceContext)FXContext.Current).CurrentCompany.id);
            IList<RecordAdjust> lstRecord = new List<RecordAdjust>();
            if (status == "1")
            {
                lstRecord = getrecordAdjustReplace(FromDate, ToDate, username, BusinessLogType.Adjust);
                fileNameTemp = AppDomain.CurrentDomain.BaseDirectory + string.Format("Report/BCHD/DieuChinh.xlsx");
            }
            else
            {
                lstRecord = getrecordAdjustReplace(FromDate, ToDate, username, BusinessLogType.Replace);
                fileNameTemp = AppDomain.CurrentDomain.BaseDirectory + string.Format("Report/BCHD/ThayThe.xlsx");
            }

            FileInfo FileTemp = new FileInfo(fileNameTemp);
            //mau file excel
            using (ExcelPackage package = new ExcelPackage(FileTemp, true))
            {
                ExcelWorksheet ws = package.Workbook.Worksheets[1];
                ws.Cells[2, 4].Value = _currentCom.Name;
                ws.Cells[3, 4].Value = _currentCom.Address;
                ws.Cells[4, 4].Value = _currentCom.TaxCode;
                ws.Cells[6, 2].Value = string.Format("Từ ngày: {0} đến ngày: {1}", FromDate, ToDate);
                ws.Cells[7, 2].Value = string.Format("Tổng số hóa đơn trên bảng kê:{0} hóa đơn", lstRecord.Count());
                int row = 11;
                int stt = 1;
                foreach (RecordAdjust item in lstRecord)
                {
                    ImportExcelAdjust(item, ws, row, stt);
                    row++;
                    stt++;
                }
                deleteStyleBorderReport(ws, row - 1, 18);
                //người lập
                ws.Cells[row + 1, 3].Value = "Người lập";
                ws.Cells[row + 1, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["C" + (row + 1) + ":" + "D" + (row + 1)].Merge = true;
                ws.Cells[row + 1, 3].Style.Font.Bold = true;

                ws.Cells[row + 2, 3].Value = "Họ tên, chữ ký";
                ws.Cells[row + 2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["C" + (row + 2) + ":" + "D" + (row + 2)].Merge = true;
                ws.Cells[row + 2, 3].Style.Font.Italic = true;
                //kế toán trưởng
                ws.Cells[row + 1, 8].Value = "Kế toán trưởng";
                ws.Cells[row + 1, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["H" + (row + 1) + ":" + "I" + (row + 1)].Merge = true;
                ws.Cells[row + 1, 8].Style.Font.Bold = true;

                ws.Cells[row + 2, 8].Value = "(Họ tên, chữ ký)";
                ws.Cells[row + 2, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["H" + (row + 2) + ":" + "I" + (row + 2)].Merge = true;
                ws.Cells[row + 2, 8].Style.Font.Italic = true;
                //giám đốc
                ws.Cells[row, 15].Value = "Ngày...tháng...năm";
                ws.Cells[row, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["O" + row + ":" + "R" + row].Merge = true;
                ws.Cells[row, 15].Style.Font.Italic = true;

                ws.Cells[row + 1, 15].Value = "Giám đốc";
                ws.Cells[row + 1, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["O" + (row + 1) + ":" + "R" + (row + 1)].Merge = true;
                ws.Cells[row + 1, 15].Style.Font.Bold = true;

                ws.Cells[row + 2, 15].Value = "(Họ tên, chữ ký, dấu đơn vị)";
                ws.Cells[row + 2, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["O" + (row + 2) + ":" + "R" + (row + 2)].Merge = true;
                ws.Cells[row + 2, 15].Style.Font.Italic = true;

                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BaocaoHD.xlsx");
            }
        }
        //thuc hien import vao thay thế chỉnh sửa
        private void ImportExcelAdjust(RecordAdjust recordAdjust, ExcelWorksheet ws, int row, int stt)
        {
            ws.Cells[row, 2].Value = stt;

            ws.Cells[row, 3].Value = recordAdjust.patternOlder;
            ws.Cells[row, 4].Value = recordAdjust.serialOlder;
            ws.Cells[row, 5].Value = Convert.ToInt32(recordAdjust.noOlder).ToString("0000000");
            ws.Cells[row, 6].Value = recordAdjust.publishDateOlder.ToString("dd/MM/yyyy");
            ws.Cells[row, 7].Value = recordAdjust.totalMoneyOlder;

            ws.Cells[row, 8].Value = recordAdjust.patternNew;
            ws.Cells[row, 9].Value = recordAdjust.serialNew;
            ws.Cells[row, 10].Value = Convert.ToInt32(recordAdjust.noNew).ToString("0000000");
            ws.Cells[row, 11].Value = recordAdjust.publishDateNew;
            ws.Cells[row, 12].Value = recordAdjust.totalMoneyNew;

            ws.Cells[row, 13].Value = recordAdjust.cusnameNew;
            ws.Cells[row, 14].Value = recordAdjust.addressCusNew;
            ws.Cells[row, 15].Value = recordAdjust.cuscodeNew;
            ws.Cells[row, 16].Value = recordAdjust.cusTaxcode;
            ws.Cells[row, 17].Value = recordAdjust.username;
        }
        private void deleteStyleBorderReport(ExcelWorksheet ws, int row, int col)
        {
            //lặp cột
            for (int i = 2; i < col + 1; i++)
            {
                ws.Cells[row + 1, i, row + 100, i].Style.Border.BorderAround(ExcelBorderStyle.None);
            }
            //lặp xóa dòng
            for (int j = row + 1; j < row + 100; j++)
            {
                ws.Cells[j, 2, j, col].Style.Border.BorderAround(ExcelBorderStyle.None);
            }
        }
        #endregion "end excel report"
        #region "Báo cáo hủy hóa đơn"
        [RBACAuthorize(Permissions = "Report_CancelInvNotRe")]
        public ActionResult ReportCancelInv(ReportEinv_Cancel model, int? page)
        {
            int defautPagesize = 10;
            int total = 0;
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            IList<RecordInvCancel> lstRecordInvCancel = getrecordInvCancel(model.FromDate, model.ToDate, model.username);
            total = lstRecordInvCancel.Count();
            model.PageListRecordInvCancel = new PagedList<RecordInvCancel>(lstRecordInvCancel, currentPageIndex, defautPagesize, total);
            return View(model);
        }
        private static IList<string> Partition(IList<FileInfo> source, string keyword)
        {
            IList<string> lstpartition = new List<string>();
            foreach (FileInfo fi in source)
            {
                var fs = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using (var sr = new StreamReader(fs))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains(keyword))
                        {
                            lstpartition.Add(line);
                        }
                    }
                }
            }
            return lstpartition;
        }


        private IList<RecordInvCancel> getrecordInvCancel(string FromDate, string ToDate, string username)
        {
            IList<RecordInvCancel> lstRecordInvCancel = new List<RecordInvCancel>();
            try
            {
                DateTime? DateFrom = null;
                DateTime? DateTo = null;
                IList<BusinessLog> logLst = new List<BusinessLog>();
                if (!string.IsNullOrEmpty(ToDate) && !string.IsNullOrEmpty(FromDate))
                {
                    if (!string.IsNullOrWhiteSpace(FromDate)) DateFrom = DateTime.ParseExact(FromDate, "dd/MM/yyyy", null);
                    if (!string.IsNullOrWhiteSpace(ToDate)) DateTo = DateTime.ParseExact(ToDate, "dd/MM/yyyy", null);
                    if (DateFrom.Value.AddDays(31) <= DateTo)
                    {
                        Messages.AddErrorMessage("Bạn chỉ được chọn ngày bắt đầu và ngày kết thúc trong khoảng 1 tháng!");
                        return lstRecordInvCancel;
                    }
                    else
                    {
                        //files = d.GetFiles().Where(p => p.LastWriteTime >= DateFrom && p.LastWriteTime < DateTo.Value.AddDays(1)).ToList();
                        //lay du lieu
                        int comID = ((EInvoiceContext)FXContext.Current).CurrentCompany.id;
                        ILogSystemService logSrv = IoC.Resolve<ILogSystemService>();
                        logLst = (List<BusinessLog>)logSrv.getByDate(comID, username, DateFrom, DateTo, BusinessLogType.Cancel);
                    }
                    if (logLst.Count() > 0)
                    {
                        int i = 1;
                        foreach (var loginfor in logLst)
                        {
                            string dt = loginfor.Data;
                            if (dt.IndexOf("&") >= 0)
                                dt = dt.Replace("&", "");
                            XElement xe = XElement.Parse(dt);
                            RecordInvCancel item = new RecordInvCancel();
                            item.stt = i;
                            item.Username = loginfor.UserName;
                            item.pattern = xe.Element("Pattern").Value;
                            item.serial = xe.Element("Serial").Value;
                            item.no = xe.Element("NoData").Value;
                            item.publishDate = DateTime.ParseExact(xe.Element("PublishDate").Value, "dd/MM/yyyy", new CultureInfo("en-US"));                                
                            item.totalAmount = Convert.ToDecimal(xe.Element("Amount").Value);

                            item.cusName = xe.Element("CusName").Value;
                            item.addressCus = xe.Element("CusAddress").Value;
                            item.cusCode = xe.Element("CusCode").Value;
                            item.cusTaxCode = xe.Element("CusTaxCode").Value;

                            item.DayCancelInv = loginfor.Datetime.ToString("dd/MM/yyyy");
                            lstRecordInvCancel.Add(item);
                            i++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return lstRecordInvCancel;
        }
        public ActionResult DownloadExcelInvCancel(string FromDate, string ToDate, string username)
        {
            try
            {

                ICompanyService _comSrv = IoC.Resolve<ICompanyService>();
                Company _currentCom = _comSrv.Getbykey(((EInvoiceContext)FXContext.Current).CurrentCompany.id);
                IList<RecordInvCancel> lstRecord = getrecordInvCancel(FromDate, ToDate, username);
                string fileNameTemp = AppDomain.CurrentDomain.BaseDirectory + string.Format("Report/BCHD/Huy.xlsx");
                FileInfo FileTemp = new FileInfo(fileNameTemp);
                //mau file excel
                using (ExcelPackage package = new ExcelPackage(FileTemp, true))
                {
                    ExcelWorksheet ws = package.Workbook.Worksheets[1];
                    ws.Cells[2, 4].Value = _currentCom.Name;
                    ws.Cells[3, 4].Value = _currentCom.Address;
                    ws.Cells[4, 4].Value = _currentCom.TaxCode;
                    ws.Cells[7, 2].Value = string.Format("Từ ngày: {0} đến ngày: {1} ", FromDate, ToDate);
                    ws.Cells[8, 2].Value = string.Format("Tổng số hóa đơn trên bảng kê:{0} hóa đơn", lstRecord.Count());
                    int row = 11;
                    int stt = 1;
                    foreach (RecordInvCancel item in lstRecord)
                    {
                        ImportExcelCancel(item, ws, row, stt);
                        row++;
                        stt++;
                    }
                    deleteStyleBorderReport(ws, row - 1, 14);
                    //người lập
                    ws.Cells[row + 1, 3].Value = "Người lập";
                    ws.Cells[row + 1, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["C" + (row + 1) + ":" + "D" + (row + 1)].Merge = true;
                    ws.Cells[row + 1, 3].Style.Font.Bold = true;

                    ws.Cells[row + 2, 3].Value = "(Họ tên, chữ ký)";
                    ws.Cells[row + 2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["C" + (row + 2) + ":" + "D" + (row + 2)].Merge = true;
                    ws.Cells[row + 2, 3].Style.Font.Italic = true;
                    //kế toán trưởng
                    ws.Cells[row + 1, 8].Value = "Kế toán trưởng";
                    ws.Cells[row + 1, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["H" + (row + 1) + ":" + "I" + (row + 1)].Merge = true;
                    ws.Cells[row + 1, 8].Style.Font.Bold = true;

                    ws.Cells[row + 2, 8].Value = "(Họ tên, chữ ký)";
                    ws.Cells[row + 2, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["H" + (row + 2) + ":" + "I" + (row + 2)].Merge = true;
                    ws.Cells[row + 2, 8].Style.Font.Italic = true;
                    //giám đốc
                    ws.Cells[row, 13].Value = "Ngày...tháng...năm";
                    ws.Cells[row, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["M" + row + ":" + "N" + row].Merge = true;
                    ws.Cells[row, 13].Style.Font.Italic = true;

                    ws.Cells[row + 1, 13].Value = "Giám đốc";
                    ws.Cells[row + 1, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["M" + (row + 1) + ":" + "N" + (row + 1)].Merge = true;
                    ws.Cells[row + 1, 13].Style.Font.Bold = true;

                    ws.Cells[row + 2, 13].Value = "(Họ tên, chữ ký, dấu đơn vị)";
                    ws.Cells[row + 2, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["M" + (row + 2) + ":" + "N" + (row + 2)].Merge = true;
                    ws.Cells[row + 2, 13].Style.Font.Italic = true;
                    return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BaocaoHuy.xlsx");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return null;
            }

        }
        //xuat excel huy hoa đơn
        private void ImportExcelCancel(RecordInvCancel recordInvCancel, ExcelWorksheet ws, int row, int stt)
        {
            ws.Cells[row, 2].Value = stt;

            ws.Cells[row, 3].Value = recordInvCancel.pattern;
            ws.Cells[row, 4].Value = recordInvCancel.serial;
            ws.Cells[row, 5].Value = Convert.ToInt32(recordInvCancel.no).ToString("0000000");
            ws.Cells[row, 6].Value = recordInvCancel.publishDate;
            ws.Cells[row, 7].Value = recordInvCancel.totalAmount;

            ws.Cells[row, 8].Value = recordInvCancel.cusName;
            ws.Cells[row, 9].Value = recordInvCancel.addressCus;
            ws.Cells[row, 10].Value = recordInvCancel.cusCode;
            ws.Cells[row, 11].Value = recordInvCancel.cusTaxCode;
            ws.Cells[row, 12].Value = recordInvCancel.Username;
            ws.Cells[row, 13].Value = recordInvCancel.DayCancelInv;
        }
        #endregion
    }
}
