using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FX.Core;
using EInvoice.Core.Domain;
using EInvoice.Core.IService;
using FX.Utils.MvcPaging;
using FX.Utils.MVCMessage;
using EInvoice.Core;
using IdentityManagement.Authorization;
using log4net;
using System.Web;
using FX.Context;

namespace EInvoice.CAdmin.Controllers
{
    public class ProductController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProductController));
        private readonly IProductsService _proSvc;
        private readonly Company _currentCompany;

        public ProductController()
        {
            _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            _proSvc = IoC.Resolve<IProductsService>();
        }

        #region "Index product"
        [RBACAuthorize(Permissions = "Search_prod")]
        public ActionResult Index(string name, string code, int? page)
        {
            int defautPagesize = 10;
            int total = 0;
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            IList<Products> lst = _proSvc.SearchByNameCode(name, code, _currentCompany.id, currentPageIndex, defautPagesize, out total);
            IPagedList<Products> model = new PagedList<Products>(lst, currentPageIndex, defautPagesize, total);
            ViewData["name"] = name;
            ViewData["code"] = code;
            return View(model);
        }
        #endregion

        #region "Create product"
        [HttpGet]
        [RBACAuthorize(Permissions = "Add_prod")]
        public ActionResult Create()
        {
            Products model = new Products();
            return View(model);
        }
        [HttpPost]
        [RBACAuthorize(Permissions = "Add_prod")]
        public ActionResult Create(Products model)
        {
            try
            {
                var pro = _proSvc.Query.Where(i => i.Code.ToUpper() == model.Code.Trim().ToUpper() && i.ComID == _currentCompany.id);
                if (pro.Count() > 0)
                {
                    Messages.AddErrorMessage(Resources.Message.Prod_IMesCodeExits);
                    return View(model);
                }
                model.ComID = _currentCompany.id;
                _proSvc.Save(model);
                _proSvc.CommitChanges();
                Messages.AddFlashMessage(Resources.Message.Prod_IMesSuccess);
                log.Info("Create Products by: " + HttpContext.User.Identity.Name);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error(" Create -" + ex);
                Messages.AddErrorMessage(Resources.Message.Prod_IMesUnsuccess);
                return View(model);
            }
        }
        #endregion

        #region "Edit product"
        [HttpGet]
        [RBACAuthorize(Permissions = "Edit_prod")]
        public ActionResult Edit(int id)
        {
            Products model = _proSvc.Getbykey(id);
            return View(model);
        }
        [HttpPost]
        [RBACAuthorize(Permissions = "Edit_prod")]
        public ActionResult Update(int id)
        {
            if (id <= 0)
                throw new HttpRequestValidationException();
            Products model = _proSvc.Getbykey(id);
            try
            {
                TryUpdateModel<Products>(model);
                _proSvc.Save(model);
                _proSvc.CommitChanges();
                log.Info("Edit Product by: " + HttpContext.User.Identity.Name);
                Messages.AddFlashMessage(Resources.Message.Prod_UMesSuccess);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error(" Edit -" + ex);
                Messages.AddErrorMessage(Resources.Message.Prod_UMesUnsuccess);
                return View("Edit", model);
            }
        }
        #endregion

        #region "Delete Product"
        [RBACAuthorize(Permissions = "Del_prod")]
        public ActionResult Delete(int id)
        {
            if (id <= 0)
                throw new HttpRequestValidationException();
            try
            {
                Products model = _proSvc.Getbykey(id);
                _proSvc.Delete(model);
                _proSvc.CommitChanges();
                log.Info("Delete Products by: " + HttpContext.User.Identity.Name);
                Messages.AddFlashMessage(Resources.Message.Prod_DMesSuccess);
            }
            catch (Exception ex)
            {
                log.Error(" Delete -" + ex);
                Messages.AddErrorFlashMessage(Resources.Message.Prod_DMesUnsuccess);
            }
            return RedirectToAction("Index");
        }
        #endregion

        public JsonResult SeachByName(string searchText = "")
        {
            searchText = searchText.Trim().ToLower();

            var values = _proSvc.Query.Where(p =>
                p.ComID == _currentCompany.id &&
                p.NameProduct.ToLower().Contains(searchText))
                .Select(p => new
                {
                    Name = p.NameProduct,
                    Unit = p.Unit,
                    Price = p.Price
                }).Take(15).ToList();

            return Json(values, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SeachByCode(string code = "")
        {
            var qr = from c in _proSvc.Query
                     where c.ComID == _currentCompany.id
                         && c.Code.ToUpper().Contains(code.ToUpper())
                     select (new
                     {
                         Code = c.Code,
                         Name = c.NameProduct,
                         Unit = c.Unit,
                         Price = c.Price
                     });

            return Json(qr, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetDataById(string idSPDV)
        {
            Products qr = (from c in _proSvc.Query
                           where c.ComID == _currentCompany.id
                           && c.Id == Convert.ToInt32(idSPDV)
                           select c).FirstOrDefault<Products>();
            if (qr == null)
                return null;
            return Json(new
            {
                qr.Code,
                qr.NameProduct,
                qr.Price,
                qr.Unit,
            }
            );
        }        
    }
}
