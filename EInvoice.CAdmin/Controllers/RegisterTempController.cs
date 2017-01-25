using EInvoice.CAdmin.Models;
using EInvoice.Core;
using EInvoice.Core.Domain;
using EInvoice.Core.IService;
using EInvoice.Core.Viewer;
using FX.Context;
using FX.Core;
using FX.Utils.MvcPaging;
using IdentityManagement.Authorization;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml;

namespace EInvoice.CAdmin.Controllers
{
    public class RegisterTempController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RegisterTempController));

        [RBACAuthorize(Roles = "Admin")]
        public ActionResult Index(int? page)
        {
            Company currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            int pagesize = 10;
            IPagedList<RegisterTemp> model = IoC.Resolve<IRegisterTempService>().GetRegPageList(currentCompany.id, pagesize, currentPageIndex);
            if (model.Count == 0)
                return RedirectToAction("ChooseTemp");
            return View(model);
        }

        [RBACAuthorize(Roles = "Admin")]
        public ActionResult ChooseTemp()
        {
            RegisterTempChoiseModels model = new RegisterTempChoiseModels();
            IInvCategoryService invCateSrv = IoC.Resolve<IInvCategoryService>();
            IRegisterTempService _regSrc = IoC.Resolve<IRegisterTempService>();
            model.InvCategories = invCateSrv.Query.OrderBy(p => p.id).ToList();
            IInvTemplateService _invTempSrc = IoC.Resolve<IInvTemplateService>();
            model.CurrentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            return View(model);
        }

        public ActionResult Create(int tempid)
        {
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IInvTemplateService tempSrv = IoC.Resolve<IInvTemplateService>();
            IInvCategoryService cateSrv = IoC.Resolve<IInvCategoryService>();
            IRegisterTempService _regSrc = IoC.Resolve<IRegisterTempService>();
            if (_regSrc.Query.Where(p => p.InvoiceTemp.Id == tempid && p.ComId == currentCom.id).Count() > 0)
            {
                Messages.AddErrorFlashMessage("Mẫu hóa đơn đã được đăng ký, vui lòng chọn mẫu khác.");
                return RedirectToAction("Choosetemp");
            }
            RegisterTempModels model = new RegisterTempModels();

            InvTemplate invTemp = tempSrv.Getbykey(tempid);
            decimal i = _regSrc.GetMaxPatternOrder(invTemp.InvCateID, currentCom.id);
            RegisterTemp temp = new RegisterTemp();
            temp.Name = invTemp.TemplateName;
            temp.NameInvoice = invTemp.InvCateName;
            temp.InvCateID = invTemp.InvCateID;
            temp.InvPattern = cateSrv.Getbykey(invTemp.InvCateID).InvPattern + "0";
            temp.PatternOrder = i + 1;
            temp.CssData = invTemp.CssData;
            temp.IsCertify = invTemp.IsCertify;
            model.RegisTemp = temp;
            model.CurrentCom = currentCom;
            model.tempId = tempid;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(RegisterTemp temp, string actionName, int tempId)
        {
            HttpPostedFileBase logoImg = Request.Files["logoImg"];
            HttpPostedFileBase bgrImg = Request.Files["bgrImg"];
            string _logoFile = Request["logoFile"];
            string _imgFile = Request["imgFile"];
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IInvTemplateService tempSrv = IoC.Resolve<IInvTemplateService>();
            IRegisterTempService regisTempSrv = IoC.Resolve<IRegisterTempService>();
            string registerPattern = temp.InvPattern + "/" + temp.PatternOrder.ToString("000");
            if (regisTempSrv.Query.Where(p => p.InvPattern.ToUpper() == registerPattern.ToUpper() && p.ComId == temp.ComId).Count() > 0)
            {
                Messages.AddErrorMessage(string.Format("Mẫu số [{0}] đã được đăng ký cho công ty.", registerPattern));
                RegisterTempModels model = new RegisterTempModels();
                model.CurrentCom = currentCom;
                model.RegisTemp = temp;
                model.tempId = tempId;
                return View(model);
            }
            try
            {
                InvTemplate invTemp = tempSrv.Getbykey(tempId);
                temp.InvoiceTemp = invTemp;
                string logoKey = string.Format("{0}_{1}", currentCom.id, "logo");
                string backgroudKey = string.Format("{0}_{1}", currentCom.id, "backgroud");

                if (logoImg != null && logoImg.ContentLength > 0)
                {
                    byte[] byteLogoImg = readContentFilePosted(logoImg);
                    setCacheContext(logoKey, updateCss(invTemp.CssLogo, byteLogoImg));
                }
                if (string.IsNullOrWhiteSpace(_logoFile) && !string.IsNullOrWhiteSpace(invTemp.CssLogo))
                    setCacheContext(logoKey, invTemp.CssLogo);

                if (bgrImg != null && bgrImg.ContentLength > 0)
                {
                    byte[] bytebgrImg = readContentFilePosted(bgrImg);
                    setCacheContext(backgroudKey, updateCss(invTemp.CssBackgr, bytebgrImg, false));
                }
                if (string.IsNullOrWhiteSpace(_imgFile) && !string.IsNullOrWhiteSpace(invTemp.CssBackgr))
                    setCacheContext(backgroudKey, invTemp.CssBackgr);

                temp.CssData = !string.IsNullOrWhiteSpace(temp.CssData) ? temp.CssData : invTemp.CssData;

                if (actionName == "preview")
                {
                    RegisterTempModels model = new RegisterTempModels();
                    model.CurrentCom = currentCom;
                    model.RegisTemp = temp;
                    model.tempId = tempId;
                    model.imgFile = _imgFile;
                    model.logoFile = _logoFile;
                    StringBuilder sumSb = new StringBuilder();
                    sumSb.AppendFormat("{0}{1}{2}", temp.CssData, getCacheContext(logoKey), getCacheContext(backgroudKey));
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.PreserveWhitespace = true;
                    xdoc.LoadXml(invTemp.XmlFile);
                    if (xdoc.GetElementsByTagName("ComName")[0] != null)
                        xdoc.GetElementsByTagName("ComName")[0].InnerText = currentCom.Name;
                    if (xdoc.GetElementsByTagName("ComAddress")[0] != null)
                        xdoc.GetElementsByTagName("ComAddress")[0].InnerText = currentCom.Address;
                    if (xdoc.GetElementsByTagName("ComPhone")[0] != null)
                        xdoc.GetElementsByTagName("ComPhone")[0].InnerText = currentCom.Phone;
                    if (xdoc.GetElementsByTagName("ComBankNo")[0] != null)
                        //xdoc.GetElementsByTagName("ComFax")[0].InnerText = comFax;
                        xdoc.GetElementsByTagName("ComBankNo")[0].InnerText = currentCom.BankNumber;
                    if (xdoc.GetElementsByTagName("ComTaxCode")[0] != null)
                        xdoc.GetElementsByTagName("ComTaxCode")[0].InnerText = currentCom.TaxCode;
                    XmlNode root = xdoc.DocumentElement;

                    //Create a new node.
                    XmlElement elem = xdoc.CreateElement("CssData");
                    elem.InnerText = sumSb.ToString();

                    //Add the node to the document.
                    root.AppendChild(elem);

                    XmlProcessingInstruction newPI;
                    String PItext = "type='text/xsl' href='" + FX.Utils.UrlUtil.GetSiteUrl() + "/RegisterTemp/getXSLTbyTempName?tempname=" + invTemp.TemplateName + "'";
                    newPI = xdoc.CreateProcessingInstruction("xml-stylesheet", PItext);
                    xdoc.InsertBefore(newPI, xdoc.DocumentElement);
                    IViewer _iViewerSrv = InvServiceFactory.GetViewer(invTemp.TemplateName);
                    ViewData["previewContent"] = _iViewerSrv.GetHtml(System.Text.Encoding.UTF8.GetBytes(xdoc.OuterXml));
                    return View(model);
                }
                temp.ICertifyProvider = temp.IsCertify ? temp.ICertifyProvider : "";
                temp.InvPattern = registerPattern;
                if (regisTempSrv.GetbyPattern(temp.InvPattern, currentCom.id) != null)
                {
                    Messages.AddErrorFlashMessage("Mẫu số đã được đăng ký, sử dụng mẫu số khác!");
                    RegisterTempModels model = new RegisterTempModels();
                    model.CurrentCom = currentCom;
                    model.RegisTemp = temp;
                    model.tempId = tempId;
                    model.imgFile = _imgFile;
                    model.logoFile = _logoFile;
                    return View(model);
                }
                temp.ComId = currentCom.id;
                temp.InvCateID = invTemp.InvCateID;
                temp.CssLogo = getCacheContext(logoKey);
                temp.CssBackgr = getCacheContext(backgroudKey);
                regisTempSrv.CreateNew(temp);
                regisTempSrv.CommitChanges();
                Messages.AddFlashMessage("Đăng ký mẫu hóa đơn thành công!");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                Messages.AddErrorMessage("Có lỗi xảy ra, vui lòng thực hiện lại.");
                RegisterTempModels model = new RegisterTempModels();
                model.CurrentCom = currentCom;
                model.RegisTemp = temp;
                model.tempId = tempId;
                model.imgFile = _imgFile;
                model.logoFile = _logoFile;
                return View(model);
            }
        }

        public ActionResult Edit(int id)
        {
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IRegisterTempService registerTempSrv = IoC.Resolve<IRegisterTempService>();
            IPublishInvoiceService publishInvSrv = IoC.Resolve<IPublishInvoiceService>();
            if (publishInvSrv.GetPubOfReg(id, currentCom.id).Count > 0)
            {
                Messages.AddErrorFlashMessage("Không được sửa mẫu hóa đơn đang sử dụng.");
                return RedirectToAction("Index");
            }
            string logoKey = string.Format("{0}_{1}", currentCom.id, "logo");
            string backgroudKey = string.Format("{0}_{1}", currentCom.id, "backgroud");
            RegisterTemp registerTemp = registerTempSrv.Getbykey(id);
            RegisterTempModels model = new RegisterTempModels();
            model.tempId = registerTemp.InvoiceTemp.Id;
            model.CurrentCom = currentCom;
            model.logoFile = model.imgFile = registerTemp.Name;
            model.RegisTemp = registerTemp;
            if (!string.IsNullOrWhiteSpace(registerTemp.CssLogo))
                setCacheContext(logoKey, registerTemp.CssLogo);
            if (!string.IsNullOrWhiteSpace(registerTemp.CssBackgr))
                setCacheContext(backgroudKey, registerTemp.CssBackgr);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(int id, string actionName, int tempId)
        {
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IRegisterTempService registerTempSrv = IoC.Resolve<IRegisterTempService>();
            HttpPostedFileBase logoImg = Request.Files["logoImg"];
            HttpPostedFileBase bgrImg = Request.Files["bgrImg"];
            string _logoFile = Request["logoFile"];
            string _imgFile = Request["imgFile"];
            RegisterTemp registerTemp = registerTempSrv.Getbykey(id);
            try
            {
                TryUpdateModel<RegisterTemp>(registerTemp);
                string logoKey = string.Format("{0}_{1}", currentCom.id, "logo");
                string backgroudKey = string.Format("{0}_{1}", currentCom.id, "backgroud");

                if (logoImg != null && logoImg.ContentLength > 0)
                {
                    byte[] byteLogoImg = readContentFilePosted(logoImg);
                    setCacheContext(logoKey, updateCss(registerTemp.CssLogo, byteLogoImg));
                }
                if (string.IsNullOrWhiteSpace(_logoFile))
                    setCacheContext(logoKey, registerTemp.CssLogo);

                if (bgrImg != null && bgrImg.ContentLength > 0)
                {
                    byte[] bytebgrImg = readContentFilePosted(bgrImg);
                    setCacheContext(backgroudKey, updateCss(registerTemp.CssBackgr, bytebgrImg, false));
                }
                if (string.IsNullOrWhiteSpace(_imgFile))
                    setCacheContext(backgroudKey, registerTemp.CssBackgr);
                if (actionName == "preview")
                {
                    RegisterTempModels model = new RegisterTempModels();
                    model.CurrentCom = currentCom;
                    model.RegisTemp = registerTemp;
                    model.tempId = tempId;
                    model.imgFile = _imgFile;
                    model.logoFile = _logoFile;
                    StringBuilder sumSb = new StringBuilder();
                    sumSb.AppendFormat("{0}{1}{2}", registerTemp.CssData, getCacheContext(logoKey), getCacheContext(backgroudKey));
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.PreserveWhitespace = true;
                    InvTemplate invTemp = registerTemp.InvoiceTemp;
                    xdoc.LoadXml(invTemp.XmlFile);
                    if (xdoc.GetElementsByTagName("ComName")[0] != null)
                        xdoc.GetElementsByTagName("ComName")[0].InnerText = currentCom.Name;
                    if (xdoc.GetElementsByTagName("ComAddress")[0] != null)
                        xdoc.GetElementsByTagName("ComAddress")[0].InnerText = currentCom.Address;
                    if (xdoc.GetElementsByTagName("ComPhone")[0] != null)
                        xdoc.GetElementsByTagName("ComPhone")[0].InnerText = currentCom.Phone;
                    if (xdoc.GetElementsByTagName("ComBankNo")[0] != null)
                        //xdoc.GetElementsByTagName("ComFax")[0].InnerText = comFax;
                        xdoc.GetElementsByTagName("ComBankNo")[0].InnerText = currentCom.BankNumber;
                    if (xdoc.GetElementsByTagName("ComTaxCode")[0] != null)
                        xdoc.GetElementsByTagName("ComTaxCode")[0].InnerText = currentCom.TaxCode;
                    XmlNode root = xdoc.DocumentElement;

                    //Create a new node.
                    XmlElement elem = xdoc.CreateElement("CssData");
                    elem.InnerText = sumSb.ToString();

                    //Add the node to the document.
                    root.AppendChild(elem);

                    XmlProcessingInstruction newPI;
                    String PItext = "type='text/xsl' href='" + FX.Utils.UrlUtil.GetSiteUrl() + "/RegisterTemp/getXSLTbyTempName?tempname=" + invTemp.TemplateName + "'";
                    newPI = xdoc.CreateProcessingInstruction("xml-stylesheet", PItext);
                    xdoc.InsertBefore(newPI, xdoc.DocumentElement);
                    IViewer _iViewerSrv = InvServiceFactory.GetViewer(invTemp.TemplateName);
                    ViewData["previewContent"] = _iViewerSrv.GetHtml(System.Text.Encoding.UTF8.GetBytes(xdoc.OuterXml));
                    return View(model);
                }
                registerTemp.ComId = currentCom.id;
                registerTemp.CssLogo = getCacheContext(logoKey);
                registerTemp.CssBackgr = getCacheContext(backgroudKey);
                registerTempSrv.Save(registerTemp);
                registerTempSrv.CommitChanges();
                Messages.AddFlashMessage("Chỉnh sửa mẫu hóa đơn thành công.");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                Messages.AddErrorMessage("Có lỗi xảy ra, vui lòng thực hiện lại.");
                RegisterTempModels model = new RegisterTempModels();
                model.CurrentCom = currentCom;
                model.RegisTemp = registerTemp;
                model.tempId = tempId;
                model.imgFile = _imgFile;
                model.logoFile = _logoFile;
                return View(model);
            }
        }

        public ActionResult Delete(int id)
        {
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IRegisterTempService registerTempSrv = IoC.Resolve<IRegisterTempService>();
            IPublishInvoiceService publishInvSrv = IoC.Resolve<IPublishInvoiceService>();
            try
            {
                if (publishInvSrv.GetPubOfReg(id, currentCom.id).Count > 0)
                {
                    Messages.AddErrorFlashMessage("Không được xóa mẫu hóa đơn đang sử dụng.");
                    return RedirectToAction("Index");
                }
                RegisterTemp model = registerTempSrv.Getbykey(id);
                registerTempSrv.Delete(model);
                registerTempSrv.CommitChanges();
                Messages.AddFlashMessage("Xóa mẫu hóa đơn thành công.");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                Messages.AddErrorFlashMessage("Có lỗi xảy ra, vui lòng thực hiện lại.");
            }
            return RedirectToAction("Index");
        }

        public ActionResult getXSLTbyTempName(string tempname)
        {
            IInvTemplateService src = IoC.Resolve<IInvTemplateService>();
            InvTemplate temp = InvServiceFactory.GetTemplateByName(tempname);
            byte[] xsltData = System.Text.Encoding.UTF8.GetBytes(temp.XsltFile);
            return File(xsltData, "text/xsl");
        }

        private void setCacheContext(string key, string value)
        {
            double totalSeconds = FormsAuthentication.Timeout.TotalSeconds;
            if (HttpContext.Cache[key] != null)
                HttpContext.Cache.Remove(key);
            HttpContext.Cache.Insert(key, value, null, DateTime.Now.AddSeconds(totalSeconds), Cache.NoSlidingExpiration);
        }

        private string getCacheContext(string key)
        {
            if (HttpContext.Cache[key] == null)
                return null;
            return (string)HttpContext.Cache[key];
        }

        private string updateCss(string cssData, byte[] data, bool isLogo = true)
        {
            string[] cssArray = cssData.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder strBuild = new StringBuilder();
            string base64String = cssArray.Where(p => p.TrimStart().StartsWith("base64,")).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(base64String))
                return cssData;
            string cssValue;
            if (isLogo)
                cssValue = "base64," + Convert.ToBase64String(data) + ") no-repeat;";
            else
                cssValue = "base64," + Convert.ToBase64String(data) + ") no-repeat scroll transparent center;";
            if (base64String.Contains("}"))
                cssValue += "}";
            foreach (string strCss in cssArray)
            {
                if (strCss.Contains("base64,"))
                    strBuild.Append(cssValue);
                else
                    strBuild.AppendFormat("{0};", strCss);
            }
            return strBuild.ToString().TrimEnd(';');
        }

        private byte[] readContentFilePosted(HttpPostedFileBase file)
        {
            if (file == null) return null;
            Stream fileStream = file.InputStream;
            var mStreamer = new MemoryStream();
            mStreamer.SetLength(fileStream.Length);
            fileStream.Read(mStreamer.GetBuffer(), 0, (int)fileStream.Length);
            mStreamer.Seek(0, SeekOrigin.Begin);
            byte[] fileBytes = mStreamer.GetBuffer();
            return fileBytes;
        }
    }
}
