using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EInvoice.Core.Domain;
using EInvoice.Core.IService;
using FX.Core;
using FX.Utils.MvcPaging;
using FX.Utils.MVCMessage;
using EInvoice.Core;
using EInvoice.CAdmin.Models;
using IdentityManagement.Domain;
using IdentityManagement.Service;
using IdentityManagement.Authorization;
using FX.Data;
using log4net;
using IdentityManagement.WebProviders;
using System.Web;
using FX.Context;

namespace EInvoice.CAdmin.Controllers
{
    public class StaffController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(StaffController));

        // GET: /Staff/
        [RBACAuthorize(Permissions = "View_Staff")]
        public ActionResult Index(StaffModel model, int? page)
        {
            Company _currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IStaffService _staSrv = IoC.Resolve<IStaffService>();
            int defautPagesize = 10;
            int total = 0;
            IList<Staff> lst = _staSrv.SearchByName(model.fullname, model.division, model.account, _currentCom.id, page.HasValue ? page.Value - 1 : 0, defautPagesize, out total);
            model.PageListStaff = new PagedList<Staff>(lst, page.HasValue ? page.Value - 1 : 0, defautPagesize, total);
            return View(model);
        }
        [RBACAuthorize(Permissions = "Add_Staff")]
        public ActionResult Create()
        {
            Company _currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            Staff model = new Staff();
            model.ComID = _currentCom.id;
            return View(model);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [RBACAuthorize(Permissions = "Add_Staff")]
        public ActionResult Create(Staff model)
        {
            if (string.IsNullOrWhiteSpace(model.FullName) || string.IsNullOrWhiteSpace(model.AccountName))
            {
                Messages.AddErrorMessage("Cần nhập các thông tin bắt buộc!");
                return View(model);
            }
            IStaffService _staSrv = IoC.Resolve<IStaffService>();
            if (_staSrv.Query.Where(p => p.AccountName.ToUpper() == model.AccountName.Trim().ToUpper()).Count() > 0)
            {
                Messages.AddErrorMessage("Tồn tại tài khoản trên hệ thống.");
                return View("Create", model);
            }
            Company _currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            string ErrorMessage = "";
            if (_staSrv.CreateStaff(model, _currentCom.id, out ErrorMessage) == true)
            {
                log.Info("Create staff by: " + HttpContext.User.Identity.Name + " Info-- TenNhanVien: " + model.FullName + " TaiKhoanNhanVien: " + model.AccountName + " Email: " + model.Email);
                Messages.AddFlashMessage(Resources.Message.Staff_IMesSuccess);
                return RedirectToAction("Index");
            }
            else
            {
                Messages.AddErrorMessage("Lỗi: " + ErrorMessage);
                return View(model);
            }
        }
        [RBACAuthorize(Permissions = "Edit_Staff")]
        public ActionResult Edit(int id)
        {
            IStaffService _staSrv = IoC.Resolve<IStaffService>();
            Staff model = _staSrv.Getbykey(id);
            return View(model);
        }
        [RBACAuthorize(Permissions = "Edit_Staff")]
        [HttpPost]
        public ActionResult Update(int id)
        {
            if (id <= 0)
                throw new HttpRequestValidationException();
            IStaffService _staSrv = IoC.Resolve<IStaffService>();
            var model = _staSrv.Getbykey(id);            
            try
            {
                TryUpdateModel<Staff>(model);
                _staSrv.Save(model);
                _staSrv.CommitChanges();
                log.Info("Edit staff by: " + HttpContext.User.Identity.Name + " Info-- TenNhanVien: " + model.FullName + " TaiKhoanNhanVien: " + model.AccountName + " Email: " + model.Email);
                Messages.AddFlashMessage(Resources.Message.Staff_UMesSuccess);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error("Edit-" + ex);
                Messages.AddErrorMessage(Resources.Message.Staff_UMesUnsuccess);
                return View("Edit", model);
            }
        }
        #region"Xóa nhân viên"
        [RBACAuthorize(Permissions = "Del_Staff")]
        public ActionResult Delete(int id)
        {
            if (id <= 0)
                throw new HttpRequestValidationException();
            string ErrorMessage = "";
            IStaffService _staSrv = IoC.Resolve<IStaffService>();
            Staff model = _staSrv.Getbykey(id);
            
            if (model.AccountName == HttpContext.User.Identity.Name)
            {
                Messages.AddErrorFlashMessage("Tài khoản của nhân viên này đang đăng nhập lên không thể xóa!");
                return RedirectToAction("Index");
            }
            if (_staSrv.DeleteStaff(id, out ErrorMessage) == true)
            {
                log.Info("Delete staff by: " + HttpContext.User.Identity.Name + " Info-- ID: " + id.ToString());
                Messages.AddFlashMessage(Resources.Message.Staff_DMesSuccess);
            }
            else
            {
                Messages.AddErrorFlashMessage(Resources.Message.Staff_DMesUnsuccess);
            }
            return RedirectToAction("Index");
        }
        #endregion
        
        public JsonResult SearchByAccountName(string searchText)
        {
            Company _currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IuserService _SvcUser = IoC.Resolve<IuserService>();
            IList<user> lst = _SvcUser.GetbyHQuery("select u from user u where u.GroupName = :comid AND u.username like :searchText AND u.username Not IN (select AccountName from Customer)", new SQLParam("comid", _currentCom.id), new SQLParam("searchText", "%" + searchText + "%"));
            var qr = from u in lst select (new { u.username });
            return Json(qr, JsonRequestBehavior.AllowGet);
        }
    }
}
