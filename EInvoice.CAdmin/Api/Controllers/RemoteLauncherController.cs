using EInvoice.Core;
using EInvoice.Core.Domain;
using EInvoice.Core.IService;
using FX.Context;
using FX.Core;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;
using System.Xml.Linq;

namespace EInvoice.CAdmin.Api.Controllers
{
    [RoutePrefix("api/remotelaucher")]
    public class RemoteLauncherController : ApiController
    {
        
        /// <summary>
        /// Tạo mới danh sách hd, hash dl hóa đơn gửi lại cho tool ký số
        /// </summary>
        /// <param name="data">Dạng json dữ liệu: InvPattern, InvSerial, xmlData</param>
        /// <returns></returns>
        [APIAuthenticate(Permissions = "Release_invInList")]
        [Route("getDigest")]
        [HttpPost]
        public IHttpActionResult GetDigest(RemoteInvoice data)
        {
            ILog log = LogManager.GetLogger(typeof(RemoteLauncherController));
            XmlSchemaValidator validator = new XmlSchemaValidator();
            if (!validator.ValidXmlDoc(data.InvData, "", AppDomain.CurrentDomain.BaseDirectory + @"xmlvalidate\VATInVoice_ws.xsd"))
            {
                log.Error("ERR:3-" + validator.ValidationError);
                return Ok("ERR:3-" + validator.ValidationError);  //du lieu dau vao khong hop le
            }

            Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int comID = _currentCompany.id;
            if (_currentCompany == null) return Ok("ERR:4");
            string nameInvoice = "";
            IRegisterTempService regisSrv = IoC.Resolve<IRegisterTempService>();
            IPublishInvoiceService pSrv = IoC.Resolve<IPublishInvoiceService>();
            RegisterTemp rtemp = regisSrv.GetbyPattern(data.InvPattern, _currentCompany.id);
            nameInvoice = rtemp.NameInvoice;
            regisSrv.UnbindSession(rtemp);

            XElement elem = XElement.Parse(data.InvData);
            IList<IInvoice> lstINV = new List<IInvoice>();
            IEnumerable<XElement> listTemp = elem.Elements("Inv");
            IList<String> lstKey = new List<String>();

            if (listTemp.Count() > 10)
            {
                return Ok("ERR:10");    // lo hoa don truyen vao lon hon maxBlockInv;
            }
            try
            {
                foreach (XElement e in listTemp)
                {
                    XElement ele = e.Element("Invoice");
                    string fKey = e.Element("key").Value.Trim();
                    lstKey.Add(fKey);
                    IInvoice inv = InvServiceFactory.NewInstance(data.InvPattern, comID);
                    string read = string.Concat(ele);
                    inv.DeserializeFromXML(read);
                    inv.No = 0;
                    inv.Name = nameInvoice;
                    inv.Pattern = data.InvPattern;
                    inv.Serial = data.InvSerial;
                    inv.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
                    inv.Fkey = fKey;
                    lstINV.Add(inv);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return Ok("ERR:5");
            }
            byte[] raw = Convert.FromBase64String(data.CertBase64String);
            X509Certificate2 x509Cert = new X509Certificate2(raw);
            IDictionary<string, string> dict = EInvoice.Core.Launching.Launcher.Instance.GetDigestForRemote(data.InvPattern, data.InvSerial, lstINV.ToArray(), x509Cert);
            return Ok(dict);
        }

        [APIAuthenticate(Permissions = "Release_invInList")]
        [Route("launcherInv")]
        public IHttpActionResult LauncherInv(RemoteInvoice data)
        {
            try
            {
                Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                XElement elem = XElement.Parse(data.InvData);
                IEnumerable<XElement> listTemp = elem.Elements("Data");
                IDictionary<string, byte[]> dictInv = new Dictionary<string, byte[]>();
                foreach (XElement e in listTemp)
                {
                    string id = e.Element("id").Value;
                    string Signed = e.Element("Signed").Value;
                    dictInv.Add(id, Convert.FromBase64String(Signed));
                }
                IList<IInvoice> lst = EInvoice.Core.Launching.Launcher.Instance.RemoteLaunch(data.InvPattern, data.InvSerial, dictInv);
                return Ok(lst.Select(p => new DataUpdate() { Data = p.Data, No = p.No, Serial = p.Serial }));
            }
            catch (Exception ex)
            {
                return Ok("ERROR");
            }
        }
        
        [Route("wrapandlaunch")]
        [HttpPost]
        public IHttpActionResult WrapAndLauch(JsonLaunch data)
        {                        
            Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            string[] keysArray = JsonConvert.DeserializeObject<string[]>(data.keys);
            string[] signedsArray = JsonConvert.DeserializeObject<string[]>(data.signeds);
            if (keysArray.Length != signedsArray.Length)
            {
                //log.ErrorFormat("Keys Length: {0}, Signed Length: {1}", data.keys.Length, data.signeds.Length);
                return Json("Lỗi: Chưa điều chỉnh được hóa đơn, vui lòng thực hiện lại.");
            }
            IDictionary<string, byte[]> dictInv = new Dictionary<string, byte[]>();
            for (int i = 0; i < keysArray.Length; i++)
            {
                dictInv.Add(keysArray[i], Convert.FromBase64String(signedsArray[i]));
            }
            EInvoice.Core.Launching.Launcher.Instance.RemoteLaunch(data.InvPattern, data.InvSerial, dictInv);
            return Ok("OK");
        }

        [APIAuthenticate(Permissions = "Search_Adjust")]
        [Route("digestForAdjust")]
        [HttpPost]
        public IHttpActionResult GetDigestForAdjust(RemoteAdjustInvoice data)
        {
            ILog log = LogManager.GetLogger(typeof(RemoteLauncherController));
            XmlSchemaValidator validator = new XmlSchemaValidator();
            if (!validator.ValidXmlDoc(data.InvData, "", AppDomain.CurrentDomain.BaseDirectory + @"xmlvalidate\adjustvatinvoice.xsd"))
            {
                log.Error("ERR:3-" + validator.ValidationError);
                return Ok("ERR:3-" + validator.ValidationError);  //du lieu dau vao khong hop le
            }

            Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int comID = _currentCompany.id;
            if (_currentCompany == null) return Ok("ERR:4");
            IInvoiceService invSrv = InvServiceFactory.GetService(data.OriPattern, comID);
            IInvoice OriInv = invSrv.GetByNo(comID, data.OriPattern, data.OriSerial, data.OriNo);
            if (!checkInvNumber(comID, OriInv.Pattern, OriInv.Serial))
            {
                log.Error("het so hoa don trong dai.");
                return Ok<string>("ERR:6");   //het so hoa don trong dai
            }
            IRegisterTempService regisSrv = IoC.Resolve<IRegisterTempService>();

            ICustomerService _cusSvr = IoC.Resolve<ICustomerService>();
            XElement xeles = XElement.Parse(data.InvData);

            IInvoice inv = (InvoiceBase)InvServiceFactory.NewInstance(OriInv.Pattern, comID);
            string read = data.InvData;
            DataHelper.DeserializeEinvFromXML(read, inv);
            inv.No = 0;
            inv.Name = OriInv.Name;
            inv.Pattern = OriInv.Pattern;
            inv.Serial = OriInv.Serial;
            inv.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
            inv.ProcessInvNote = "";
            DateTime dt = OriInv.ArisingDate;

            string strAdjust = (_currentCompany.Config.Keys.Contains("AdjustString")) ? _currentCompany.Config["AdjustString"] : "@AdjustType cho hóa đơn số: @InvNo, Ký hiệu hóa đơn: @InvSerial, Ngày @InvDay Tháng @InvMonth Năm @InvYear";
            strAdjust = strAdjust.Replace("@AdjustType", Utils.GetNameInvoiceType(inv.Type)).Replace("@InvNo", OriInv.No.ToString(Utils.MAX_NO_LENGTH)).Replace("@InvSerial", OriInv.Serial).Replace("@InvDay", dt.Day.ToString()).Replace("@InvMonth", dt.Month.ToString()).Replace("@InvYear", dt.Year.ToString());
            inv.ProcessInvNote = strAdjust;
            inv.Type = (InvoiceType)data.InvType;
            var Typecus = (from c in _cusSvr.Query where c.ComID == comID && c.Code == inv.CusCode && c.CusType == 1 select c.CusType).SingleOrDefault();
            if (Typecus == 0)
                inv.CusSignStatus = cusSignStatus.NocusSignStatus;
            else
                inv.CusSignStatus = cusSignStatus.NoSignStatus;
            if (OriInv.Status == InvoiceStatus.SignedInv || OriInv.Status == InvoiceStatus.AdjustedInv || OriInv.Status == InvoiceStatus.InUseInv)
            {
                byte[] raw = Convert.FromBase64String(data.CertBase64String);
                X509Certificate2 x509Cert = new X509Certificate2(raw);
                IDictionary<string, string> dict = EInvoice.Core.Launching.Launcher.Instance.GetDigestForRemote(data.OriPattern, data.OriSerial, new IInvoice[] { inv }, x509Cert);
                return Ok(dict);
            }
            return NotFound();
        }

        [APIAuthenticate(Permissions = "Search_Adjust")]
        [Route("adjustInvoice")]
        [HttpPost]
        public IHttpActionResult RemoteAdjustPublish(RemoteAdjustInvoice data)
        {
            try
            {
                Company currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                XElement elem = XElement.Parse(data.InvData);
                IEnumerable<XElement> listTemp = elem.Elements("Data");
                IDictionary<string, byte[]> dictInv = new Dictionary<string, byte[]>();
                foreach (XElement e in listTemp)
                {
                    string id = e.Element("id").Value;
                    string Signed = e.Element("Signed").Value;
                    dictInv.Add(id, Convert.FromBase64String(Signed));
                }
                IInvoiceService invSrv = InvServiceFactory.GetService(data.OriPattern, currentCompany.id);
                IInvoice OriInv = invSrv.GetByNo(currentCompany.id, data.OriPattern, data.OriSerial, data.OriNo);
                IInvoice p = EInvoice.Core.Launching.Launcher.Instance.AdjustRemote(OriInv, data.OriPattern, data.OriSerial, dictInv);
                return Ok(new DataUpdate() { Data = p.Data, No = p.No, Serial = p.Serial });
            }
            catch (Exception ex)
            {
                return Ok("ERROR");
            }
        }

        [APIAuthenticate(Permissions = "Search_Replace")]
        [Route("digestForReplace")]
        [HttpPost]
        public IHttpActionResult GetDigestForReplace(RemoteAdjustInvoice data)
        {
            ILog log = LogManager.GetLogger(typeof(RemoteLauncherController));
            XmlSchemaValidator validator = new XmlSchemaValidator();
            if (!validator.ValidXmlDoc(data.InvData, "", AppDomain.CurrentDomain.BaseDirectory + @"xmlvalidate\replacevatinvoice.xsd"))
            {
                log.Error("ERR:3-" + validator.ValidationError);
                return Ok("ERR:3-" + validator.ValidationError);  //du lieu dau vao khong hop le
            }

            Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int comID = _currentCompany.id;
            if (_currentCompany == null) return Ok("ERR:4");
            IInvoiceService invSrv = InvServiceFactory.GetService(data.OriPattern, comID);
            IInvoice OriInv = invSrv.GetByNo(comID, data.OriPattern, data.OriSerial, data.OriNo);
            if (!checkInvNumber(comID, OriInv.Pattern, OriInv.Serial))
            {
                log.Error("het so hoa don trong dai.");
                return Ok<string>("ERR:6");   //het so hoa don trong dai
            }
            IRegisterTempService regisSrv = IoC.Resolve<IRegisterTempService>();

            ICustomerService _cusSvr = IoC.Resolve<ICustomerService>();

            IInvoice inv = (InvoiceBase)InvServiceFactory.NewInstance(OriInv.Pattern, comID);
            string read = data.InvData;//string.Concat(invs.ElementAt(i));
            DataHelper.DeserializeEinvFromXML(read, inv);
            inv.No = 0;
            inv.Name = OriInv.Name;
            inv.Pattern = OriInv.Pattern;
            inv.Serial = OriInv.Serial;
            inv.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
            inv.ProcessInvNote = "";
            string strReplace = (_currentCompany.Config.ContainsKey("ReplaceString")) ? _currentCompany.Config["ReplaceString"] : "Hóa đơn này thay thế hóa đơn số: @InvNo, Ký hiệu: @InvSerial, Gửi ngày @InvDay Tháng @InvMonth Năm @InvYear";
            strReplace = strReplace.Replace("@InvNo", OriInv.No.ToString(Utils.MAX_NO_LENGTH)).Replace("@InvSerial", OriInv.Serial).Replace("@InvDay", OriInv.ArisingDate.Day.ToString()).Replace("@InvMonth", OriInv.ArisingDate.Month.ToString()).Replace("@InvYear", OriInv.ArisingDate.Year.ToString());
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
                byte[] raw = Convert.FromBase64String(data.CertBase64String);
                X509Certificate2 x509Cert = new X509Certificate2(raw);
                IDictionary<string, string> dict = EInvoice.Core.Launching.Launcher.Instance.GetDigestForRemote(data.OriPattern, data.OriSerial, new IInvoice[] { inv }, x509Cert);
                return Ok(dict);
            }
            return NotFound();
        }

        [APIAuthenticate(Permissions = "Search_Replace")]
        [Route("replaceInvoice")]
        [HttpPost]
        public IHttpActionResult RemoteReplacePublish(RemoteAdjustInvoice data)
        {
            try
            {
                Company currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                XElement elem = XElement.Parse(data.InvData);
                IEnumerable<XElement> listTemp = elem.Elements("Data");
                IDictionary<string, byte[]> dictInv = new Dictionary<string, byte[]>();
                foreach (XElement e in listTemp)
                {
                    string id = e.Element("id").Value;
                    string Signed = e.Element("Signed").Value;
                    dictInv.Add(id, Convert.FromBase64String(Signed));
                }
                IInvoiceService invSrv = InvServiceFactory.GetService(data.OriPattern, currentCompany.id);
                IInvoice OriInv = invSrv.GetByNo(currentCompany.id, data.OriPattern, data.OriSerial, data.OriNo);
                IInvoice p = EInvoice.Core.Launching.Launcher.Instance.ReplaceRemote(OriInv, data.OriPattern, data.OriSerial, dictInv);
                return Ok(new DataUpdate() { Data = p.Data, No = p.No, Serial = p.Serial });
            }
            catch (Exception ex)
            {
                return Ok("ERROR");
            }
        }

        [APIAuthenticate(Permissions = "CancelInv")]
        [Route("cancelInvoice")]
        [HttpPost]
        public IHttpActionResult RemoteCancel(RemoteInvoice data)
        {
            try
            {
                Company currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                XElement elem = XElement.Parse(data.InvData);
                IEnumerable<XElement> listTemp = elem.Elements("key");
                IDictionary<string, byte[]> dictInv = new Dictionary<string, byte[]>();
                IInvoiceService IInvSrv = InvServiceFactory.GetService(data.InvPattern, currentCompany.id);
                var context = (EInvoiceContext)FXContext.Current;
                ILogSystemService businessLog = IoC.Resolve<ILogSystemService>();
                int succCount = 0;
                foreach (var item in listTemp)
                {
                    IInvoice Oinvoice = IInvSrv.GetByFkey(currentCompany.id, item.Value);
                    if (Oinvoice == null || Oinvoice.Status == InvoiceStatus.CanceledInv || Oinvoice.Status == InvoiceStatus.ReplacedInv || Oinvoice.Status == InvoiceStatus.AdjustedInv)
                        continue;
                    succCount++;
                    Oinvoice.Status = InvoiceStatus.CanceledInv;
                    Oinvoice.Note += "  || Thực hiện hủy Hóa đơn (Không thay thế):   Người hủy:" + context.CurrentUser.username + "   Ngày hủy:" + DateTime.Now.ToString();
                    IInvSrv.Update(Oinvoice);
                    businessLog.WriteLogCancel(currentCompany.id, context.CurrentUser.username, Oinvoice.Pattern, Oinvoice.Serial, Oinvoice.No.ToString("0000000"), Oinvoice.ArisingDate, Oinvoice.Amount, Oinvoice.CusName, Oinvoice.CusAddress, Oinvoice.CusCode, Oinvoice.CusTaxCode, BusinessLogType.Cancel);
                }
                if (succCount == 0)
                {
                    return Ok<string>("ERROR:Kiểm tra lại hóa đơn hủy, có thể đã được thanh toán hoặc sửa đổi.");
                }
                else
                {
                    IInvSrv.CommitTran();
                    return Ok<string>("OK:Tổng số hóa đơn hủy là:" + succCount);
                }                
            }
            catch (Exception ex)
            {
                return Ok("ERROR");
            }
        }


        private int getMaxInvNumber(int comID, string pattern, string serial)
        {
            int rv = 0;
            IPublishInvoiceService _srv = IoC.Resolve<IPublishInvoiceService>();
            IList<PublishInvoice> LstPubInvoice = _srv.GetbyPattern(comID, pattern, 1, 2).Where(item => item.InvSerial == serial).ToList();
            if (LstPubInvoice == null || LstPubInvoice.Count == 0) throw new Exception("No publishInvoice for Notransaction.");
            foreach (PublishInvoice p in LstPubInvoice)
            {
                rv += (int)(p.ToNo - p.CurrentNo);
                if (p.CurrentNo == 0) rv = rv - 1;
                _srv.UnbindSession(p);
            }
            return rv;
        }

        private bool checkInvNumber(int ComID, string pattern, string serial)
        {
            IPublishInvoiceService _pubSrv = IoC.Resolve<IPublishInvoiceService>();
            decimal MaxNo = (from pub in _pubSrv.Query where (pub.ComId == ComID && pub.InvPattern == pattern && pub.InvSerial == serial && (pub.Status == 1 || pub.Status == 2)) select pub.ToNo).Max();
            decimal CurrentNo = (from opub in _pubSrv.Query where (opub.ComId == ComID && opub.InvPattern == pattern && opub.InvSerial == serial && (opub.Status == 1 || opub.Status == 2)) select opub.CurrentNo).Max();
            if (MaxNo > CurrentNo)
            {
                return true;
            }
            return false;
        }
    }
}