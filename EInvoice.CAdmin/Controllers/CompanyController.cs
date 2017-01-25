using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FX.Core;
using EInvoice.Core.Domain;
using EInvoice.Core.IService;
using EInvoice.Core;
using IdentityManagement.Authorization;
using log4net;
using System.Web;
using FX.Context;
using EInvoice.CAdmin.Models;

namespace EInvoice.CAdmin.Controllers
{
    public class CompanyController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CompanyController));
        //Hien thi thong tin chi tiet company
        [RBACAuthorize(Permissions = "View_com")]
        public ActionResult DetailCurrent()
        {
            int _comid = ((EInvoiceContext)FXContext.Current).CurrentCompany.id;
            try
            {
                ICompanyService _comSrv = IoC.Resolve<ICompanyService>();
                Company model = _comSrv.Getbykey(_comid);
                ITaxAuthorityService _taxSvc = IoC.Resolve<ITaxAuthorityService>();
                if (!string.IsNullOrWhiteSpace(model.TaxAuthorityCode))
                {
                    var tax = _taxSvc.Getbykey(model.TaxAuthorityCode);
                    ViewData["taxname"] = tax != null ? tax.Name : "";
                }
                else
                {
                    ViewData["taxname"] = "";
                }
                return View(model);
            }
            catch (Exception ex)
            {
                log.Error(" DetailCurrent  -" + ex.Message);
                return RedirectToAction("Index", "Home");
            }

        }
        // Sửa thông tin Company
        [HttpGet]
        [RBACAuthorize(Permissions = "Edit_com")]
        public ActionResult Edit()
        {
            int id = ((EInvoiceContext)FXContext.Current).CurrentCompany.id;
            ITaxAuthorityService _taxSvc = IoC.Resolve<ITaxAuthorityService>();
            IList<TaxAuthority> tax = _taxSvc.GetAll();
            ViewData["tax"] = tax;
            ICompanyService _comSrv = IoC.Resolve<ICompanyService>();
            Company model = _comSrv.Getbykey(id);
            return View(model);
        }

        [HttpPost]
        [RBACAuthorize(Permissions = "Edit_com")]
        public ActionResult Update(int id)
        {
            Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            if (id != _currentCompany.id)
                return Redirect("/Home/PotentiallyError");
            ICompanyService _comSrv = IoC.Resolve<ICompanyService>();
            Company model = _comSrv.Getbykey(_currentCompany.id);
            try
            {
                TryUpdateModel<Company>(model);
                model.TaxCode = Utils.formatTaxcode(model.TaxCode);
                _comSrv.Save(model);
                _comSrv.CommitChanges();
                Messages.AddFlashMessage(Resources.Message.Com_UMesInfSuccess);
                log.Info("Update Company: " + HttpContext.User.Identity.Name);                
                return RedirectToAction("DetailCurrent");
            }
            catch (HttpRequestValidationException ex)
            {
                return Redirect("/Home/PotentiallyError");
            }
            catch (ArgumentException ex)
            {
                return Redirect("/Home/PotentiallyError");
            }
            catch (Exception ex)
            {
                Messages.AddErrorMessage("Có lỗi xảy ra, vui lòng thực hiện lại!");
                log.Error("Error Update-" + ex);
                ITaxAuthorityService _taxSvc = IoC.Resolve<ITaxAuthorityService>();
                IList<TaxAuthority> tax = _taxSvc.GetAll();
                ViewData["tax"] = tax;
                return View("Edit", model);
            }
        }
        //Kiểm tra mã số thuế
        public ActionResult checkMST(string mst)
        {
            return Json(Utils.CheckMST(mst));
        }

        //Hiển thị thông tin Keystore
        [RBACAuthorize(Permissions = "View_store")]
        public ActionResult ViewKeyStores()
        {
            IKeyStoresService _ikeySer = IoC.Resolve<IKeyStoresService>();
            Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            ICertificateService _cerSVC = IoC.Resolve<ICertificateService>();
            try
            {
                KeyStores oKeyStores = _ikeySer.Query.Where(a => a.ComID == _currentCompany.id).SingleOrDefault();
                if (oKeyStores == null)
                {
                    Messages.AddErrorFlashMessage("Chưa đăng ký chữ ký số, liên hệ nhà cung cấp để được hỗ trợ.");
                    return RedirectToAction("Index", "Home");
                }
                if (oKeyStores.Type == 2 || oKeyStores.KeyStoresOf == 1)
                {
                    Certificate cer = new Certificate();
                    cer.id = oKeyStores.Id;
                    cer.ComID = oKeyStores.ComID;
                    cer.SerialCert = oKeyStores.SerialCert;
                    cer.Cert = "";
                    cer.OrganizationCA = "";
                    cer.OwnCA = "";
                    cer.ValidFrom = new DateTime();
                    cer.ValidTo = new DateTime();
                    return View(cer);
                }                
                Certificate cert = (from c in _cerSVC.Query where c.ComID == _currentCompany.id && oKeyStores.SerialCert == c.SerialCert select c).SingleOrDefault();
                if (cert == null)
                {
                    Messages.AddErrorFlashMessage("Bạn vẫn chưa cấu hình keyStore");
                    return RedirectToAction("Index", "Home");
                }
                return View(cert);
            }
            catch (Exception ex)
            {
                Messages.AddErrorFlashMessage("Bạn vẫn chưa cấu hình keyStore");
                log.Error(" ViewKeyStores  -" + ex.Message);
                return RedirectToAction("Index", "Home");
            }
        }

        [ChildActionOnly]
        public ActionResult MenuBycomp(string viewname, int position = 0)
        {            
            IList<MenuModels> model = MenuModels.GetTree(position); 
            if (!string.IsNullOrWhiteSpace(viewname))
                return PartialView(viewname, model);
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult MenuTop(int position = 1)
        {            
            IList<MenuModels> model = MenuModels.GetTree(position);
            return PartialView(model);
        }
        
        [RBACAuthorize(Permissions = "Edit_com")]
        public ActionResult GenActiveCode()
        {
            string activeCode = Encrypt.EncryptString(FX.Utils.UrlUtil.GetSiteUrl());
            ViewBag.ActiveCode = activeCode;
            return View();
        }

        //Tạo cấu hình định danh xác thực
        public ActionResult ConfigCertify()
        {
            Company company = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            //Mã chi cục thuế nơi đơn vị khai báo thuế
            if (!company.Config.ContainsKey("VAN_TAX_OFFICE_CODE"))
                company.Config.Add("VAN_TAX_OFFICE_CODE", "#NA");
            //Mã định danh do cục thuế cung cấp khi đăng ký
            if (!company.Config.ContainsKey("VAN_AUT_CODE"))
                company.Config.Add("VAN_AUT_CODE", "#NA");
            //Mã doanh nghiệp tự đặt
            if (!company.Config.ContainsKey("VAN_SYSTEM_CODE"))
                company.Config.Add("VAN_SYSTEM_CODE", "LHD_VDC");

            return View(new { VAN_TAX_OFFICE_CODE = company.Config["VAN_TAX_OFFICE_CODE"], VAN_AUT_CODE = company.Config["VAN_AUT_CODE"], VAN_SYSTEM_CODE = company.Config["VAN_SYSTEM_CODE"] });
        }

        [HttpPost]
        public ActionResult ConfigCertify(string VAN_TAX_OFFICE_CODE, string VAN_AUT_CODE, string VAN_SYSTEM_CODE)
        {
            if (String.IsNullOrEmpty(VAN_AUT_CODE) || String.IsNullOrEmpty(VAN_SYSTEM_CODE) || String.IsNullOrEmpty(VAN_TAX_OFFICE_CODE))
            {
                Messages.AddErrorFlashMessage("Xin lỗi, cả ba trường dữ liệu trên là bắt buộc, bạn xem lại hướng dẫn bên dưới để cập nhật chính xác!");
                return View(new { VAN_AUT_CODE = VAN_AUT_CODE, VAN_SYSTEM_CODE = VAN_SYSTEM_CODE, VAN_TAX_OFFICE_CODE = VAN_TAX_OFFICE_CODE });
            }
            try
            {
                Company company = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                ICompanyService _comSrv = IoC.Resolve<ICompanyService>();
                IConfigService configSrv = IoC.Resolve<IConfigService>();
                var authenCode = configSrv.Query.FirstOrDefault(c => c.ComID == company.id && c.Key == "VAN_AUT_CODE");

                if (authenCode != null)
                    authenCode.Value = VAN_AUT_CODE;
                else
                    configSrv.CreateNew(new Config { ComID = company.id, Key = "VAN_AUT_CODE", Value = VAN_AUT_CODE });


                var taxOfficeCode = configSrv.Query.FirstOrDefault(c => c.ComID == company.id && c.Key == "VAN_TAX_OFFICE_CODE");

                if (taxOfficeCode != null)
                    taxOfficeCode.Value = VAN_TAX_OFFICE_CODE;
                else
                    configSrv.CreateNew(new Config { ComID = company.id, Key = "VAN_TAX_OFFICE_CODE", Value = VAN_TAX_OFFICE_CODE });

                var systemCode = configSrv.Query.FirstOrDefault(c => c.ComID == company.id && c.Key == "VAN_SYSTEM_CODE");

                if (systemCode != null)
                    systemCode.Value = VAN_SYSTEM_CODE;
                else
                    configSrv.CreateNew(new Config { ComID = company.id, Key = "VAN_SYSTEM_CODE", Value = VAN_SYSTEM_CODE });
                configSrv.CommitChanges();
                Messages.AddFlashMessage("Cập nhật thông tin chuỗi định danh mật khẩu thành công!");
                return RedirectToAction(actionName: "Index", controllerName: "Home");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                Messages.AddErrorMessage("Có lỗi xảy ra, vui lòng thực hiện lại.");
            }
            return View(new { VAN_AUT_CODE = VAN_AUT_CODE, VAN_TAX_CODE = VAN_TAX_OFFICE_CODE });
        }
    }
}
