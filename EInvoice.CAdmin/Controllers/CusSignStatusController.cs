using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;
using EInvoice.Core.IService;
using EInvoice.Core.Domain;
using FX.Core;
using FX.Utils;
using EInvoice.Core;
using FX.Utils.MVCMessage;
using FX.Utils.MvcPaging;
using IdentityManagement.Authorization;
using EInvoice.Core.ServiceImp;
using EInvoice.CAdmin.Models;
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Style;
using FX.Context;

namespace EInvoice.CAdmin.Controllers
{
    public class CusSignStatusController : ShareController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CusSignStatusController));
        //
        // GET: /CusSignStatus/
        [RBACAuthorize(Permissions = "view_CusSignStatus")]
        public ActionResult Index(CusSignIndexModel model, int? page, int? Pagesize)
        {
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IPublishInvoiceService _PubIn = IoC.Resolve<IPublishInvoiceService>();
            IList<string> lstPattern = _PubIn.LstByPattern(currentCom.id, 2);
            model.lstpattern = new SelectList(lstPattern);
            if (lstPattern.Count == 0)
            {
                Messages.AddErrorFlashMessage(Resources.Message.MInv_SMesNoPubAccess);
                return RedirectToAction("/");
            }
            if (model == null) model = new CusSignIndexModel();
            //khoi tao khi pattern rong
            if (string.IsNullOrEmpty(model.Pattern)) model.Pattern = lstPattern[0];
            IList<IInvoice> lstInv;
            //tham so phan trang
            int defautPagesize = Pagesize.HasValue ? Convert.ToInt32(Pagesize) : 10;
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            int totalRecords = 0;
            //su dung khoi tao service
            IInvoiceService IInvSrv = InvServiceFactory.GetService(model.Pattern, currentCom.id);
            decimal No;
            decimal.TryParse(model.InvNo, out No); if (No == 0) No = -1;
            //tim kiem theo mot hoa don theo so hoa don
            if (!string.IsNullOrWhiteSpace(model.InvNo))
            {
                IInvoice Invoice = IInvSrv.GetByNo(currentCom.id, model.Pattern, model.Serial, No);
                lstInv = new List<IInvoice>();
                if (Invoice != null) lstInv.Add(Invoice);
                lstInv = lstInv.Where(x => x.CusSignStatus != cusSignStatus.NocusSignStatus && x.CusSignStatus != cusSignStatus.ViewNocusSignStatus).ToList();
                totalRecords = lstInv.Count;
            }
            //tim kiem danh sach hoa don voi cac tham so con lai
            else
            {
                DateTime? DateFrom = null;
                DateTime? DateTo = null;
                if (!string.IsNullOrWhiteSpace(model.FromDate)) DateFrom = DateTime.ParseExact(model.FromDate, "dd/MM/yyyy", null);
                if (!string.IsNullOrWhiteSpace(model.ToDate)) DateTo = DateTime.ParseExact(model.ToDate, "dd/MM/yyyy", null);
                if (model.SignStatus == 0)
                {
                    lstInv = IInvSrv.SearchByCusSign(currentCom.id, model.Pattern, model.Serial, DateFrom, DateTo, null, currentPageIndex, defautPagesize, out totalRecords, cusSignStatus.NoSignStatus, cusSignStatus.ViewNoSignStatus);
                }
                else if (model.SignStatus == 1)
                {
                    lstInv = IInvSrv.SearchByCusSign(currentCom.id, model.Pattern, model.Serial, DateFrom, DateTo, null, currentPageIndex, defautPagesize, out totalRecords, cusSignStatus.SignStatus);
                }
                else
                {
                    lstInv = IInvSrv.SearchByCusSign(currentCom.id, model.Pattern, model.Serial, DateFrom, DateTo, null, currentPageIndex, defautPagesize, out totalRecords, cusSignStatus.NoSignStatus, cusSignStatus.SignStatus, cusSignStatus.ViewNoSignStatus);
                }
            }

            log.Info("Infomation Search = pattern = " + model.Pattern + "| status = " + model.SignStatus + "| FromDate = " + model.FromDate + "| ToDate =" + model.ToDate);
            List<string> olstserial = _PubIn.LstBySerial(currentCom.id, model.Pattern, 1);
            model.lstserial = new SelectList(olstserial);
            model.PageListCusSign = new PagedList<IInvoice>(lstInv, currentPageIndex, defautPagesize, totalRecords);
            model.defautPagesize = defautPagesize;
            return View(model);
        }
        public ActionResult GetSerial(string Pattern)
        {
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IPublishInvoiceService _PubIn = IoC.Resolve<IPublishInvoiceService>();
            var pu = _PubIn.LstBySerial(currentCom.id, Pattern, 1);
            return Json(new
            {
                pu
            });
        }

        //xuất ra file excel nếu chỉ có 200000 bản ghi.
        public ActionResult ImportExcel(CusSignIndexModel model)
        {
            try
            {
                IList<IInvoice> lstInv;
                int totalRecords = 0;
                Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                //su dung khoi tao service
                IInvoiceService IInvSrv = InvServiceFactory.GetService(model.Pattern, currentCom.id);
                decimal No;
                decimal.TryParse(model.InvNo, out No); if (No == 0) No = -1;
                //tim kiem theo mot hoa don theo so hoa don
                if (!string.IsNullOrWhiteSpace(model.InvNo))
                {
                    IInvoice Invoice = IInvSrv.GetByNo(currentCom.id, model.Pattern, model.Serial, No);
                    lstInv = new List<IInvoice>();
                    if (Invoice != null) lstInv.Add(Invoice);
                    lstInv = lstInv.Where(x => x.CusSignStatus != cusSignStatus.NocusSignStatus && x.CusSignStatus != cusSignStatus.ViewNocusSignStatus).ToList();
                    totalRecords = lstInv.Count;
                }
                //tim kiem danh sach hoa don voi cac tham so con lai
                else
                {
                    DateTime? DateFrom = null;
                    DateTime? DateTo = null;
                    if (!string.IsNullOrWhiteSpace(model.FromDate)) DateFrom = DateTime.ParseExact(model.FromDate, "dd/MM/yyyy", null);
                    if (!string.IsNullOrWhiteSpace(model.ToDate)) DateTo = DateTime.ParseExact(model.ToDate, "dd/MM/yyyy", null);
                    if (model.SignStatus.HasValue)
                        lstInv = IInvSrv.SearchByCusSign(currentCom.id, model.Pattern, model.Serial, DateFrom, DateTo, "1", 0, 0, out totalRecords, (cusSignStatus)model.SignStatus.Value);
                    else
                        lstInv = IInvSrv.SearchByCusSign(currentCom.id, model.Pattern, model.Serial, DateFrom, DateTo, "1", 0, 0, out totalRecords, cusSignStatus.NoSignStatus, cusSignStatus.SignStatus, cusSignStatus.ViewNoSignStatus);
                }
                if (lstInv == null)
                {
                    Response.Write("Số bản ghi nhiều quá 200000!");
                    Response.End();
                    return null;
                }
                string fileTemp = AppDomain.CurrentDomain.BaseDirectory + string.Format("Report/BAOCAO_NOIBO/MauTrangThaiKyHoaDon.xlsx");
                FileInfo FileNameTemp = new FileInfo(fileTemp);
                using (ExcelPackage package = new ExcelPackage(FileNameTemp, true))
                {
                    ExcelWorksheet ws = package.Workbook.Worksheets[1];
                    int row = 11;
                    int stt = 1;
                    ws.Cells[3, 2].Value = "Ký hiệu mẫu: " + model.Pattern;
                    ws.Cells[4, 2].Value = "Ký hiệu hóa đơn: " + model.Serial;
                    ws.Cells[5, 2].Value = "Trạng thái Ký: " + model.SignStatus;
                    ws.Cells[6, 2].Value = "Từ ngày: " + model.FromDate;
                    ws.Cells[6, 5].Value = "Đến ngày: " + model.ToDate;
                    ws.Cells[7, 2].Value = "Số hóa đơn: " + model.InvNo;
                    foreach (var item in lstInv)
                    {
                        importExcel(item, ws, row, stt);
                        stt++;
                        row++;
                    }
                    deleteStyleBorderRomdom(ws, row, 8);
                    string today = DateTime.Now.ToShortDateString();
                    ws.Cells[row + 2, 6].Value = "Hà nội, ngày: " + today;
                    ws.Cells[row + 2, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["F" + (row + 2) + ":" + "H" + (row + 2)].Merge = true;

                    ws.Cells[row + 3, 6].Style.Font.Bold = true;
                    ws.Cells[row + 3, 6].Value = "Người lập báo cáo";
                    ws.Cells[row + 3, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["F" + (row + 3) + ":" + "H" + (row + 3)].Merge = true;
                    return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TrangThaiKyHoaDon.xlsx");
                }
            }
            catch (Exception ex)
            {
                log.Error("Error:" + ex);
                Response.Clear();
                Response.Write("<script type='text/javascript'>alert('Có lỗi trong quá trình tải dữ liệu, kiểm tra log để biết thêm chi tiết');</script>");
                Response.End();
                Response.Flush();
                return null;
            }

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
        private void importExcel(IInvoice Inv, ExcelWorksheet ws, int row, int stt)
        {
            ws.Cells[row, 2].Value = stt;
            ws.Cells[row, 3].Value = Inv.Pattern;
            ws.Cells[row, 4].Value = Inv.Serial;
            ws.Cells[row, 5].Value = Inv.No.ToString("0000000");
            ws.Cells[row, 6].Value = Inv.CusName;
            ws.Cells[row, 7].Value = Inv.ArisingDate.ToString("dd/MM/yyyy");
            if (Inv.CusSignStatus == cusSignStatus.NoSignStatus || Inv.CusSignStatus == cusSignStatus.ViewNoSignStatus) ws.Cells[row, 8].Value = "Chưa ký";
            else ws.Cells[row, 8].Value = "Đã ký";
        }        
    }
}
