using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EInvoice.Core.Domain;
using EInvoice.Core.IService;
using EInvoice.Core;
using FX.Core;
using FX.Utils;
using System.IO;
using FX.Utils.MVCMessage;
using FX.Utils.MvcPaging;
using IdentityManagement.Authorization;
using log4net;
using EInvoice.CAdmin.Models;
using System.Collections;
using System.Web;
using EInvoice.CAdmin.IService;
using FX.Context;
using Newtonsoft.Json;
namespace EInvoice.CAdmin.Controllers
{
    [MessagesFilter]
    public class AdJustController : ShareController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AdJustController));
        private readonly IRecordsInvService _recordsCancelSrv;
        private readonly IAdjustInvService _adjustInvSrv;
        public AdJustController()
        {
            _adjustInvSrv = IoC.Resolve<IAdjustInvService>();
            _recordsCancelSrv = IoC.Resolve<IRecordsInvService>();
        }
        #region Danh sách hóa đơn Thay thế
        [RBACAuthorize(Permissions = "Search_Replace")]
        public ActionResult ReplaceInvIndex(AdjustInvoiceModel model, int? page)
        {
            IPublishInvoiceService PubIn = IoC.Resolve<IPublishInvoiceService>();
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            List<string> lstpattern = PubIn.LstByPattern(currentCom.id, 2);
            if (lstpattern.Count == 0)
            {
                Messages.AddErrorFlashMessage(Resources.Message.MInv_SMesNoPubAccess);
                return Redirect("/");
            }
            model.lstPattern = new SelectList(lstpattern);
            if (string.IsNullOrEmpty(model.pattern) && lstpattern.Count > 0) model.pattern = lstpattern[0];
            //lay serial
            List<string> LstSerial = (from p in PubIn.Query where p.Status != 0 && p.InvPattern == model.pattern && p.ComId == currentCom.id select p.InvSerial).Distinct().ToList<string>();
            model.lstSerial = new SelectList(LstSerial);
            if (string.IsNullOrEmpty(model.Serial))
            {
                model.Serial = LstSerial[0];
            }
            model.ComId = currentCom.id;
            //phân trang
            int defautPagesize = 10;
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            int totalcount;
            decimal InvNo = model.InvNo.HasValue ? model.InvNo.Value : 0;
            DateTime? DateFrom = null;
            DateTime? DateTo = null;
            if (!string.IsNullOrWhiteSpace(model.FromDate)) DateFrom = DateTime.ParseExact(model.FromDate, "dd/MM/yyyy", null);
            if (!string.IsNullOrWhiteSpace(model.ToDate)) DateTo = DateTime.ParseExact(model.ToDate, "dd/MM/yyyy", null);
            //IList<AdjustInv> lst = _adjustInvSrv.GetAdjustInv(model.ComId, model.pattern, model.Serial, DateFrom, DateTo, InvNo, ProcessingStatus.ReplacedInv, model.nameCus, model.code, model.CodeTax);
            //model.PageListAdjustInv = new PagedList<AdjustInv>(lst.OrderByDescending(e => e.InvId).ToList(), currentPageIndex, defautPagesize, lst.Count());
            IList<AjustSearchModel> lst = _adjustInvSrv.SearchAdjust(model.ComId, model.pattern, model.Serial, DateFrom, DateTo, InvNo, ProcessingStatus.ReplacedInv, model.nameCus, model.code, model.CodeTax, currentPageIndex, defautPagesize, out totalcount);
            model.PageListAdjustSearch = new PagedList<AjustSearchModel>(lst, currentPageIndex, defautPagesize, totalcount);
            ViewData["Title1"] = "Hóa đơn bị thay thế";
            ViewData["Title2"] = "Hóa đơn thay thế";
            ViewData["TypeView"] = 1;
            return View(model);
        }
        #endregion
        #region Danh sách hóa đơn chỉnh sửa
        [RBACAuthorize(Permissions = "Search_Adjust")]
        public ActionResult AdjustInvIndex(AdjustInvoiceModel model, int? page)
        {
            IPublishInvoiceService PubIn = IoC.Resolve<IPublishInvoiceService>();
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            List<string> lstpattern = PubIn.LstByPattern(currentCom.id, 2);
            if (lstpattern.Count == 0)
            {
                Messages.AddErrorFlashMessage(Resources.Message.MInv_SMesNoPubAccess);
                return Redirect("/");
            }
            model.lstPattern = new SelectList(lstpattern);
            if (string.IsNullOrEmpty(model.pattern) && lstpattern.Count > 0) model.pattern = lstpattern[0];
            //lay serial
            List<string> LstSerial = (from p in PubIn.Query where p.Status != 0 && p.InvPattern == model.pattern && p.ComId == currentCom.id select p.InvSerial).Distinct().ToList<string>();
            model.lstSerial = new SelectList(LstSerial);
            if (string.IsNullOrEmpty(model.Serial))
            {
                model.Serial = LstSerial[0];
            }
            // call service
            model.ComId = currentCom.id;
            //phân trang
            int defautPagesize = 10;
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            int totalcount;
            decimal InvNo = model.InvNo.HasValue ? model.InvNo.Value : 0;
            DateTime? DateFrom = null;
            DateTime? DateTo = null;
            if (!string.IsNullOrWhiteSpace(model.FromDate)) DateFrom = DateTime.ParseExact(model.FromDate, "dd/MM/yyyy", null);
            if (!string.IsNullOrWhiteSpace(model.ToDate)) DateTo = DateTime.ParseExact(model.ToDate, "dd/MM/yyyy", null);
            //IList<AdjustInv> lst = _adjustInvSrv.GetAdjustInv(model.ComId, model.pattern, model.Serial, DateFrom, DateTo, InvNo, ProcessingStatus.AdjustedInv, model.nameCus, model.code, model.CodeTax);
            IList<AjustSearchModel> lst = _adjustInvSrv.SearchAdjust(model.ComId, model.pattern, model.Serial, DateFrom, DateTo, InvNo, ProcessingStatus.AdjustedInv, model.nameCus, model.code, model.CodeTax, currentPageIndex, defautPagesize, out totalcount);
            model.PageListAdjustSearch = new PagedList<AjustSearchModel>(lst, currentPageIndex, defautPagesize, totalcount);
            ViewData["Title1"] = "Hóa đơn bị điều chỉnh";
            ViewData["Title2"] = "Hóa đơn điều chỉnh";
            ViewData["TypeView"] = 2;
            return View(model);
        }
        #endregion

        #region "Hóa đơn thay thế"
        //b1: Tìm kiếm hóa một hóa đơn bằng thông tin(pattern,serial,số hóa đơn)
        //b2: Tạo ra một actionResult=>view là form nhập hóa đơn cần thay thế(pattern,serial,đơn vị phát hành, khách hàng)
        //b3: Phát hành hóa đơn mới và lưu vào cơ sở dữ liệu
        //end
        [RBACAuthorize(Permissions = "Edit_ajst")]
        public ActionResult SearchReplaceInv()
        {
            try
            {
                AdjustModel model = new AdjustModel();
                IPublishInvoiceService PubIn = IoC.Resolve<IPublishInvoiceService>();
                Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                IList<string> lstpattern = PubIn.LstByPattern(currentCom.id, 2);
                model.lstpattern = new SelectList(lstpattern);
                model.lstserial = new SelectList("");
                model.currentcom = currentCom;
                return View(model);
            }
            catch (Exception ex)
            {
                Messages.AddErrorFlashMessage("Có lỗi xảy ra, vui lòng thực hiện lại!");
                log.Error(" SearchReplaceInv-:" + ex.Message);
                return RedirectToAction("Index", "Home");
            }
        }

        private int getTaxDeclaration(int comId, string pattern, string serial, string invNo)
        {
            int result = -2;
            IPublishInvoiceService PubIn = IoC.Resolve<IPublishInvoiceService>();

            string latestTaxDeclarationDate = PubIn.GetLatestTaxDeclarationDate();

            if(latestTaxDeclarationDate != null)
            {
                string createDateByFKey = PubIn.GetArisingDateByinvNo(comId, pattern, serial, invNo);
                DateTime latestTaxDeclarationDateTime = System.Convert.ToDateTime(latestTaxDeclarationDate);
                DateTime createDateFKey = System.Convert.ToDateTime(createDateByFKey);

                result = DateTime.Compare(createDateFKey, latestTaxDeclarationDateTime);
            }

            return result;


        }

        [RBACAuthorize(Permissions = "Create_ajst")]
        [HttpGet]
        public ActionResult CreateReplaceInv(AdjustModel model)
        {
            //khai báo service
            try
            {
                IPublishInvoiceService PubIn = IoC.Resolve<IPublishInvoiceService>();
                ICompanyService comSrv = IoC.Resolve<ICompanyService>();
                Company currentCom = comSrv.Getbykey(((EInvoiceContext)FXContext.Current).CurrentCompany.id);
                IInvoiceService IInvSrv = InvServiceFactory.GetService(model.pattern, currentCom.id);
                string ErrorMessage = "";
                string newPattern = null, newSerial = null;

                int result = getTaxDeclaration(currentCom.id, model.pattern, model.serial, model.invNo);

                if (result < 0 && result != -2)
                {
                    log.Error("Error CreateReplaceInv! Hóa đơn này đã chốt kê khai thuế, vì vậy không thể thực hiện thay thế. Hãy thực hiện nghiệp vụ điều chỉnh hóa đơn ");
                    Messages.AddErrorFlashMessage("Hóa đơn này đã chốt kê khai thuế, vì vậy không thể thực hiện thay thế. Hãy thực hiện nghiệp vụ điều chỉnh hóa đơn");
                    return RedirectToAction("SearchReplaceInv", new { pattern = model.pattern, serial = model.serial });
                }

                //kiểm tra số hóa đơn của giải còn hay hết
                if (!LaunchInvoices.Instance.ExistNoInPubInv(currentCom.id, model.pattern, model.serial, out newPattern, out newSerial, out ErrorMessage))
                {
                    Messages.AddErrorFlashMessage(ErrorMessage);
                    return RedirectToAction("SearchReplaceInv", new { pattern = model.pattern, serial = model.serial });
                }
                //Lấy ra đối tượng hóa đơn cần thay thế
                IInvoice inv = IInvSrv.GetByNo(currentCom.id, model.pattern, model.serial, Convert.ToDecimal(model.invNo));
                //kiểm tra trạng thái thái hóa đơn
                //trạng thái hóa đơn thay thế
                if (inv.Status != InvoiceStatus.ReplacedInv && inv.Status != InvoiceStatus.CanceledInv && inv.Status != InvoiceStatus.AdjustedInv)
                {
                    model.serial = newSerial ?? model.serial;
                    model.pattern = newPattern ?? model.pattern;
                    ViewData["NewPattern"] = model.pattern;
                    ViewData["NewSerial"] = model.serial;
                    ViewData["Data"] = inv;
                    ViewData["company"] = currentCom;
                    int signPlugin = 0;
                    if (currentCom.Config.Keys.Contains("SignPlugin"))
                        int.TryParse(currentCom.Config["SignPlugin"], out signPlugin);
                    ViewData["SignPlugin"] = signPlugin;
                    if (inv.Pattern != model.pattern)
                    {
                        IInvoice invNew = InvServiceFactory.NewInstance(model.pattern, currentCom.id);
                        invNew.ComID = currentCom.id;
                        invNew.Name = inv.Name;
                        invNew.CusName = inv.CusName;
                        invNew.CusCode = inv.CusCode;
                        invNew.CusAddress = inv.CusAddress;
                        invNew.Buyer = inv.Buyer;
                        invNew.No = inv.No;
                        invNew.Serial = inv.Serial;
                        invNew.Note = "";
                        invNew.Type = InvoiceType.ForReplace;
                        invNew.PaymentMethod = inv.PaymentMethod;
                        invNew.Products = inv.Products;
                        invNew.Pattern = inv.Pattern;
                        invNew.Amount = inv.Amount;
                        invNew.Total = inv.Total;
                        invNew.VATAmount = inv.VATAmount;
                        string adjustView = InvServiceFactory.GetView(model.pattern, currentCom.id) + "CreateReplaceInv";
                        return View(adjustView, invNew);
                    }
                    inv.Note = "";
                    inv.Type = InvoiceType.ForReplace;
                    string ViewName = InvServiceFactory.GetView(model.pattern, currentCom.id) + "CreateReplaceInv";
                    return View(ViewName, inv);
                }
                else
                {
                    Messages.AddErrorFlashMessage("Hóa đơn đã được điều chỉnh hoặc bị hủy");
                    return RedirectToAction("SearchReplaceInv", model);
                }
            }
            catch (Exception ex)
            {
                log.Error("CreateReplaceInv -:" + ex.Message);
                Messages.AddErrorFlashMessage("Không tồn tại hóa đơn này!");
                return RedirectToAction("SearchReplaceInv", new { pattern = model.pattern, serial = model.serial });
            }

        }
        [HttpPost]
        [RBACAuthorize(Permissions = "Create_ajst")]
        public ActionResult CreateReplaceInv(string NewPattern, string NewSerial, string Pattern, string PubDatasource, string NoInv, string SerialNo, HttpPostedFileBase FileUpload, string NoPayment, string Payment)
        {
            ICompanyService _comSrv = IoC.Resolve<ICompanyService>();
            Company currentCom = _comSrv.Getbykey(((EInvoiceContext)FXContext.Current).CurrentCompany.id);
            //user currentUser = ((EInvoiceContext)FXContext.Current).CurrentUser;
            ICustomerService _CusSvc = IoC.Resolve<ICustomerService>();
            InvoiceBase model = (InvoiceBase)InvServiceFactory.NewInstance(NewPattern, currentCom.id);
            TryUpdateModelFromType(model.GetType(), model);
            model.Pattern = NewPattern;
            model.Serial = NewSerial;
            try
            {
                IInvoiceService IInvSrv = InvServiceFactory.GetService(Pattern, currentCom.id);
                IInvoice inv = IInvSrv.GetByNo(currentCom.id, Pattern, SerialNo, Convert.ToDecimal(NoInv));
                IList<ProductInv> lst = (IList<ProductInv>)PubDatasource.DeserializeJSON<ProductInv>(typeof(IList<ProductInv>));
                if (lst.Count() == 0)
                {
                    Messages.AddErrorFlashMessage(Resources.Message.MRequestProd);
                    log.Error("AdJustController CreateReplaceInv  Error5:" + Resources.Message.MRequestProd);
                    return RedirectToAction("CreateReplaceInv", new { Pattern = Pattern, Serial = SerialNo, invNo = NoInv });
                }
                var Typecus = (from c in _CusSvc.Query where c.Code == model.CusCode && c.CusType == 1 && c.ComID == currentCom.id select c.CusType).SingleOrDefault();
                if (Typecus == 0)
                {
                    model.CusSignStatus = cusSignStatus.NocusSignStatus;
                }
                else
                {
                    model.CusSignStatus = cusSignStatus.NoSignStatus;
                }
                if (inv.Status == InvoiceStatus.SignedInv)
                {
                    //launcher inv
                    if (lst != null && lst.Count > 0) model.Products = (from pr in lst select pr as IProductInv).ToList();
                    string xmldata = string.Empty;
                    string message = string.Empty;
                    string resultLauncher = string.Empty;
                    model.Fkey = "";
                    model.id = 0;
                    string strPath = "";
                    if (FileUpload != null)
                    {
                        string fileext = (Path.GetExtension(FileUpload.FileName).Length > 1) ? Path.GetExtension(FileUpload.FileName).Substring(1) : "";
                        if (fileext.ToLower() == "docx" || fileext.ToLower() == "pdf" || fileext.ToLower() == "doc")
                        {
                            string strFullPath = "";
                            strPath = @"\RecordsInv\" + currentCom.id + @"\" + Pattern.Replace("/", "") + @"\" + SerialNo.Replace("/", "") + "_" + NoInv + "." + fileext;
                            strFullPath = GetFullPathRecordsCancel(strPath);
                            FileUpload.SaveAs(strFullPath);
                        }
                    }
                    model.Type = InvoiceType.ForReplace;

                    ILauncherService _launcher = IoC.Resolve(Type.GetType(currentCom.Config["LauncherType"])) as ILauncherService;
                    _launcher.PublishReplace(inv, lst, model, strPath);
                    resultLauncher = _launcher.Message;
                    if (resultLauncher.Contains("OK:"))
                    {
                        Messages.AddFlashMessage(Resources.Message.AdjReInv_MReplaceSuccess);
                    }
                    else
                    {
                        Messages.AddErrorFlashMessage(StringErrorService(resultLauncher));
                    }
                }
            }
            catch (Exception ex)
            {
                Messages.AddErrorFlashMessage("Có lỗi xảy ra, vui lòng thực hiện lại!");
                log.Error("CreateReplaceInv  -: " + ex);
            }
            return RedirectToAction("ReplaceInvIndex", new { pattern = Pattern, Serial = SerialNo });
        }
        #endregion

        #region Điều chỉnh hóa đơn
        //b1: Tìm kiếm một hóa đơn bằng thông tin(pattern,serial,số hóa đơn)
        //b2: Tạo ra một actionResult=>view là form nhập hóa đơn cần điều chỉnh(pattern,serial,đơn vị phát hành, khách hàng)
        //b3: Phát hành hóa đơn mới và lưu vào cơ sở dữ liệu
        //end
        [RBACAuthorize(Permissions = "Edit_ajst")]
        public ActionResult SearchAdJustInv()
        {
            try
            {
                AdjustModel model = new AdjustModel();
                IPublishInvoiceService PubIn = IoC.Resolve<IPublishInvoiceService>();
                Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                IList<string> lstpattern = PubIn.LstByPattern(currentCom.id, 2);
                model.lstpattern = new SelectList(lstpattern);
                model.lstserial = new SelectList("");
                model.currentcom = currentCom;
                return View(model);
            }
            catch (Exception ex)
            {
                log.Error(" SearchAdJustInv -:" + ex);
                Messages.AddErrorFlashMessage("Có lỗi xảy ra, vui lòng thực hiện lại!");
                return RedirectToAction("Index", "Home");
            }

        }
        [RBACAuthorize(Permissions = "Create_ajst")]
        [HttpGet]
        public ActionResult CreateAdJustInv(AdjustModel model)
        {
            try
            {
                ICompanyService comSrv = IoC.Resolve<ICompanyService>();
                Company currentCom = comSrv.Getbykey(((EInvoiceContext)FXContext.Current).CurrentCompany.id);
                IPublishInvoiceService PubIn = IoC.Resolve<IPublishInvoiceService>();

                IInvoiceService IInvSrv = InvServiceFactory.GetService(model.pattern, currentCom.id);
                string ErrorMessage = "";
                string newPattern = null, newSerial = null;

                int result = getTaxDeclaration(currentCom.id, model.pattern, model.serial, model.invNo);

                if (result > 0 && result != -2)
                {
                    log.Error("Error CreateReplaceInv! Hóa đơn này chưa chốt kê khai thuế, vì vậy không thể thực hiện điều chỉnh. Hãy thực hiện nghiệp vụ thay thế hóa đơn!");
                    Messages.AddErrorFlashMessage("Hóa đơn này chưa chốt kê khai thuế, vì vậy không thể thực hiện điều chỉnh. Hãy thực hiện nghiệp vụ thay thế hóa đơn!");
                    return RedirectToAction("SearchAdJustInv", new { pattern = model.pattern, serial = model.serial });
                }

                //kiểm tra số hóa đơn của giải còn hay hết
                if (!LaunchInvoices.Instance.ExistNoInPubInv(currentCom.id, model.pattern, model.serial, out newPattern, out newSerial, out ErrorMessage))
                {
                    Messages.AddErrorFlashMessage(ErrorMessage);
                    return RedirectToAction("SearchAdJustInv", new { pattern = model.pattern, serial = model.serial });
                }
                
                //Lấy ra đối tượng hóa đơn cần thay thế
                //Hóa đơn đã thay thế và kê khai thuế thì không được chỉnh sửa
                IInvoice inv = IInvSrv.GetByNo(currentCom.id, model.pattern, model.serial, Convert.ToDecimal(model.invNo));
                if (inv.Status == InvoiceStatus.ReplacedInv || inv.Status == InvoiceStatus.CanceledInv)
                {
                    Messages.AddErrorFlashMessage("Hóa đơn đã bị thay thế hoặc bị hủy!");
                    return RedirectToAction("SearchAdJustInv");
                }
                int type = 0;
                int.TryParse(model.type, out type);
                model.pattern = newPattern ?? model.pattern;
                model.serial = newSerial ?? model.serial;
                ViewData["NewPattern"] = model.pattern;
                ViewData["NewSerial"] = model.serial;
                ViewData["Data"] = inv;
                ViewData["company"] = currentCom;
                ViewData["type"] = model.type;
                //them dong chu tang hay giam                 
                ViewData["typeName"] = model.typeName;
                int signPlugin = 0;
                if (currentCom.Config.Keys.Contains("SignPlugin"))
                    int.TryParse(currentCom.Config["SignPlugin"], out signPlugin);
                ViewData["SignPlugin"] = signPlugin;
                if (inv.Pattern != model.pattern)
                {
                    IInvoice invNew = InvServiceFactory.NewInstance(model.pattern, currentCom.id);
                    invNew.ComID = currentCom.id;
                    invNew.Name = inv.Name;
                    invNew.CusName = inv.CusName;
                    invNew.CusCode = inv.CusCode;
                    invNew.CusAddress = inv.CusAddress;
                    invNew.Buyer = inv.Buyer;
                    invNew.No = inv.No;
                    invNew.Serial = inv.Serial;
                    invNew.Note = "";
                    invNew.PaymentMethod = inv.PaymentMethod;
                    invNew.Products = inv.Products;
                    invNew.Pattern = inv.Pattern;
                    invNew.Type = (InvoiceType)type;
                    invNew.Amount = inv.Amount;
                    invNew.Total = inv.Total;
                    invNew.VATAmount = inv.VATAmount;
                    string adjustView = InvServiceFactory.GetView(model.pattern, currentCom.id) + "CreateAdJustInv";
                    return View(adjustView, invNew);
                }
                inv.Note = "";
                inv.Type = (InvoiceType)type;
                string ViewName = InvServiceFactory.GetView(model.pattern, currentCom.id) + "CreateAdJustInv";
                return View(ViewName, inv);
            }
            catch (Exception ex)
            {
                log.Error(" CreateAdJustInv -:" + ex);
                Messages.AddErrorFlashMessage("Không tồn tại hóa đơn này!");
                return RedirectToAction("SearchAdJustInv");
            }
        }
        [RBACAuthorize(Permissions = "Create_ajst")]
        [HttpPost]
        public ActionResult CreateAdJustInv(string NewPattern, string NewSerial, string Pattern, string PubDatasource, string NoInv, string SerialNo, string type, HttpPostedFileBase FileUpload)
        {
            ICompanyService _comSrv = IoC.Resolve<ICompanyService>();
            Company currentCom = _comSrv.Getbykey(((EInvoiceContext)FXContext.Current).CurrentCompany.id);
            ICustomerService _CusSvc = IoC.Resolve<ICustomerService>();
            InvoiceBase model = (InvoiceBase)InvServiceFactory.NewInstance(NewPattern, currentCom.id);
            TryUpdateModelFromType(model.GetType(), model);
            model.Pattern = NewPattern;
            model.Serial = NewSerial;
            model.ComID = currentCom.id;
            IInvoiceService IInvSrv = InvServiceFactory.GetService(Pattern, currentCom.id);
            IInvoice inv = IInvSrv.GetByNo(currentCom.id, Pattern, SerialNo, Convert.ToDecimal(NoInv));
            IList<ProductInv> lst = (IList<ProductInv>)PubDatasource.DeserializeJSON<ProductInv>(typeof(IList<ProductInv>));
            if (lst.Count() == 0)
            {
                Messages.AddErrorFlashMessage(Resources.Message.MRequestProd);
                return RedirectToAction("CreateAdJustInv", new { Pattern = Pattern, Serial = SerialNo, invNo = NoInv });
            }
            try
            {
                var Typecus = (from c in _CusSvc.Query where c.Code == model.CusCode && c.CusType == 1 && c.ComID == currentCom.id select c.CusType).SingleOrDefault();
                if (Typecus == 0) model.CusSignStatus = cusSignStatus.NocusSignStatus;
                else model.CusSignStatus = cusSignStatus.NoSignStatus;
                if ((inv.Status == InvoiceStatus.SignedInv || inv.Status == InvoiceStatus.InUseInv || inv.Status == InvoiceStatus.AdjustedInv))// =1 hoa don dieu chinh
                {

                    if (lst != null && lst.Count > 0) model.Products = (from pr in lst select pr as IProductInv).ToList();
                    string xmldata = string.Empty;
                    string message = string.Empty;
                    string resultLauncher = string.Empty;
                    model.Fkey = "";
                    model.id = 0;
                    model.Type = (InvoiceType)Convert.ToInt32(type);
                    //save file and get path
                    string strPath = "";
                    if (FileUpload != null)
                    {
                        string fileext = (Path.GetExtension(FileUpload.FileName).Length > 1) ? Path.GetExtension(FileUpload.FileName).Substring(1) : "";
                        if (fileext.ToLower() == "docx" || fileext.ToLower() == "pdf" || fileext.ToLower() == "doc")
                        {
                            strPath = @"\RecordsInv\" + currentCom.id + @"\" + Pattern.Replace("/", "") + @"\" + SerialNo.Replace("/", "") + "_" + NoInv + "." + fileext;
                            string strFullPath = GetFullPathRecordsCancel(strPath);
                            FileUpload.SaveAs(strFullPath);
                        }
                    }
                    ILauncherService _launcher = IoC.Resolve(Type.GetType(currentCom.Config["LauncherType"])) as ILauncherService;
                    _launcher.PublishAdjust(inv, lst, model, strPath);
                    resultLauncher = _launcher.Message;
                    if (resultLauncher.Contains("OK:"))
                    {
                        Messages.AddFlashMessage("Điều chỉnh hóa đơn thành công.");
                    }
                    else
                    {
                        Messages.AddErrorFlashMessage(StringErrorService(resultLauncher));
                    }
                }
            }
            catch (Exception ex)
            {
                Messages.AddErrorFlashMessage("Có lỗi xảy ra, vui lòng thực hiện lại.");
                log.Error(" CreateAdJustInv -: " + ex);
            }
            return RedirectToAction("AdjustInvIndex", new { pattern = Pattern, Serial = SerialNo });
        }
        #endregion

        #region "Ajax Doi trang thai hoa don"
        /// <summary>
        /// doi trang thai hoa don
        /// status=1 den status=3
        /// </summary>
        /// <param name="pat"></param>
        /// <param name="seri"></param>
        /// <param name="invNo"></param>
        /// <returns></returns>
        [HttpPost]
        [RBACAuthorize]
        public ActionResult searchAdjust(string pat, string seri, string invNo)
        {
            try
            {
                Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                IInvoiceService IInvSrv = InvServiceFactory.GetService(pat, currentCom.id);
                decimal No;
                decimal.TryParse(invNo, out No); if (No == 0) No = -1;
                IInvoice inv = IInvSrv.GetByNo(currentCom.id, pat, seri, Convert.ToDecimal(No));
                if (inv.Status == InvoiceStatus.SignedInv)
                {
                    return Json(new
                    {
                        inv.Pattern,
                        inv.Serial
                    });
                }
                else
                {
                    return Json("");
                }
            }
            catch (Exception ex)
            {
                log.Error(" searchAdjust -:" + ex.Message);
                Messages.AddErrorFlashMessage(Resources.Message.AdjReInv_MInvalidInv);
                return RedirectToAction("SearchInvReplace");
            }
        }
        /// <summary>
        /// Doi trang thai hoa don
        /// status=2 den status=4
        /// </summary>
        /// <param name="pat"></param>
        /// <param name="seri"></param>
        /// <param name="invNo"></param>
        /// <returns></returns>        
        public ActionResult adInv(string pat, string seri, string invNo)
        {
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IInvoiceService IInvSrv = InvServiceFactory.GetService(pat, currentCom.id);
            try
            {
                //sua lai
                decimal No;
                decimal.TryParse(invNo, out No); if (No == 0) No = -1;
                IInvoice inv = IInvSrv.GetByNo(currentCom.id, pat, seri, Convert.ToDecimal(No));
                if (inv.Status == InvoiceStatus.SignedInv)
                {
                    return Json(new
                    {
                        inv.Pattern,
                        inv.Serial
                    });
                }
                else
                {
                    return Json("");
                }
            }
            catch (Exception ex)
            {
                log.Error(" adInv -:" + ex.Message);
                Messages.AddErrorFlashMessage(Resources.Message.AdjReInv_MInvalidInv);
                return RedirectToAction("SearchInvAdjust");
            }

        }
        public ActionResult getserial(string pattern)
        {
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IPublishInvoiceService PubIn = IoC.Resolve<IPublishInvoiceService>();
            var q = (from p in PubIn.Query where (p.ComId == currentCom.id) && (p.Status == 2 || p.Status == 1 || p.Status == 3) && (p.InvPattern == pattern) select p.InvSerial).Distinct();
            return Json(new { q });
        }
        #endregion

        #region function other
        /// <summary>
        /// Kiểm tra các giải hóa đơn còn hay hết số hóa đơn có cùng(pattern,serial)
        /// </summary>
        /// <param name="ComID"></param>
        /// <param name="pattern"></param>
        /// <param name="serial"></param>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>

        [RBACAuthorize]
        public ActionResult ajxPage()
        {
            ViewData["pt"] = TempData["pt"];
            ViewData["sr"] = TempData["sr"];
            return View();
        }

        public ActionResult seq()
        {
            return Json(TempData["str"]);
        }
        private string GetFullPathRecordsCancel(string path)
        {
            try
            {
                string fullPath = Server.MapPath("~") + path;
                string parentDir = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(parentDir))
                {
                    Directory.CreateDirectory(parentDir);
                }
                return fullPath;
            }
            catch (Exception ex)
            {
                log.Error("AdJustController  loi tao duong dan file Van ban huy hoa don:" + ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// Tải văn bản hủy hóa đơn đính kèm
        /// </summary>
        /// <param name="id">ID của hóa đơn bị thay thế</param>
        /// <returns></returns>
        public ActionResult DownloadRecordsCancel(int id)
        {
            try
            {
                AdjustInv obj = _adjustInvSrv.Getbykey(id);
                if (obj != null)
                {
                    if (!string.IsNullOrEmpty(obj.Attachefile))
                    {
                        string extension = Path.GetExtension(obj.Attachefile);
                        string contentType = "application/pdf";
                        if (extension.Equals("docx") || extension.Equals("doc"))
                        {
                            contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        }
                        if (System.IO.File.Exists(GetFullPathRecordsCancel(obj.Attachefile)))
                            return File(GetFullPathRecordsCancel(obj.Attachefile), contentType, Path.GetFileName(obj.Attachefile));
                    }
                    Messages.AddErrorFlashMessage("Không tồn tại văn bản hủy đính kèm hóa đơn !");
                    return RedirectToAction("Index", "Home");
                }
                Messages.AddErrorFlashMessage("Không tồn tại văn bản hủy đính kèm hóa đơn !");

            }
            catch (Exception ex)
            {
                log.Error(" DownloadRecordsCancel -:" + ex.ToString());
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }
        /// <summary>
        /// Đưa ra lỗi từ webservice từ mã lỗi trong quá trinh thay thế chỉnh sửa
        /// </summary>
        /// <param name="resultLauncher"></param>
        /// <returns></returns>
        private string StringErrorService(string resultLauncher)
        {
            string[] CodeError = resultLauncher.Split(' ');
            string Error = CodeError[0].ToString();
            switch (Error)
            {
                case "ERR:1":
                    {
                        return "Bạn không có quyền!";
                    }
                case "ERR:2":
                    {
                        return "Không tồn tại hóa đơn!";
                    }
                case "ERR:3":
                    {
                        return "Định dạng dữ liệu xml không đúng:" + resultLauncher.Substring(5, resultLauncher.Length - 5);
                    }
                case "ERR:5":
                    {
                        return "Lỗi phát hành hóa đơn";
                    }
                case "ERR:6":
                    {
                        return "Hết số hóa đơn trong giải!";
                    }
                case "ERR:8":
                    {
                        return "Hóa đơn đã được thay thế rồi!";
                    }
                case "ERR:9":
                    {
                        return "Trạng thái hóa đơn không đúng";
                    }

                case "ERR:14":
                    {
                        return "Có lô hóa đơn khác đang được phát hành, xin vui lòng thực hiện lại.";
                    }

                default:
                    {
                        return resultLauncher;
                    }
            }
        }
        #endregion

        #region Serialization einvoice to xml and save file report
        private bool uploadfile(HttpPostedFileBase FileUpload, string idInv, string Pattern, string SerialNo, string NoInv, Company currentCom, string resultLauncher, out string mess)
        {
            try
            {
                if (FileUpload != null)
                {
                    bool check = false;
                    //get adjustid
                    string[] listResultLaunchera = resultLauncher.Split(';');
                    string[] listResultLauncherb = listResultLaunchera[2].ToString().Split('_');
                    IInvoiceService IInvSrv = InvServiceFactory.GetService(Pattern, currentCom.id);
                    IInvoice invoiceNew = IInvSrv.GetByNo(currentCom.id, Pattern, listResultLaunchera[1].ToString(), Convert.ToDecimal(Convert.ToInt32(listResultLauncherb[1].ToString().Replace(',', ' '))));
                    int idadjust = (from a in IoC.Resolve<IAdjustInvService>().Query where a.InvId == Convert.ToInt32(idInv) && a.AdjustInvId == invoiceNew.id select a.id).SingleOrDefault();
                    //end
                    RecordsInv tbRecordsInv = IoC.Resolve<IRecordsInvService>().Query.Where(idAdjust => idAdjust.id == Convert.ToInt32(idadjust)).FirstOrDefault();
                    IRecordsInvService _SvcRecordsInv = IoC.Resolve<IRecordsInvService>();
                    string strPath = "";
                    string strFullPath = "";
                    string fileext = (Path.GetExtension(FileUpload.FileName).Length > 1) ? Path.GetExtension(FileUpload.FileName).Substring(1) : "";
                    if (fileext.ToLower() != "docx" && fileext.ToLower() != "pdf" && fileext.ToLower() != "doc")
                    {
                        mess = "File biên bản hủy đính kèm không đúng định dạng !";
                        return check;
                    }
                    strPath = @"\RecordsInv\" + currentCom.id + @"\" + Pattern.Replace("/", "") + @"\" + SerialNo.Replace("/", "") + "_" + NoInv + "." + fileext;
                    strFullPath = GetFullPathRecordsCancel(strPath);
                    if (!string.IsNullOrEmpty(strFullPath))
                    {
                        tbRecordsInv.InvPattern = Pattern;
                        tbRecordsInv.Path = strPath;
                    }
                    _SvcRecordsInv.Update(tbRecordsInv);
                    _SvcRecordsInv.CommitTran();
                    if (!string.IsNullOrEmpty(strFullPath))
                    {
                        FileUpload.SaveAs(strFullPath);
                        mess = "Upload biên bản thành công";
                    }
                    else
                    {
                        mess = "Không upload biên bản thành công!";
                    }
                    return true;
                }
                else
                {
                    mess = "Không tồn tại file update!";
                    return false;
                }

            }
            catch (Exception ex)
            {
                mess = string.Format("Attach file RecordCancel: Pattern={0}, Serial={1}, NoInv={2}, Exception:{3}", Pattern, SerialNo, NoInv, ex.ToString());
                return false;
            }

        }
        #endregion

        #region "Update Note"
        //private void UpdateNode(IInvoice invoiceVat, IInvoiceService IInvSrv, int style)
        //{
        //    if (style == 0)
        //    {
        //        invoiceVat.Note += "||   Hóa đơn bị điều chỉnh:  bởi: " + HttpContext.User.Identity.Name + "    Thời gian điều chỉnh:" + DateTime.Now.ToString();
        //    }
        //    else
        //    {
        //        invoiceVat.Note += "||   Hóa đơn bị thay thế:  bởi: " + HttpContext.User.Identity.Name + " Thời gian thay thế: " + DateTime.Now.ToString();
        //    }
        //    IInvSrv.Update(invoiceVat);
        //    IInvSrv.CommitChanges();
        //}

        private void UpdateNode(int comid, string pattern, string serial, string no, int style)
        {
            IInvoiceService _IInvService = InvServiceFactory.GetService(pattern, comid);
            IInvoice Oriinv = _IInvService.GetByNo(comid, pattern, serial, Convert.ToDecimal(no));

            if (style == 0)
            {
                Oriinv.Note += "||   Hóa đơn bị điều chỉnh:  bởi: " + HttpContext.User.Identity.Name + "    Thời gian điều chỉnh:" + DateTime.Now.ToString();
            }
            else
            {
                Oriinv.Note += "||   Hóa đơn bị thay thế:  bởi: " + HttpContext.User.Identity.Name + " Thời gian thay thế: " + DateTime.Now.ToString();
            }
            _IInvService.Update(Oriinv);
            _IInvService.CommitChanges();
        }
        #endregion
    }
}
