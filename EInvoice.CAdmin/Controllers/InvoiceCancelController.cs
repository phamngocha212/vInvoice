using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EInvoice.Core;
using EInvoice.Core.IService;
using FX.Core;
using EInvoice.Core.Domain;
using FX.Utils.MvcPaging;
using FX.Utils.MVCMessage;
using FX.Utils;
using IdentityManagement.Authorization;
using log4net;
using EInvoice.CAdmin.Models;
using EInvoice.Core.ServiceImp;
using System.Xml;
using EInvoice.CAdmin.IService;
using EInvoice.CAdmin.ServiceImp;
using System.Text;
using FX.Context;

namespace EInvoice.CAdmin.Controllers
{
    public class InvoiceCancelController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(InvoiceCancelController));
        private int pagesize = 10;

        [RBACAuthorize(Permissions = "Search_cancel")]
        public ActionResult Index(InvCancelModel model, int? page)
        {
            int total = 0;
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            Company _currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IInvCancelService _InvCancelSrv = IoC.Resolve<IInvCancelService>();
            DateTime DateFrom = String.IsNullOrEmpty(model.dateFrom) ? DateTime.MinValue : DateTime.ParseExact(model.dateFrom, "dd/MM/yyyy", null);
            DateTime DateTo = String.IsNullOrEmpty(model.dateTo) ? DateTime.MaxValue : DateTime.ParseExact(model.dateTo, "dd/MM/yyyy", null);
            IList<InvCancel> lst = _InvCancelSrv.SearchInvCancel(model.creater, DateFrom, DateTo, currentPageIndex, pagesize, out total, _currentCom.id);
            model.PageListIC = new PagedList<InvCancel>(lst, currentPageIndex, pagesize, total);
            return View(model);
        }
        
        public ActionResult ajxGetPattern(int id)
        {
            IPublishInvoiceService _ipiSer = IoC.Resolve<IPublishInvoiceService>();
            Company _currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IList<PublishInvoice> listPI = _ipiSer.GetPublishedPub(id, _currentCom.id);
            List<pubInfo> pList = new List<pubInfo>();
            foreach (PublishInvoice pi in listPI)
            {
                pubInfo p = new pubInfo();
                p.InvPattern = pi.InvPattern;
                p.PubInvId = pi.id;
                p.InvSerial = pi.InvSerial;
                p.CurrentNo = pi.CurrentNo;
                p.ToNo = pi.ToNo;
                pList.Add(p);
            }
            return Json(pList);
        }
        
        public ActionResult ajxGetSerial(int id)
        {
            IPublishInvoiceService _ipiSer = IoC.Resolve<IPublishInvoiceService>();
            PublishInvoice pi = _ipiSer.Getbykey(id);            
            return Json(pi);
        }
        
        [RBACAuthorize(Permissions = "Add_cancel")]
        public ActionResult Create()
        {
            InvCancel invc = new InvCancel();
            CreateInvCancelModel model = new CreateInvCancelModel();
            ICompanyService comSrv = IoC.Resolve<ICompanyService>();
            Company _currentCom = comSrv.Getbykey(((EInvoiceContext)FXContext.Current).CurrentCompany.id);
            invc.ComID = _currentCom.id;
            invc.ComName = _currentCom.Name;
            invc.ComTaxCode = _currentCom.TaxCode;
            invc.ComAddress = _currentCom.Address;
            IInvCategoryService _icateSer = IoC.Resolve<IInvCategoryService>();
            IList<InvCategory> lst = _icateSer.GetAll();
            model.lstInvCategory = new SelectList(lst, "id", "Name");
            model.CancelTemp = invc;
            return View(model);
        }
        
        [RBACAuthorize(Permissions = "Add_cancel")]
        [HttpPost]
        public ActionResult Create(InvCancel invCancel, string LstCancelDetail)
        {
            ICompanyService comSrv = IoC.Resolve<ICompanyService>();
            Company _currentCom = comSrv.Getbykey(((EInvoiceContext)FXContext.Current).CurrentCompany.id);
            invCancel.ComID = _currentCom.id;
            invCancel.ComName = _currentCom.Name;
            invCancel.ComTaxCode = _currentCom.TaxCode;
            invCancel.ComAddress = _currentCom.Address;
            if (string.IsNullOrWhiteSpace(LstCancelDetail))
            {
                CreateInvCancelModel md = new CreateInvCancelModel();
                md.CancelTemp = invCancel;
                IInvCategoryService _icateSer = IoC.Resolve<IInvCategoryService>();
                IList<InvCategory> lst = _icateSer.GetAll();
                md.lstInvCategory = new SelectList(lst, "id", "Name");
                Messages.AddErrorMessage("Chưa chọn dải hóa đơn.");
                return View(md);
            }
            invCancel.InvCancels = (IList<InvCancelDetails>)LstCancelDetail.DeserializeJSON<InvCancelDetails>(typeof(IList<InvCancelDetails>));
            IInvCancelService _invCancelSrv = IoC.Resolve<IInvCancelService>();
            string errMes;
            if (_invCancelSrv.CancelInv(invCancel, out errMes))
            {
                StringBuilder strInfo = new StringBuilder();
                try
                {
                    for (int i = 0; i < invCancel.InvCancels.Count; i++)
                    {
                        strInfo.Append("----- pattern: " + invCancel.InvCancels[i].InvPattern + "    Serial: " + invCancel.InvCancels[i].InvSerial + "   From: " + invCancel.InvCancels[i].FromNo + "   To: " + invCancel.InvCancels[i].ToNo + "-------");
                    }
                }
                catch (Exception ex)
                {
                    log.Error(" Create -" + ex.Message);
                }
                log.Info("Cancel Range Invoice  by: " + HttpContext.User.Identity.Name + " Info-- Ngay Huy: " + invCancel.CancelDate + " Thong tin dai so: " + strInfo.ToString());
                Messages.AddFlashMessage(Resources.Message.ReCancel_MesSuccess);
                return RedirectToAction("Index");
            }
            CreateInvCancelModel model = new CreateInvCancelModel();
            IInvCategoryService icateSer = IoC.Resolve<IInvCategoryService>();
            IList<InvCategory> list = icateSer.GetAll();
            model.CancelTemp = invCancel;
            model.lstInvCategory = new SelectList(list, "id", "Name");
            Messages.AddErrorMessage(Resources.Message.ReCancel_MesCanceled);
            return View(model);
        }
        
        [RBACAuthorize(Permissions = "View_cancel")]
        public ActionResult Detail(int id)
        {
            IInvCancelService _icsSer = IoC.Resolve<IInvCancelService>();
            InvCancel ic = _icsSer.Getbykey(id);
            return View(ic);
        }
        #region "Huy hoa don"
        /// <summary>
        /// Danh sach cac hoa don huy
        /// </summary>
        /// <param name="model"></param>
        /// <param name="page"></param>
        /// <param name="Pagesize"></param>
        /// <returns>tra ve danh sach cac hoa don</returns>
        [RBACAuthorize(Permissions = "Seach_CancelInvNoReAdj")]
        public ActionResult CancelInvNotReIndex(EInvoiceIndexModel model, int? page, int? Pagesize)
        {
            IPublishInvoiceService _PubIn = IoC.Resolve<IPublishInvoiceService>();
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IList<string> lstpattern = _PubIn.LstByPattern(currentCom.id, 1);
            if (lstpattern.Count == 0)
            {
                Messages.AddErrorFlashMessage(Resources.Message.MInv_SMesNoPubAccess);
                return RedirectToAction("Index", "Home");
            }
            if (model == null) model = new EInvoiceIndexModel();
            //lay pattern
            if (string.IsNullOrEmpty(model.Pattern))
            {
                model.Pattern = lstpattern[0];
            }
            model.lstpattern = lstpattern;
            //lay serial
            List<string> LstSerial = (from p in _PubIn.Query where p.Status != 0 && p.InvPattern == model.Pattern && p.ComId == currentCom.id select p.InvSerial).Distinct().ToList<string>();
            model.lstserial = LstSerial;
            if (string.IsNullOrEmpty(model.Serial))
            {
                model.Serial = LstSerial[0];
            }
            int defautPagesize = Pagesize.HasValue ? Convert.ToInt32(Pagesize) : 10;
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            int totalRecords = 0;

            IInvoiceService IInvSrv = InvServiceFactory.GetService(model.Pattern, currentCom.id);
            IList<IInvoice> lst;
            if (!model.InvNo.HasValue)
            {
                DateTime? DateFrom = null;
                DateTime? DateTo = null;
                if (!string.IsNullOrWhiteSpace(model.FromDate)) DateFrom = DateTime.ParseExact(model.FromDate, "dd/MM/yyyy", null);
                if (!string.IsNullOrWhiteSpace(model.ToDate)) DateTo = DateTime.ParseExact(model.ToDate, "dd/MM/yyyy", null);
                if (DateFrom != null && DateTo != null && DateFrom > DateTo)
                {
                    Messages.AddErrorMessage("Nhập đúng dữ liệu tìm kiếm theo ngày!");
                    DateFrom = DateTo = null;
                }
                InvoiceStatus[] status = new InvoiceStatus[] { InvoiceStatus.SignedInv, InvoiceStatus.AdjustedInv, InvoiceStatus.ReplacedInv, InvoiceStatus.InUseInv };
                lst = IInvSrv.SearchByStatus(currentCom.id, model.Pattern, model.Serial, DateFrom, DateTo, model.nameCus, model.code, model.CodeTax, (InvoiceType)model.typeInvoice, currentPageIndex, defautPagesize, out totalRecords, status);                
            }
            else
            {
                lst = new List<IInvoice>();
                IInvoice inv = IInvSrv.GetByNo(currentCom.id, model.Pattern, model.Serial, model.InvNo.Value);
                if (inv != null) lst.Add(inv);
                totalRecords = lst.Count();
            }
            model.PageListINV = new PagedList<IInvoice>(lst, currentPageIndex, defautPagesize, totalRecords);
            return View(model);
        }
        
        /// <summary>
        /// Hủy từng hóa đơn
        /// </summary>
        /// <param name="cbeinv"></param>
        /// <param name="hdPattern"></param>
        /// <param name="Serial"></param>
        /// <returns></returns>
        [RBACAuthorize(Permissions = "CancelInv")]
        [HttpPost]
        public ActionResult CancelInvApprove(string[] cbeinv, string hdPattern, string Serial)
        {
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IInvoiceService IInvSrv = InvServiceFactory.GetService(hdPattern, currentCom.id);
            try
            {
                int[] invIds = (from s in cbeinv select Convert.ToInt32(s)).ToArray();
                if (invIds.Length < 0)
                {
                    Messages.AddErrorMessage("Bạn chưa chọn hóa đơn để hủy.");
                    return RedirectToAction("CancelInvNotReIndex", new { Pattern = hdPattern, Serial = Serial });
                }
                ILogSystemService businessLog = IoC.Resolve<ILogSystemService>();
                IList<IInvoice> invLst = IInvSrv.GetByID(currentCom.id, invIds);
                int succCount = 0;
                IInvSrv.BeginTran();
                var context = (EInvoiceContext)FXContext.Current;
                foreach (var item in invIds)
                {
                    IInvoice Oinvoice = IInvSrv.GetByID(currentCom.id, hdPattern, item);
                    if (Oinvoice.Status == InvoiceStatus.CanceledInv || Oinvoice.Status == InvoiceStatus.ReplacedInv || Oinvoice.Status == InvoiceStatus.AdjustedInv)
                        continue;
                    succCount++;
                    Oinvoice.Status = InvoiceStatus.CanceledInv;
                    Oinvoice.Note += "  || Thực hiện hủy Hóa đơn (Không thay thế):   Người hủy:" + context.CurrentUser.username + "   Ngày hủy:" + DateTime.Now.ToString();
                    IInvSrv.Update(Oinvoice);
                    log.Info("INVCANCEL_" + context.CurrentUser.username + "_" + hdPattern + "_" + Oinvoice.Serial + "_" + Oinvoice.No + "_" + DateTime.Now);
                    businessLog.WriteLogCancel(currentCom.id, context.CurrentUser.username, Oinvoice.Pattern, Oinvoice.Serial, Oinvoice.No.ToString("0000000"), Oinvoice.ArisingDate, Oinvoice.Amount, Oinvoice.CusName, Oinvoice.CusAddress, Oinvoice.CusCode, Oinvoice.CusTaxCode, BusinessLogType.Cancel);                    
                }
                if (succCount == 0)
                {
                    Messages.AddErrorFlashMessage("Kiểm tra lại hóa đơn hủy, có thể đã được thanh toán hoặc sửa đổi.");
                }
                else
                {
                    IInvSrv.CommitTran();
                    Messages.AddFlashMessage("Tổng số hóa đơn hủy là:" + succCount);
                }
                return RedirectToAction("CancelInvNotReIndex", new { Pattern = hdPattern, Serial = Serial });
            }
            catch (Exception ex)
            {
                Messages.AddErrorFlashMessage("Có lỗi trong quá trình xử lý, vui lòng thực hiện lại!");
                log.Error(" CancelInvApprove -" + ex.Message);
                IInvSrv.RolbackTran();
                return RedirectToAction("CancelInvNotReIndex", new { Pattern = hdPattern, Serial = Serial });
            }
        }
        #endregion        

        private class pubInfo
        {
            string invPattern;

            public string InvPattern
            {
                get { return invPattern; }
                set { invPattern = value; }
            }
            int pubInvId;

            public int PubInvId
            {
                get { return pubInvId; }
                set { pubInvId = value; }
            }
            string invSerial;

            public string InvSerial
            {
                get { return invSerial; }
                set { invSerial = value; }
            }
            decimal currentNo;

            public decimal CurrentNo
            {
                get { return currentNo; }
                set { currentNo = value; }
            }
            decimal toNo;

            public decimal ToNo
            {
                get { return toNo; }
                set { toNo = value; }
            }
        }
    }
}
