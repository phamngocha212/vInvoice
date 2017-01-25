using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EInvoice.Core.Domain;
using EInvoice.Core.IService;
using EInvoice.Core;
using FX.Core;
using FX.Utils;
using FX.Utils.MVCMessage;
using FX.Utils.MvcPaging;
using IdentityManagement.Authorization;
using System.Xml.Linq;
using log4net;
using EInvoice.CAdmin.Models;
using System.Web.Script.Serialization;
using System.Xml;
using EInvoice.Core.Viewer;
using System.Text;
using System.Web;
using FX.Context;
namespace EInvoice.CAdmin.Controllers
{
    public class PublishController : ShareController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PublishController));

        #region "Publish"
        [RBACAuthorize(Permissions = "Search_pub")]
        public ActionResult Index(int? page, PubIndexModels model)
        {
            Company currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;            
            int curentPageIndex = page.HasValue ? page.Value - 1 : 0;
            int pageSize = 10;
            int totalRecords = 0;

            DateTime DateFrom = String.IsNullOrEmpty(model.fromdate) ? DateTime.MinValue : DateTime.ParseExact(model.fromdate, "dd/MM/yyyy", null);
            DateTime DateTo = String.IsNullOrEmpty(model.todate) ? DateTime.MaxValue : DateTime.ParseExact(model.todate, "dd/MM/yyyy", null);
            if (DateFrom != null && DateTo != null && DateFrom > DateTo)
            {
                Messages.AddErrorMessage("Nhập đúng dữ liệu tìm kiếm theo ngày!");
                DateFrom = DateTime.MinValue;
                DateTo = DateTime.MaxValue;
                model.fromdate = model.todate = "";
            }
            int pstatus = (String.IsNullOrEmpty(model.status) ? -1 : Convert.ToInt32(model.status));

            IPublishService pubSrc = IoC.Resolve<IPublishService>();            
            IList<Publish> lst = pubSrc.SearchByDate(currentCompany.id, DateFrom, DateTo, pstatus, curentPageIndex, pageSize, out totalRecords);

            model.PageListPUB = new PagedList<Publish>(lst, curentPageIndex, pageSize, totalRecords);
            return View(model);
        }

        //xem thông tin chi tiết thông báo phát hành
        [RBACAuthorize(Permissions = "Detail_pub")]
        public ActionResult DetailRPublish(int id)
        {
            if (id <= 0)
                throw new HttpRequestValidationException();
            IPublishService pubSrc = IoC.Resolve<IPublishService>();
            Publish model = pubSrc.Getbykey(id);
            IPublishInvoiceService pubinvSrc = IoC.Resolve<IPublishInvoiceService>();
            var qr = (from pi in pubinvSrc.Query where pi.PublishID == model.id select pi);
            IList<PublishInvoice> lstpubinv = qr.ToList();
            ViewData["lstpubinv"] = lstpubinv;
            return View(model);
        }

        //Tạo thông báo phát hành
        [RBACAuthorize(Permissions = "Add_pub")]
        public ActionResult CreateRPublish()
        {
            Company _currentcompany = IoC.Resolve<ICompanyService>().Getbykey(((EInvoiceContext)FXContext.Current).CurrentCompany.id);
            PublishModel model = new PublishModel();
            ITaxAuthorityService taxSrc = IoC.Resolve<ITaxAuthorityService>();
            IRegisterTempService regisSrc = IoC.Resolve<IRegisterTempService>();
            model.TaxList = new SelectList(from tax in taxSrc.Query select tax, "Code", "Name");
            model.RegTempList = new SelectList(from re in regisSrc.Query where re.ComId == _currentcompany.id select re, "Id", "Name");
            Publish mPub = new Publish();
            mPub.ComID = _currentcompany.id;
            mPub.ComName = _currentcompany.Name;
            mPub.ComAddress = _currentcompany.Address;
            mPub.ComTaxCode = _currentcompany.TaxCode;
            mPub.ComPhone = _currentcompany.Phone;
            mPub.RepresentPerson = _currentcompany.RepresentPerson;
            mPub.City = "Hà Nội";
            model.mPublish = mPub;
            model.PubInvoiceList = "[]";
            return View(model);
        }
        [HttpPost]
        [RBACAuthorize(Permissions = "Add_pub")]
        public ActionResult CreateRPublish(Publish mPub, string PubInvoiceList)
        {
            IPublishService pubSrc = IoC.Resolve<IPublishService>();
            ITaxAuthorityService taxSrv = IoC.Resolve<ITaxAuthorityService>();
            ICompanyService _comSrv = IoC.Resolve<ICompanyService>();
            string message = "";
            try
            {
                mPub.TaxAuthorityName = (from tax in taxSrv.Query where tax.Code == mPub.TaxAuthorityCode select tax.Name).FirstOrDefault();
                JavaScriptSerializer jss = new JavaScriptSerializer();
                IList<PublishInvoice> lst = jss.Deserialize<IList<PublishInvoice>>(PubInvoiceList);
                Company _currentcompany = _comSrv.Getbykey(((EInvoiceContext)FXContext.Current).CurrentCompany.id);
                foreach (var it in lst)
                {
                    IPublishInvoiceService pubinvSrv = IoC.Resolve<IPublishInvoiceService>();
                    string invSer = string.Format("{0}/{1}E", it.InvSerialPrefix, it.InvSerialSuffix);
                    var old = pubinvSrv.Query.Where(p => p.RegisterID == it.RegisterID && p.InvSerial == invSer && p.StartDate > it.StartDate && p.ComId == _currentcompany.id).FirstOrDefault();
                    if (old != null)
                    {
                        Messages.AddErrorMessage(string.Format("Thông báo có ký hiệu {0}, phải có ngày bắt đầu sau ngày: {1}", old.InvSerial, old.StartDate.ToString("dd/MM/yyyy")));
                        PublishModel model = new PublishModel();
                        model.mPublish = mPub;
                        ITaxAuthorityService taxSrc = IoC.Resolve<ITaxAuthorityService>();
                        model.TaxList = new SelectList(from tax in taxSrc.Query select tax, "Code", "Name", mPub.TaxAuthorityCode);
                        IRegisterTempService regisSrc = IoC.Resolve<IRegisterTempService>();
                        model.RegTempList = new SelectList(from re in regisSrc.Query where re.ComId == _currentcompany.id select re, "Id", "Name");
                        model.PubInvoiceList = lst.SerializeJSON<PublishInvoice>();
                        return View(mPub);
                    }

                    var pub = pubinvSrv.Query.Where(p => p.RegisterID == it.RegisterID && p.InvSerial == invSer && p.FromNo == it.FromNo && p.ComId == _currentcompany.id);
                    if (pub.Count() > 0)
                    {
                        Messages.AddErrorMessage(string.Format("Đã tồn tại giải hóa đơn tương ứng ký hiệu {0}", it.InvSerial));
                        PublishModel model = new PublishModel();
                        model.mPublish = mPub;
                        ITaxAuthorityService taxSrc = IoC.Resolve<ITaxAuthorityService>();
                        model.TaxList = new SelectList(from tax in taxSrc.Query select tax, "Code", "Name", mPub.TaxAuthorityCode);
                        IRegisterTempService regisSrc = IoC.Resolve<IRegisterTempService>();
                        model.RegTempList = new SelectList(from re in regisSrc.Query where re.ComId == _currentcompany.id select re, "Id", "Name");
                        model.PubInvoiceList = lst.SerializeJSON<PublishInvoice>();
                        return View(mPub);
                    }
                }

                if (pubSrc.CreateNew(mPub, lst, out message) == true)
                {
                    _currentcompany.IsUsed = true;
                    _comSrv.Update(_currentcompany);
                    _comSrv.CommitChanges();
                    StringBuilder InvInfo = new StringBuilder();
                    for (int i = 0; i < lst.Count; i++)
                    {
                        InvInfo.AppendFormat("{0};{1};{2};{3};{4}_", lst[i].RegisterID, lst[i].InvSerial, lst[i].Quantity, lst[i].FromNo, lst[i].ToNo);
                    }
                    log.Info("Create Publish by: " + HttpContext.User.Identity.Name + "|InvInfo:" + InvInfo.ToString());
                    Messages.AddFlashMessage("Tạo phát hành thành công!");
                    return RedirectToAction("Index");
                }
                else
                {
                    log.Error("Create Publish:" + message);
                    Messages.AddErrorMessage("Có lỗi xảy ra, vui lòng thực hiện lại!");
                    PublishModel model = new PublishModel();
                    model.mPublish = mPub;
                    ITaxAuthorityService taxSrc = IoC.Resolve<ITaxAuthorityService>();
                    model.TaxList = new SelectList(from tax in taxSrc.Query select tax, "Code", "Name", mPub.TaxAuthorityCode);
                    IRegisterTempService regisSrc = IoC.Resolve<IRegisterTempService>();
                    model.RegTempList = new SelectList(from re in regisSrc.Query where re.ComId == _currentcompany.id select re, "Id", "Name");
                    model.PubInvoiceList = lst.SerializeJSON<PublishInvoice>();
                    return View(mPub);
                }
            }
            catch (Exception ex)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                IList<PublishInvoice> lst = jss.Deserialize<IList<PublishInvoice>>(PubInvoiceList);
                log.Error(" CreateRPublish -" + ex);
                Messages.AddErrorMessage("Có lỗi xảy ra, vui lòng thực hiện lại.");
                Company _currentcompany = _comSrv.Getbykey(((EInvoiceContext)FXContext.Current).CurrentCompany.id);
                PublishModel model = new PublishModel();
                model.mPublish = mPub;
                ITaxAuthorityService taxSrc = IoC.Resolve<ITaxAuthorityService>();
                model.TaxList = new SelectList(from tax in taxSrc.Query select tax, "Code", "Name", mPub.TaxAuthorityCode);
                IRegisterTempService regisSrc = IoC.Resolve<IRegisterTempService>();
                model.RegTempList = new SelectList(from re in regisSrc.Query where re.ComId == _currentcompany.id select re, "Id", "Name");
                model.PubInvoiceList = lst.SerializeJSON<PublishInvoice>();
                return View(mPub);
            }
        }

        //Sửa thông báo phát hành
        [RBACAuthorize(Permissions = "Edit_pub")]
        public ActionResult EditRPublish(int id)
        {
            IPublishService pubSrc = IoC.Resolve<IPublishService>();
            Publish opub = pubSrc.Getbykey(id);
            if (opub.Status != PublishStatus.InUse)
            {
                PublishModel model = new PublishModel();
                Company _currentcompany = IoC.Resolve<ICompanyService>().Getbykey(((EInvoiceContext)FXContext.Current).CurrentCompany.id);

                ITaxAuthorityService taxSrc = IoC.Resolve<ITaxAuthorityService>();
                model.TaxList = new SelectList(from tax in taxSrc.Query select tax, "Code", "Name", opub.TaxAuthorityCode);
                IRegisterTempService regisSrc = IoC.Resolve<IRegisterTempService>();
                model.RegTempList = new SelectList(from re in regisSrc.Query where re.ComId == _currentcompany.id select re, "Id", "Name");

                model.mPublish = opub;
                model.PubInvoiceList = opub.PublishInvoices.SerializeJSON<PublishInvoice>();
                return View(model);
            }
            else
            {
                Messages.AddErrorFlashMessage("Thông báo phát hành này không được phép sửa");
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [RBACAuthorize(Permissions = "Edit_pub")]
        public ActionResult UpdatePublish(int id, string PubInvoiceList)
        {
            if (id <= 0)
                throw new HttpRequestValidationException();
            IPublishService pubSrc = IoC.Resolve<IPublishService>();
            ITaxAuthorityService taxSrv = IoC.Resolve<ITaxAuthorityService>();
            Publish opub = pubSrc.Getbykey(id);
            try
            {
                TryUpdateModel<Publish>(opub);
                opub.TaxAuthorityName = (from tax in taxSrv.Query where tax.Code == opub.TaxAuthorityCode select tax.Name).FirstOrDefault();
                JavaScriptSerializer jss = new JavaScriptSerializer();
                IList<PublishInvoice> lst = jss.Deserialize<IList<PublishInvoice>>(PubInvoiceList);
                foreach (var it in lst)
                {
                    IPublishInvoiceService pubinvSrv = IoC.Resolve<IPublishInvoiceService>();
                    ICompanyService _comSrv = IoC.Resolve<ICompanyService>();
                    Company _currentcompany = _comSrv.Getbykey(((EInvoiceContext)FXContext.Current).CurrentCompany.id);
                    string invSer = string.Format("{0}/{1}E", it.InvSerialPrefix, it.InvSerialSuffix);
                    var old = pubinvSrv.Query.Where(p => p.RegisterID == it.RegisterID && p.InvSerial == invSer && p.StartDate > it.StartDate && p.FromNo < it.FromNo && p.ComId == _currentcompany.id).FirstOrDefault();
                    if (old != null)
                    {
                        Messages.AddErrorMessage(string.Format("Thông báo có ký hiệu {0}, phải có ngày bắt đầu từ ngày: {1}", old.InvSerial, old.StartDate.ToString("dd/MM/yyyy")));
                        PublishModel model = new PublishModel();
                        model.mPublish = opub;
                        ITaxAuthorityService taxSrc = IoC.Resolve<ITaxAuthorityService>();
                        model.TaxList = new SelectList(from tax in taxSrc.Query select tax, "Code", "Name", opub.TaxAuthorityCode);
                        IRegisterTempService regisSrc = IoC.Resolve<IRegisterTempService>();
                        model.RegTempList = new SelectList(from re in regisSrc.Query where re.ComId == _currentcompany.id select re, "Id", "Name");
                        model.PubInvoiceList = opub.PublishInvoices.SerializeJSON<PublishInvoice>();
                        return View("EditRPublish", model);
                    }
                    else
                    {
                        old = pubinvSrv.Query.Where(p => p.RegisterID == it.RegisterID && p.InvSerial == invSer && p.StartDate < it.StartDate && p.FromNo > it.FromNo && p.ComId == _currentcompany.id).FirstOrDefault();
                        if (old != null)
                        {
                            Messages.AddErrorMessage(string.Format("Thông báo có ký hiệu {0}, phải có ngày bắt đầu nhỏ hơn ngày: {1}", old.InvSerial, old.StartDate.ToString("dd/MM/yyyy")));
                            PublishModel model = new PublishModel();
                            model.mPublish = opub;
                            ITaxAuthorityService taxSrc = IoC.Resolve<ITaxAuthorityService>();
                            model.TaxList = new SelectList(from tax in taxSrc.Query select tax, "Code", "Name", opub.TaxAuthorityCode);
                            IRegisterTempService regisSrc = IoC.Resolve<IRegisterTempService>();
                            model.RegTempList = new SelectList(from re in regisSrc.Query where re.ComId == _currentcompany.id select re, "Id", "Name");
                            model.PubInvoiceList = opub.PublishInvoices.SerializeJSON<PublishInvoice>();
                            return View("EditRPublish", model);
                        }
                    }
                }
                string mess = "";
                if (pubSrc.Update(opub, lst, out mess) == true)
                {
                    StringBuilder InvInfo = new StringBuilder();
                    for (int i = 0; i < lst.Count; i++)
                    {
                        InvInfo.AppendFormat("{0};{1};{2};{3};{4}_", lst[i].RegisterID, lst[i].InvSerial, lst[i].Quantity, lst[i].FromNo, lst[i].ToNo);
                    }
                    log.Info("Edit Publish by: " + HttpContext.User.Identity.Name + "|InvInfo:" + InvInfo.ToString());
                    Messages.AddFlashMessage("Sửa thành công");
                    return RedirectToAction("Index");
                }
                else
                {
                    log.Error("Update Publish:" + mess);
                    ICompanyService _comSrv = IoC.Resolve<ICompanyService>();
                    Company _currentcompany = _comSrv.Getbykey(((EInvoiceContext)FXContext.Current).CurrentCompany.id);
                    PublishModel model = new PublishModel();
                    model.mPublish = opub;
                    ITaxAuthorityService taxSrc = IoC.Resolve<ITaxAuthorityService>();
                    model.TaxList = new SelectList(from tax in taxSrc.Query select tax, "Code", "Name", opub.TaxAuthorityCode);
                    IRegisterTempService regisSrc = IoC.Resolve<IRegisterTempService>();
                    model.RegTempList = new SelectList(from re in regisSrc.Query where re.ComId == _currentcompany.id select re, "Id", "Name");
                    model.PubInvoiceList = opub.PublishInvoices.SerializeJSON<PublishInvoice>();
                    Messages.AddErrorMessage(mess);
                    return View("EditRPublish", model);
                }
            }
            catch (Exception ex)
            {
                log.Error("EditRPublish -" + ex);
                Messages.AddErrorMessage("Có lỗi xảy ra, vui lòng thực hiện lại!");
                PublishModel model = new PublishModel();
                model.mPublish = opub;
                ICompanyService _comSrv = IoC.Resolve<ICompanyService>();
                Company _currentcompany = _comSrv.Getbykey(((EInvoiceContext)FXContext.Current).CurrentCompany.id);
                ITaxAuthorityService taxSrc = IoC.Resolve<ITaxAuthorityService>();
                model.TaxList = new SelectList(from tax in taxSrc.Query select tax, "Code", "Name", opub.TaxAuthorityCode);
                IRegisterTempService regisSrc = IoC.Resolve<IRegisterTempService>();
                model.RegTempList = new SelectList(from re in regisSrc.Query where re.ComId == _currentcompany.id select re, "Id", "Name");
                model.PubInvoiceList = opub.PublishInvoices.SerializeJSON<PublishInvoice>();
                return View("EditRPublish", model);
            }
        }
        //Xóa thông báo phát hành
        [RBACAuthorize(Permissions = "Del_pub")]
        public ActionResult DeleteRPublish(int id)
        {
            if (id <= 0)
                throw new HttpRequestValidationException();
            string message = "";
            IPublishService pubSrc = IoC.Resolve<IPublishService>();
            if (pubSrc.DeletePub(id, out message) == true)
            {
                log.Info("Delete Publish by: " + HttpContext.User.Identity.Name + " Info-- ID: " + id.ToString());
                Messages.AddFlashMessage("Xóa thành công!");
            }
            else
            {
                log.Error("Delete Publish:" + message);
                Messages.AddErrorFlashMessage("Có lỗi xảy ra, vui lòng thực hiện lại!");
            }
            return RedirectToAction("Index");
        }

        //chuyển đổi trạng thái publish
        [HttpPost]
        [RBACAuthorize(Permissions = "Accept_pub")]
        public ActionResult SelectedPublish(int id)
        {
            if (id <= 0)
                throw new HttpRequestValidationException();
            Company _currentcompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IPublishService pubSrc = IoC.Resolve<IPublishService>();
            string ERRMessage = "";
            Publish pub = pubSrc.Getbykey(id);
            if (pub.Status == PublishStatus.Newpub)
            {
                if (pubSrc.SendforApprove(pub, out ERRMessage) == true)
                {
                    log.Info("SelectedPublish Publish by: " + HttpContext.User.Identity.Name + " Info-- ID: " + id.ToString());
                    Messages.AddFlashMessage("Gửi thành công");
                }
                else
                {
                    log.Error("SelectedPublish Publish:" + ERRMessage);
                    Messages.AddErrorFlashMessage(ERRMessage);
                }
                return RedirectToAction("DetailRPublish/" + id + "");
            }
            if (pub.Status == PublishStatus.Waiting)
            {
                if (pubSrc.Approve(pub, out ERRMessage) == true)
                {
                    log.Info("SelectedPublish Publish by: " + HttpContext.User.Identity.Name + " Info-- ID: " + id.ToString());
                    Messages.AddFlashMessage("Phát hành thành công!");
                }
                else
                {
                    log.Error("SelectedPublish Publish:" + ERRMessage);
                    Messages.AddErrorFlashMessage(ERRMessage);
                }
                return RedirectToAction("DetailRPublish/" + id + "");
            }
            Messages.AddErrorFlashMessage("Không thực hiện thành công");
            return RedirectToAction("DetailRPublish/" + id + "");
        }
        #endregion

        #region "Decision"
        //Hiển thị danh sách quyết định phát hành
        [RBACAuthorize(Permissions = "Search_dec")]
        public ActionResult ListDecision(DecIndexModels model, int? page)
        {
            IDecisionService decSrc = IoC.Resolve<IDecisionService>();
            Company _currentcompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int dstatus = (String.IsNullOrEmpty(model.status) ? -1 : Convert.ToInt32(model.status));
            int defautPageSize = 10;
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            int ItemCount = 0;
            IList<Decision> lst = decSrc.SearhByName(_currentcompany.id, dstatus, currentPageIndex, defautPageSize, out ItemCount);
            model.PageListDEC = new PagedList<Decision>(lst, currentPageIndex, defautPageSize, ItemCount);
            return View(model);
        }

        //Tạo quyết định phát hành
        [RBACAuthorize(Permissions = "Add_dec")]
        public ActionResult CreateDecision()
        {
            DecisionModels model = new DecisionModels();
            IDecisionService decSrc = IoC.Resolve<IDecisionService>();
            Company _currentcompany = IoC.Resolve<ICompanyService>().Getbykey(((EInvoiceContext)FXContext.Current).CurrentCompany.id);
            model.ComID = _currentcompany.id;
            model.ComName = _currentcompany.Name;
            model.ParentCompany = _currentcompany.Name;
            model.ComAddress = _currentcompany.Address;
            model.TaxCode = _currentcompany.TaxCode;
            //model.EffectiveDate = DateTime.Now;

            //đưa ra danh sách các đăng ký mẫu 
            IRegisterTempService regisSrc = IoC.Resolve<IRegisterTempService>();
            model.RegTempList = new SelectList(from re in regisSrc.Query where re.ComId == _currentcompany.id select re, "InvPattern", "InvPattern");

            //khởi tạo đăng ký loại hóa đơn
            model.DecDatasource = "[]";
            return View(model);
        }


        [HttpPost]
        [RBACAuthorize(Permissions = "Add_dec")]
        public ActionResult CreateDecision(DecisionModels model)
        {
            IDecisionService decSrc = IoC.Resolve<IDecisionService>();
            try
            {
                IList<Pupor> lstp = (IList<Pupor>)model.DecDatasource.DeserializeJSON<Pupor>(typeof(IList<Pupor>));
                Decision oDec = model.UpdateDecision(new Decision());
                //oDec.EffectiveDate = DateTime.ParseExact(Request["EffectiveDate"], "dd/MM/yyyy", null);
                //lấy thông tin về mẫu hóa đơn đăng ký
                string json = "<Root>";
                string lstPattern = "";
                foreach (Pupor p in lstp)
                {
                    json += "<Purpose>" + p.Mucdich + "</Purpose>";
                    lstPattern += p.InvPattern + ',';
                }
                json += "</Root>";
                lstPattern = lstPattern.Remove(lstPattern.Length - 1, 1);
                oDec.ListInvPattern = lstPattern;
                oDec.Purpose = json;
                decSrc.CreateNew(oDec);
                decSrc.CommitChanges();
                Messages.AddFlashMessage(Resources.Message.Dec_IMesSuccess);
                log.Info("Create Decision by: " + HttpContext.User.Identity.Name);
                return RedirectToAction("ListDecision");
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
                ICompanyService _comSrv = IoC.Resolve<ICompanyService>();
                Company _currentcompany = _comSrv.Getbykey(((EInvoiceContext)FXContext.Current).CurrentCompany.id);
                IRegisterTempService regisSrc = IoC.Resolve<IRegisterTempService>();
                model.RegTempList = new SelectList(from re in regisSrc.Query where re.ComId == _currentcompany.id select re, "InvPattern", "InvPattern");
                Messages.AddErrorMessage(Resources.Message.Dec_IMesUnsuccess);
                log.Error("CreateDecision -" + ex);
                return View(model);
            }
        }

        //Hiển thị chi tiết quyết định phát hành
        [RBACAuthorize(Permissions = "Detail_dec")]
        public ActionResult DetailsDecision(int id)
        {
            if (id <= 0)
                throw new HttpRequestValidationException();
            IDecisionService decSrc = IoC.Resolve<IDecisionService>();
            ICompanyService _comSrv = IoC.Resolve<ICompanyService>();
            Company _currentcompany = _comSrv.Getbykey(((EInvoiceContext)FXContext.Current).CurrentCompany.id);
            IRegisterTempService regisSrc = IoC.Resolve<IRegisterTempService>();
            Decision model = decSrc.Getbykey(id);

            //lấy thông tin ve mẫu phát hành
            string arr = model.Purpose;
            XElement elem = XElement.Parse(arr);
            IEnumerable<XElement> list = (from c in elem.Elements("Purpose") select c).ToList<XElement>();
            IList<Pupor> lstp = new List<Pupor>();
            string pt = model.ListInvPattern;
            string[] str = pt.Split(',');
            List<RegisterTemp> qr = (from r in regisSrc.Query where str.Contains(r.InvPattern) && r.ComId == _currentcompany.id select r).ToList();
            int i = 0;
            if (i < list.Count())
            {
                foreach (XElement xe in list)
                {
                    Pupor p = new Pupor();
                    p.Mucdich = xe.Value;
                    p.InvPattern = qr[i].InvPattern;
                    p.InvCateName = qr[i].InvoiceTemp.InvCateName;
                    lstp.Add(p);
                    i++;
                }
            }
            ViewData["Data"] = lstp;
            //end
            //string mmddyy = model.EffectiveDate.Day.ToString() + "/" + model.EffectiveDate.Month.ToString() + "/" + model.EffectiveDate.Year.ToString();
            //ViewData["mmddyy"] = mmddyy;
            return View(model);
        }

        //Sửa chi tiết quyết định phát hành
        [RBACAuthorize(Permissions = "Edit_dec")]
        public ActionResult EditDecision(int id)
        {
            IDecisionService decSrc = IoC.Resolve<IDecisionService>();
            Company _currentcompany = IoC.Resolve<ICompanyService>().Getbykey(((EInvoiceContext)FXContext.Current).CurrentCompany.id);
            IRegisterTempService regisSrc = IoC.Resolve<IRegisterTempService>();

            Decision oDec = decSrc.Getbykey(id);
            //khoi tao mot decision models
            DecisionModels model = new DecisionModels();
            model.City = oDec.City;
            model.ComAddress = oDec.ComAddress;
            model.ComID = oDec.ComID;
            model.ComName = oDec.ComName;
            model.DecisionNo = oDec.DecisionNo;
            model.Director = oDec.Director;
            model.EffectiveDate = oDec.EffectiveDate;
            model.EffectDate = oDec.EffectDate;
            model.id = oDec.id;
            model.ParentCompany = oDec.ParentCompany;
            model.Requester = oDec.Requester;
            model.SystemName = oDec.SystemName;
            model.SoftApplication = oDec.SoftApplication;
            model.TechDepartment = oDec.TechDepartment;
            model.Workflow = oDec.Workflow;
            model.Responsibility = oDec.Responsibility;
            model.Destination = oDec.Destination;
            model.RegTempList = new SelectList(from re in regisSrc.Query where re.ComId == _currentcompany.id select re, "InvPattern", "InvPattern");
            //lấy thông tin về mẫu hóa đơn 
            string arr = oDec.Purpose;
            XElement elem = XElement.Parse(arr);
            IEnumerable<XElement> list = (from c in elem.Elements("Purpose") select c).ToList<XElement>();
            IList<Pupor> lstp = new List<Pupor>();
            string pt = oDec.ListInvPattern;
            string[] str = pt.Split(',');
            List<RegisterTemp> qr = (from r in regisSrc.Query where str.Contains(r.InvPattern) && r.ComId == _currentcompany.id select r).ToList();
            int i = 0;
            if (i < list.Count())
            {
                foreach (XElement xe in list)
                {
                    Pupor p = new Pupor();
                    p.Mucdich = xe.Value;
                    p.InvPattern = qr[i].InvPattern;
                    p.InvCateName = qr[i].InvoiceTemp.InvCateName;
                    lstp.Add(p);
                    i++;
                }
            }
            model.DecDatasource = lstp.SerializeJSON<Pupor>();
            //end 
            if (oDec.Status != 2)
            {
                model.TaxCode = _currentcompany.TaxCode;
                return View(model);
            }
            else return RedirectToAction("ListDecision");
        }

        [HttpPost]
        [RBACAuthorize(Permissions = "Edit_dec")]
        public ActionResult EditDecision(DecisionModels model)
        {
            if (model.id <= 0)
                throw new HttpRequestValidationException();
            IDecisionService decSrc = IoC.Resolve<IDecisionService>();
            try
            {
                Decision oDec = model.UpdateDecision(new Decision());

                //lấy thông tin về mẫu hóa đơn
                IList<Pupor> lstp = (IList<Pupor>)model.DecDatasource.DeserializeJSON<Pupor>(typeof(IList<Pupor>));
                string json = "<Root>";
                string lstPattern = "";
                foreach (Pupor p in lstp)
                {
                    json += "<Purpose>" + p.Mucdich + "</Purpose>";
                    lstPattern += p.InvPattern + ',';
                }
                json += "</Root>";
                lstPattern = lstPattern.Remove(lstPattern.Length - 1, 1);
                oDec.ListInvPattern = lstPattern;
                oDec.Purpose = json;
                //end

                //oDec.EffectiveDate = DateTime.ParseExact(Request["EffectiveDate"], "dd/MM/yyyy", null);
                decSrc.Save(oDec);
                decSrc.CommitChanges();
                Messages.AddFlashMessage(Resources.Message.Dec_UMesSuccess);
                log.Info("EditDecision by: " + HttpContext.User.Identity.Name);
                return RedirectToAction("ListDecision");
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
                ICompanyService _comSrv = IoC.Resolve<ICompanyService>();
                Company _currentcompany = _comSrv.Getbykey(((EInvoiceContext)FXContext.Current).CurrentCompany.id);
                IRegisterTempService regisSrc = IoC.Resolve<IRegisterTempService>();
                model.RegTempList = new SelectList(from re in regisSrc.Query where re.ComId == _currentcompany.id select re, "InvPattern", "InvPattern");
                Messages.AddErrorMessage(Resources.Message.Dec_UMesUnsuccess);
                log.Error(" EditDecision -" + ex.Message);
                return View(model);
            }
        }

        //Xóa quyết định phát hành
        [RBACAuthorize(Permissions = "Del_dec")]
        public ActionResult DeleteDecision(int id)
        {
            if (id <= 0)
                throw new HttpRequestValidationException();
            IDecisionService decSrc = IoC.Resolve<IDecisionService>();
            Decision model = decSrc.Getbykey(id);
            try
            {
                if (model.Status == 0)
                {
                    decSrc.Delete(model);
                    decSrc.CommitChanges();
                    Messages.AddFlashMessage(Resources.Message.Dec_DMesSuccess);
                    log.Info("Delete Decision by: " + HttpContext.User.Identity.Name);
                }
                else Messages.AddFlashMessage(Resources.Message.Dec_DMesCantDel);
            }
            catch (Exception ex)
            {
                Messages.AddErrorFlashMessage(Resources.Message.Dec_DMesUnsuccess);
                log.Error("DeleteDecision -" + ex);
            }
            return RedirectToAction("ListDecision");
        }
        #endregion

        #region "ManagePublishInv"
        //quan ly chi tiet thong bao phat hanh hoa don
        [RBACAuthorize(Permissions = "Search_PublishDetail")]
        public ActionResult ManagePublishInv(int? page, PublishInvModel model)
        {
            int PageIndex = page.HasValue ? page.Value - 1 : 0;
            int PageSize = 10;
            int recordItem = 0;
            Company _currentcompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            //lay danh sach các pattern
            //chon mot pattern
            model.lstpattern = IoC.Resolve<IPublishInvoiceService>().LstByPattern(_currentcompany.id, 1);
            if (model.lstpattern.Count == 0)
            {
                Messages.AddErrorFlashMessage(Resources.Message.MInv_SMesNoPubAccess);
                return RedirectToAction("Index");
            }
            if (string.IsNullOrEmpty(model.Pattern)) model.Pattern = model.lstpattern[0];
            //lay ra danh sach serial
            //chon một serial
            model.lstserial = (from p in IoC.Resolve<IPublishInvoiceService>().Query where p.ComId == _currentcompany.id && p.Status > 1 && p.InvPattern == model.Pattern select p.InvSerial).Distinct().ToList<string>();
            if (string.IsNullOrEmpty(model.Serial)) model.Serial = null;
            //xu ly thoi gian truyen vao ham service
            DateTime? DateFrom = null;
            DateTime? DateTo = null;
            if (!string.IsNullOrWhiteSpace(model.FromDate)) DateFrom = DateTime.ParseExact(model.FromDate, "dd/MM/yyyy", null);
            if (!string.IsNullOrWhiteSpace(model.ToDate)) DateTo = DateTime.ParseExact(model.ToDate, "dd/MM/yyyy", null);
            IList<PublishInvoice> lstPublishInv = IoC.Resolve<IPublishInvoiceService>().SeachPubInvInfo(model.Pattern, model.Serial, DateFrom, DateTo, _currentcompany.id, PageIndex, PageSize, out recordItem);
            //phan trang va dua ra view 
            model.PageListPubInv = new PagedList<PublishInvoice>(lstPublishInv, PageIndex, PageSize, recordItem);
            return View(model);

        }
        #endregion

        public ActionResult AjxPreviewPubInv(int tempid, string serialNo)
        {
            IRegisterTempService regisTempSrv = IoC.Resolve<IRegisterTempService>();
            Company currCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            RegisterTemp temp = regisTempSrv.Getbykey(tempid);
            InvTemplate it = temp.InvoiceTemp;
            XmlDocument xdoc = new XmlDocument();
            xdoc.PreserveWhitespace = true;
            xdoc.LoadXml(it.XmlFile);
            //
            if (xdoc.GetElementsByTagName("ComName")[0] != null)
                xdoc.GetElementsByTagName("ComName")[0].InnerText = currCom.Name;
            if (xdoc.GetElementsByTagName("ParentName")[0] != null)
                xdoc.GetElementsByTagName("ParentName")[0].InnerText = currCom.ParentName;
            if (xdoc.GetElementsByTagName("ComAddress")[0] != null)
                xdoc.GetElementsByTagName("ComAddress")[0].InnerText = currCom.Address;
            if (xdoc.GetElementsByTagName("ComPhone")[0] != null)
                xdoc.GetElementsByTagName("ComPhone")[0].InnerText = currCom.Phone;
            if (xdoc.GetElementsByTagName("ComFax")[0] != null)
                xdoc.GetElementsByTagName("ComFax")[0].InnerText = currCom.Fax;
            if (xdoc.GetElementsByTagName("ComBankName")[0] != null)
                xdoc.GetElementsByTagName("ComBankName")[0].InnerText = currCom.BankName;            
            if (xdoc.GetElementsByTagName("ComBankNo")[0] != null)
                xdoc.GetElementsByTagName("ComBankNo")[0].InnerText = currCom.BankNumber;
            if (xdoc.GetElementsByTagName("ComTaxCode")[0] != null)
                xdoc.GetElementsByTagName("ComTaxCode")[0].InnerText = currCom.TaxCode;
            if (xdoc.GetElementsByTagName("SerialNo")[0] != null)
                xdoc.GetElementsByTagName("SerialNo")[0].InnerText = serialNo;

            XmlProcessingInstruction newPI;
            string urlPub = FX.Utils.UrlUtil.GetSiteUrl();
            String PItext = "type='text/xsl' href='";
            if (it.CssData != null)
                PItext += urlPub + "/InvoiceTemplate/GetXSLTbyPattern/1?pattern=" + temp.InvPattern + "'";
            else
                PItext += urlPub + "/InvoiceTemplate/GetXSLTbyTempName?tempname=" + it.TemplateName + "'";
            newPI = xdoc.CreateProcessingInstruction("xml-stylesheet", PItext);
            xdoc.InsertBefore(newPI, xdoc.DocumentElement);
            //logtest.Info("tempName: " + tempName + " href: " + PItext);

            // IViewer _iViewerSrv = IoC.Resolve<IViewer>();
            IViewer _iViewerSrv = InvServiceFactory.GetViewer(it.TemplateName);
            return Json(_iViewerSrv.GetHtml(System.Text.Encoding.UTF8.GetBytes(xdoc.OuterXml)));
        }

        #region "Ajax"
        //ten loại hóa đơn lấy từ pattern
        public ActionResult getInv(string pattern)
        {
            Company _currentcompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IRegisterTempService regisSrc = IoC.Resolve<IRegisterTempService>();
            string invcate = (from i in regisSrc.Query
                              where
                                  i.InvPattern == pattern && i.ComId == _currentcompany.id
                              select i.InvoiceTemp.InvCateName).Single();
            return Json(new { invcate });
        }

        //Tự động thay đổi ToNo khi nhập Invserialprefix và InvSerialSuffix
        public ActionResult ajx(int Id, string InvSerialPrefix, string InvSerialSuffix)
        {
            Company _currentcompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IPublishInvoiceService pubinvSrc = IoC.Resolve<IPublishInvoiceService>();
            var qr = from p in pubinvSrc.Query
                     where
                         (p.RegisterID == Id) && (p.InvSerialPrefix == InvSerialPrefix) &&
                         (p.InvSerialSuffix == InvSerialSuffix) && (p.Status == 0 || p.Status == 1 || p.Status == 2 || p.Status == 3) && (p.ComId == _currentcompany.id)
                     select p.ToNo;
            decimal ic = qr.Count() == 0 ? 0 : qr.Max();
            return Json(new
            {
                ic
            }
            );
        }
        //chuyển đổi trạng thái quyết định phát hành
        [HttpPost]
        [RBACAuthorize(Permissions = "Accept_dec")]
        public ActionResult EditStatus(int id)
        {
            if (id <= 0)
                throw new HttpRequestValidationException();
            IDecisionService decSrc = IoC.Resolve<IDecisionService>();
            Decision dec = decSrc.Getbykey(id);
            try
            {
                string ErrMessage = "";
                if (dec.Status == 0)
                {
                    if (decSrc.SendDecision(dec, out ErrMessage) == true)
                    {
                        Messages.AddFlashMessage("Quyết định đã được gửi");
                    }
                    else Messages.AddErrorFlashMessage(ErrMessage);
                }
                else if (dec.Status == 1)
                {
                    if (decSrc.ApproveDecision(dec, out ErrMessage))
                    {
                        Messages.AddFlashMessage("Quyết định đã được cơ quan thuế chấp nhận!");
                    }
                    else Messages.AddErrorFlashMessage(ErrMessage);
                }
            }
            catch (Exception ex)
            {
                Messages.AddFlashException(ex);
                log.Error("EditStatus-" + ex.Message);
            }
            return RedirectToAction("DetailsDecision/" + id + "");
        }
        #endregion
    }
}
