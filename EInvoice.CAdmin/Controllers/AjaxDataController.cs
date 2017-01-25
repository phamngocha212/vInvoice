using EInvoice.CAdmin.IService;
using EInvoice.CAdmin.Models;
using EInvoice.Core;
using EInvoice.Core.Domain;
using EInvoice.Core.IService;
using FX.Context;
using FX.Core;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

using FX.Utils;
using System.Xml;
using EInvoice.Core.Viewer;

namespace EInvoice.CAdmin.Controllers
{
    public class AjaxDataController : Controller
    {
        static ILog log = LogManager.GetLogger(typeof(AjaxDataController));
        public ActionResult LaunchChoiceByPlugin(string cbid, string Pattern, string Serial, string certificate)
        {
            try
            {
                string[] idsStr = cbid.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                int[] ids = (from s in idsStr select Convert.ToInt32(s)).ToArray();
                if (ids.Length < 0)
                    return Json("ERROR:1");
                Company currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                byte[] raw = Convert.FromBase64String(certificate);
                X509Certificate2 x509Cert = new X509Certificate2(raw);
                KeyStores _keyStore = KeyStoresManagement.GetKeyStore(currentCompany.id);
                if (_keyStore == null)
                {
                    log.Error("keyStores invalid");
                    return Json("ERROR:2");
                }
                string serial4Keystore = _keyStore.SerialCert.TrimEnd().ToUpper();
                string serial4X509Cert = x509Cert.GetSerialNumberString().TrimEnd().ToUpper();
                if (serial4Keystore.CompareTo(serial4X509Cert) != 0)
                {
                    log.Error("1|" + _keyStore.SerialCert.ToUpper());
                    log.Error("2|" + x509Cert.GetSerialNumberString().ToUpper());
                    return Json("ERROR:2");
                }
                if (DateTime.Parse(x509Cert.GetExpirationDateString()) < DateTime.Today)
                    return Json("ERROR:3");
                IInvoiceService IInvSrv = InvServiceFactory.GetService(Pattern, currentCompany.id);
                IInvoice[] invoicesByIds = IInvSrv.GetByID(currentCompany.id, ids).OrderBy(p => p.id).ToArray();
                IList<InvoiceForPlugin> data = new List<InvoiceForPlugin>();
                IDictionary<string, string> dictHash = EInvoice.Core.Launching.Launcher.Instance.GetDigestForRemote(Pattern, Serial, invoicesByIds, x509Cert);
                foreach (KeyValuePair<string, string> pair in dictHash)
                {
                    data.Add(new InvoiceForPlugin() { Key = pair.Key, Hash = pair.Value });
                }
                IInvSrv.UnbindSession();
                return Json(new { hashdata = data });
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return Json("ERROR:4");
            }
        }

        public JsonResult WrapAndLaunch(string signData)
        {
            try
            {
                Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                Dictionary<string, object> dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(signData);
                string[] keysArray = JsonConvert.DeserializeObject<string[]>(dict["keys"].ToString());
                string[] signedsArray = JsonConvert.DeserializeObject<string[]>(dict["signeds"].ToString());
                var dictInv = new Dictionary<string, byte[]>();
                for (int i = 0; i < keysArray.Length; i++)
                {
                    dictInv.Add(keysArray[i], Convert.FromBase64String(signedsArray[i]));
                }
                EInvoice.Core.Launching.Launcher.Instance.RemoteLaunch(dict["pattern"].ToString(), dict["serial"].ToString(), dictInv);
                return Json("OK");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return Json("Lỗi: Chưa phát hành được hóa đơn, vui lòng thực hiện lại.");
            }
        }

        public ActionResult WrapAdjustInvoices(string key, string signed, string pattern, string serial, string no)
        {
            try
            {
                Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                IDictionary<string, byte[]> dictInv = new Dictionary<string, byte[]>();
                dictInv.Add(key, Convert.FromBase64String(signed));
                Company currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                IInvoiceService invSrv = InvServiceFactory.GetService(pattern, currentCompany.id);
                IInvoice OriInv = invSrv.GetByNo(currentCompany.id, pattern, serial, decimal.Parse(no));
                IInvoice p = EInvoice.Core.Launching.Launcher.Instance.AdjustRemote(OriInv, pattern, serial, dictInv);

                return Json("OK");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return Json("Lỗi: Chưa điều chỉnh được hóa đơn, vui lòng thực hiện lại.");
            }
        }

        public ActionResult LaunchAdjustByPlugin(string invData, string NewPattern, string NewSerial, int OriNo, string OriPattern, string OriSerial, int type, string CertBase64String)
        {
            try
            {
                Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                int comID = _currentCompany.id;

                if (_currentCompany == null) return Json(new { error = "ERR:2" });
                byte[] raw = Convert.FromBase64String(CertBase64String);
                X509Certificate2 x509Cert = new X509Certificate2(raw);
                KeyStores _keyStore = KeyStoresManagement.GetKeyStore(_currentCompany.id);
                if (_keyStore.SerialCert.ToUpper().CompareTo(x509Cert.GetSerialNumberString().ToUpper()) != 0)
                    return Json("ERROR:2");
                if (DateTime.Parse(x509Cert.GetExpirationDateString()) < DateTime.Today)
                    return Json("ERROR:3");

                IInvoiceService invSrv = InvServiceFactory.GetService(OriPattern, comID);
                IInvoice OriInv = invSrv.GetByNo(comID, OriPattern, OriSerial, OriNo);
                IRegisterTempService regisSrv = IoC.Resolve<IRegisterTempService>();
                ICustomerService _cusSvr = IoC.Resolve<ICustomerService>();

                IInvoice inv = InvServiceFactory.NewInstance(NewPattern, comID);
                inv.DeserializeFromJSON(invData);
                inv.Pattern = NewPattern;
                inv.Serial = NewSerial;
                inv.Status = InvoiceStatus.NewInv;
                inv.Type = (InvoiceType)type;
                DateTime dt = OriInv.ArisingDate;

                string strAdjust = (_currentCompany.Config.Keys.Contains("AdjustString")) ? _currentCompany.Config["AdjustString"] : "@AdjustType cho hóa đơn số: @InvNo, Ký hiệu hóa đơn: @InvSerial, Ngày @InvDay Tháng @InvMonth Năm @InvYear";
                strAdjust = strAdjust.Replace("@AdjustType", Utils.GetNameInvoiceType(inv.Type)).Replace("@InvNo", OriInv.No.ToString(Utils.MAX_NO_LENGTH)).Replace("@InvPattern", OriInv.Pattern).Replace("@InvSerial", OriInv.Serial).Replace("@InvDay", dt.Day.ToString()).Replace("@InvMonth", dt.Month.ToString()).Replace("@InvYear", dt.Year.ToString());
                inv.ProcessInvNote = strAdjust;

                var Typecus = (from c in _cusSvr.Query where c.ComID == comID && c.Code == inv.CusCode && c.CusType == 1 select c.CusType).SingleOrDefault();
                if (Typecus == 0)
                    inv.CusSignStatus = cusSignStatus.NocusSignStatus;
                else
                    inv.CusSignStatus = cusSignStatus.NoSignStatus;
                if (OriInv.Status == InvoiceStatus.SignedInv || OriInv.Status == InvoiceStatus.AdjustedInv || OriInv.Status == InvoiceStatus.InUseInv)
                {
                    IDictionary<string, string> dict = EInvoice.Core.Launching.Launcher.Instance.GetDigestForRemote(OriPattern, OriSerial, new IInvoice[] { inv }, x509Cert);
                    if (dict.Count == 0)
                        return Json("ERROR:4");
                    var dictFirst = dict.FirstOrDefault();
                    invSrv.UnbindSession();
                    return Json(new { hashdata = dictFirst });
                }
                return Json("ERROR:4");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return Json("ERROR:4");
            }
        }

        public ActionResult LaunchReplaceByPlugin(string invData, string NewPattern, string NewSerial, int OriNo, string OriPattern, string OriSerial, string CertBase64String)
        {
            try
            {
                Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                int comID = _currentCompany.id;
                if (_currentCompany == null) return Json(new { error = "ERR:2" });
                byte[] raw = Convert.FromBase64String(CertBase64String);
                X509Certificate2 x509Cert = new X509Certificate2(raw);
                KeyStores _keyStore = KeyStoresManagement.GetKeyStore(_currentCompany.id);
                if (_keyStore.SerialCert.ToUpper().CompareTo(x509Cert.GetSerialNumberString().ToUpper()) != 0)
                    return Json("ERROR:2");
                if (DateTime.Parse(x509Cert.GetExpirationDateString()) < DateTime.Today)
                    return Json("ERROR:3");

                IInvoiceService invSrv = InvServiceFactory.GetService(OriPattern, comID);
                IInvoice OriInv = invSrv.GetByNo(comID, OriPattern, OriSerial, OriNo);
                IRegisterTempService regisSrv = IoC.Resolve<IRegisterTempService>();
                ICustomerService _cusSvr = IoC.Resolve<ICustomerService>();

                IInvoice inv = InvServiceFactory.NewInstance(NewPattern, comID);
                inv.DeserializeFromJSON(invData);
                inv.Pattern = NewPattern;
                inv.Serial = NewSerial;
                inv.Status = InvoiceStatus.NewInv;
                DateTime dt = OriInv.ArisingDate;

                string strReplace = (_currentCompany.Config.ContainsKey("ReplaceString")) ? _currentCompany.Config["ReplaceString"] : "Hóa đơn này thay thế hóa đơn số: @InvNo, Ký hiệu: @InvSerial, Gửi ngày @InvDay Tháng @InvMonth Năm @InvYear";
                strReplace = strReplace.Replace("@InvNo", OriInv.No.ToString(Utils.MAX_NO_LENGTH)).Replace("@InvSerial", OriInv.Serial).Replace("@InvPattern", OriInv.Pattern).Replace("@InvDay", OriInv.ArisingDate.Day.ToString()).Replace("@InvMonth", OriInv.ArisingDate.Month.ToString()).Replace("@InvYear", OriInv.ArisingDate.Year.ToString());
                inv.ProcessInvNote = strReplace;
                inv.Type = InvoiceType.ForReplace;
                var Typecus = (from c in _cusSvr.Query where c.ComID == comID && c.Code == inv.CusCode && c.CusType == 1 select c.CusType).SingleOrDefault();
                if (Typecus == 0)
                {
                    inv.CusSignStatus = cusSignStatus.NocusSignStatus;
                }
                else
                {
                    inv.CusSignStatus = cusSignStatus.NoSignStatus;
                }
                if (OriInv.Status == InvoiceStatus.SignedInv || OriInv.Status == InvoiceStatus.AdjustedInv)
                {

                    IDictionary<string, string> dict = EInvoice.Core.Launching.Launcher.Instance.GetDigestForRemote(OriPattern, OriSerial, new IInvoice[] { inv }, x509Cert);
                    if (dict.Count == 0)
                        return Json("ERROR:4");
                    var dictFirst = dict.FirstOrDefault();
                    invSrv.UnbindSession();
                    return Json(new { hashdata = dictFirst });
                }

                return Json("ERROR:4");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return Json("ERROR:4");
            }
        }
        public ActionResult WrapReplaceInvoices(string key, string signed, string pattern, string serial, string no)
        {
            try
            {
                Company currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                IDictionary<string, byte[]> dictInv = new Dictionary<string, byte[]>();
                dictInv.Add(key, Convert.FromBase64String(signed));

                IInvoiceService invSrv = InvServiceFactory.GetService(pattern, currentCompany.id);
                IInvoice OriInv = invSrv.GetByNo(currentCompany.id, pattern, serial, decimal.Parse(no));
                IInvoice p = EInvoice.Core.Launching.Launcher.Instance.ReplaceRemote(OriInv, pattern, serial, dictInv);

                return Json("OK");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return Json("Lỗi: Chưa  thay thế được hóa đơn, vui lòng thực hiện lại.");
            }
        }

        public JsonResult getTempsbyInvCateId(int invCateId)
        {
            IInvTemplateService _invTempSrc = IoC.Resolve<IInvTemplateService>();
            var listTemps = _invTempSrc.GetTempOfCate(invCateId);
            return Json(new {listTemps});
        }

        public ActionResult previewTemplate(int tempId)
        {
            InvTemplate it = new InvTemplate();
            IInvTemplateService _invTempSrc = IoC.Resolve<IInvTemplateService>();
            it = _invTempSrc.Getbykey(tempId);
            XmlDocument xdoc = new XmlDocument();
            xdoc.PreserveWhitespace = true;
            xdoc.LoadXml(it.XmlFile);

            XmlProcessingInstruction newPI;
            String PItext = "type='text/xsl' href='" + FX.Utils.UrlUtil.GetSiteUrl() + "/InvoiceTemplate/GetXSLTbyTempName?tempname=" + it.TemplateName + "'";
            newPI = xdoc.CreateProcessingInstruction("xml-stylesheet", PItext);
            xdoc.InsertBefore(newPI, xdoc.DocumentElement);
            IViewer _iViewerSrv = InvServiceFactory.GetViewer(it.TemplateName);
            return Json(_iViewerSrv.GetHtml(System.Text.Encoding.UTF8.GetBytes(xdoc.OuterXml)));
        }
    }
}
