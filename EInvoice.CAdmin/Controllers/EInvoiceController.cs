using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;
using EInvoice.Core.IService;
using EInvoice.Core.Domain;
using FX.Core;
using FX.Utils;
using EInvoice.Core;
using FX.Utils.MvcPaging;
using IdentityManagement.Authorization;
using EInvoice.CAdmin.Models;
using FX.Context;
using System.Collections;
namespace EInvoice.CAdmin.Controllers
{
    public class EInvoiceController : ShareController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EInvoiceController));
        //tìm kiếm và hiển thị danh sách hóa đơn
        [RBACAuthorize(Permissions = "Search_inv")]
        public ActionResult Index(EInvoiceIndexModel model, int? page, int? Pagesize)
        {
            IPublishInvoiceService _PubIn = IoC.Resolve<IPublishInvoiceService>();
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            try
            {
                IList<string> lstpattern = _PubIn.LstByPattern(currentCom.id, 1);
                if (lstpattern.Count == 0)
                {
                    Messages.AddErrorFlashMessage("Cần tạo thông báo phát hành.");
                    return RedirectToAction("Index", "Publish");
                }
                if (model == null) model = new EInvoiceIndexModel();
                model.Pattern = string.IsNullOrEmpty(model.Pattern) ? lstpattern[0] : model.Pattern;
                model.lstpattern = lstpattern;
                List<string> LstSerial = _PubIn.ListSerialByPattern(currentCom.id, model.Pattern, new int[] { 1, 2, 3 }).Distinct().ToList<string>();
                model.lstserial = LstSerial;
                int defautPagesize = Pagesize.HasValue ? Convert.ToInt32(Pagesize) : 10;
                int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                int totalRecords = 0;

                IInvoiceService IInvSrv = InvServiceFactory.GetService(model.Pattern, currentCom.id);
                IList<IInvoice> lst;
                if (!model.InvNo.HasValue || model.InvNo == null)
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
                    lst = IInvSrv.SearchByCustomer(currentCom.id, model.Pattern, model.Serial, DateFrom, DateTo, (InvoiceStatus)model.Status, model.nameCus, model.code, model.CodeTax, (InvoiceType)model.typeInvoice, currentPageIndex, defautPagesize, out totalRecords);
                }
                else
                {
                    lst = IInvSrv.GetListByNo(currentCom.id, model.Pattern, model.Serial, model.InvNo.Value);
                    totalRecords = lst.Count();
                }
                model.PageListINV = new PagedList<IInvoice>(lst, currentPageIndex, defautPagesize, totalRecords);
                int signPlugin = 0;
                if (currentCom.Config.Keys.Contains("SignPlugin"))
                    int.TryParse(currentCom.Config["SignPlugin"], out signPlugin);
                model.SignPlugin = signPlugin;
                return View(model);
            }
            catch (UnAuthorizedException e)
            {
                log.Error(e);
                Messages.AddErrorMessage("Có lỗi xảy ra, vui lòng thực hiện lại!");
                return RedirectToAction("Index", "Home");
            }
            catch (ArgumentException ex)
            {
                log.Error(ex);
                return Redirect("/Home/PotentiallyError");
            }
            catch (HttpRequestValidationException ex)
            {
                log.Error(ex);
                return RedirectToAction("PotentiallyError", "Home");
            }
        }

        //Tự chọn id của từng hóa đơn để phát hành
        [RBACAuthorize(Permissions = "Release_invInList")]
        [HttpPost]
        public ActionResult LaunchChoice(string[] cbid, string hdPattern, string Serial)
        {
            try
            {
                Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                int[] ids = (from s in cbid select Convert.ToInt32(s)).ToArray();
                if (ids.Length < 0)
                {
                    Messages.AddErrorFlashMessage("Bạn chưa chọn hóa đơn.");
                    return RedirectToAction("Index", new { Pattern = hdPattern, Serial = Serial });
                }
                string strResult = "";
                LaunchInvoices.Instance.Launch(ids, hdPattern, Serial, out strResult);

                if (strResult.Contains("OK"))
                {
                    log.Info("Publish EInvoice by: " + HttpContext.User.Identity.Name + " Info-- Pattern: " + hdPattern + "; Serial: " + Serial + "; SoLuongPhatHanh: " + ids.Length.ToString());
                    Messages.AddFlashMessage("Phát hành thành công: " + ids.Length + " hóa đơn");
                }
                else if (strResult.Contains("ERR:14"))
                {
                    log.Error("ERR:14 " + strResult);
                    Messages.AddErrorFlashMessage("Có lô hóa đơn khác đang được phát hành, xin vui lòng thực hiện lại.");
                }
                else
                {
                    log.Error("ERR: " + strResult);
                    Messages.AddErrorFlashMessage(strResult);
                }
                return RedirectToAction("Index", new { Pattern = hdPattern, Serial = Serial });
            }
            catch (Exception ex)
            {
                log.Error("Exception: " + ex);
                Messages.AddErrorFlashMessage("Có lỗi xảy ra, vui lòng thực hiện lại!");
                return RedirectToAction("Index", new { Pattern = hdPattern, Serial = Serial });
            }
        }

        //hiển thị form nhập thông tin để phát hành hóa đơn theo lô
        [RBACAuthorize(Permissions = "Release_invInTime")]
        public ActionResult Launch()
        {
            IPublishInvoiceService _PubIn = IoC.Resolve<IPublishInvoiceService>();
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            LaunchModel model = new LaunchModel();
            try
            {
                //lstpattern
                List<string> lstpattern = _PubIn.LstByPattern(currentCom.id, 1);
                if (lstpattern.Count == 0)
                {
                    Messages.AddErrorFlashMessage("Cần tạo thông báo phát hành.");
                    return RedirectToAction("Index", "Publish");
                }
                model.Listpattern = new SelectList(lstpattern);
                //lstserial
                List<string> oserial = (from s in _PubIn.Query where ((s.ComId == currentCom.id) && (s.Status == 1 || s.Status == 2)) select s.InvSerial).ToList<string>();
                model.Listserial = new SelectList(oserial);
            }
            catch (Exception ex)
            {
                Messages.AddErrorFlashMessage("Có lỗi xảy ra, vui lòng thực hiện lại.");
                log.Error(" Launch -" + ex.Message);
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        //phát hành hóa đơn theo lô
        [RBACAuthorize(Permissions = "Release_invInTime")]
        public bool LaunchCollectInvoice(string FromDate, string ToDate, string pattern, string serial)
        {
            DateTime? DateFrom = null;
            DateTime? DateTo = null;
            if (!string.IsNullOrWhiteSpace(FromDate)) DateFrom = DateTime.ParseExact(FromDate, "dd/MM/yyyy", null);
            if (!string.IsNullOrWhiteSpace(ToDate)) DateTo = DateTime.ParseExact(ToDate, "dd/MM/yyyy", null);
            try
            {
                Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                IInvoiceService IInvSrv = InvServiceFactory.GetService(pattern, currentCom.id);
                // chỉ tìm NewInv
                int totalRecords = 0;
                IList<IInvoice> listInvoice = IInvSrv.SearchInvoice(currentCom.id, pattern, serial, DateFrom, DateTo, 0, 0, out totalRecords, InvoiceStatus.NewInv);
                if (listInvoice.Count > 0)
                {
                    //edit phat hanh hoa don 03/07/2014
                    int[] ids = listInvoice.Select(c => c.id).ToArray();
                    string resultMessage = "";
                    LaunchInvoices.Instance.Launch(ids, pattern, serial, out resultMessage);
                    log.Info("Publish EInvoice by: " + HttpContext.User.Identity.Name + " Info-- Pattern: " + pattern + " Serial: " + serial + " SoLuongPhatHanh: " + listInvoice.Count.ToString());
                    if (resultMessage.Contains("OK"))
                        Messages.AddFlashMessage(resultMessage);
                    else if (resultMessage.Contains("ERR:14"))
                        Messages.AddErrorFlashMessage("Có lô hóa đơn khác đang được phát hành, xin vui lòng thực hiện lại.");
                    else
                        Messages.AddErrorFlashMessage(resultMessage);
                    log.Info("Launch Result: " + resultMessage);
                }
                else
                {
                    Messages.AddErrorFlashMessage("Không có hóa đơn mới tạo.");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Messages.AddErrorFlashMessage("Có lỗi xảy ra, vui lòng thực hiện lại.");
                log.Error(" LaunchCollectInvoice -:" + ex);
                return false;
            }
        }

        //Tạo mới hóa đơn
        [RBACAuthorize(Permissions = "Create_inv")]
        public ActionResult Create(string Pattern)
        {
            ICompanyService comSrv = IoC.Resolve<ICompanyService>();
            Company currentCom = comSrv.Getbykey(((EInvoiceContext)FXContext.Current).CurrentCompany.id);
            IPublishInvoiceService _PubIn = IoC.Resolve<IPublishInvoiceService>();
            IRegisterTempService _ReTemSvc = IoC.Resolve<IRegisterTempService>();
            string ViewName = InvServiceFactory.GetView(Pattern, currentCom.id) + "Create";
            IInvoice model = InvServiceFactory.NewInstance(Pattern, currentCom.id);
            model.Pattern = Pattern;
            model.ComID = currentCom.id;
            model.Name = _ReTemSvc.SeachNameInv(Pattern, currentCom.id);
            List<string> ser = _PubIn.GetSerialByPatter(Pattern, currentCom.id);
            if (ser.Count == 0)
            {
                Messages.AddErrorFlashMessage("Dải số đã hết hoặc đã bị hủy.");
                return RedirectToAction("Index", new { Pattern = Pattern });
            }
            ViewData["ser"] = ser;
            ViewData["company"] = currentCom;
            return View(ViewName, model);
        }

        [HttpPost]
        [RBACAuthorize(Permissions = "Create_inv")]
        public ActionResult Create(string Pattern, string PubDatasource)
        {
            ICompanyService comSrv = IoC.Resolve<ICompanyService>();
            Company currentCom = comSrv.Getbykey(((EInvoiceContext)FXContext.Current).CurrentCompany.id);
            IPublishInvoiceService _PubIn = IoC.Resolve<IPublishInvoiceService>();
            IRegisterTempService _ReTemSvc = IoC.Resolve<IRegisterTempService>();
            string ErrorMessage = "";
            IInvoice model = InvServiceFactory.NewInstance(Pattern, currentCom.id);
            IInvoiceService IInvSrv = InvServiceFactory.GetService(Pattern, currentCom.id);
            try
            {
                TryUpdateModelFromType(model.GetType(), model);
                model.CusCode = !string.IsNullOrWhiteSpace(model.CusCode) ? model.CusCode : model.CusTaxCode;
                var tem = InvServiceFactory.GetInvoiceType(Pattern, currentCom.id);
                if (!String.IsNullOrEmpty(PubDatasource))
                {
                    ICustomerService _CusSvc = FX.Core.IoC.Resolve<ICustomerService>();
                    var Typecus = (from c in _CusSvc.Query where c.Code == model.CusCode && c.CusType == 1 && c.ComID == currentCom.id select c.CusType).SingleOrDefault();
                    if (Typecus == 0)
                    {
                        model.CusSignStatus = cusSignStatus.NocusSignStatus;
                    }
                    else
                    {
                        model.CusSignStatus = cusSignStatus.NoSignStatus;
                    }
                    IList<ProductInv> lstproduct = (IList<ProductInv>)PubDatasource.DeserializeJSON<ProductInv>(typeof(IList<ProductInv>));
                    if (IInvSrv.CreateInvoice(lstproduct, model, out ErrorMessage) == true)
                    {
                        log.Info("Create EInvoice by: " + HttpContext.User.Identity.Name + " Info-- TenKhachHang: " + model.CusName + " MaKhachHang: " + model.CusCode + " SoTien: " + model.Amount);
                        Messages.AddFlashMessage("Tạo mới hóa đơn thành công.");
                        return RedirectToAction("Index", new { Pattern = Pattern, Serial = model.Serial });
                    }
                    else Messages.AddErrorMessage(ErrorMessage);
                    model.Products = (from pr in lstproduct select pr as IProductInv).ToList();
                }
                else Messages.AddErrorMessage("Chưa nhập thông tin sản phẩm, dịch vụ");
                List<string> ser = _PubIn.GetSerialByPatter(Pattern, currentCom.id);
                ViewData["ser"] = ser;
                ViewData["company"] = currentCom;
                string ViewName = InvServiceFactory.GetView(Pattern, currentCom.id) + "Create";
                return View(ViewName, model);
            }
            catch (HttpRequestValidationException ex)
            {
                log.Error("ArgumentException: " + ex);
                Messages.AddErrorMessage("Dữ liệu không hợp lệ hoặc có chứa mã gây nguy hiểm tiềm tàng cho hệ thống!");
                model = InvServiceFactory.NewInstance(Pattern, currentCom.id);
                List<string> ser = _PubIn.GetSerialByPatter(Pattern, currentCom.id);
                ViewData["ser"] = ser;
                ViewData["company"] = currentCom;
                string ViewName = InvServiceFactory.GetView(Pattern, currentCom.id) + "Create";
                return View(ViewName, model);
            }
            catch (Exception ex)
            {
                log.Error(" Create - " + ex);
                Messages.AddErrorMessage("Có lỗi xảy ra, vui lòng thực hiện lại!");
                List<string> ser = _PubIn.GetSerialByPatter(Pattern, currentCom.id);
                ViewData["ser"] = ser;
                ViewData["company"] = currentCom;
                if (!String.IsNullOrEmpty(PubDatasource))
                {
                    IList<ProductInv> lstproduct = (IList<ProductInv>)PubDatasource.DeserializeJSON<ProductInv>(typeof(IList<ProductInv>));
                    model.Products = (from pr in lstproduct select pr as IProductInv).ToList();
                }
                string ViewName = InvServiceFactory.GetView(Pattern, currentCom.id) + "Create";
                return View(ViewName, model);
            }
        }

        //chỉnh sửa hóa đơn chưa phát hành
        [RBACAuthorize(Permissions = "Edit_inv")]
        public ActionResult Edit(string Pattern, int id)
        {
            ICompanyService comSrv = IoC.Resolve<ICompanyService>();
            Company currentCom = comSrv.Getbykey(((EInvoiceContext)FXContext.Current).CurrentCompany.id);
            IPublishInvoiceService _PubIn = IoC.Resolve<IPublishInvoiceService>();
            IRegisterTempService _ReTemSvc = IoC.Resolve<IRegisterTempService>();
            string ViewName = InvServiceFactory.GetView(Pattern, currentCom.id) + "Edit";
            //khoi tao service
            IInvoiceService IInvSrv = InvServiceFactory.GetService(Pattern, currentCom.id);
            //lay ve mot ban ghi hoa don 
            IInvoice model = IInvSrv.Getbykey<IInvoice>(id);
            if (model.Status != InvoiceStatus.NewInv)
            {
                Messages.AddErrorFlashMessage("Không thể sửa hóa đơn này.");
                return RedirectToAction("Index", new { Pattern = Pattern, Serial = model.Serial });
            }
            //lay va doi danh sach cac san pham thanh doi tuong json
            //nếu Unit=null thì mặc định hiển thị ""
            foreach (var item in model.Products)
            {
                if (item.Unit == null)
                {
                    item.Unit = "";
                }
            }
            //lay ra danh sach cac serial
            List<string> ser = _PubIn.GetSerialByPatter(model.Pattern, currentCom.id);
            ViewData["ser"] = ser;
            //lay thong tin ve don vi ban hang
            ViewData["company"] = currentCom;
            model.Note = "";
            return View(ViewName, model);
        }

        [HttpPost]
        [RBACAuthorize(Permissions = "Edit_inv")]
        public ActionResult Edit(string Pattern, int id, string PubDatasource)
        {
            if (id <= 0)
                throw new HttpRequestValidationException();
            ICompanyService comSrv = IoC.Resolve<ICompanyService>();
            Company currentCom = comSrv.Getbykey(((EInvoiceContext)FXContext.Current).CurrentCompany.id);
            IPublishInvoiceService _PubIn = IoC.Resolve<IPublishInvoiceService>();
            IRegisterTempService _ReTemSvc = IoC.Resolve<IRegisterTempService>();
            IInvoiceService IInvSrv = InvServiceFactory.GetService(Pattern, currentCom.id);
            IInvoice model = IInvSrv.Getbykey<IInvoice>(id);
            string NoteOrder = model.Note;
            try
            {
                string ErrorMessage = "";
                TryUpdateModelFromType(model.GetType(), model);
                model.CusCode = !string.IsNullOrWhiteSpace(model.CusCode) ? model.CusCode : model.CusTaxCode;
                model.Note = NoteOrder + " || " + model.Note;
                if (!String.IsNullOrEmpty(PubDatasource) && PubDatasource != "[]")
                {
                    ICustomerService _CusSvc = FX.Core.IoC.Resolve<ICustomerService>();
                    var Typecus = (from c in _CusSvc.Query where c.Code == model.CusCode && c.CusType == 1 select c.CusType).FirstOrDefault();
                    if (Typecus == 0)
                    {
                        model.CusSignStatus = cusSignStatus.NocusSignStatus;
                    }
                    else
                    {
                        model.CusSignStatus = cusSignStatus.NoSignStatus;
                    }
                    IList<ProductInv> lstproduct = (IList<ProductInv>)PubDatasource.DeserializeJSON<ProductInv>(typeof(IList<ProductInv>));

                    if (IInvSrv.UpdateInvoice(lstproduct, model, out ErrorMessage) == true)
                    {
                        log.Info("Edit EInvoice by: " + HttpContext.User.Identity.Name + " Info-- TenKhachHang: " + model.CusName + " MaKhachHang: " + model.CusCode + " SoTien: " + model.Amount);
                        Messages.AddFlashMessage("Cập nhật hóa đơn thành công.");
                        return RedirectToAction("Index", new { Pattern = Pattern, Serial = model.Serial });
                    }
                    else Messages.AddErrorMessage(ErrorMessage);
                    model.Products = (from pr in lstproduct select pr as IProductInv).ToList();
                }
                else Messages.AddErrorMessage("Chưa nhập thông tin sản phẩm");
                model.Name = _ReTemSvc.SeachNameInv(Pattern, currentCom.id);
                List<string> ser = _PubIn.GetSerialByPatter(model.Pattern, currentCom.id);
                ViewData["ser"] = ser;
                ViewData["company"] = currentCom;
                string ViewName = InvServiceFactory.GetView(Pattern, currentCom.id) + "Edit";
                return View(ViewName, model);
            }
            catch (HttpRequestValidationException ex)
            {
                log.Error("ArgumentException: " + ex);
                Messages.AddErrorMessage("Dữ liệu không hợp lệ hoặc có chứa mã gây nguy hiểm tiềm tàng cho hệ thống!");
                model = IInvSrv.Getbykey<IInvoice>(id);
                List<string> ser = _PubIn.GetSerialByPatter(Pattern, currentCom.id);
                ViewData["ser"] = ser;
                ViewData["company"] = currentCom;
                string ViewName = InvServiceFactory.GetView(Pattern, currentCom.id) + "Edit";
                return View(ViewName, model);
            }
            catch (Exception ex)
            {
                log.Error("Edit -", ex);
                model.Name = _ReTemSvc.SeachNameInv(Pattern, currentCom.id);
                List<string> ser = _PubIn.GetSerialByPatter(model.Pattern, currentCom.id);
                ViewData["ser"] = ser;
                ViewData["company"] = currentCom;
                Messages.AddErrorMessage("Có lỗi xảy ra, vui lòng thực hiện lại");
                if (!String.IsNullOrEmpty(PubDatasource))
                {
                    IList<ProductInv> lstproduct = (IList<ProductInv>)PubDatasource.DeserializeJSON<ProductInv>(typeof(IList<ProductInv>));
                    model.Products = (from pr in lstproduct select pr as IProductInv).ToList();
                }
                string ViewName = InvServiceFactory.GetView(Pattern, currentCom.id) + "Edit";
                return View(ViewName, model);
            }
        }

        [HttpPost]
        [RBACAuthorize(Permissions = "Edit_inv")]
        public ActionResult UpdateCoupon(string Pattern, int id)
        {
            if (id <= 0)
                throw new HttpRequestValidationException();
            ICompanyService comSrv = IoC.Resolve<ICompanyService>();
            Company currentCom = comSrv.Getbykey(((EInvoiceContext)FXContext.Current).CurrentCompany.id);
            IPublishInvoiceService _PubIn = IoC.Resolve<IPublishInvoiceService>();
            IRegisterTempService _ReTemSvc = IoC.Resolve<IRegisterTempService>();
            IInvoiceService IInvSrv = InvServiceFactory.GetService(Pattern, currentCom.id);
            IInvoice model = IInvSrv.Getbykey<IInvoice>(id);
            string NoteOrder = model.Note;
            try
            {
                TryUpdateModelFromType(model.GetType(), model);
                model.Note = NoteOrder + " || " + model.Note;
                IInvSrv.Update(model);
                IInvSrv.CommitChanges();
                log.Info("Edit EInvoice by: " + HttpContext.User.Identity.Name + " Info-- TenKhachHang: " + model.CusName + " MaKhachHang: " + model.CusCode + " SoTien: " + model.Amount);
                Messages.AddFlashMessage("Sửa phiếu thu thành công.");
                return RedirectToAction("Index", new { Pattern = Pattern, Serial = model.Serial });
            }
            catch (HttpRequestValidationException ex)
            {
                log.Error("ArgumentException: " + ex);
                Messages.AddErrorMessage("Dữ liệu không hợp lệ hoặc có chứa mã gây nguy hiểm tiềm tàng cho hệ thống!");
                model = IInvSrv.Getbykey<IInvoice>(id);
                List<string> ser = _PubIn.GetSerialByPatter(Pattern, currentCom.id);
                ViewData["ser"] = ser;
                ViewData["company"] = currentCom;
                string ViewName = InvServiceFactory.GetView(Pattern, currentCom.id) + "Edit";
                return View(ViewName, model);
            }
            catch (Exception ex)
            {
                log.Error("Edit -", ex);
                model.Name = _ReTemSvc.SeachNameInv(Pattern, currentCom.id);
                List<string> ser = _PubIn.GetSerialByPatter(model.Pattern, currentCom.id);
                ViewData["ser"] = ser;
                ViewData["company"] = currentCom;
                Messages.AddErrorMessage("Có lỗi xảy ra, vui lòng thực hiện lại.");
                string ViewName = InvServiceFactory.GetView(Pattern, currentCom.id) + "Edit";
                return View(ViewName, model);
            }
        }
        //xóa hóa đơn chưa phát hành
        [RBACAuthorize(Permissions = "Del_inv")]
        public ActionResult Delete(string Pattern, int id)
        {
            if (id <= 0)
                throw new HttpRequestValidationException();
            log.Debug("Access - Delete");
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IInvoiceService IInvSrv = InvServiceFactory.GetService(Pattern, currentCom.id);
            IInvoice model = IInvSrv.Getbykey<IInvoice>(id);
            try
            {
                string ErrorMessage = "";
                if (IInvSrv.DeleteInvoice(model, out ErrorMessage) == true)
                {
                    log.Info("Delete EInvoice by: " + HttpContext.User.Identity.Name + " Info-- ID: " + model.id + "Pattern: " + model.Pattern + " Serial: " + model.Serial + " SoHoaDon: " + model.No.ToString() + " TenKhachHang: " + model.CusName + " MaKhachHang: " + model.CusCode + " SoTien: " + model.Amount);
                    Messages.AddFlashMessage("Xóa hóa đơn thành công.");
                    log.Info("Delete successfull id =" + id);
                }
                else
                {
                    Messages.AddErrorFlashMessage(ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                log.Error(" Delete-" + ex.Message);
            }
            return RedirectToAction("Index", new { Pattern = Pattern, Serial = model.Serial });
        }
        //lấy danh sách các serial từ pattern
        //status=1,status=2
        public ActionResult GetSerialByPatter(string opattern)
        {
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IPublishInvoiceService _PubIn = IoC.Resolve<IPublishInvoiceService>();
            var pu = (from c in _PubIn.Query where (c.InvPattern == opattern) && c.ComId == currentCom.id && (c.Status == 1 || c.Status == 2) select c.InvSerial).Distinct().ToList();
            return Json(new
            {
                pu
            });
        }
        //lấy danh sách các serial từ pattern
        //status!=0
        public ActionResult GetSerial(string Pattern)
        {
            IPublishInvoiceService _PubIn = IoC.Resolve<IPublishInvoiceService>();
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            var pu = _PubIn.LstBySerial(currentCom.id, Pattern);
            return Json(new
            {
                pu
            });
        }
        //lấy danh sách thông tin khách hàng từ id khách hàng
        public ActionResult ajx(int id)
        {
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            ICustomerService _CusSvc = IoC.Resolve<ICustomerService>();
            Customer ic = (from c in _CusSvc.Query where (c.id == id && c.ComID == currentCom.id) select c).Single();
            return Json(new
            {
                ic.Name,
                ic.TaxCode,
                ic.Address,
                ic.BankNumber,
                ic.BankName,
            }
            );
        }
        #region
        //viet ghi chu trong hoa don
        [HttpGet]
        public ActionResult WriteNote(string Pattern, int id, string TypeView)
        {
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IInvoiceService IInvSrv = InvServiceFactory.GetService(Pattern, currentCom.id);
            IInvoice Einv = IInvSrv.Getbykey<IInvoice>(id);
            //thong tin view model
            WriteNoteModel model = new WriteNoteModel();
            model.id = id;
            model.pattern = Pattern;
            model.TypeView = TypeView;
            if (string.IsNullOrEmpty(Einv.Note)) model.Note = "";
            else model.Note = Einv.Note;
            return View(model);
        }
        [HttpPost]
        public ActionResult WriteNote(WriteNoteModel model)
        {
            if (model.id <= 0)
                throw new HttpRequestValidationException();
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IInvoiceService IInvSrv = InvServiceFactory.GetService(model.pattern, currentCom.id);
            IInvoice EInvoiceEditNote = IInvSrv.Getbykey<IInvoice>(model.id);
            try
            {
                EInvoiceEditNote.Note = model.Note;
                IInvSrv.Save(EInvoiceEditNote);
                IInvSrv.CommitChanges();
                log.Info("Writenote EInvoice by: " + HttpContext.User.Identity.Name);
                Messages.AddFlashMessage("Ghi chú thành công!");
            }
            catch (Exception ex)
            {
                log.Error(" WriteNote-" + ex.Message);
                Messages.AddErrorFlashMessage("Ghi chú không thành công!");
            }
            if (model.TypeView == "0") return RedirectToAction("Index", new { Pattern = model.pattern, Serial = EInvoiceEditNote.Serial });
            else if (model.TypeView == "1")
                return RedirectToAction("ReplaceInvIndex", "Adjust", new { pattern = model.pattern, Serial = EInvoiceEditNote.Serial });
            else
                return RedirectToAction("AdjustInvIndex", "Adjust", new { pattern = model.pattern, Serial = EInvoiceEditNote.Serial });

        }
        #endregion
    }
}
