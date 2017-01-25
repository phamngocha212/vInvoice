using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;
using FX.Core;
using FX.Utils.MVCMessage;
using FX.Utils.MvcPaging;
using IdentityManagement.Authorization;
using EInvoice.Core;
using EInvoice.Core.IService;
using EInvoice.Core.Domain;
using EInvoice.CAdmin.Models;
using EInvoice.Core.Launching;
using System.Data.OleDb;
using System.Data;
using System.IO;
using FX.Context;
namespace EInvoice.CAdmin.Controllers
{
    public class PaymentController : ShareController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EInvoiceController));
        private IDeliver _deliver;

        [RBACAuthorize(Permissions = "View_payer")]
        public ActionResult Index(PaymentModel model, int? page, int? Pagesize)
        {
            IPublishInvoiceService _PubIn = IoC.Resolve<IPublishInvoiceService>();
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IList<string> lstPubinv = _PubIn.LstByPattern(currentCom.id, 2);
            if (lstPubinv.Count == 0) // chưa đăng ký mẫu hóa đơn
            {
                Messages.AddErrorFlashMessage(Resources.Message.MNullInvInLst);
                return RedirectToAction("Index", "Publish");
            }
            if (model == null) model = new PaymentModel();
            if (String.IsNullOrEmpty(model.Pattern))
            {
                model.Pattern = lstPubinv[0];   // mặc định lấy pattern đầu tiên
            }
            model.PatternList = new SelectList(lstPubinv);
            List<string> se = _PubIn.LstBySerial(currentCom.id, model.Pattern, 1);
            model.SerialList = new SelectList(se);
            int defautPagesize = Pagesize.HasValue ? Convert.ToInt32(Pagesize) : 10;
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            int totalRecords = 0;
            IInvoiceService IInvSrv = InvServiceFactory.GetService(model.Pattern, currentCom.id);
            IList<IInvoice> lstInv;
            if (model.InvNo.HasValue && model.InvNo > 0) // lấy hóa đơn theo số, sêri
            {
                IInvoice Invoice = IInvSrv.GetByNo(currentCom.id, model.Pattern, model.Serial, model.InvNo.Value);
                lstInv = new List<IInvoice>();
                if (Invoice != null) lstInv.Add(Invoice);
                totalRecords = 1;
            }
            else // tìm theo tham số khác
            {
                DateTime? DateFrom = null;
                DateTime? DateTo = null;
                if (!string.IsNullOrWhiteSpace(model.FromDate)) DateFrom = DateTime.ParseExact(model.FromDate, "dd/MM/yyyy", null);
                if (!string.IsNullOrWhiteSpace(model.ToDate)) DateTo = DateTime.ParseExact(model.ToDate, "dd/MM/yyyy", null);
                if (model.PaymentStatus >= 0)
                    lstInv = IInvSrv.SearchPayment(currentCom.id, model.Pattern, model.Serial, model.nameCus, model.code, DateFrom, DateTo, currentPageIndex, defautPagesize, out totalRecords, (Payment)model.PaymentStatus);
                else lstInv = IInvSrv.SearchPayment(currentCom.id, model.Pattern, model.Serial, model.nameCus, model.code, DateFrom, DateTo, currentPageIndex, defautPagesize, out totalRecords);
            }
            model.PageListINV = new PagedList<IInvoice>(lstInv, currentPageIndex, defautPagesize, totalRecords);
            return View(model);
        }
        public ActionResult GetSerialPayments(string Pattern)
        {
            IPublishInvoiceService _PubIn = IoC.Resolve<IPublishInvoiceService>();
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            var pu = _PubIn.LstBySerial(currentCom.id, Pattern, 1);
            return Json(new
            {
                pu
            });
        }
        /// <summary>
        /// Chấp nhận thanh toán
        /// Thực hiện chấp nhận một hay nhiều hóa đơn để chấp nhận thanh toán
        /// </summary>
        /// <param name="cbid"></param>
        /// <param name="hdPattern"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PaymentInvoice(string[] cbid, string hdPattern)
        {
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int[] IDs = (from s in cbid select Convert.ToInt32(s)).ToArray();
            if (IDs.Length <= 0)
            {
                Messages.AddErrorFlashMessage("Chưa chọn hóa đơn thanh toán.");
                return RedirectToAction("Index");
            }
            IInvoiceService IInvSrv = InvServiceFactory.GetService(hdPattern, currentCom.id);
            string strNote = "Thực hiện gạch nợ:   Người gạch nợ: " + HttpContext.User.Identity.Name + "  Ngày gạch nợ: " + DateTime.Now.ToString();
            IList<IInvoice> lstInvoice = IInvSrv.GetByID(currentCom.id, IDs);
            if (IInvSrv.ConfirmPayment(lstInvoice, strNote))
            {
                Messages.AddFlashMessage(Resources.Message.Pay_MesSuccess);
                //if (currentCom.Config.Keys.Contains("PaymentToDeliver"))
                //    if (currentCom.Config["PaymentToDeliver"] == "1")
                //    {
                //        //IDeliver _deliver = IoC.Resolve<IDeliver>();
                //        if (currentCom.Config.Keys.Contains("IDeliver"))
                //            _deliver = InvServiceFactory.GetDeliver(currentCom.Config["IDeliver"]);
                //        else _deliver = IoC.Resolve<IDeliver>();
                //        _deliver.Deliver(lstInvoice.ToArray(), currentCom);
                //    }
                log.Info("Change Payment Status EInvoice by: " + HttpContext.User.Identity.Name + " Info-- ID: " + string.Join(";", IDs));
                return RedirectToAction("Index");
            }
            else
            {
                log.Error("Change Payment Status EInvoice error: " + Resources.Message.Pay_MesUnsuccess);
                Messages.AddErrorFlashMessage(Resources.Message.Pay_MesUnsuccess);
                return RedirectToAction("Index");
            }
        }

        #region gạch nợ theo lô
        public ActionResult PaymentTransactionIndex(PaymentTransactionIndexModels model, int? page, int? pagesize)
        {
            try
            {
                int defautPagesize = pagesize.HasValue ? pagesize.Value : 10;
                int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                int totalRecords = 0;
                Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                IPaymentTransactionService tranSrv = IoC.Resolve<IPaymentTransactionService>();
                Guid gID = Guid.Empty;
                try
                {
                    gID = !string.IsNullOrEmpty(model.key) ? Guid.Parse(model.key.Trim()) : Guid.Empty;
                }
                catch (Exception ex)
                {
                    gID = Guid.NewGuid();
                    log.Error("Error:" + ex.Message);
                    Messages.AddErrorMessage("Vui lòng nhập đúng mã giao dịch!");
                }
                DateTime? DateFrom = null;
                DateTime? DateTo = null;
                if (!string.IsNullOrWhiteSpace(model.FromDate)) DateFrom = DateTime.ParseExact(model.FromDate, "dd/MM/yyyy", null);
                if (!string.IsNullOrWhiteSpace(model.ToDate)) DateTo = DateTime.ParseExact(model.ToDate, "dd/MM/yyyy", null);
                IList<PaymentTransaction> lst = tranSrv.GetByFilter(gID, currentCom.id, model.status, model.accountName, DateFrom, DateTo, currentPageIndex, defautPagesize, out totalRecords);
                model.PagedListTransaction = new PagedList<PaymentTransaction>(lst, currentPageIndex, defautPagesize, totalRecords);
            }
            catch (Exception ex)
            {
                log.Error("Error:" + ex);
                Messages.AddErrorMessage("Không có giao dịch trong hệ thống");
            }
            return View(model);
        }

        [RBACAuthorize(Permissions = "View_payer")]
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [RBACAuthorize(Permissions = "View_payer")]
        public ActionResult Upload(HttpPostedFileBase FilePath)
        {
            try
            {
                IPaymentTransactionService tranSrv = IoC.Resolve<IPaymentTransactionService>();
                Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                if (FilePath != null && FilePath.ContentLength > 0)
                {
                    byte[] bf = new byte[FilePath.ContentLength];
                    FilePath.InputStream.Read(bf, 0, FilePath.ContentLength);
                    if (!FilePath.FileName.ToLower().Contains(".xls"))
                    {
                        Messages.AddErrorFlashMessage("File upload phải là file .xls hoặc .XLS!");
                        return RedirectToAction("Upload");
                    }
                    Guid guid = Guid.NewGuid();
                    //thanh toan theo lo
                    PaymentTransaction mTran = paymentViaBlock(bf);
                    tranSrv.CreateNew(mTran);
                    tranSrv.CommitChanges();
                    Messages.AddFlashMessage("Upload file và gạch nợ xong, mã giao dịch: " + mTran.id);
                    return RedirectToAction("PaymentTransactionIndex");
                }
                Messages.AddErrorFlashMessage("Chọn file cần upload!");
                return RedirectToAction("Upload");
            }
            catch (Exception ex)
            {
                Messages.AddErrorMessage("Có lỗi xảy ra, vui lòng kiểm tra log để biết thêm chi tiết.");
                return View("Upload");
            }
        }

        private PaymentTransaction paymentViaBlock(byte[] file)
        {
            //gach no theo lo
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            PaymentTransaction mTran = new PaymentTransaction();
            mTran.Data = file;
            mTran.ComID = currentCom.id;
            mTran.id = Guid.NewGuid();
            mTran.AccountName = HttpContext.User.Identity.Name;
            mTran.Status = PaymentTransactionStatus.NewUpload;
            string message = "";
            IPaymentExcel paymentExcelSrv = IoC.Resolve<IPaymentExcel>();
            List<string> lstFkey = paymentExcelSrv.GetFkey(file, mTran.id.ToString(), out message);
            if (lstFkey == null)
            {
                mTran.Messages = message;
                mTran.Status = PaymentTransactionStatus.Failed;
                return mTran;
            }
            List<IInvoice> invLst = new List<IInvoice>();
            PublishInvoice pub = DefaultPublishInvoice(currentCom.id);
            IInvoiceService invSrv = InvServiceFactory.GetService(pub.InvPattern, currentCom.id);
            int count = lstFkey.Count();

            log.Info("paymentViaBlock get Inv");
            List<IInvoice> foundLst = new List<IInvoice>();
            foundLst = (List<IInvoice>)invSrv.GetByFkey(currentCom.id, lstFkey.ToArray());
            log.Info("paymentViaBlock loc ket qua");
            List<string> foundFkey = foundLst.Select(inv => inv.Fkey).ToList();
            //khong ton tai
            lstFkey.RemoveAll(inv => foundFkey.Contains(inv));

            //đã gạch nợ rồi
            List<IInvoice> updateInv = foundLst.GetRange(0, foundLst.Count);
            updateInv.RemoveAll(inv => inv.PaymentStatus == Payment.Paid);      //cần gạch nợ
            foundLst.RemoveAll(inv => inv.PaymentStatus == Payment.Unpaid);     //đã gạch nợ rồi.

            List<string> unpaiedLst = updateInv.Select(inv => inv.Fkey).ToList();
            List<string> paiedList = foundLst.Select(inv => inv.Fkey).ToList();

            //ko ton tai    lstFkey
            //da gach no    paiedList
            //can gach no   unpaiedLst

            //ghi file excel ket qua
            MemoryStream stream;
            ExcelWriter writer;
            if (lstFkey.Count > 0 || paiedList.Count > 0)
            {
                stream = new MemoryStream();
                writer = new ExcelWriter(stream);
                writer.BeginWrite();
                int i = 0;
                for (; i < lstFkey.Count; i++)
                {
                    string s = lstFkey.ElementAt(i);
                    writer.WriteCell(i, 0, s);
                    writer.WriteCell(i, 1, "Khong ton tai");
                }
                for (int j = 0; j < paiedList.Count; j++)
                {
                    string s = paiedList.ElementAt(j);
                    i++;
                    writer.WriteCell(i, 0, s);
                    writer.WriteCell(i, 1, "Da thanh toan tu truoc");
                }
                writer.EndWrite();
                mTran.FailResult = stream.ToArray();
                stream.Close();
            }

            //thuc hien gach no va ghi note moi
            int[] ids = updateInv.Select(inv => inv.id).ToArray();
            string noteAppends = "    ||    Thực hiện gạch nợ:   Người gạch nợ: " + HttpContext.User.Identity.Name + "  Ngày gạch nợ: " + DateTime.Now.ToString();
            log.Info("paymentViaBlock gach no");
            if (invSrv.ConfirmPayment(ids, noteAppends))
            //if (invSrv.ConfirmPayment(updateInv))
            {
                //thanh cong het
                //tao file ket qua thanh cong
                log.Info("paymentViaBlock ghi file thanh cong");
                stream = new MemoryStream();
                writer = new ExcelWriter(stream);
                writer.BeginWrite();
                for (int j = 0; j < updateInv.Count; j++)
                {
                    string s = updateInv.ElementAt(j).Fkey;
                    writer.WriteCell(j, 0, s);
                }
                writer.EndWrite();
                mTran.CompleteResult = stream.ToArray();
                stream.Close();
                if (lstFkey.Count == 0)
                {
                    mTran.Status = PaymentTransactionStatus.Completed;
                }
                else
                {
                    mTran.Status = PaymentTransactionStatus.NotComplete;
                }
                log.Info("Change Payment Status Einvoices by: " + HttpContext.User.Identity.Name + " Info-- GUID: " + mTran.id.ToString() + "Tổng số: " + (count) + ", gạch nợ thành công " + updateInv.Count + ", không tồn tại: " + lstFkey.Count + ", đã gạch nợ trước: " + paiedList.Count + " hóa đơn");
                mTran.Messages = "Tổng số: " + (count) + ", gạch nợ thành công " + updateInv.Count + ", không tồn tại: " + lstFkey.Count + ", đã gạch nợ trước: " + paiedList.Count + " hóa đơn";
                //thuc hien deliveriy
                IDeliver _deliver = IoC.Resolve<IDeliver>();
                _deliver.Deliver(updateInv.ToArray(), currentCom);
                log.Info("paymentViaBlock end " + mTran.id);
                return mTran;
            }
            mTran.Status = PaymentTransactionStatus.Failed;
            log.Info("Change Payment Status Einvoices by: " + HttpContext.User.Identity.Name + " Info-- GUID: " + mTran.id.ToString() + "Tổng số: " + (count) + " hóa đơn, gạch nợ fail " + updateInv.Count + ", không tồn tại: " + lstFkey.Count + ", đã gạch nợ trước: " + paiedList.Count + " hóa đơn");
            if (count == 0)
            {
                mTran.Messages = "File không đúng mẫu hoặc không chứa dữ liệu";
            }
            else
                mTran.Messages = "Tổng số: " + (count) + " hóa đơn, gạch nợ fail " + updateInv.Count + ", không tồn tại: " + lstFkey.Count + ", đã gạch nợ trước: " + paiedList.Count + " hóa đơn";
            return mTran;
        }

        public void Download(Guid id)
        {
            IPaymentTransactionService tranSrv = IoC.Resolve<IPaymentTransactionService>();
            PaymentTransaction model = tranSrv.Getbykey(id);
            if (model == null)
            {
                Response.Clear();
                Response.Write("<script type='text/javascript'>alert('Có lỗi xảy ra, vui lòng thực hiện lại!')</script>");
                Response.End();
                Response.Flush();
            }
            byte[] buffer = model.FailResult;
            if (buffer == null) return;
            Response.ContentType = "text/plain";
            Response.OutputStream.Write(buffer, 0, buffer.Length);
            Response.AddHeader("Content-Disposition", "attachment;filename=gachnofailes.xls");
            return;
        }

        public void Completed(Guid id)
        {
            IPaymentTransactionService tranSrv = IoC.Resolve<IPaymentTransactionService>();
            PaymentTransaction model = tranSrv.Getbykey(id);
            byte[] buffer = model.CompleteResult;
            if (buffer == null) return;
            Response.ContentType = "text/plain";
            Response.OutputStream.Write(buffer, 0, buffer.Length);
            Response.AddHeader("Content-Disposition", "attachment;filename=gachno.xls");
            return;
        }

        //lay default Pattern va serial
        //lay pattern dau tien cua cong ty, van con hoa don trong dai
        private PublishInvoice DefaultPublishInvoice(int ComID)
        {
            PublishInvoice ret = null;
            IPublishInvoiceService _PubSrv = IoC.Resolve<IPublishInvoiceService>();
            ret = _PubSrv.Query.Where(pi => pi.ComId == ComID && (pi.Status == 1 || pi.Status == 2)).First();
            return ret;
        }

        [Authorize]
        public ActionResult Delete(Guid id)
        {
            try
            {
                IPaymentTransactionService tranSrv = IoC.Resolve<IPaymentTransactionService>();
                PaymentTransaction model = tranSrv.Getbykey(id);
                tranSrv.Delete(model);
                tranSrv.CommitChanges();
                log.Info("Delete PaymentTransaction Einvoices by: " + HttpContext.User.Identity.Name + " Info-- GUID: " + id.ToString());
                Messages.AddFlashMessage("Xóa thành công!");
            }
            catch (Exception ex)
            {
                log.Error(" Delete -" + ex);
                Messages.AddErrorFlashMessage("Có lỗi xảy ra, vui lòng thực hiện lại!");
            }
            return RedirectToAction("PaymentTransactionIndex");
        }

        [Authorize]
        public ActionResult Details(Guid id)
        {
            IPaymentTransactionService tranSrv = IoC.Resolve<IPaymentTransactionService>();
            PaymentTransaction model = tranSrv.Getbykey(id);
            return View(model);
        }

        #endregion
    }

    public interface IPaymentExcel
    {
        List<string> GetFkey(byte[] file, string guid, out string message);
    }

    public class HCMPaymentExcel : IPaymentExcel
    {
        private readonly ILog log = LogManager.GetLogger(typeof(HCMPaymentExcel));
        public List<string> GetFkey(byte[] file, string guid, out string message)
        {
            log.Info("start create file " + guid);
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "/Tempfile/" + guid + ".xls";
            using (System.IO.FileStream fs = System.IO.File.Create(fileName))
            {
                fs.Write(file, 0, file.Length);
            }
            log.Info("read file");
            DataTable objTable = UtilExcel.readExcelFile(fileName, "Sheet1$", true);
            List<string> lstFkey = new List<string>();
            message = "";
            try
            {
                DataRow row = objTable.Rows[0];
                objTable.Rows.Remove(row);
                const int THANHTOAN_ID = 2;
                const int CHU_KY_NO = 5;
                //const string THANHTOAN_ID = "THANHTOAN_ID";
                //const string CHU_KY_NO = "CHU_KY_NO";
                DataRowCollection drc = objTable.Rows;
                foreach (DataRow dr in drc)
                {
                    if (string.IsNullOrEmpty(dr[THANHTOAN_ID].ToString().Trim())) continue;
                    if (string.IsNullOrEmpty(dr[CHU_KY_NO].ToString()) || string.IsNullOrWhiteSpace(dr[CHU_KY_NO].ToString())) continue;
                    string[] kyHd = dr[CHU_KY_NO].ToString().Trim().Split(',');
                    foreach (string s in kyHd)
                    {
                        string[] temp = s.Trim().Split('/');
                        int month = Convert.ToInt32(temp[0]);
                        int year = Convert.ToInt32(temp[1]);
                        lstFkey.Add(dr[THANHTOAN_ID].ToString().Trim() + year.ToString() + month.ToString("00"));
                    }
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                message = "Lỗi khi đọc file excel :File không đúng định dạng";
                return null;
            }
            catch (FormatException ex)
            {
                message = "Lỗi khi đọc file excel: Không phải báo cáo 1.15-HDDT-Gửi cập nhật đồng bộ";
                return null;
            }
            catch (Exception ex)
            {
                log.Error("PaymentTransactionIndex paymentViaBlock Error5" + ex.Message);
                message = "Lỗi khi đọc file excel :" + ex;
                return null;
            }
            message = "";
            return lstFkey;
        }
    }

    public class ComonPaymentExcel : IPaymentExcel
    {
        private readonly ILog log = LogManager.GetLogger(typeof(ComonPaymentExcel));
        public List<string> GetFkey(byte[] file, string guid, out string message)
        {
            log.Info("start create file " + guid);
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "/Tempfile/" + guid + ".xls";
            using (System.IO.FileStream fs = System.IO.File.Create(fileName))
            {
                fs.Write(file, 0, file.Length);
            }
            log.Info("read file");
            DataTable objTable = UtilExcel.readExcelFile(fileName, "Sheet1$", true);
            List<string> lstFkey = new List<string>();
            message = "";
            try
            {
                DataRow row = objTable.Rows[0];
                const string Fkey = "Fkey";
                DataRowCollection drc = objTable.Rows;
                foreach (DataRow dr in drc)
                {
                    if (string.IsNullOrEmpty(dr[Fkey].ToString().Trim())) continue;
                    lstFkey.Add(dr[Fkey].ToString().Trim());
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                message = "Lỗi khi đọc file excel :File không đúng định dạng";
                return null;
            }
            catch (FormatException ex)
            {
                message = "Lỗi khi đọc file excel: Không phải báo cáo 1.15-HDDT-Gửi cập nhật đồng bộ";
                return null;
            }
            catch (Exception ex)
            {
                log.Error("PaymentTransactionIndex paymentViaBlock Error5" + ex.Message);
                message = "Lỗi khi đọc file excel :" + ex;
                return null;
            }
            message = "";
            return lstFkey;
        }
    }

    public class VTPaymentExcel : IPaymentExcel
    {
        private readonly ILog log = LogManager.GetLogger(typeof(HCMPaymentExcel));
        public List<string> GetFkey(byte[] file, string guid, out string message)
        {
            log.Info("start create file " + guid);
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "/Tempfile/" + guid + ".xls";
            using (System.IO.FileStream fs = System.IO.File.Create(fileName))
            {
                fs.Write(file, 0, file.Length);
            }
            log.Info("read file");
            DataTable objTable = UtilExcel.readExcelFile(fileName, "Sheet1$", true);
            List<string> lstFkey = new List<string>();
            message = "";
            try
            {
                //DataRow row = objTable.Rows[0];
                //objTable.Rows.Remove(row);
                //const int THANHTOAN_ID = 2;
                //const int CHU_KY_NO = 5;
                const string THANHTOAN_ID = "THANHTOAN_ID";
                const string CHU_KY_NO = "CHU_KY_NO";
                DataRowCollection drc = objTable.Rows;
                foreach (DataRow dr in drc)
                {
                    if (string.IsNullOrEmpty(dr[THANHTOAN_ID].ToString().Trim())) continue;
                    if (string.IsNullOrEmpty(dr[CHU_KY_NO].ToString()) || string.IsNullOrWhiteSpace(dr[CHU_KY_NO].ToString())) continue;
                    string[] kyHd = dr[CHU_KY_NO].ToString().Trim().Split(',');
                    foreach (string s in kyHd)
                    {
                        string[] temp = s.Trim().Split('/');
                        int month = Convert.ToInt32(temp[0]);
                        int year = Convert.ToInt32(temp[1]);
                        lstFkey.Add(dr[THANHTOAN_ID].ToString().Trim() + year.ToString() + month.ToString("00"));
                    }
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                message = "Lỗi khi đọc file excel :File không đúng định dạng";
                return null;
            }
            catch (FormatException ex)
            {
                message = "Lỗi khi đọc file excel: Không phải báo cáo 1.15-HDDT-Gửi cập nhật đồng bộ";
                return null;
            }
            catch (Exception ex)
            {
                log.Error("PaymentTransactionIndex paymentViaBlock Error5" + ex.Message);
                message = "Lỗi khi đọc file excel :" + ex;
                return null;
            }
            message = "";
            return lstFkey;
        }
    }

}
