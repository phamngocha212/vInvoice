using System.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Xml.Linq;
using EInvoice.Core;
using EInvoice.Core.Launching;
using EInvoice.Core.Domain;
using EInvoice.Core.IService;
using IdentityManagement.Domain;
using log4net;
using FX.Context;
using FX.Core;
using EInvoice.CAdmin;
using IdentityManagement.WebProviders;
using EInvoice.CAdmin.Api;
namespace EInvoice.CAdmin.Api.Controllers
{

    [RoutePrefix("api/business")]
    public class BusinessController : ApiController
    {
        ILog log = LogManager.GetLogger(typeof(BusinessController));

        /// <summary>
        /// Thay thế hóa đơn
        /// </summary>
        /// <param name="business">{"xmlInvData":"","account":"","password":"","fkey":"","Attachfile":""}</param>
        /// <returns></returns>
        [APIAuthenticate(Permissions = "Create_ajst")]
        [Route("replaceInv")]
        [HttpPost]
        public string replaceInv(Business business)
        {
            return processReplace(business.xmlData, business.pattern, business.fkey, business.serial, business.invNo, business.convert);
        }

        /// <summary>
        /// Điều chỉnh hóa đơn
        /// </summary>
        /// <param name="business">{"xmlInvData":"","account":"","password":"","fkey":"","Attachfile":""}</param>
        /// <returns></returns>
        [APIAuthenticate(Permissions = "Create_ajst")]
        [Route("adjustInv")]
        [HttpPost]
        public string adjustInv(Business business)
        {
            return processAdjust(business.xmlData, business.pattern, business.fkey, business.serial, business.invNo, business.convert);
        }

        /// <summary>
        /// Hủy hóa đơn
        /// </summary>
        /// <param name="business">{"xmlInvData":"","account":"","password":"","fkey":"","Attachfile":""}</param>
        /// <returns></returns>
        [APIAuthenticate(Permissions = "CancelInv")]
        [Route("cancelInv")]
        [HttpPost]
        public string cancelInv(Business business)
        {
            return processCancelInv(business.fkey, business.pattern, business.serial, business.invNo);
        }

        private string processCancelInv(string fkey, string pattern = null, string serial = null, decimal invNo = 0)
        {
            Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int comID = _currentCompany.id;
            if (_currentCompany == null) return "ERR:7";//username khong phu hop - ko tim thay company phu hop voi [username]

            int isPattern = string.IsNullOrEmpty(pattern) ? 0 : 1;
            IPublishInvoiceService _PubInvSrv = IoC.Resolve<IPublishInvoiceService>();
            switch ((isPattern))
            {
                case 0:
                    PublishInvoice pubinv = _PubInvSrv.GetFirst(comID, new int[] { 1, 2 });
                    if (pubinv != null)
                    {
                        pattern = pubinv.InvPattern;
                        serial = pubinv.InvSerial;
                        _PubInvSrv.UnbindSession(pubinv);
                    }
                    else return "ERR:20"; //tham so pattern va serial khong hop le
                    break;
                case 1:
                    PublishInvoice pubFirst = _PubInvSrv.GetbyPattern(comID, pattern, new int[] { 1, 2 }).FirstOrDefault();
                    if (pubFirst == null)
                    {
                        return "ERR:20";
                    }
                    else
                    {
                        serial = !string.IsNullOrWhiteSpace(serial) ? serial : pubFirst.InvSerial;
                        _PubInvSrv.UnbindSession(pubFirst);
                    }
                    break;
            }
            IInvoiceService iinvSrc = InvServiceFactory.GetService(pattern, comID);
            try
            {
                IInvoice currentInv = InvServiceFactory.NewInstance(pattern, comID);
                if (invNo > 0)
                    currentInv = iinvSrc.GetByNo(comID, pattern, serial, invNo);
                else
                    currentInv = iinvSrc.GetByFkey(comID, fkey);
                if (null == currentInv)
                {
                    return "ERR:2";  //khong ton tai hoa don
                }                
                Launcher t = Launcher.Instance;
                t.Cancel(new IInvoice[] { currentInv });
                ILogSystemService businessLog = IoC.Resolve<ILogSystemService>();
                businessLog.WriteLogCancel(comID, HttpContext.Current.User.Identity.Name, currentInv.Pattern, currentInv.Serial, currentInv.No.ToString("0000000"), currentInv.PublishDate, currentInv.Amount, currentInv.CusName, currentInv.CusAddress, currentInv.CusCode, currentInv.CusTaxCode, BusinessLogType.Cancel);
                businessLog.CommitChanges();
                return "OK:";
            }
            catch (Exception ex)
            {
                log.Error("cancelInv " + ex);
                return "ERR:6 " + ex.Message;
            }
        }

        private string processAdjust(string xmlData, string pattern, string fkey, string serial = null, decimal invNo = 0, int convert = 0)
        {
            if (FXContext.Current == null)
                return "ERR:1";
            try
            {
                XmlSchemaValidator validator = new XmlSchemaValidator();
                //xmlInvData = convertSpecialCharacter(xmlInvData);
                if (!validator.ValidXmlDoc(xmlData, "", AppDomain.CurrentDomain.BaseDirectory + @"xmlvalidate\adjustvatinvoice.xsd"))
                {
                    log.Info("adjustInv " + validator.ValidationError);
                    return "ERR:3 err:" + validator.ValidationError;  //du lieu dau vao khong hop le                    
                }
                if (convert == 1)
                {
                    xmlData = DataHelper.convertTCVN3ToUnicode(xmlData);
                }
                ICustomerService _cusSvr = IoC.Resolve<ICustomerService>();
                Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                int comID = _currentCompany.id;
                if (_currentCompany == null) return "ERR:7";//username khong phu hop - ko tim thay company phu hop voi [username]

                string invNumList = "";
                int isPattern = string.IsNullOrEmpty(pattern) ? 0 : 1;
                IPublishInvoiceService _PubInvSrv = IoC.Resolve<IPublishInvoiceService>();
                switch ((isPattern))
                {
                    case 0:
                        PublishInvoice pubinv = _PubInvSrv.GetFirst(comID, new int[] { 1, 2 });
                        if (pubinv != null)
                        {
                            pattern = pubinv.InvPattern;
                            serial = pubinv.InvSerial;
                            _PubInvSrv.UnbindSession(pubinv);
                        }
                        else return "ERR:20"; //tham so pattern va serial khong hop le
                        break;
                    case 1:
                        PublishInvoice pubFirst = _PubInvSrv.GetbyPattern(comID, pattern, new int[] { 1, 2 }).FirstOrDefault();
                        if (pubFirst == null)
                        {
                            return "ERR:20";
                        }
                        else
                        {
                            serial = !string.IsNullOrWhiteSpace(serial) ? serial : pubFirst.InvSerial;
                            _PubInvSrv.UnbindSession(pubFirst);
                        }
                        break;
                }

                IInvoiceService iinvSrc = InvServiceFactory.GetService(pattern, comID);
                IInvoice currentInv = InvServiceFactory.NewInstance(pattern, comID);
                if (invNo > 0)
                    currentInv = iinvSrc.GetByNo(comID, pattern, serial, invNo);
                else
                    currentInv = iinvSrc.GetByFkey(comID, fkey);
                if (null == currentInv)
                {
                    return "ERR:2";  //khong ton tai hoa don
                }

                string invPattern = null, invSerial = null, errorMessage = "";
                if (!LaunchInvoices.Instance.ExistNoInPubInv(comID, currentInv.Pattern, currentInv.Serial, out invPattern, out invSerial, out errorMessage))
                {
                    log.Error(errorMessage);
                    return "ERR:6 " + fkey;   //het so hoa don trong dai
                }
                try
                {
                    XElement xeles = XElement.Parse(xmlData);
                    InvoiceBase inv = (InvoiceBase)InvServiceFactory.NewInstance(invPattern, comID);
                    string read = xmlData;
                    DataHelper.DeserializeEinvFromXML(read, inv);
                    inv.No = 0;
                    inv.Name = currentInv.Name;
                    inv.Pattern = invPattern;
                    inv.Serial = invSerial;
                    inv.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;                    
                    XElement elem = XElement.Parse(xmlData);//invs.ElementAt(i);
                    //loai hoa don sua doi
                    if (elem.Element("Type") != null && !string.IsNullOrWhiteSpace(elem.Element("Type").Value))
                        inv.Type = (InvoiceType)Convert.ToInt32(elem.Element("Type").Value);
                    else inv.Type = (InvoiceType)0;
                    //inv.Type = 0;
                    var Typecus = (from c in _cusSvr.Query where c.ComID == comID && c.Code == inv.CusCode && c.CusType == 1 select c.CusType).SingleOrDefault();
                    if (Typecus == 0)
                    {
                        inv.CusSignStatus = cusSignStatus.NocusSignStatus;
                    }
                    else
                    {
                        inv.CusSignStatus = cusSignStatus.NoSignStatus;
                    }
                    if (currentInv.Status == InvoiceStatus.SignedInv || currentInv.Status == InvoiceStatus.AdjustedInv || inv.Status == InvoiceStatus.InUseInv)
                    {
                        IList<ProductInv> products = new List<ProductInv>();
                        foreach (IProductInv ii in inv.Products)
                        {
                            products.Add((ProductInv)ii);
                        }
                        Launcher.Instance.PublishAdjust(currentInv, products, (InvoiceBase)inv, "");
                        invNumList += inv.Fkey ?? "";
                        ILogSystemService businessLog = IoC.Resolve<ILogSystemService>();
                        businessLog.WriteLogReplaceAdjust(comID, currentInv.Pattern, currentInv.Serial, currentInv.No, currentInv.PublishDate, currentInv.Amount, inv.Pattern, inv.Serial, inv.No, inv.PublishDate, inv.Amount, inv.CusName, inv.CusAddress, inv.CusCode, inv.CusTaxCode, HttpContext.Current.User.Identity.Name, BusinessLogType.Adjust);
                        businessLog.CommitChanges();
                    }
                    else return "ERR:9 " + currentInv.Status;

                }
                catch (EInvoice.Core.Launching.NoFactory.OpenTranException ex)
                {
                    return "ERR:14";
                }
                catch (Exception ex)
                {
                    //iinvSrc.CommitTran();
                    log.Error("adjustInv err5: " + ex);
                    return "ERR:5 " + ex.Message;//loi phat hanh hoa don
                }
                //invNumList = invNumList.Remove(invNumList.Length - 1, 1);
                return "OK_" + invNumList;
            }
            catch (Exception ex)
            {
                log.Error("adjustInv " + ex);
                return "ERR:5 " + ex.Message;
            }
        }

        private string processReplace(string xmlData, string pattern, string fkey, string serial = null, decimal invNo = 0, int convert = 0)
        {
            if (FXContext.Current == null)
                return "ERR:1";
            try
            {
                XmlSchemaValidator validator = new XmlSchemaValidator();
                if (!validator.ValidXmlDoc(xmlData, "", AppDomain.CurrentDomain.BaseDirectory + @"xmlvalidate\replacevatinvoice.xsd"))
                {
                    //customer xml string not valid, don't do any thing
                    log.Info("processReplace " + validator.ValidationError);
                    return "ERR:3 err:" + validator.ValidationError;  //du lieu dau vao khong hop le                    
                }

                if (convert == 1)
                    xmlData = DataHelper.convertTCVN3ToUnicode(xmlData);
                Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                int comID = _currentCompany.id;
                if (_currentCompany == null) return "ERR:7";//username khong phu hop - ko tim thay company phu hop voi [username]

                ICustomerService _cusSvr = IoC.Resolve<ICustomerService>();

                string invNumList = "";

                int isPattern = string.IsNullOrEmpty(pattern) ? 0 : 1;
                IPublishInvoiceService _PubInvSrv = IoC.Resolve<IPublishInvoiceService>();
                switch ((isPattern))
                {
                    case 0:
                        PublishInvoice pubinv = _PubInvSrv.GetFirst(comID, new int[] { 1, 2 });
                        if (pubinv != null)
                        {
                            pattern = pubinv.InvPattern;
                            serial = pubinv.InvSerial;
                            _PubInvSrv.UnbindSession(pubinv);
                        }
                        else return "ERR:20"; //tham so pattern va serial khong hop le
                        break;
                    case 1:
                        PublishInvoice pubFirst = _PubInvSrv.GetbyPattern(comID, pattern, new int[] { 1, 2 }).FirstOrDefault();
                        if (pubFirst == null)
                        {
                            return "ERR:20";
                        }
                        else
                        {
                            serial = !string.IsNullOrWhiteSpace(serial) ? serial : pubFirst.InvSerial;
                            _PubInvSrv.UnbindSession(pubFirst);
                        }
                        break;
                }

                IInvoiceService iinvSrc = InvServiceFactory.GetService(pattern, comID);
                IInvoice currentInv = InvServiceFactory.NewInstance(pattern, comID);
                if (invNo > 0)
                    currentInv = iinvSrc.GetByNo(comID, pattern, serial, invNo);
                else
                    currentInv = iinvSrc.GetByFkey(comID, fkey);
                if (null == currentInv)
                {
                    log.Error("Không tồn tại hóa đơn.");
                    return "ERR:2";  //khong ton tai hoa don, hoa don da duoc thay the roi; 
                }

                string invPattern = null, invSerial = null, errorMessage = "";
                if (!LaunchInvoices.Instance.ExistNoInPubInv(comID, currentInv.Pattern, currentInv.Serial, out invPattern, out invSerial, out errorMessage))
                {
                    log.Error("het so hoa don trong dai.");
                    return "ERR:6 " + fkey;   //het so hoa don trong dai
                }
                IInvoice inv = (InvoiceBase)InvServiceFactory.NewInstance(invPattern, comID);
                try
                {
                    XElement xeles = XElement.Parse(xmlData);
                    DataHelper.DeserializeEinvFromXML(xmlData, inv);
                    inv.No = 0;
                    inv.Name = currentInv.Name;
                    inv.Pattern = invPattern;
                    inv.Serial = invSerial;
                    inv.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;                    
                    var Typecus = (from c in _cusSvr.Query where c.ComID == comID && c.Code == inv.CusCode && c.CusType == 1 select c.CusType).SingleOrDefault();
                    if (Typecus == 0)
                    {
                        inv.CusSignStatus = cusSignStatus.NocusSignStatus;
                    }
                    else
                    {
                        inv.CusSignStatus = cusSignStatus.NoSignStatus;
                    }
                    if (currentInv.Status == InvoiceStatus.SignedInv || currentInv.Status == InvoiceStatus.AdjustedInv)
                    {
                        IList<ProductInv> products = new List<ProductInv>();
                        foreach (IProductInv ii in inv.Products)
                        {
                            products.Add((ProductInv)ii);
                        }
                        Launcher.Instance.PublishReplace(currentInv, products, (InvoiceBase)inv, "");
                        invNumList += inv.Fkey ?? " ";
                    }
                    else return "ERR:9 " + currentInv.Status;

                }
                catch (EInvoice.Core.Launching.NoFactory.OpenTranException ex)
                {
                    log.Error(ex);
                    return "ERR:14";
                }
                catch (Exception ex)
                {
                    log.Error("replaceInv err5: " + ex);
                    return "ERR:5 " + ex.Message;//loi phat hanh hoa don
                }
                ILogSystemService businessLog = IoC.Resolve<ILogSystemService>();
                businessLog.WriteLogReplaceAdjust(comID, currentInv.Pattern, currentInv.Serial, currentInv.No, currentInv.PublishDate, currentInv.Amount, inv.Pattern, inv.Serial, inv.No, inv.PublishDate, inv.Amount, inv.CusName, inv.CusAddress, inv.CusCode, inv.CusTaxCode, HttpContext.Current.User.Identity.Name, BusinessLogType.Replace);
                businessLog.CommitChanges();
                //invNumList = invNumList.Remove(invNumList.Length - 1, 1);
                return "OK_" + invNumList;
            }
            catch (Exception ex)
            {
                log.Error("replaceInv: " + ex);
                return "ERR:5 " + ex.Message;
            }
        }        

        private string convertSpecialCharacter(string xmlData)
        {
            //string rv = "";
            //rv = xmlData.Replace("&", "&amp;");
            return xmlData;
        }

        [APIAuthenticate(Roles = "Admin")]
        [Route("ResetTem")]
        [HttpPost]
        public string ResetTem(string username, string password)
        {
            try
            {
                IRBACMembershipProvider _MemberShipProvider = IoC.Resolve<IRBACMembershipProvider>();
                user model = _MemberShipProvider.GetUser(username, true);
                if (model.password == FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5"))
                {
                    string name = (from e in model.Roles where e.name.Contains("Root") select e.name).FirstOrDefault();
                    if (name == "Root")
                    {
                        InvServiceFactory.Initial();
                        return "Reset thanh cong!";
                    }
                }
                return "Reset khong thanh cong!";
            }
            catch (Exception ex)
            {
                return "Reset khong thanh cong!";
            }

        }
    }
}