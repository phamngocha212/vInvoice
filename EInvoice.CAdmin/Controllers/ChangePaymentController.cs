using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;
using FX.Core;
using FX.Utils.MvcPaging;
using IdentityManagement.Authorization;
using EInvoice.Core;
using EInvoice.Core.IService;
using EInvoice.Core.Domain;
using EInvoice.CAdmin.Models;
using FX.Context;
namespace EInvoice.CAdmin.Controllers
{
    public class ChangePaymentController : ShareController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EInvoiceController));

        [RBACAuthorize(Permissions = "Search_UnPayment")]
        public ActionResult Index(PaymentModel model, int? page, int? Pagesize)
        {
            IPublishInvoiceService _PubIn = IoC.Resolve<IPublishInvoiceService>();
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IList<string> lstPubinv = _PubIn.LstByPattern(currentCom.id, 2);            
            if (lstPubinv.Count == 0)
            {
                Messages.AddErrorFlashMessage(Resources.Message.MInv_SMesNoPubAccess);
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

        [HttpPost]
        [RBACAuthorize(Permissions = "Exc_UnPayment")]
        public ActionResult PaymentInvoice(string[] cbid, string hdPattern)
        {
            try
            {
                Company _currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                int[] _IDs = (from _s in cbid select Convert.ToInt32(_s)).ToArray();
                if (_IDs.Length <= 0)
                {
                    Messages.AddErrorFlashMessage("Chưa chọn hóa đơn thanh toán.");
                    return RedirectToAction("Index");
                }
                IInvoiceService _IInSrv = InvServiceFactory.GetService(hdPattern, _currentCom.id);
                IList<IInvoice> _lstInvoice = _IInSrv.GetByID(_currentCom.id, _IDs);

                _IInSrv.BeginTran();
                foreach (IInvoice item in _lstInvoice)
                {
                    if (item.PaymentStatus == Payment.Paid)
                    {
                        item.PaymentStatus = Payment.Unpaid;
                        item.Note += " || Thực hiện bỏ gạch nợ: người thực hiện " + HttpContext.User.Identity.Name + " ngày thực hiện " + DateTime.Now;
                        _IInSrv.Save(item);
                    }
                }
                _IInSrv.CommitTran();
                log.Info("PaymentInvoice by: " + HttpContext.User.Identity.Name + " Info-- cbid: " + cbid);
                Messages.AddFlashMessage("Chuyển trạng thái thành công!");
                return RedirectToAction("Index", new { Pattern = hdPattern });
            }
            catch (Exception ex)
            {
                log.Error("PaymentInvoice-" + ex);
                Messages.AddErrorFlashMessage("Có lỗi trong quá trình xử lý, vui lòng thực hiện lại!");
                return RedirectToAction("Index", new { Pattern = hdPattern });
            }

        }        
    }
}
