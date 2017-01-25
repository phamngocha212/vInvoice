using EInvoice.CAdmin.Models;
using EInvoice.Core;
using EInvoice.Core.Domain;
using EInvoice.Core.IService;
using FX.Context;
using FX.Core;
using FX.Utils.MVCMessage;
using FX.Utils.MvcPaging;
using IdentityManagement.Authorization;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EInvoice.CAdmin.Controllers
{
    public class MenuController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MenuController));

        [RBACAuthorize(Permissions = "Search_user")]
        public ActionResult Index(int? parentId, int? page, int position = 0)
        {
            int pageIndex = page.HasValue ? page.Value - 1 : 0;
            int pageSize = 20;
            int totalRecord = 0;
            Company company = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IMenusService menuSrv = IoC.Resolve<IMenusService>();
            MenusModels model = new MenusModels();
            model.RootMenus = MenuModel.GetRoots(company.id, position);
            int pId = parentId.HasValue ? parentId.Value : model.RootMenus[0].Id;
            var list = MenuModel.GetTree(company.id, pId);
            totalRecord = list.Count;
            model.PagedListMenus = new PagedList<MenuModel>(list, pageIndex, pageSize, totalRecord);
            return View(model);
        }

        [RBACAuthorize(Permissions = "Search_user")]
        public ActionResult Edit(int id)
        {
            IMenusService menuSrv = IoC.Resolve<IMenusService>();
            ICompanyService compSrv = IoC.Resolve<ICompanyService>();
            Menu model = menuSrv.Getbykey(id);
            ViewBag.ParentMenus = menuSrv.GetParent(model.ComID);
            return View(model);
        }

        [HttpPost]
        [RBACAuthorize(Permissions = "Search_user")]
        public ActionResult Edit(string Name, int id)
        {
            if (id <= 0)
                throw new HttpRequestValidationException();
            IMenusService menuSrv = IoC.Resolve<IMenusService>();
            Menu model = menuSrv.Getbykey(id);
            try
            {
                TryUpdateModel<Menu>(model);
                model.Name = Name;
                menuSrv.Save(model);
                menuSrv.CommitChanges();
                Messages.AddFlashMessage("Cập nhật thành công!");
                log.Info("Update menu by:" + HttpContext.User.Identity.Name + ", Date:" + DateTime.Now);
                MenuModels.ResetMenu();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error("Update menu error", ex);
                ViewBag.ParentMenus = menuSrv.GetParent(model.ComID);
                Messages.AddErrorMessage("Có lỗi trong quá trình xử lý, vui lòng thực hiện lại!");
                return View(model);
            }
        }
    }
}
