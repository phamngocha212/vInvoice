using EInvoice.CAdmin.Models;
using FX.Core;
using FX.Utils.MVCMessage;
using FX.Utils.MvcPaging;
using IdentityManagement.Authorization;
using IdentityManagement.Domain;
using IdentityManagement.Service;
using IdentityManagement.WebProviders;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EInvoice.CAdmin.Controllers
{
    [MessagesFilter]
    public class IDManagerController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(IDManagerController));

        [RBACAuthorize(Roles = "Admin")]
        public ActionResult Index(int? page)
        {
            IroleService _RoleSrc = IoC.Resolve<IroleService>();
            IRBACMembershipProvider _MemberShipProvider = IoC.Resolve<IRBACMembershipProvider>();
            int defautPageSize = 20;
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            IList<role> model;
            model = _RoleSrc.Query.Where(c => c.AppID == _MemberShipProvider.Application.AppID && c.name != "Root").OrderByDescending(a => a.roleid).ToList<role>();
            IPagedList<role> LstRoles = new PagedList<role>(model, currentPageIndex, defautPageSize, model.Count());
            return View(LstRoles);
        }

        [HttpGet]
        [RBACAuthorize(Roles = "Admin")]
        public ActionResult Create()
        {
            IpermissionService _PermissionSrc = IoC.Resolve<IpermissionService>();
            IRBACMembershipProvider _MemberShipProvider = IoC.Resolve<IRBACMembershipProvider>();
            RoleModel model = new RoleModel();
            model.Permissions = new List<permission>();
            List<permission> lPermissions = _PermissionSrc.Query.Where(e => e.AppID == _MemberShipProvider.Application.AppID).OrderBy(p=>p.Description).ToList<permission>();
            ViewData["Permissions"] = lPermissions;
            return View(model);
        }
        [HttpPost]
        [RBACAuthorize(Roles = "Admin")]
        public ActionResult CreateRole(RoleModel model, string[] permissions)
        {
            IroleService _RoleSrc = IoC.Resolve<IroleService>();
            IpermissionService _PermissionSrc = IoC.Resolve<IpermissionService>();
            IRBACMembershipProvider _MemberShipProvider = IoC.Resolve<IRBACMembershipProvider>();
            try
            {
                int cRole = _RoleSrc.Query.Where(p => p.AppID == _MemberShipProvider.Application.AppID && p.name.ToUpper() == model.name.Trim().ToUpper()).Count();
                if (cRole > 0)
                {
                    Messages.AddErrorMessage("Tên quyền này đã tồn tại trong hệ thống.");
                    List<permission> lPermissions = _PermissionSrc.Query.Where(a => a.AppID == _MemberShipProvider.Application.AppID).ToList<permission>();
                    ViewData["Permissions"] = lPermissions;
                    model.Permissions = new List<permission>();
                    return View("Create", model);
                }
                role omodel = new role();
                model.Permissions = permissions == null ? new List<permission>() : _PermissionSrc.Query.Where(p => permissions.Contains(p.name)).OrderBy(p => p.Description).ToList<permission>();
                //lay cac thong tin cho role
                omodel.name = model.name;
                omodel.Permissions = model.Permissions;
                omodel.AppID = _MemberShipProvider.Application.AppID;
                _RoleSrc.CreateNew(omodel);
                _RoleSrc.CommitChanges();
                Messages.AddFlashMessage("Tạo quyền thành công.");
                log.Info("Create Role by:" + HttpContext.User.Identity.Name + " Info-- NameRole " + model.name);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error("Create Role-" + ex);
                Messages.AddErrorMessage("Có lỗi xảy ra, vui lòng thực hiện lại.");
                List<permission> lPermissions = _PermissionSrc.Query.Where(a => a.AppID == _MemberShipProvider.Application.AppID).OrderBy(p => p.Description).ToList<permission>();
                ViewData["Permissions"] = lPermissions;
                model.Permissions = new List<permission>();
                return View("Create", model);
            }
        }

        [RBACAuthorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            IroleService _RoleSrc = IoC.Resolve<IroleService>();
            IpermissionService _PermissionSrc = IoC.Resolve<IpermissionService>();
            IRBACMembershipProvider _MemberShipProvider = IoC.Resolve<IRBACMembershipProvider>();
            RoleModel model = new RoleModel();
            model.Permissions = new List<permission>();
            List<permission> lPermissions = (from p in _PermissionSrc.Query where p.AppID == _MemberShipProvider.Application.AppID select p).OrderBy(p => p.Description).ToList();
            ViewData["Permissions"] = lPermissions;
            role orole = _RoleSrc.Getbykey(id);
            model.name = orole.name;
            model.Permissions = orole.Permissions.ToList<permission>();
            model.Id = id;
            return View(model);
        }
        [HttpPost]
        [RBACAuthorize(Roles = "Admin")]
        public ActionResult EditRole(int roleid, string[] permissions)
        {
            if (roleid <= 0)
                throw new HttpRequestValidationException();
            IroleService _RoleSrc = IoC.Resolve<IroleService>();
            IpermissionService _PermissionSrc = IoC.Resolve<IpermissionService>();
            IRBACMembershipProvider _MemberShipProvider = IoC.Resolve<IRBACMembershipProvider>();
            role omodel = _RoleSrc.Getbykey(roleid);
            try
            {
                TryUpdateModel<role>(omodel);
                if (omodel != null)
                {
                    omodel.Permissions = permissions == null ? new List<permission>() : _PermissionSrc.Query.Where(p => permissions.Contains(p.name)).OrderBy(p => p.Description).ToList<permission>();
                    _RoleSrc.Update(omodel);
                    _RoleSrc.CommitChanges();
                    Messages.AddFlashMessage("Sửa role thành công.");
                    log.Info("Edit Role by:" + HttpContext.User.Identity.Name + " Info--NameRole " + omodel.name);
                    return RedirectToAction("Index");
                }
                else
                {
                    RoleModel model = new RoleModel();
                    model.Id = roleid;
                    model.name = omodel.name;
                    model.Permissions = omodel.Permissions.ToList<permission>();
                    List<permission> lPermissions = _PermissionSrc.Query.Where(a => a.AppID == _MemberShipProvider.Application.AppID).OrderBy(p => p.Description).ToList<permission>();
                    ViewData["Permissions"] = lPermissions;
                    Messages.AddErrorMessage("Có lỗi xảy ra, vui lòng thực hiện lại.");
                    log.Error("Edit Role - role null");
                    return View("Edit", model);
                }
            }
            catch (Exception ex)
            {
                log.Error("Edit Role-" + ex);
                RoleModel model = new RoleModel();
                model.Id = roleid;
                model.name = omodel.name;
                model.Permissions = omodel.Permissions.ToList<permission>();
                List<permission> lPermissions = _PermissionSrc.Query.Where(a => a.AppID == _MemberShipProvider.Application.AppID).OrderBy(p => p.Description).ToList<permission>();
                ViewData["Permissions"] = lPermissions;
                Messages.AddErrorMessage("Có lỗi trong quá trình sửa role.");
                return View("Edit", model);
            }
        }

        [RBACAuthorize(Roles = "Admin")]
        public ActionResult Delete(int roleid)
        {
            if (roleid <= 0)
                throw new HttpRequestValidationException();
            IroleService _RoleSrc = IoC.Resolve<IroleService>();
            role model = _RoleSrc.Getbykey(roleid);
            if (model == null)
            {
                Messages.AddErrorFlashMessage("Role không tồn tại, không thể xóa.");
                return RedirectToAction("Index");
            }
            if (model.name == "Admin" || model.name == "ServiceRole")
            {
                Messages.AddErrorFlashMessage("Không thể xóa role này.");
                return RedirectToAction("Index");
            }
            try
            {

                _RoleSrc.Delete(model);
                _RoleSrc.CommitChanges();
                Messages.AddFlashMessage("Xóa role thành công.");
                log.Info("Delete Role by:" + HttpContext.User.Identity.Name + " Info--id " + model.roleid + " NameRole " + model.name);
            }
            catch (Exception ex)
            {
                log.Error("Delete Role-" + ex);
                Messages.AddErrorFlashMessage("Có lỗi xảy ra, vui lòng thực hiện lại.");
            }
            return RedirectToAction("Index");
        }

        private MessageViewData Messages
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
