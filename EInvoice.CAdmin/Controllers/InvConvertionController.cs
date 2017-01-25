using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EInvoice.Core.IService;
using FX.Core;
using EInvoice.Core.Domain;
using EInvoice.Core;
using EInvoice.CAdmin.Models;
using FX.Utils.MVCMessage;
using IdentityManagement.Authorization;
using FX.Utils.MvcPaging;
using log4net;
using System.Xml;
using System.IO;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.Text;
using IdentityManagement.Domain;
using FX.Context;

namespace EInvoice.CAdmin.Controllers
{
    public class InvConvertionController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(InvConvertionController));
        [RBACAuthorize(Permissions = "View_Convert")]
        public ActionResult Index(ConvertionModel model, int? page, int? Pagesize)
        {
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IPublishInvoiceService _PubIn = IoC.Resolve<IPublishInvoiceService>();
            IList<string> lstPubinv = _PubIn.LstByPattern(currentCom.id, 2);
            if (lstPubinv.Count == 0)
            {
                Messages.AddErrorFlashMessage(Resources.Message.MNullInvInLst);
                return RedirectToAction("Index", "Publish");
            }
            if (model == null) model = new ConvertionModel();
            if (String.IsNullOrEmpty(model.Pattern)) model.Pattern = lstPubinv[0];
            model.PatternList = new SelectList(lstPubinv);
            List<string> se = _PubIn.LstBySerial(currentCom.id, model.Pattern, 1);
            model.SerialList = new SelectList(se);

            int defautPagesize = Pagesize.HasValue ? Convert.ToInt32(Pagesize) : 10;
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            int totalRecords = 0;
            IInvoiceService IInvSrv = InvServiceFactory.GetService(model.Pattern, currentCom.id);
            IList<IInvoice> lstInv;
            if (model.InvNo.HasValue && model.InvNo > 0) // lấy hóa đơn theo số và  sêri
            {
                IInvoice Invoice = IInvSrv.GetByNo(currentCom.id, model.Pattern, model.Serial, model.InvNo.Value);
                lstInv = new List<IInvoice>();
                if (Invoice != null) lstInv.Add(Invoice);
                totalRecords = lstInv.Count;
            }
            else // tìm theo các điều kiện khác
            {
                DateTime? DateFrom = null;
                DateTime? DateTo = null;
                if (!string.IsNullOrWhiteSpace(model.FromDate)) DateFrom = DateTime.ParseExact(model.FromDate, "dd/MM/yyyy", null);
                if (!string.IsNullOrWhiteSpace(model.ToDate)) DateTo = DateTime.ParseExact(model.ToDate, "dd/MM/yyyy", null);
                if (model.Converted != 0)
                    lstInv = IInvSrv.SearchByConvertStatus(currentCom.id, model.Pattern, model.Serial, model.Converted > 0, DateFrom, DateTo, model.cusName, model.cuscode, currentPageIndex, defautPagesize, out totalRecords);
                else
                    lstInv = IInvSrv.SearchPublish(currentCom.id, model.Pattern, model.Serial, DateFrom, DateTo, model.cusName, model.cuscode, currentPageIndex, defautPagesize, out totalRecords);
            }
            model.PageListINV = new PagedList<IInvoice>(lstInv, currentPageIndex, defautPagesize, totalRecords);
            return View(model);
        }
        
        public ActionResult ConvertForStore(int id, string patt)
        {
            if (id <= 0)
                throw new HttpRequestValidationException();
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            user currentUser = ((EInvoiceContext)FXContext.Current).CurrentUser;
            string err;
            IInvoiceService InvSrv = InvServiceFactory.GetService(patt, currentCom.id);
            IInvoice inv = InvSrv.GetByID(currentCom.id, patt, id);
            if (inv.Status == InvoiceStatus.CanceledInv || inv.Status == InvoiceStatus.ReplacedInv)
                return Json("nosuccess");
            
            IStaffService _staSrv = IoC.Resolve<IStaffService>();
            Staff staff = _staSrv.SearchByAccountName(currentUser.username, currentCom.id);
            string name = "";
            if (null != staff)
            {
                name = staff.FullName;
            }
            string HtmlRet = InvSrv.ConvertForStore(inv, name, out err);
            if (err == string.Empty)
            {                
                log.Info("Convert Invoice (Luu tru) By : " + HttpContext.User.Identity.Name + " Info-- Pattern: " + inv.Pattern + "   Serial: " + inv.Serial + "   No: " + inv.No);
                return Json(HtmlRet);
            }            
            return Json("nosuccess");
        }
        
        public ActionResult ConvertForVerify(int id, string patt)
        {
            if (id <= 0)
                throw new HttpRequestValidationException();
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            user currentUser = ((EInvoiceContext)FXContext.Current).CurrentUser;
            string err;
            IInvoiceService InvSrv = InvServiceFactory.GetService(patt, currentCom.id);
            IInvoice inv = InvSrv.GetByID(currentCom.id, patt, id);
            if (inv.Status == InvoiceStatus.CanceledInv || inv.Status == InvoiceStatus.ReplacedInv)
                return Json("nosuccess"); 
            IStaffService _staSrv = IoC.Resolve<IStaffService>();
            Staff staff = _staSrv.SearchByAccountName(currentUser.username, currentCom.id);
            string name = "";
            if (null != staff)
            {
                name = staff.FullName;
            }
            string HtmlRet = InvSrv.ConvertForVerify(inv, name, out err);
            if (err == string.Empty)
            {
                log.Info("Convert Invoice (Chung minh nguon goc) By : " + HttpContext.User.Identity.Name + " Info-- Pattern: " + inv.Pattern + "   Serial: " + inv.Serial + "   No: " + inv.No);
                return Json(HtmlRet);
            }            
            return Json("nochange");
        }        
    }
}
