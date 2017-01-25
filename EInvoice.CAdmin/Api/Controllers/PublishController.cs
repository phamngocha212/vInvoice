using System.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Security;
using System.Xml.Linq;
using EInvoice.Core;
using EInvoice.Core.Launching;
using EInvoice.Core.Domain;
using EInvoice.Core.IService;
using IdentityManagement.Domain;
using IdentityManagement.Service;
using log4net;
using FX.Context;
using FX.Core;
using EInvoice.CAdmin;
using EInvoice.CAdmin.Api;
using System.Collections;
using IdentityManagement.Authorization;
using System.Web;
using System.Net.Http;
using FX.Utils.EmailService;
using EInvoice.CAdmin.IService;

namespace EInvoice.Api.Controllers
{
    [RoutePrefix("api/publish")]
    public class PublishController : ApiController
    {
        /// <summary>
        /// Tạo và phát hành danh sách hóa đơn
        /// </summary>
        /// <param name="publish">{"username":"","password":"","xmlInvData":"","pattern":"", "serial":""}</param>
        /// <returns>String kết quả</returns>
        [Route("importAndPublishInv")]
        [HttpPost]
        [APIAuthenticate(Permissions = "Release_invInList")]
        public IHttpActionResult ImportAndPublishInv(ApiPublish publish)
        {
            ILog log = LogManager.GetLogger(typeof(PublishController));
            //check valiadate xml
            XmlSchemaValidator validator = new XmlSchemaValidator();
            //xmlInvData = convertSpecialCharacter(xmlInvData);
            if (!validator.ValidXmlDoc(publish.xmlData, "", AppDomain.CurrentDomain.BaseDirectory + @"xmlvalidate\VATInVoice_ws.xsd"))
            {
                log.Error(publish.xmlData);
                log.Error("ERR:3-" + validator.ValidationError);
                return Ok("ERR:3-" + validator.ValidationError);  //du lieu dau vao khong hop le
            }

            if (publish.convert == 1)
            {
                publish.xmlData = DataHelper.convertTCVN3ToUnicode(publish.xmlData);
            }

            Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IDeliver _DeliverService = _currentCompany.Config.ContainsKey("IDeliver") ? InvServiceFactory.GetDeliver(_currentCompany.Config["IDeliver"]) : null;
            int comID = _currentCompany.id;
            if (_currentCompany == null) return NotFound();
            IPublishInvoiceService _PubInvSrv = IoC.Resolve<IPublishInvoiceService>();
            int patternNull = string.IsNullOrEmpty(publish.pattern) ? 0 : 1;
            int serialNull = string.IsNullOrEmpty(publish.serial) ? 0 : 1;
            string nameInvoice = "";

            switch ((patternNull + serialNull))
            {
                case 0:
                    PublishInvoice pubinv = _PubInvSrv.GetFirst(comID, new int[] { 1, 2 });
                    if (pubinv != null)
                    {
                        IRegisterTempService regisSrv = IoC.Resolve<IRegisterTempService>();
                        RegisterTemp rtemp = regisSrv.Getbykey(pubinv.RegisterID);
                        publish.pattern = pubinv.InvPattern;
                        publish.serial = pubinv.InvSerial;
                        nameInvoice = rtemp.NameInvoice;
                        _PubInvSrv.UnbindSession(pubinv);
                        regisSrv.UnbindSession(rtemp);
                    }
                    else return Ok("ERR:20"); //tham so pattern va serial khong hop le
                    break;
                case 1:
                    PublishInvoice pubFirst = null;
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(publish.pattern))
                            pubFirst = (from p in _PubInvSrv.Query where (p.ComId == comID) && (p.InvPattern == publish.pattern) && (p.Status == 1 || p.Status == 2) select p).FirstOrDefault();
                        else
                            pubFirst = (from p in _PubInvSrv.Query where (p.ComId == comID) && (p.InvSerial == publish.serial) && (p.Status == 1 || p.Status == 2) select p).FirstOrDefault();
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        return Ok("ERR:20");   //khong can return cung duoc
                    }
                    if (pubFirst == null)
                    {
                        return Ok("ERR:20");
                    }
                    else
                    {
                        IRegisterTempService regisSrv = IoC.Resolve<IRegisterTempService>();
                        RegisterTemp rtemp = regisSrv.Getbykey(pubFirst.RegisterID);
                        nameInvoice = rtemp.NameInvoice;
                        publish.pattern = pubFirst.InvPattern;
                        publish.serial = pubFirst.InvSerial;
                        _PubInvSrv.UnbindSession(pubFirst);
                        regisSrv.UnbindSession(rtemp);
                    }
                    break;
                case 2:
                    // PublishInvoice pub = _PubInvSrv.CurrentPubInv(comID, pattern, serial, 1);
                    PublishInvoice pub = null;
                    try
                    {
                        pub = (from p in _PubInvSrv.Query where (p.ComId == comID) && (p.InvPattern == publish.pattern) && (p.InvSerial == publish.serial) && (p.Status == 1 || p.Status == 2) select p).First();
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        return Ok("ERR:20");    //khong can return cung duoc
                    }
                    if (pub == null)
                        return Ok("ERR:20");
                    else
                    {
                        IRegisterTempService regisSrv = IoC.Resolve<IRegisterTempService>();
                        RegisterTemp rtemp = regisSrv.Getbykey(pub.RegisterID);
                        nameInvoice = rtemp.NameInvoice;
                        _PubInvSrv.UnbindSession(pub);
                        regisSrv.UnbindSession(rtemp);
                    }
                    break;
            }
            log.Info("pattern-serial: " + publish.pattern + "-" + publish.serial);
            XElement elem = XElement.Parse(publish.xmlData);
            IList<IInvoice> lstINV = new List<IInvoice>();
            string invKeyAndNumList = "";
            IEnumerable<XElement> listTemp = elem.Elements("Inv");
            IList<String> lstKey = new List<String>();

            //check xem con du so hoa don ko
            if (getMaxInvNumber(comID, publish.pattern, publish.serial) < listTemp.Count())
            {
                return Ok("ERR:6");    //khong con du so hoa don cho lo phat hanh
            }
            //check xem lo truyen vao lon hon MaxBlockInv hay ko
            int maxBlockInv;
            string temp = System.Configuration.ConfigurationManager.AppSettings["MaxBlockInv"];
            if (string.IsNullOrEmpty(temp) || !Int32.TryParse(temp, out maxBlockInv))
            {
                maxBlockInv = 5000;
            }
            if (listTemp.Count() > maxBlockInv)
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
                    IInvoice inv = InvServiceFactory.NewInstance(publish.pattern, comID);
                    string read = string.Concat(ele);
                    inv.DeserializeFromXML(read);
                    inv.Name = nameInvoice;
                    inv.Pattern = publish.pattern;
                    inv.Serial = publish.serial;
                    inv.CreateBy = inv.CreateBy ?? System.Web.HttpContext.Current.User.Identity.Name;
                    inv.Fkey = fKey;
                    lstINV.Add(inv);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return Ok("ERR:5");//loi phat hanh hoa don
            }

            try
            {
                Launcher.Instance.Launch(publish.pattern, publish.serial, lstINV.ToArray());
                if (_DeliverService != null)
                    _DeliverService.PrepareDeliver(lstINV.ToArray(), _currentCompany);
                for (int i = 0; i < lstINV.Count; i++)
                {
                    invKeyAndNumList += lstKey[i] + "_" + lstINV[i].No + ",";
                }
                //remove the last "," character
                invKeyAndNumList = invKeyAndNumList.Remove(invKeyAndNumList.Length - 1, 1);
                return Ok("OK:" + publish.pattern + ";" + publish.serial + "-" + invKeyAndNumList);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return Ok("ERR:13");
            }
        }

        [Route("importInv")]
        [HttpPost]
        [APIAuthenticate(Permissions = "Release_invInList")]
        public IHttpActionResult ImportInv(ApiPublish publish)
        {
            ILog log = LogManager.GetLogger(typeof(PublishController));

            XmlSchemaValidator validator = new XmlSchemaValidator();
            //xmlInvData = convertSpecialCharacter(xmlInvData);
            if (!validator.ValidXmlDoc(publish.xmlData, "", AppDomain.CurrentDomain.BaseDirectory + @"xmlvalidate\VATInVoice_ws.xsd"))
            {
                //customer xml string not valid, don't do any thing
                log.Error("ERR3: " + validator.ValidationError);
                return Ok("ERR:3");
            }

            if (publish.convert == 1)
            {
                publish.xmlData = DataHelper.convertTCVN3ToUnicode(publish.xmlData);
            }

            Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int comID = _currentCompany.id;
            IPublishInvoiceService _PubInvSrv = IoC.Resolve<IPublishInvoiceService>();
            string nameInvoice = "", serial = null;

            int patternNull = string.IsNullOrEmpty(publish.pattern) ? 0 : 1;
            switch (patternNull)
            {
                case 0:
                    PublishInvoice pubinv = _PubInvSrv.GetFirst(comID, new int[] { 1, 2 });
                    if (pubinv != null)
                    {
                        IRegisterTempService regisSrv = IoC.Resolve<IRegisterTempService>();
                        RegisterTemp rtemp = regisSrv.Getbykey(pubinv.RegisterID);
                        publish.pattern = pubinv.InvPattern;
                        nameInvoice = rtemp.NameInvoice;
                        serial = pubinv.InvSerial;
                        _PubInvSrv.UnbindSession(pubinv);
                        regisSrv.UnbindSession(rtemp);
                    }
                    else return Ok("ERR:20"); //tham so pattern va serial khong hop le
                    break;
                case 1:
                    PublishInvoice pub = null;
                    try
                    {
                        pub = (from p in _PubInvSrv.Query where (p.ComId == comID) && (p.InvPattern == publish.pattern) && (p.Status == 1 || p.Status == 2) select p).FirstOrDefault();
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        return Ok("ERR:20");   //khong can return cung duoc
                    }
                    if (pub == null)
                    {
                        return Ok("ERR:20");
                    }
                    else
                    {
                        IRegisterTempService regisSrv = IoC.Resolve<IRegisterTempService>();
                        RegisterTemp rtemp = regisSrv.Getbykey(pub.RegisterID);
                        nameInvoice = rtemp.NameInvoice;
                        serial = pub.InvSerial;
                        _PubInvSrv.UnbindSession(pub);
                        regisSrv.UnbindSession(rtemp);
                    }
                    break;
            }

            XElement elem = XElement.Parse(publish.xmlData);
            IList<IInvoice> lstINV = new List<IInvoice>();
            string invKeyAndNumList = "";
            IEnumerable<XElement> listTemp = elem.Elements("Inv");
            IList<String> lstKey = new List<String>();

            //check xem lo truyen vao lon hon MaxBlockInv hay ko
            int maxBlockInv;
            string temp = System.Configuration.ConfigurationManager.AppSettings["MaxBlockInv"];
            if (string.IsNullOrEmpty(temp) || !Int32.TryParse(temp, out maxBlockInv))
            {
                maxBlockInv = 5000;
            }
            if (listTemp.Count() > maxBlockInv)
            {
                return Ok("ERR:10");    // lo hoa don truyen vao lon hon maxBlockInv;
            }
            try
            {
                int cc = listTemp.Count();
                foreach (XElement e in listTemp)
                {
                    XElement ele = e.Element("Invoice");
                    string fKey = e.Element("key").Value.Trim();
                    lstKey.Add(fKey);

                    IInvoice inv = InvServiceFactory.NewInstance(publish.pattern, comID);
                    string read = string.Concat(ele);
                    inv.DeserializeFromXML(read);
                    inv.No = 0;
                    inv.Name = nameInvoice;
                    inv.Pattern = publish.pattern;
                    inv.Serial = serial;
                    inv.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
                    inv.Fkey = fKey;
                    lstINV.Add(inv);
                }
            }
            catch (Exception ex)
            {
                log.Error("ERR:5-", ex);
                return Ok("ERR:5");//loi phat hanh hoa don
            }

            string mess = "";
            bool isImport = false;
            IInvoiceService t = EInvoice.Core.InvServiceFactory.GetService(publish.pattern, _currentCompany.id);
            t.BeginTran();
            try
            {
                if (lstINV.Count > 50)
                    t.isStateLess = true;
                isImport = t.CreateInvoice(lstINV.ToArray(), out mess);
                t.CommitTran();
            }
            catch (Exception ex)
            {
                t.RolbackTran();
                log.Error(ex);
                return Ok("ERR:5");
            }
            finally
            {
                t.isStateLess = false;
            }
            if (isImport)
            {
                for (int i = 0; i < lstINV.Count; i++)
                {
                    invKeyAndNumList += lstKey[i] + ",";
                }
                //remove the last "," character
                invKeyAndNumList = invKeyAndNumList.Remove(invKeyAndNumList.Length - 1, 1);
                return Ok("OK:" + publish.pattern + ";" + serial + "-" + invKeyAndNumList);
            }
            log.Error(mess);
            return Ok("ERR:5");
        }

        /// <summary>
        /// Phat hanh danh sach hoa don
        /// </summary>
        /// <param name="publish">{danh sach id, pattern, serial}</param>
        /// <returns></returns>
        [Route("publishInv")]
        [HttpPost]
        [APIAuthenticate(Permissions = "Release_invInList")]
        public string publishInv(ApiPublish publish)
        {
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            ILog log = LogManager.GetLogger(typeof(PublishController));
            if (!LockTable.Contains(String.Format("{0}${1}", publish.pattern, currentCom.id)))
            {
                object lockobj = new object();
                LockTable.Add(String.Format("{0}${1}", publish.pattern, currentCom.id), lockobj);
            }
            lock (LockTable[String.Format("{0}${1}", publish.pattern, currentCom.id)])
            {
                IDeliver _DeliverService = currentCom.Config.ContainsKey("IDeliver") ? InvServiceFactory.GetDeliver(currentCom.Config["IDeliver"]) : null;
                IInvoiceService IInvSrv = InvServiceFactory.GetService(publish.pattern, currentCom.id);
                IList<IInvoice> lst = IInvSrv.GetByID(currentCom.id, publish.invIDs).OrderBy(p => p.CreateDate).ToList();
                try
                {
                    if (lst.Count() <= 50)
                        Launcher.Instance.Launch(publish.pattern, publish.serial, lst.ToArray());
                    else
                    {
                        for (int i = 0; i < lst.Count() / 50; i++)
                        {
                            Launcher.Instance.Launch(publish.pattern, publish.serial, lst.Skip(i * 50).Take(50).ToArray());
                        }
                    }
                    try
                    {
                        if (_DeliverService != null)
                            _DeliverService.PrepareDeliver(lst.ToArray(), currentCom);
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                    return "OK";
                }
                catch (EInvoice.Core.Launching.NoFactory.OpenTranException ex)
                {
                    log.Error(ex);
                    return "ERR:14";
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    return "ERR:5 " + ex.Message;//loi phat hanh hoa don
                }
            }
        }

        [Route("updateCus")]
        [HttpPost]
        [APIAuthenticate(Permissions = "Add_cus")]
        public string UpdateCus(ApiPublish publish)
        {
            IuserService _userSvc = IoC.Resolve<IuserService>();
            ICustomerService _cusSvc = IoC.Resolve<ICustomerService>();
            IApplicationsService _appSvc = IoC.Resolve<IApplicationsService>();
            IRegisterEmailService _emailSvc = IoC.Resolve<IRegisterEmailService>();
            ILog log = LogManager.GetLogger(typeof(PublishController));

            try
            {
                log.Info("updateCus DATA:" + publish.xmlData);

                //check valiadate xml
                XmlSchemaValidator validator = new XmlSchemaValidator();
                //XMLCusData = convertSpecialCharacter(XMLCusData);
                if (!validator.ValidXmlDoc(publish.xmlData, "", AppDomain.CurrentDomain.BaseDirectory + @"XMLValidate\CustomerValidate.xsd"))
                {
                    log.Error("updateCus ERR:3-UpdateCus " + validator.ValidationError);
                    return "ERR:3";//du lieu dau vao khong hop le
                }

                if (publish.convert == 1)
                {
                    publish.xmlData = DataHelper.convertTCVN3ToUnicode(publish.xmlData);
                }

                Applications _app = _appSvc.GetByName("EInvoice");  //Chu y fix cung phu hop voi services.config/IRBACMembershipProvider

                EInvoiceContext _Einvoicecontext = (EInvoiceContext)FXContext.Current;
                Company _currentCom = _Einvoicecontext.CurrentCompany;

                // get dữ liệu từ string xml
                XElement xelement = XElement.Parse(publish.xmlData);

                List<XElement> cusLst = (from item in xelement.Elements("Customer") select item).ToList();
                List<string> codeLst = (from item in cusLst select item.Element("Code").Value).ToList();

                string defaultPass = (_currentCom.Config.ContainsKey("SetDefaultCusPass")) ? _currentCom.Config["SetDefaultCusPass"] : "Hddt123456";
                string labelEmail = _currentCom.Config.Keys.Contains("LabelMail") ? _currentCom.Config["LabelMail"] : "hoadondientu@v-invoice.vn";
                string portalLink = _currentCom.Config.Keys.Contains("PortalLink") ? _currentCom.Config["PortalLink"] : "http://hddt.v-invoice.vn";

                _cusSvc.BeginTran();
                StringBuilder msg = new StringBuilder();
                foreach (var item in cusLst)
                {
                    try
                    {
                        string code = item.Element("Code").Value.Trim();
                        var taxCode = Utils.formatTaxcode(item.Element("TaxCode").Value.Trim());

                        var cusDb = _cusSvc.Query.FirstOrDefault(x => x.Code == code && x.ComID == _currentCom.id);
                        var userDb = _userSvc.Query.FirstOrDefault(x => x.username == code && x.GroupName == _currentCom.id.ToString());

                        // create
                        if (cusDb == null)
                        {
                            if (userDb != null)
                            {
                                msg.AppendFormat("Ma KH: {0}. Khong the tao khach hang do user {0} da ton tai trong he thong.", code).AppendLine();
                                log.WarnFormat("Ma KH: {0}. Khong the tao khach hang do user {0} da ton tai trong he thong.", code);
                                continue;
                            }

                            if (!String.IsNullOrWhiteSpace(taxCode) && _cusSvc.Query.Any(x => x.TaxCode == taxCode && x.ComID == _currentCom.id))
                            {
                                msg.AppendFormat("Ma KH: {0}. Khong the tao khach hang do ma so thue {0} da ton tai trong he thong.", taxCode).AppendLine();
                                log.WarnFormat("Ma KH: {0}. Khong the tao khach hang do ma so thue {1} da ton tai trong he thong.", code, taxCode);
                                continue;
                            }

                            var cus = new Customer
                            {
                                Code = code,
                                AccountName = code,
                                TaxCode = taxCode,
                                Name = item.Element("Name").Value,
                                Address = item.Element("Address").Value,
                                BankAccountName = item.Element("BankAccountName").Value,
                                BankName = item.Element("BankName").Value,
                                BankNumber = item.Element("BankNumber").Value,
                                Email = item.Element("Email").Value,
                                Fax = item.Element("Fax").Value,
                                Phone = item.Element("Phone").Value,
                                ContactPerson = item.Element("ContactPerson").Value,
                                RepresentPerson = item.Element("RepresentPerson").Value,
                                CusType = String.IsNullOrWhiteSpace(item.Element("Name").Value) ? Int32.Parse(item.Element("Name").Value) : 0,
                                DeliverMethod = 2,
                                ComID = _currentCom.id
                            };

                            string createCusErr;
                            if (_cusSvc.CreateCus(cus, new Certificate(), _currentCom.id, out createCusErr))
                            {
                                log.Info("updateCus Create Customer by: " + HttpContext.Current.User.Identity.Name + " Info-- TenKhachHang: " + cus.Name + " TaiKhoanKhachHang: " + cus.AccountName + " Email: " + cus.Email);
                                // send Mail--
                                try
                                {
                                    if (!string.IsNullOrEmpty(cus.Email))
                                    {
                                        Dictionary<string, string> subjectParams = new Dictionary<string, string>(1);
                                        subjectParams.Add("$subject", "");
                                        Dictionary<string, string> bodyParams = new Dictionary<string, string>(3);
                                        bodyParams.Add("$company", _currentCom.Name);
                                        bodyParams.Add("$cusname", cus.Name);
                                        bodyParams.Add("$username", cus.AccountName);
                                        bodyParams.Add("$password", defaultPass);
                                        bodyParams.Add("$portalLink", portalLink);
                                        _emailSvc.ProcessEmail(labelEmail, cus.Email, "RegisterCustomer", subjectParams, bodyParams);
                                    }
                                }
                                catch (Exception ex)
                                { log.Warn("updateCus", ex); }
                            }
                            else
                            {
                                msg.AppendFormat("Ma KH: {0}. Khong the tao khach hang {0}. Thong bao loi: {1}", code, createCusErr).AppendLine();
                                log.Warn("updateCus Error: " + createCusErr);
                            }
                        }
                        // update
                        else
                        {
                            // update taxcode
                            if(!String.IsNullOrWhiteSpace(taxCode) && cusDb.TaxCode!= taxCode)
                            {
                                if (_cusSvc.Query.Any(x => x.TaxCode == taxCode && x.ComID == _currentCom.id))
                                {
                                    msg.AppendFormat("Ma KH: {0}. Khong the chinh sua thong tin khach hang {0} do ma so thue {1} da ton tai trong he thong.", code, taxCode).AppendLine();
                                    continue;
                                }

                                cusDb.TaxCode = taxCode;
                            }

                            cusDb.Name = item.Element("Name").Value;
                            cusDb.Address = item.Element("Address").Value;
                            cusDb.BankAccountName = item.Element("BankAccountName").Value;
                            cusDb.BankName = item.Element("BankName").Value;
                            cusDb.BankNumber = item.Element("BankNumber").Value;
                            cusDb.Email = item.Element("Email").Value;
                            cusDb.Fax = item.Element("Fax").Value;
                            cusDb.Phone = item.Element("Phone").Value;
                            cusDb.ContactPerson = item.Element("ContactPerson").Value;
                            cusDb.RepresentPerson = item.Element("RepresentPerson").Value;
                            cusDb.CusType = String.IsNullOrWhiteSpace(item.Element("Name").Value) ? Int32.Parse(item.Element("Name").Value) : 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        _cusSvc.RolbackTran();
                        log.Error("Loi tao hoac cap nhat khach hang: " + item.Element("Code"), ex);
                        return "ERR:2";//loi cap nhat khach hang le vao csdl
                    }
                }

                _cusSvc.CommitTran();
                return "OK:" + msg;
            }
            catch (Exception ex)
            {
                log.Error("updateCus: " + ex);
                throw ex;
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

        private static Hashtable LockTable = new Hashtable();
    }
}
