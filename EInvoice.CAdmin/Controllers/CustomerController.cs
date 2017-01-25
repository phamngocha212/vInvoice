using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EInvoice.Core.Domain;
using EInvoice.Core.IService;
using FX.Core;
using FX.Utils.MvcPaging;
using EInvoice.Core;
using IdentityManagement.Domain;
using IdentityManagement.Authorization;
using log4net;
using IdentityManagement.WebProviders;
using EInvoice.CAdmin.Models;
using System.Web;
using FX.Context;
namespace EInvoice.CAdmin.Controllers
{
    public class CustomerController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CustomerController));
        //Danh sách các khách hàng
        [RBACAuthorize(Permissions = "Search_cus")]
        public ActionResult Index(CusIndexModel model, int? page)
        {
            Company _currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            ICustomerService cusSrv = IoC.Resolve<ICustomerService>();
            int defautPagesize = 15;
            int total = 0;
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            IList<Customer> lst = cusSrv.SearchByNameCode(model.name, model.code, _currentCom.id, currentPageIndex, defautPagesize, out total);
            model.PagedListCUS = new PagedList<Customer>(lst, currentPageIndex, defautPagesize, total);
            return View(model);
        }

        //Tạo mới khách hàng
        [RBACAuthorize(Permissions = "Add_cus")]
        public ActionResult Create()
        {
            Company _currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            CustomerModel model = new CustomerModel();
            Customer cus = new Customer();
            cus.ComID = _currentCom.id;
            cus.DeliverMethod = -1;
            model.tmpCustomer = cus;            
            return View(model);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [RBACAuthorize(Permissions = "Add_cus")]
        public ActionResult Create(Customer tmp, CustomerModel model, string[] DeliverMethod)
        {            
            if (string.IsNullOrWhiteSpace(tmp.Name) || string.IsNullOrWhiteSpace(tmp.Code))
            {                
                Messages.AddErrorMessage("Cần nhập các thông tin bắt buộc.");
                model.tmpCustomer = tmp;
                return View(model);
            }
            IRBACMembershipProvider _MemberShipProvider = FX.Core.IoC.Resolve<IRBACMembershipProvider>();
            // kiểm tra tài khoản được sử dụng chưa
            user us = _MemberShipProvider.GetUser(tmp.AccountName, true);
            if (us != null)
            {                
                Messages.AddErrorMessage("Tài khoản có trong hệ thống.");
                model.tmpCustomer = tmp;
                return View(model);
            }
            Company _currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            ICustomerService cusSrv = IoC.Resolve<ICustomerService>();            
            string ErrorMessage = "";
            var qr = cusSrv.Query.Where(p => p.ComID == _currentCom.id);            
            
            if (!string.IsNullOrWhiteSpace(tmp.TaxCode))
                qr = qr.Where(p => p.TaxCode.ToUpper() == tmp.TaxCode.ToUpper() || p.Code.ToUpper() == tmp.Code.Trim().ToUpper());
            else
                qr = qr.Where(p => p.Code.ToUpper() == tmp.Code.Trim().ToUpper());
            if (qr.Count() > 0)
            {
                model.tmpCustomer = tmp;
                Messages.AddErrorMessage("Mã số thuế hoặc mã khách hàng đã tồn tại trên hệ thống!");
                return View(model);
            }            
            Certificate cer = model.UpdateCertificate(new Certificate());
            // user user = new user();
            //add delivermethod
            if (DeliverMethod == null) tmp.DeliverMethod = -1;
            else if (DeliverMethod.Length == 1)
            {
                if (DeliverMethod[0] == "0") tmp.DeliverMethod = 0;
                else if (DeliverMethod[0] == "1") tmp.DeliverMethod = 1;
            }
            else if (DeliverMethod.Length == 2) tmp.DeliverMethod = 2;

            tmp.TaxCode = Utils.formatTaxcode(tmp.TaxCode);
            //end delivermethod
            if (cusSrv.CreateCus(tmp, cer, _currentCom.id, out ErrorMessage))
            {
                log.Info("Create Customer by: " + HttpContext.User.Identity.Name + " Info-- TenKhachHang: " + tmp.Name + " TaiKhoanKhachHang: " + tmp.AccountName + " Email: " + tmp.Email);
                Messages.AddFlashMessage(Resources.Message.Cus_IMesSuccess);
                // send Mail--
                try
                {
                    if (!string.IsNullOrEmpty(tmp.Email))
                    {
                        string randompass = (_currentCom.Config.Keys.Contains("SetDefaultCusPass")) ? _currentCom.Config["SetDefaultCusPass"] : "Hddt123456";
                        string labelEmail = _currentCom.Config.Keys.Contains("LabelMail") ? _currentCom.Config["LabelMail"] : "hoadondientu@vinvoice.vn";
                        string portalLink = _currentCom.Config.Keys.Contains("PortalLink") ? _currentCom.Config["PortalLink"] : "http://hddt.vinvoice.vn";
                        IService.IRegisterEmailService emailSrv = FX.Core.IoC.Resolve<IService.IRegisterEmailService>();
                        Dictionary<string, string> subjectParams = new Dictionary<string, string>(1);
                        subjectParams.Add("$subject", "");
                        Dictionary<string, string> bodyParams = new Dictionary<string, string>(3);
                        bodyParams.Add("$company", _currentCom.Name);
                        bodyParams.Add("$cusname", tmp.Name);
                        bodyParams.Add("$username", tmp.AccountName);
                        bodyParams.Add("$password", randompass);
                        bodyParams.Add("$portalLink", portalLink);
                        emailSrv.ProcessEmail(labelEmail, tmp.Email, "RegisterCustomer", subjectParams, bodyParams);
                    }
                }
                catch (Exception ex)
                { log.Error(ex); }
                return RedirectToAction("Index");
            }
            else
            {
                model.tmpCustomer = tmp;
                Messages.AddErrorMessage(ErrorMessage);
                log.Error(" Create  -:" + ErrorMessage);
                return View(model);
            }
        }

        //Sửa thông tin khách hàng
        [RBACAuthorize(Permissions = "Edit_cus")]
        public ActionResult Edit(int id)
        {
            ICustomerService cusSrv = IoC.Resolve<ICustomerService>();
            Customer oCus = cusSrv.Getbykey(id);
            CustomerModel model = new CustomerModel();
            model.tmpCustomer = oCus;
            try
            {
                //thong tin ve khach hang               
                model.SerialCert = oCus.SerialCert;                
                if (oCus.SerialCert != null)
                {
                    ICertificateService _cerSVC = IoC.Resolve<ICertificateService>();
                    Certificate cerQr = (from c in _cerSVC.Query where c.SerialCert.ToUpper().Contains(oCus.SerialCert.ToUpper()) && (c.ComID == oCus.ComID) select c).SingleOrDefault();
                    model.Cerid = cerQr.id;
                    model.serialcer = cerQr.SerialCert;
                }
            }
            catch (Exception ex)
            {
                log.Error(" Edit  -" + ex.Message);
            }
            return View(model);
        }
        [HttpPost]
        [RBACAuthorize(Permissions = "Edit_cus")]
        public ActionResult Edit(int id, CustomerModel model, string[] DeliverMethod)
        {
            if (id <= 0)
                throw new HttpRequestValidationException();
            ICustomerService cusSrv = IoC.Resolve<ICustomerService>();
            Company _currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            string ErroMessage = "";
            Customer oCus = cusSrv.Getbykey(id);
            try
            {
                TryUpdateModel<Customer>(oCus);
                if (!string.IsNullOrWhiteSpace(oCus.TaxCode))
                {
                    if (cusSrv.Query.Where(p => p.TaxCode.ToUpper() == oCus.TaxCode.ToUpper() && p.id != oCus.id).Count() > 0)
                    {
                        model.tmpCustomer = oCus;
                        Messages.AddErrorMessage("Mã số thuế đã tồn tại trên hệ thống!");
                        return View("Edit", model);
                    }
                    oCus.TaxCode = Utils.formatTaxcode(oCus.TaxCode);
                }
                if (DeliverMethod == null) oCus.DeliverMethod = -1;
                else if (DeliverMethod.Length == 1)
                {
                    if (DeliverMethod[0] == "0") oCus.DeliverMethod = 0;
                    else if (DeliverMethod[0] == "1") oCus.DeliverMethod = 1;
                }
                else if (DeliverMethod.Length == 2) oCus.DeliverMethod = 2;
                //end delivermethod
                Certificate cer = model.UpdateCertificate(new Certificate());
                if (cusSrv.UpdateCus(oCus, cer, _currentCom.id, out ErroMessage) == true)
                {
                    log.Info("Edit Customer by: " + HttpContext.User.Identity.Name + " Info-- TenKhachHang: " + oCus.Name + " TaiKhoanKhachHang: " + oCus.AccountName + " Email: " + oCus.Email);
                    Messages.AddFlashMessage(Resources.Message.Cus_UMesSuccess);
                }
                else
                {
                    model.tmpCustomer = oCus;
                    Messages.AddErrorMessage(ErroMessage);
                    return View(model);
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                model.tmpCustomer = oCus;
                Messages.AddErrorMessage("Có lỗi xảy ra, vui lòng thực hiện lại.");
                return View(model);
            }

        }
        //xóa khách hàng
        [RBACAuthorize(Permissions = "Del_cus")]
        public ActionResult Delete(int id)
        {
            if (id <= 0)
                throw new HttpRequestValidationException();
            string ErrorMessage = "";
            ICustomerService cusSrv = IoC.Resolve<ICustomerService>();
            if (cusSrv.DeleteCus(id, out ErrorMessage) == true)
            {
                log.Info("Delete Customer by: " + HttpContext.User.Identity.Name + " Info-- ID: " + id.ToString());
                Messages.AddFlashMessage("Xóa khách hàng thành công!");
            }
            else
            {
                Messages.AddErrorFlashMessage(ErrorMessage);
                log.Error("Delete -" + ErrorMessage);
            }
            return RedirectToAction("Index");
        }
        
        // Tìm kiếm tên của khách hàng(Auto)
        public JsonResult SeachByCusTaxCode(string searchText)
        {
            ICustomerService cusSrv = IoC.Resolve<ICustomerService>();
            Company _currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            var values = cusSrv.Query
                .Where(c =>
                    c.ComID == _currentCom.id &&
                    c.TaxCode.ToUpper().Contains(searchText.Trim().ToUpper()))
                .Select(c => new
                {
                    cusid = c.id,
                    cusname = c.Name,
                    TaxCode = c.TaxCode,
                    Address = c.Address,
                    Phone = c.Phone,
                    Code = c.Code,
                    CusBankNo = c.BankNumber,
                    CusBankName = c.BankName
                });
            return Json(values.Take(15), JsonRequestBehavior.AllowGet);
        }
        public JsonResult SeachByName(string searchText)
        {
            ICustomerService cusSrv = IoC.Resolve<ICustomerService>();
            Company _currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            var values = cusSrv.Query
                .Where(c =>
                    c.ComID == _currentCom.id &&
                    c.Name.ToUpper().Contains(searchText.Trim().ToUpper()))
                .Select(c => new
                {
                    cusid = c.id,
                    Name = c.Name,
                    TaxCode = c.TaxCode,
                    Address = c.Address,
                    Phone = c.Phone,
                    Code = c.Code,
                    CusBankNo = c.BankNumber,
                    CusBankName = c.BankName
                });
            return Json(values.Take(15), JsonRequestBehavior.AllowGet);
        }

        // Tìm kiếm theo max của khách hàng(Auto)
        public JsonResult SeachByCusCode(string searchText)
        {
            ICustomerService cusSrv = IoC.Resolve<ICustomerService>();
            Company _currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            var values1 = cusSrv.Query
                .Where(c =>
                    c.ComID == _currentCom.id
                    && (c.Name.ToUpper().Contains(searchText.Trim().ToUpper()) || c.Code.ToUpper().Contains(searchText.Trim().ToUpper())))
                .Select(c => new
                {
                    cusid = c.id,
                    cusname = c.Name,
                    TaxCode = c.TaxCode,
                    Address = c.Address,
                    Phone = c.Phone,
                    Code = c.Code,
                    CusBankNo = c.BankNumber,
                    CusBankName = c.BankName
                }).Take(15);

            return Json(values1, JsonRequestBehavior.AllowGet);
        }
        // Đưa ra thông tin khách hàng( Form nhập hóa đơn)
        public ActionResult GetDataById(string cusId)
        {
            ICustomerService cusSrv = IoC.Resolve<ICustomerService>();
            Company _currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            Customer qr = cusSrv.SearchID(Convert.ToInt32(cusId), _currentCom.id);
            if (qr == null)
                return null;
            return Json(new
            {
                TaxCode = qr.TaxCode ?? "",
                Address = qr.Address ?? "",
                Phone = qr.Phone ?? "",
                Code = qr.Code ?? ""
            }
            );
        }
    }
}
