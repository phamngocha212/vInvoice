using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EInvoice.Core.IService;
using FX.Core;
using EInvoice.Core.Domain;
using FX.Utils.MvcPaging;
using FX.Utils.MVCMessage;
using EInvoice.CAdmin.Models;
using EInvoice.Core;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.IO;
using System.Text;
using log4net;
using IdentityManagement.Authorization;
using FX.Context;

namespace EInvoice.CAdmin.Controllers
{
    public class SendMailController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SendMailController));
        private readonly ISendEmailService _SendmailSvc;
        public SendMailController()
        {
            _SendmailSvc = IoC.Resolve<ISendEmailService>();
        }
        //
        // GET: /danh sach mail
        [RBACAuthorize(Permissions = "Search_Email")]
        public ActionResult Index(int? page, MailsIndexModel model)
        {
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int pageIndex = page.HasValue ? page.Value - 1 : 0;
            int pageSize = 10;
            int totalRecord = 0;
            DateTime? FromDate = null;
            DateTime? ToDate = null;
            if (!string.IsNullOrWhiteSpace(model.FromSendedDate)) FromDate = DateTime.ParseExact(model.FromSendedDate, "dd/MM/yyyy", null);
            if (!string.IsNullOrWhiteSpace(model.ToSendedDate)) ToDate = DateTime.ParseExact(model.ToSendedDate, "dd/MM/yyyy", null);
            if (FromDate != null && ToDate != null && FromDate > ToDate)
            {
                Messages.AddErrorMessage("Nhập đúng dữ liệu tìm kiếm theo ngày!");
                FromDate = ToDate = null;
                model.FromSendedDate = model.ToSendedDate = "";
            }
            IList<SendMail> lstSendMail = _SendmailSvc.SearchByMail(currentCom.id, model.Subject, FromDate, ToDate, model.Status, model.EmailTo, pageIndex, pageSize, out totalRecord);
            model.PageListMail = new PagedList<SendMail>(lstSendMail, pageIndex, pageSize, totalRecord);
            return View(model);
        }

        // sua mail
        [RBACAuthorize(Permissions = "Edit_Email")]
        public ActionResult Edit(Guid id)
        {
            SendMail model = _SendmailSvc.Getbykey(id);
            return View(model);
        }
        //Send again 
        //truong hop mail bi loi thi sua va gui lai
        [RBACAuthorize(Permissions = "SendAgain_Email")]
        [HttpPost]
        public ActionResult SendAgain(Guid id)
        {
            if (id == Guid.Empty)
                throw new HttpRequestValidationException();
            try
            {
                SendMail model = _SendmailSvc.Getbykey(id);
                TryUpdateModel(model);
                if (ChangeStatusMail(model, "1") == true)
                {
                    Messages.AddFlashMessage("Hệ thống đang gửi lại mail!");
                    return RedirectToAction("Index", new { EmailTo = model.Email });
                }
                _SendmailSvc.Update(model);
                _SendmailSvc.CommitChanges();
                log.Info("SendAgain Email by: " + HttpContext.User.Identity.Name);
            }
            catch (Exception ex)
            {
                log.Error(" Index -"+ex);
                Messages.AddErrorFlashMessage("Có lỗi xảy ra, vui lòng thực hiện lại!");
            }
            return RedirectToAction("Index");
        }
        //
        // GET: /SendMail/Delete/5
        [RBACAuthorize(Permissions = "Del_Email")]
        public ActionResult Delete(Guid id)
        {
            if (id == Guid.Empty)
                throw new HttpRequestValidationException();
            try
            {
                _SendmailSvc.Delete(id);
                _SendmailSvc.CommitChanges();
                Messages.AddFlashMessage("Xóa thành công!");
                log.Info("Delete Email by: " + HttpContext.User.Identity.Name);
            }
            catch (Exception ex)
            {
                log.Error(" Delete -" + ex);
                Messages.AddErrorFlashMessage("Có lỗi xảy ra, vui lòng thực hiện lại!");
            }
            return RedirectToAction("Index");
        }
        //gửi lại mail
        // chon tat ca hoac chon mot so mail de gui lai
        [RBACAuthorize(Permissions = "SendAgain_ListEmail")]
        [HttpPost]
        public ActionResult SelectSendAgain(string[] cbid)
        {
            try
            {
                if (cbid == null)
                {
                    Messages.AddErrorFlashMessage("Bạn chưa chọn mail để gửi lại");
                    return RedirectToAction("Index");
                }
                int i = 0;
                foreach (string itemId in cbid)
                {
                    SendMail model = _SendmailSvc.Getbykey(Guid.Parse(itemId));
                    if (ChangeStatusMail(model, "0") == true)
                    {
                        i++;
                    }
                }
                if (i > 0)
                {
                    Messages.AddFlashMessage("Hệ thống đang gửi lại: " + i + " mail!");
                    return RedirectToAction("Index", new { status = 3 });
                }
                else Messages.AddErrorFlashMessage("Mail đang chờ để gửi, không cần gửi lại.");
            }
            catch (Exception ex)
            {
                log.Error(" SelectSendAgain -" + ex.Message);
                Messages.AddErrorFlashMessage("Có lỗi xảy ra, vui lòng thực hiện lại!");
            }
            return RedirectToAction("Index");
        }
        /// <summary>
        /// thay doi trang thai mail
        /// </summary>
        /// <param name="Mail">Doi tuong mail</param>
        /// <returns>true(da thay doi trang thai), false(chua thay doi trang thai)</returns>
        private bool ChangeStatusMail(SendMail Mail, string EditMail)
        {
            try
            {
                if (Mail.Status == 1 || Mail.Status == 2 || EditMail == "1")
                {
                    Mail.Status = 3;
                    Mail.SendedDate = EInvoice.Core.Domain.Enumerations.MinDate;
                    Mail.Note = "Mail đã được gửi lại";
                    _SendmailSvc.Update(Mail);
                    _SendmailSvc.CommitChanges();
                    return true;
                }
                else return false;

            }
            catch (Exception ex)
            {
                log.Error(" ChangeStatusMail -" + ex.Message);
                return false;
            }

        }

        [RBACAuthorize(Permissions = "DownExcel_Email")]
        public void DownloadExcelMail(MailsIndexModel model)
        {
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int totalRecord = 0;
            DateTime? FromDate = null;
            DateTime? ToDate = null;
            if (!string.IsNullOrWhiteSpace(model.FromSendedDate)) FromDate = DateTime.ParseExact(model.FromSendedDate, "dd/MM/yyyy", null);
            if (!string.IsNullOrWhiteSpace(model.ToSendedDate)) ToDate = DateTime.ParseExact(model.ToSendedDate, "dd/MM/yyyy", null);
            //tim kiem 
            IList<SendMail> lstSendMail = _SendmailSvc.SearchByMail(currentCom.id, model.Subject, FromDate, ToDate, model.Status, model.EmailTo, 0, 0, out totalRecord);
            GridView gv = new GridView();
            //tao danh sach informail
            List<InforMail> lstMails = new List<InforMail>();
            int i = 1;
            foreach (var item in lstSendMail)
            {
                InforMail _MailInfor = new InforMail();
                _MailInfor.stt = i++;
                _MailInfor.EmailFrom = item.EmailFrom;
                _MailInfor.EmailTo = item.Email;
                _MailInfor.Subject = item.Subject;
                //ngay gui 
                if (item.SendedDate == Enumerations.MinDate) _MailInfor.SendedDate = "";
                else _MailInfor.SendedDate = String.Format("{0:dd/MM/yyyy}", item.SendedDate);
                //trang thai 
                if (item.Status == 0) _MailInfor.Status = "Mail mới tạo";
                else if (item.Status == 1 && string.IsNullOrEmpty(item.Note)) _MailInfor.Status = "Mail đã gửi";
                else if (item.Status == 1 && !string.IsNullOrEmpty(item.Note)) _MailInfor.Status = item.Note;
                else if (item.Status == 2) _MailInfor.Status = "Mail bị gửi lỗi";
                else if (item.Status == 3) _MailInfor.Status = "Mail đang chờ hệ thống gửi";
                lstMails.Add(_MailInfor);
            }
            gv.DataSource = lstMails;
            gv.DataBind();
            //cac header
            gv.HeaderRow.Cells[0].Text = "stt";
            gv.HeaderRow.Cells[1].Text = "Mail người gửi";
            gv.HeaderRow.Cells[2].Text = "Mail người nhận";
            gv.HeaderRow.Cells[3].Text = "Chủ đề";
            gv.HeaderRow.Cells[4].Text = "Ngày gửi";
            gv.HeaderRow.Cells[5].Text = "Trạng thái";
            //thuc hien tao file excel
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=MailExportExcel.xls");
            Response.ContentType = "application/excel";
            System.IO.StringWriter sw = new System.IO.StringWriter();
            sw.WriteLine("<div class='widget-header1' style='border-bottom:none; margin-top:20px'><center><h4><b>" + "DANH SÁCH MAIL" + "</b></h4></center></div>");
            sw.WriteLine("<table>");
            //cac tieu chi tim kiem
            if (!string.IsNullOrEmpty(model.Subject)) sw.WriteLine("<tr><td><b>Chủ đề</b>:</td><td>" + model.Subject + "</td></tr>");
            if (!string.IsNullOrEmpty(model.EmailTo)) sw.WriteLine("<tr><td><b>Mail người nhận</b>:</td><td>" + model.EmailTo + "</td></tr>");
            if (model.Status == 0) sw.WriteLine("<tr><td><b>Trạng thái Mail</b>:</td><td>" + "Mail mới tạo" + "</td></tr>");
            if (model.Status == 1) sw.WriteLine("<tr><td><b>Trạng thái Mail</b>:</td><td>" + "Mail đã gửi" + "</td></tr>");
            if (model.Status == 2) sw.WriteLine("<tr><td><b>Trạng thái Mail</b>:</td><td>" + "Mail bị gửi lỗi" + "</td></tr>");
            if (model.Status == 3) sw.WriteLine("<tr><td><b>Trạng thái Mail</b>:</td><td>" + "Mail đang chờ hệ thống gửi lại" + "</td></tr>");
            if (FromDate != null) sw.WriteLine("<tr><td><b> Từ ngày</b>:</td><td>" + model.FromSendedDate + "</td></tr>");
            if (ToDate != null) sw.WriteLine("<tr><td><b> Đến ngày </b>:</td><td>" + model.ToSendedDate + "</td></tr>");
            sw.WriteLine("</table>");
            sw.WriteLine("<BR><BR>");
            //xet font chu
            HttpContext.Response.Write("<font style='font-size:10.0pt; font-family:Calibri;'>");
            //thuc hien do du lieu vao excel
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            byte[] byteArray = Encoding.UTF8.GetBytes(sw.ToString());
            MemoryStream s = new MemoryStream(byteArray);
            StreamReader sr = new StreamReader(s, Encoding.UTF8);
            gv.RenderControl(htw);
            Response.Write(sw.ToString());
            sw.Close();
            Response.End();
        }
       
    }
    class InforMail
    {
        public int stt { get; set; }
        public string EmailFrom { get; set; }
        public string EmailTo { get; set; }
        public string Subject { get; set; }
        public string SendedDate { get; set; }
        public string Status { get; set; }
    }
}
