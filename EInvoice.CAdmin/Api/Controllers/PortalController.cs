using System.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using EInvoice.Core;
using EInvoice.Core.Launching;
using EInvoice.Core.Domain;
using EInvoice.Core.IService;
using IdentityManagement.Service;
using log4net;
using FX.Context;
using FX.Core;
using NReco.PdfGenerator;
using EInvoice.Core.Viewer;
using System.Web.Caching;
using System.Net;
using System.Configuration;
using EInvoice.CAdmin.Api;
namespace EInvoice.Api.Controllers
{
    [APIAuthenticate]
    [RoutePrefix("api/portal")]
    public class PortalController : ApiController
    {        
        ILog log = LogManager.GetLogger(typeof(PortalController));
        private readonly IuserService _userSVC;
        private readonly ICustomerService _cusSVC;
        private readonly IPublishInvoiceService _pubInSrv;
        private IInvoiceService _iInvoicSrv;
        private IGeneratorINV _genInv;
        private readonly ICertificateService _CerSrv;
        public PortalController()
        {
            _userSVC = IoC.Resolve<IuserService>();
            _cusSVC = IoC.Resolve<ICustomerService>();
            _pubInSrv = IoC.Resolve<IPublishInvoiceService>();
            // _genInv = IoC.Resolve<IGeneratorINV>();
            _CerSrv = IoC.Resolve<ICertificateService>();
        }        

        private readonly string FIX_LENGTH_INV_NUMBER = "0000000";

        #region su dung invToken
        private string getItemStructure(IInvoice item)
        {
            string rv = "<Item>";
            rv += "<index>" + item.PublishDate.Month + "</index>";
            rv += "<invToken>" + item.Pattern + ";" + item.Serial + ";" + item.No + "</invToken>";
            if (!string.IsNullOrEmpty(item.Fkey))
            {
                rv += "<fkey>" + item.Fkey + "</fkey>";
            }
            else
            {
                rv += "<fkey>" + "</fkey>";
            }
            rv += "<name>" + item.Name + "</name>";
            rv += "<publishDate>" + item.PublishDate + "</publishDate>";
            rv += "<signStatus>" + (int)item.CusSignStatus + "</signStatus>";
            rv += "<amount>" + item.Amount + "</amount>";
            rv += "<pattern>" + item.Pattern + "</pattern>";
            rv += "<serial>" + item.Serial + "</serial>";
            rv += "<invNum>" + item.No.ToString(FIX_LENGTH_INV_NUMBER) + "</invNum>";
            rv += "<status>" + (int)item.Status + "</status>";
            rv += "<cusname><![CDATA[" + item.CusName + "]]></cusname>";
            rv += "<payment>" + (int)item.PaymentStatus + "</payment>";
            return rv + "</Item>";
        }
        private string parseInvToken(string invToken, out string pattern, out string serial, out decimal invNo)
        {
            pattern = "";
            serial = "";
            invNo = -1;
            string[] temp = invToken.Split(';');
            if (temp.Length != 3)
            {
                return "ERR:2"; //chuoi token khong chinh xac
            }
            pattern = temp[0];
            serial = temp[1];
            if (!decimal.TryParse(temp[2], out invNo))
            {
                return "ERR:2"; // idInvoice không phải số
            }
            return "OK";
        }

        [Route("listInvByCus")]
        [HttpPost]
        public string listInvByCus(PortalAPI portal)
        {
            try
            {                
                Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                int comID = _currentCompany.id;
                if (_currentCompany == null) return "ERR:7";//username khong phu hop - ko tim thay company phu hop voi [username]
                Customer cus = _cusSVC.SearchCusCode(portal.cusCode, comID);
                //check cuscode
                if (null == cus)
                {
                    log.Info("khong ton tai khach hang: " + portal.cusCode + " comID: " + comID);
                    return "ERR:3"; //khong ton tai khach hang
                }

                //lay pattern in publish inv
                string template = "";
                IList<string> lstPubinv = _pubInSrv.LstByPattern(comID, 1);
                if (lstPubinv.Count > 0)
                {
                    template = lstPubinv[0];
                }
                else
                {
                    return "ERR:4"; //cong ty chua co mau hoa don nao
                }

                DateTime? DateFrom = null;
                DateTime? DateTo = null;
                if (!String.IsNullOrEmpty(portal.fromDate)) DateFrom = DateTime.ParseExact(portal.fromDate, "dd/MM/yyyy", null);
                if (!String.IsNullOrEmpty(portal.toDate)) DateTo = DateTime.ParseExact(portal.toDate, "dd/MM/yyyy", null);

                _iInvoicSrv = InvServiceFactory.GetService(template, comID);
                var qr = _iInvoicSrv.IQuery<IInvoice>().Where(inv => inv.ComID == cus.ComID
                                                                        && inv.CusCode == cus.Code && inv.Status != InvoiceStatus.NewInv
                                                                        );
                if (DateFrom.HasValue) qr = qr.Where(inv => inv.PublishDate >= DateFrom.Value);
                if (DateTo.HasValue) qr = qr.Where(inv => inv.PublishDate <= DateTo.Value.AddDays(1));
                qr.OrderBy(inv => inv.PublishDate.Month);
                IList<IInvoice> model = qr.ToList();
                string rv = "<Data>";
                foreach (IInvoice item in model)
                {
                    rv += getItemStructure(item);
                }
                return rv + "</Data>";
            }
            catch (Exception ex)
            {
                log.Error("listInvByCus " + ex);
                throw ex;
            }
        }

        [Route("SearchInv")]
        [HttpPost]
        public string SearchInv(PortalAPI portal)
        {
            try
            {
                //check role
                log.Info(" SearchInv cusCode: " + portal.cusCode);
                //if (CheckRole(userName, userPass, "ServiceRole") == false)
                //{
                //    return "ERR:1";//khong co quyen
                //}

                Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                int comID = _currentCompany.id;
                if (_currentCompany == null) return "ERR:7";//username khong phu hop - ko tim thay company phu hop voi [username]
                Customer cus = _cusSVC.SearchCusCode(portal.cusCode, comID);
                //check cuscode
                if (null == cus)
                {
                    log.Info("khong ton tai khach hang: " + portal.cusCode + " comID: " + comID);
                    return "ERR:3"; //khong ton tai khach hang
                }

                //lay pattern in publish inv
                IList<string> lstPubinv = _pubInSrv.LstByPattern(comID, 1);
                if (lstPubinv.Count > 0)
                {
                    if (String.IsNullOrEmpty(portal.pattern))
                    {
                        portal.pattern = lstPubinv[0];
                    }
                }
                else
                {
                    return "ERR:4"; //cong ty chua co mau hoa don nao
                }
                try
                {
                    _iInvoicSrv = InvServiceFactory.GetService(portal.pattern, comID);
                }
                catch (Exception ex)
                {
                    log.Error("listSearchInv: " + ex);
                    return "ERR:4"; //khong tim thay pattern
                }

                IList<IInvoice> invoiceList = new List<IInvoice>();
                decimal No;
                decimal.TryParse(portal.invNumber, out No); if (No == 0) No = -1;
                if (!string.IsNullOrWhiteSpace(portal.invNumber))
                {
                    IInvoice Invoice = _iInvoicSrv.GetByNo(cus.ComID, portal.pattern, portal.serial, No);
                    if (null != Invoice && Invoice.CusCode == cus.Code)
                    {
                        invoiceList.Add(Invoice);
                    }
                }
                else
                {
                    DateTime? DateFrom = null;
                    DateTime? DateTo = null;
                    if (!String.IsNullOrEmpty(portal.fromDate)) DateFrom = DateTime.ParseExact(portal.fromDate, "dd/MM/yyyy", null);
                    if (!String.IsNullOrEmpty(portal.toDate)) DateTo = DateTime.ParseExact(portal.toDate, "dd/MM/yyyy", null);
                    int total;
                    InvoiceStatus status;
                    if (portal.invStatus.HasValue)
                        status = (InvoiceStatus)portal.invStatus.Value;
                    else
                        status = InvoiceStatus.Null;
                    if (portal.page.HasValue && portal.page.Value > 0)
                    {
                        int defaultValue = 10;
                        invoiceList = _iInvoicSrv.SearchPublish(cus.ComID, portal.pattern, portal.serial, DateFrom, DateTo, status, "", cus.Code, "", portal.page.Value - 1, defaultValue, out total);
                    }
                    else
                        invoiceList = _iInvoicSrv.SearchPublish(cus.ComID, portal.pattern, portal.serial, DateFrom, DateTo, status, "", cus.Code, "", 0, -1, out total);

                    if (portal.cussignStatus.HasValue && portal.cussignStatus.Value >= 0)
                    {
                        IList<IInvoice> temp = invoiceList.Where(i => (int)i.CusSignStatus == portal.cussignStatus.Value || i.CusSignStatus == cusSignStatus.NocusSignStatus || i.CusSignStatus == cusSignStatus.ViewNocusSignStatus).ToList();
                        invoiceList = temp;
                    }

                    //check payment
                    if (portal.payment.HasValue && portal.payment.Value >= 0)
                    {
                        IList<IInvoice> temp = invoiceList.Where(i => (int)i.PaymentStatus == portal.payment.Value).ToList();
                    }
                }

                //return 
                string rv = "<Data>";
                foreach (IInvoice item in invoiceList)
                {
                    rv += getItemStructure(item);
                }
                return rv + "</Data>";
            }
            catch (Exception ex)
            {
                log.Error("searchInv " + ex);
                throw ex;
            }
        }

        private string getInvViewAction(string invToken, string functionName, bool checkPayment = true)
        {
            try
            {
                //check role
                log.Info(functionName + " token: " + invToken);
                //if (CheckRole(userName, userPass, "ServiceRole") == false)
                //{
                //    return "ERR:1";//khong co quyen
                //}
                string pattern;
                string serial;
                decimal invNo;
                string rv = parseInvToken(invToken, out pattern, out serial, out invNo);
                if (!rv.Equals("OK"))
                {
                    return rv;
                }

                Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                int comID = _currentCompany.id;
                if (_currentCompany == null) return "ERR:7";//username khong phu hop - ko tim thay company phu hop voi [username]
                try
                {
                    _iInvoicSrv = InvServiceFactory.GetService(pattern, comID);
                }
                catch (Exception ex)
                {
                    log.Error(functionName + ex);
                    return "ERR:4"; //khong tim thay pattern
                }
                //IInvoice oInvoiceBase = _iInvoicSrv.Getbykey<IInvoice>(idInvoice);
                IInvoice oInvoiceBase = _iInvoicSrv.GetByNo(comID, pattern, serial, invNo);
                if (null == oInvoiceBase)
                {
                    return "ERR:6";     //không tìm thấy hóa đơn
                }
                if (checkPayment)
                {
                    if (oInvoiceBase.PaymentStatus != Payment.Paid)
                    {
                        return "ERR:11"; //hóa đơn chưa cho deliver, ko xem được
                    }
                }
                //lay html hoa don
                //IViewer _iViewerSrv = IoC.Resolve<IViewer>();
                IViewer _iViewerSrv = InvServiceFactory.GetViewer(pattern, comID);
                IRepositoryINV _repository = IoC.Resolve<IRepositoryINV>();
                byte[] invdata = _repository.GetData(oInvoiceBase);
                //log.Error("getInvView: " + Convert.ToBase64String(invdata));
                rv = _iViewerSrv.GetHtml(invdata);
                //log.Error("getInvHtml: " + rv);
                //Check trang thai thay doi
                CheckViewEinvoice(oInvoiceBase.id.ToString(), oInvoiceBase.Pattern, oInvoiceBase.ComID.ToString());
                return rv;
            }
            catch (Exception ex)
            {
                log.Error(functionName + ex);
                throw ex;
            }
        }

        [Route("getCatalogView")]
        [HttpGet]
        public string getCatalogView(PortalAPI portal)
        {
            try
            {
                //check role
                log.Info("getCatalogView " + " token: " + portal.invToken);

                string pattern;
                string serial;
                decimal invNo;
                string rv = parseInvToken(portal.invToken, out pattern, out serial, out invNo);
                if (!rv.Equals("OK"))
                {
                    return rv;
                }

                Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                int comID = _currentCompany.id;
                if (_currentCompany == null) return "ERR:7";//username khong phu hop - ko tim thay company phu hop voi [username]
                try
                {
                    _iInvoicSrv = InvServiceFactory.GetService(pattern, comID);
                }
                catch (Exception ex)
                {
                    log.Error("getCatalogView " + ex);
                    return "ERR:4"; //khong tim thay pattern
                }
                //IInvoice oInvoiceBase = _iInvoicSrv.Getbykey<IInvoice>(idInvoice);
                IInvoice oInvoiceBase = _iInvoicSrv.GetByNo(comID, pattern, serial, invNo);
                if (null == oInvoiceBase)
                {
                    return "ERR:6";     //không tìm thấy hóa đơn
                }
                //lay html hoa don
                //IViewer _iViewerSrv = IoC.Resolve<IViewer>();
                IViewer _iViewerSrv = InvServiceFactory.GetViewer(pattern, comID);
                //IRepositoryINV _repository = IoC.Resolve<IRepositoryINV>();
                oInvoiceBase.No = 0;
                rv = _iViewerSrv.GetHtml(System.Text.Encoding.UTF8.GetBytes(oInvoiceBase.GetXMLData()));
                return rv;
            }
            catch (Exception ex)
            {
                log.Error("getCatalogView: " + ex);
                throw ex;
            }
        }


        [Route("getInvView")]
        [HttpGet]
        public string getInvView(PortalAPI portal)
        {
            return getInvViewAction(portal.invToken, "getInvView ");
        }

        [Route("getInvViewNoPay")]
        [HttpGet]
        public string getInvViewNoPay(PortalAPI portal)
        {
            return getInvViewAction(portal.invToken, "getInvViewNoPay ", false);
        }

        private string downloadInvAction(string invToken, string functionName, bool checkPayment = true)
        {
            try
            {
                //check role
                log.Info(functionName + " token: " + invToken);

                string pattern;
                string serial;
                decimal invNo;
                string rv = parseInvToken(invToken, out pattern, out serial, out invNo);
                if (!rv.Equals("OK"))
                {
                    return rv;
                }

                Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                int comID = _currentCompany.id;
                if (_currentCompany == null) return "ERR:7";//username khong phu hop - ko tim thay company phu hop voi [username]
                try
                {
                    _iInvoicSrv = InvServiceFactory.GetService(pattern, comID);
                }
                catch (Exception ex)
                {
                    log.Error(functionName + ex);
                    return "ERR:4"; //khong tim thay pattern
                }
                //IInvoice oInvoiceBase = _iInvoicSrv.Getbykey<IInvoice>(idInvoice);
                IInvoice oInvoiceBase = _iInvoicSrv.GetByNo(comID, pattern, serial, invNo);
                if (null == oInvoiceBase)
                {
                    return "ERR:6";     //không tìm thấy hóa đơn
                }
                if (checkPayment)
                {
                    if (oInvoiceBase.PaymentStatus != Payment.Paid)
                    {
                        return "ERR:11"; //hóa đơn chưa cho deliver, ko xem được
                    }
                }
                //get xml
                IRepositoryINV _repository = IoC.Resolve<IRepositoryINV>();
                byte[] invdata = _repository.GetData(oInvoiceBase);
                System.Text.UTF8Encoding _encoding = new System.Text.UTF8Encoding();
                //XmlDocument xdoc = new XmlDocument();
                //xdoc.PreserveWhitespace = true;
                //xdoc.LoadXml(_encoding.GetString(invdata));
                rv = _encoding.GetString(invdata);
                //Check trang thai thay doi
                CheckViewEinvoice(oInvoiceBase.id.ToString(), oInvoiceBase.Pattern, oInvoiceBase.ComID.ToString());
                log.Info(functionName + rv);
                return rv;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }
        [Route("downloadInv")]
        [HttpGet]
        public string downloadInv(PortalAPI portal)
        {
            return downloadInvAction(portal.invToken, "downloadInv ");
        }

        [Route("downloadInvNoPay")]
        [HttpGet]
        public string downloadInvNoPay(PortalAPI portal)
        {
            return downloadInvAction(portal.invToken, "downloadInvNoPay ", false);
        }

        /// <summary>
        /// Download hóa đơn pdf dựa theo chuỗi token.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userName"></param>
        /// <param name="userPass"></param>
        /// <param name="functionName"></param>
        /// <param name="checkPayment"></param>
        /// <returns></returns>
        private string downloadInvPDFAction(string invToken, string functionName, bool checkPayment = true)
        {
            try
            {
                //check role
                log.Info(functionName + " token: " + invToken);

                string pattern;
                string serial;
                decimal invNo;
                string rv = parseInvToken(invToken, out pattern, out serial, out invNo);
                if (!rv.Equals("OK"))
                {
                    return rv;
                }

                Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                int comID = _currentCompany.id;
                if (_currentCompany == null) return "ERR:7";//username khong phu hop - ko tim thay company phu hop voi [username]
                try
                {
                    _iInvoicSrv = InvServiceFactory.GetService(pattern, comID);
                }
                catch (Exception ex)
                {
                    log.Error(functionName + ex);
                    return "ERR:4"; //khong tim thay pattern
                }
                //IInvoice oInvoiceBase = _iInvoicSrv.Getbykey<IInvoice>(idInvoice);
                IInvoice oInvoiceBase = _iInvoicSrv.GetByNo(comID, pattern, serial, invNo);
                if (null == oInvoiceBase)
                {
                    return "ERR:6";     //không tìm thấy hóa đơn
                }
                if (checkPayment)
                {
                    if (oInvoiceBase.PaymentStatus != Payment.Paid)
                    {
                        return "ERR:11"; //hóa đơn chưa cho deliver, ko xem được
                    }
                }
                //lay html hoa don
                //IViewer _iViewerSrv = IoC.Resolve<IViewer>();
                IViewer _iViewerSrv = InvServiceFactory.GetViewer(pattern, comID);
                IRepositoryINV _repository = IoC.Resolve<IRepositoryINV>();
                byte[] invdata = _repository.GetData(oInvoiceBase);
                //log.Error("getInvView: " + Convert.ToBase64String(invdata));
                rv = _iViewerSrv.GetHtml(invdata);
                HtmlToPdfConverter createPdf = new HtmlToPdfConverter();
                byte[] resultByte = createPdf.GeneratePdf(rv);
                //Check trang thai thay doi
                CheckViewEinvoice(oInvoiceBase.id.ToString(), oInvoiceBase.Pattern, oInvoiceBase.ComID.ToString());
                return Convert.ToBase64String(resultByte);
            }
            catch (Exception ex)
            {
                log.Error(functionName + ex);
                throw ex;
            }
        }

        [Route("downloadInvPDF")]
        [HttpGet]

        public string downloadInvPDF(PortalAPI portal)
        {
            return downloadInvPDFAction(portal.token, "downloadInvPDF ", true);
        }

        [Route("downloadInvPDFNoPay")]
        [HttpGet]
        public string downloadInvPDFNoPay(PortalAPI portal)
        {
            return downloadInvPDFAction(portal.token, "downloadInvPDFNoPay ", false);
        }
        #endregion

        #region su dung FKey
        private string getItemStructureFkey(IInvoice item)
        {
            string rv = "<Item>";
            rv += "<index>" + item.PublishDate.Month + "</index>";
            rv += "<cusCode>" + item.CusCode + "</cusCode>";
            if (item.Fkey.Length > 6)
            {
                rv += "<month>" + item.Fkey.Substring(item.Fkey.Length - 6, 6) + "</month>";
            }
            else rv += "<month>-1</month>";
            rv += "<name>" + item.Name + "</name>";
            rv += "<publishDate>" + item.PublishDate + "</publishDate>";
            rv += "<signStatus>" + (int)item.CusSignStatus + "</signStatus>";
            rv += "<pattern>" + item.Pattern + "</pattern>";
            rv += "<serial>" + item.Serial + "</serial>";
            rv += "<invNum>" + item.No.ToString(FIX_LENGTH_INV_NUMBER) + "</invNum>";
            rv += "<amount>" + item.Amount + "</amount>";
            rv += "<status>" + (int)item.Status + "</status>";
            rv += "<cusname><![CDATA[" + item.CusName + "]]></cusname>";
            rv += "<payment>" + (int)item.PaymentStatus + "</payment>";
            return rv + "</Item>";
        }

        private bool checkLockTransaction(int comID, string pattern, string serial, decimal no, DateTime createDate)
        {
            ITransactionService tranSrv = IoC.Resolve<ITransactionService>();
            IList<Transaction> lst = null;
            lst = (from t in tranSrv.Query
                   where t.ComID == comID && t.InvPattern == pattern && t.InvSerial == serial && (t.CreateDate <= createDate && t.CreateDate.AddDays(1) >= createDate)
                   orderby t.CreateDate descending
                   select (new Transaction { id = t.id, AccountName = t.AccountName, InvSerial = t.InvSerial, InvPattern = t.InvPattern, Status = t.Status, CreateDate = t.CreateDate })).ToList();
            foreach (Transaction tran in lst)
            {
                if (string.IsNullOrEmpty(tran.NoRange)) continue;
                string[] temp = tran.NoRange.Split('_');
                decimal minNo, maxNo;
                if (!Decimal.TryParse(temp[0], out minNo) || !Decimal.TryParse(temp[1], out maxNo)) continue;
                if (no >= minNo && no <= maxNo)
                {
                    if (tran.TranLock == 0) return false;
                    else return true;
                }
            }
            //tranSrv.Query.Where(t => t.InvSerial == serial).Select(new Transaction { id = t.id, AccountName = t.AccountName, InvSerial = t.InvSerial, InvPattern = t.InvPattern, Status = t.Status, CreateDate = t.CreateDate }).ToList();
            return false;
        }
        [Route("listInvByCusFkey")]
        [HttpPost]
        public string listInvByCusFkey(PortalAPI portal)
        {
            try
            {


                Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                int comID = _currentCompany.id;
                if (_currentCompany == null) return "ERR:7";//username khong phu hop - ko tim thay company phu hop voi [username]
                //lay pattern in publish inv
                string template = "";
                IList<string> lstPubinv = _pubInSrv.LstByPattern(comID, 1);
                if (lstPubinv.Count > 0)
                {
                    template = lstPubinv[0];
                }
                else
                {
                    return "ERR:4"; //cong ty chua co mau hoa don nao
                }

                DateTime? DateFrom = null;
                DateTime? DateTo = null;
                if (!String.IsNullOrEmpty(portal.fromDate)) DateFrom = DateTime.ParseExact(portal.fromDate, "dd/MM/yyyy", null);
                if (!String.IsNullOrEmpty(portal.toDate)) DateTo = DateTime.ParseExact(portal.toDate, "dd/MM/yyyy", null);

                _iInvoicSrv = InvServiceFactory.GetService(template, comID);
                var qr = _iInvoicSrv.IQuery<IInvoice>().Where(inv => inv.ComID == comID
                                                                        && inv.Fkey == portal.fkey && inv.Status != InvoiceStatus.NewInv
                                                                        );
                if (DateFrom.HasValue) qr = qr.Where(inv => inv.PublishDate >= DateFrom.Value);
                if (DateTo.HasValue) qr = qr.Where(inv => inv.PublishDate <= DateTo.Value.AddDays(1));
                qr.OrderBy(inv => inv.PublishDate.Month);
                IList<IInvoice> model = qr.ToList();
                string rv = "<Data>";
                foreach (IInvoice item in model)
                {
                    rv += getItemStructureFkey(item);
                }
                return rv + "</Data>";
            }
            catch (Exception ex)
            {
                log.Error("listInvByCus " + ex);
                throw ex;
            }
        }
        [Route("listInvByCusFkeyVNP")]
        [HttpPost]
        public string listInvByCusFkeyVNP(PortalAPI portal)
        {
            try
            {
                log.Error("listInvByCusVNP key:" + portal.fkey);
                //check role

                Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                int comID = _currentCompany.id;
                if (_currentCompany == null) return "ERR:7";//username khong phu hop - ko tim thay company phu hop voi [username]
                //lay pattern in publish inv
                string template = "";
                IList<string> lstPubinv = _pubInSrv.LstByPattern(comID, 1);
                if (lstPubinv.Count > 0)
                {
                    template = lstPubinv[0];
                }
                else
                {
                    return "ERR:4"; //cong ty chua co mau hoa don nao
                }
                log.Error("listInvByCusVNP " + portal.fkey);
                DateTime? DateFrom = null;
                DateTime? DateTo = null;
                if (!String.IsNullOrEmpty(portal.fromDate)) DateFrom = DateTime.ParseExact(portal.fromDate, "dd/MM/yyyy", null);
                if (!String.IsNullOrEmpty(portal.toDate)) DateTo = DateTime.ParseExact(portal.toDate, "dd/MM/yyyy", null);

                _iInvoicSrv = InvServiceFactory.GetService(template, comID);
                var qr = _iInvoicSrv.IQuery<IInvoice>().Where(inv => inv.ComID == comID
                                                                        && inv.Fkey.Contains(portal.fkey) && inv.Status != InvoiceStatus.NewInv
                                                                        );
                if (DateFrom.HasValue) qr = qr.Where(inv => inv.PublishDate >= DateFrom.Value);
                if (DateTo.HasValue) qr = qr.Where(inv => inv.PublishDate <= DateTo.Value.AddDays(1));
                qr.OrderBy(inv => inv.PublishDate.Month);
                IList<IInvoice> model = qr.ToList();
                string rv = "<Data>";
                foreach (IInvoice item in model)
                {
                    rv += getItemStructureFkey(item);
                }
                return rv + "</Data>";
            }
            catch (Exception ex)
            {
                log.Error("listInvByCus " + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Xem bảng kê hóa đơn
        /// </summary>
        /// <param name="fkey"></param>
        /// <param name="userName"></param>
        /// <param name="userPass"></param>
        /// <param name="functionName">tên hàm để ghi log</param>
        /// <param name="checkPayment">true: có check payment</param>
        /// <returns></returns>
        [Route("getcatalogViewFkey")]
        [HttpGet]
        public string getcatalogViewFkey(PortalAPI portal)
        {
            try
            {
                //check role
                log.Info("getcatalogViewFkey " + " token: " + portal.fkey);
                Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                int comID = _currentCompany.id;
                if (_currentCompany == null) return "ERR:7";//username khong phu hop - ko tim thay company phu hop voi [username]
                IPublishInvoiceService _PubInvSrv = IoC.Resolve<IPublishInvoiceService>();
                string pattern;
                IList<string> lstPubinv = _pubInSrv.LstByPattern(comID, 1);
                if (lstPubinv.Count > 0)
                {
                    pattern = lstPubinv[0];
                }
                else
                {
                    return "ERR:4"; //cong ty chua co mau hoa don nao
                }
                IInvoiceService invSrv = InvServiceFactory.GetService(pattern, comID);
                IInvoice oInvoiceBase = invSrv.GetByFkey(comID, portal.fkey);
                if (null == oInvoiceBase)
                {
                    return "ERR:6";     //không tìm thấy hóa đơn
                }
                string rv;
                IViewer _iViewerSrv = InvServiceFactory.GetViewer(pattern, comID);
                IRepositoryINV _repository = IoC.Resolve<IRepositoryINV>();
                oInvoiceBase.No = 0;
                rv = _iViewerSrv.GetHtml(System.Text.Encoding.UTF8.GetBytes(oInvoiceBase.GetXMLData()));
                return rv;
            }
            catch (Exception ex)
            {
                log.Error("getcatalogViewFkey: " + ex);
                return "ERR: " + ex.Message;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fkey"></param>
        /// <param name="userName"></param>
        /// <param name="userPass"></param>
        /// <param name="functionName"></param>
        /// <param name="checkPayment"></param>
        /// <returns></returns>
        private string downloadInvPDFFkeyAction(string fkey, string functionName, bool checkPayment)
        {
            try
            {
                //check role
                log.Info(functionName + " fkey: " + fkey);

                Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                int comID = _currentCompany.id;
                if (_currentCompany == null) return "ERR:7";//username khong phu hop - ko tim thay company phu hop voi [username]
                string pattern;
                IList<string> lstPubinv = _pubInSrv.LstByPattern(comID, 1);
                if (lstPubinv.Count > 0)
                {
                    pattern = lstPubinv[0];
                }
                else
                {
                    return "ERR:4"; //cong ty chua co mau hoa don nao
                }
                IInvoiceService invSrv = InvServiceFactory.GetService(pattern, comID);
                IInvoice oInvoiceBase = invSrv.GetByFkey(comID, fkey);
                if (null == oInvoiceBase)
                {
                    return "ERR:6";     //không tìm thấy hóa đơn
                }
                if (checkPayment)
                {
                    if (oInvoiceBase.PaymentStatus != Payment.Paid)
                    {
                        return "ERR:11"; //hóa đơn chưa cho deliver, ko xem được
                    }
                }
                string rv;
                IViewer _iViewerSrv = InvServiceFactory.GetViewer(pattern, comID);
                IRepositoryINV _repository = IoC.Resolve<IRepositoryINV>();
                byte[] invdata = _repository.GetData(oInvoiceBase);
                rv = _iViewerSrv.GetHtml(invdata);
                HtmlToPdfConverter createPdf = new HtmlToPdfConverter();
                //3. Gen pdf' byte-contents
                //string pageBreak = "<p style='page-break-after:always;'></p>"; 
                byte[] resultByte = createPdf.GeneratePdf(rv);
                //System.IO.File.WriteAllBytes(@"C:\Result.pdf", resultByte);
                //Check trang thai thay doi
                CheckViewEinvoice(oInvoiceBase.id.ToString(), oInvoiceBase.Pattern, oInvoiceBase.ComID.ToString());//oInvoiceBase
                return Convert.ToBase64String(resultByte);
            }
            catch (Exception ex)
            {
                log.Error(functionName + ex);
                return "ERR: " + ex.Message;
            }
        }

        [Route("downloadInvPDFFkey")]
        [HttpGet]
        public string downloadInvPDFFkey(PortalAPI portal)
        {
            return downloadInvPDFFkeyAction(portal.fkey, "downloadInvPDFFkey ", true);
        }

        [Route("downloadInvPDFFkeyNoPay")]
        [HttpGet]
        public string downloadInvPDFFkeyNoPay(PortalAPI portal)
        {
            return downloadInvPDFFkeyAction(portal.fkey, "downloadInvPDFFkeyNoPay ", false);
        }


        /// <summary>
        /// Xem chi tiết hóa đơn
        /// </summary>
        /// <param name="fkey"></param>
        /// <param name="userName"></param>
        /// <param name="userPass"></param>
        /// <param name="functionName">tên hàm để ghi log</param>
        /// <param name="checkPayment">true: có check payment</param>
        /// <returns></returns>
        private string getInvViewFkeyAction(string fkey, string functionName, bool checkPayment)
        {
            try
            {
                //check role
                log.Info(functionName + " token: " + fkey);

                Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                int comID = _currentCompany.id;
                if (_currentCompany == null) return "ERR:7";//username khong phu hop - ko tim thay company phu hop voi [username]
                string pattern;
                IList<string> lstPubinv = _pubInSrv.LstByPattern(comID, 1);
                if (lstPubinv.Count > 0)
                {
                    pattern = lstPubinv[0];
                }
                else
                {
                    return "ERR:4"; //cong ty chua co mau hoa don nao
                }
                IInvoiceService invSrv = InvServiceFactory.GetService(pattern, comID);
                IInvoice oInvoiceBase = invSrv.GetByFkey(comID, fkey);
                if (null == oInvoiceBase)
                {
                    return "ERR:6";     //không tìm thấy hóa đơn
                }
                if (checkPayment)
                {
                    if (oInvoiceBase.PaymentStatus != Payment.Paid)
                    {
                        return "ERR:11"; //hóa đơn chưa cho deliver, ko xem được
                    }
                }
                string rv;
                IViewer _iViewerSrv = InvServiceFactory.GetViewer(pattern, comID);
                IRepositoryINV _repository = IoC.Resolve<IRepositoryINV>();
                byte[] invdata = _repository.GetData(oInvoiceBase);
                rv = _iViewerSrv.GetHtml(invdata);
                //Check trang thai thay doi
                CheckViewEinvoice(oInvoiceBase.id.ToString(), oInvoiceBase.Pattern, oInvoiceBase.ComID.ToString());//oInvoiceBase
                return rv;
            }
            catch (Exception ex)
            {
                log.Error(functionName + ex);
                return "ERR: " + ex.Message;
            }
        }
        private bool CheckViewEinvoice(string id, string pattern, string comid)
        {
            try
            {
                _iInvoicSrv = InvServiceFactory.GetService(pattern, Convert.ToInt32(comid));
                IInvoice oInvoiceBase = _iInvoicSrv.Getbykey<IInvoice>(Convert.ToInt32(id));
                if (oInvoiceBase.CusSignStatus == cusSignStatus.SignStatus) return false;
                else if (oInvoiceBase.CusSignStatus == cusSignStatus.NocusSignStatus) oInvoiceBase.CusSignStatus = cusSignStatus.ViewNocusSignStatus;
                else if (oInvoiceBase.CusSignStatus == cusSignStatus.NoSignStatus) oInvoiceBase.CusSignStatus = cusSignStatus.ViewNoSignStatus;

                _iInvoicSrv.Update(oInvoiceBase);
                _iInvoicSrv.CommitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        [Route("getInvViewFkey")]
        [HttpGet]
        public string getInvViewFkey(PortalAPI portal)
        {
            return getInvViewFkeyAction(portal.fkey, "getInvViewFkey ", true);
        }

        [Route("getInvViewFkeyNoPay")]
        [HttpGet]
        public string getInvViewFkeyNoPay(PortalAPI portal)
        {
            return getInvViewFkeyAction(portal.fkey, "getInvViewFkeyNoPay ", false);
        }        

        /// <summary>
        /// Thực hiện download hóa đơn
        /// </summary>
        /// <param name="fkey"></param>
        /// <param name="userName"></param>
        /// <param name="userPass"></param>
        /// <param name="functionName">Tên hàm để ghi log</param>
        /// <param name="checkPayment">true: có check thanh toán, false: ko check thanh toán</param>
        /// <returns></returns>
        private string downloadInvFkeyAction(string fkey, string functionName, bool checkPayment)
        {
            try
            {
                //check role
                log.Info(functionName + " token: " + fkey);

                Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                int comID = _currentCompany.id;
                if (_currentCompany == null) return "ERR:7";//username khong phu hop - ko tim thay company phu hop voi [username]
                string rv;
                string pattern;
                IList<string> lstPubinv = _pubInSrv.LstByPattern(comID, 1);
                if (lstPubinv.Count > 0)
                {
                    pattern = lstPubinv[0];
                }
                else
                {
                    return "ERR:4"; //cong ty chua co mau hoa don nao
                }
                IInvoiceService invSrv = InvServiceFactory.GetService(pattern, comID);
                IInvoice oInvoiceBase = invSrv.GetByFkey(comID, fkey);
                if (null == oInvoiceBase)
                {
                    return "ERR:6";     //không tìm thấy hóa đơn
                }
                if (checkPayment)
                {
                    if (oInvoiceBase.PaymentStatus != Payment.Paid)
                    {
                        return "ERR:11"; //hóa đơn chưa cho deliver, ko xem được
                    }
                }
                //get xml
                IRepositoryINV _repository = IoC.Resolve<IRepositoryINV>();
                byte[] invdata = _repository.GetData(oInvoiceBase);
                System.Text.UTF8Encoding _encoding = new System.Text.UTF8Encoding();
                rv = _encoding.GetString(invdata);
                log.Info(functionName + rv);
                //Check trang thai thay doi
                CheckViewEinvoice(oInvoiceBase.id.ToString(), oInvoiceBase.Pattern, oInvoiceBase.ComID.ToString());//oInvoiceBase
                return rv;
            }
            catch (Exception ex)
            {
                log.Error(functionName + ex);
                return "ERR:" + ex.Message;
            }
        }
        [Route("downloadInvFkey")]
        [HttpGet]
        public string downloadInvFkey(PortalAPI portal)
        {
            return downloadInvFkeyAction(portal.fkey, "downloadInvFkey ", true);
        }

        [Route("downloadInvFkeyNoPay")]
        [HttpGet]
        public string downloadInvFkeyNoPay(PortalAPI portal)
        {
            return downloadInvFkeyAction(portal.fkey, "downloadInvFkeyNoPay ", false);
        }
        #endregion

        #region dung cho ki client
        /// <summary>
        /// Lay dsig cua hoa don
        /// </summary>
        /// <param name="certinfo">serial cua cert dang ki cho khach hang</param>
        /// <param name="cusCode">ma khach hang</param>
        /// <param name="invToken">chuoi token de xac dinh hoa don</param>
        /// <param name="userName"></param>
        /// <param name="userPass"></param>
        /// <returns></returns>
        [Route("getDigest")]
        [HttpGet]
        public string getDigest(PortalAPI portal)
        {
            try
            {
                //check role
                log.Info("getStringBase64Data: token: " + portal.invToken);

                string pattern;
                string serial;
                decimal invNo;
                string rv = parseInvToken(portal.invToken, out pattern, out serial, out invNo);
                if (!rv.Equals("OK"))
                {
                    return rv;
                }

                Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                int comID = _currentCompany.id;
                if (_currentCompany == null) return "ERR:7";//username khong phu hop - ko tim thay company phu hop voi [username]
                _iInvoicSrv = InvServiceFactory.GetService(pattern, comID);
                //IInvoice oInvoiceBase = _iInvoicSrv.Getbykey<IInvoice>(idInvoice);
                IInvoice oInvoiceBase = _iInvoicSrv.GetByNo(comID, pattern, serial, invNo);
                if (null == oInvoiceBase)
                {
                    return "ERR:6";     //không tìm thấy hóa đơn
                }
                _iInvoicSrv = InvServiceFactory.GetService(pattern, comID);
                Customer cus = _cusSVC.SearchCusCode(portal.cusCode, comID);
                //check cuscode
                if (null == cus)
                {
                    log.Info("khong ton tai khach hang: " + portal.cusCode + " comID: " + comID);
                    return "ERR:3"; //khong ton tai khach hang
                }

                //lay cert
                if (null == cus.SerialCert)
                {
                    log.Error("chung thu khong dung khach hang: " + portal.cusCode + " comID: " + comID);
                    return "ERR:5"; //không đúng chứng thư đăng kí
                }
                Certificate cer = _CerSrv.GetCertFromSerial(cus.SerialCert.ToUpper());
                if (null == cer)
                {
                    log.Info("Khong tim thay chung thu " + " comID: " + comID + " pass serial: " + cus.SerialCert.ToUpper());
                }
                byte[] raw = Convert.FromBase64String(cer.Cert);
                X509Certificate2 cert = new X509Certificate2(raw);

                ////if (!cert.SerialNumber.Equals(cus.SerialCert.ToUpper()))
                //if (string.Compare(cert.SerialNumber, cus.SerialCert.ToUpper()) != 0)
                //{
                //    log.Info("chung thu khong dung khach hang: " + cusCode + " comID: " + comID + " pass serial: " + cert.SerialNumber);
                //    return "ERR:5"; //không đúng chứng thư đăng kí
                //}
                _genInv = InvServiceFactory.GetGenerator(pattern, comID);
                IRepositoryINV _repository = IoC.Resolve<IRepositoryINV>();
                byte[] invdata = _repository.GetData(oInvoiceBase);
                byte[] invDataWithCus;
                byte[] hash = _genInv.GetDigestForCus(invdata, out invDataWithCus, cert);

                string base64Hash = Convert.ToBase64String(hash);
                string xmlData = Convert.ToBase64String(invDataWithCus);
                rv = "<Data><text>" + base64Hash + "</text>";
                rv += "<xmlData>" + " " + "</xmlData></Data>";

                //cache xmlData,serial cert
                HttpRuntime.Cache.Insert(portal.invToken + "_" + comID, invDataWithCus, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration);
                HttpRuntime.Cache.Insert(portal.invToken + "_" + comID + "_cert", cus.SerialCert.ToUpper(), null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration);
                log.Info("getStringBase64Data: text: " + base64Hash + " xmlData: " + xmlData);
                return rv;
            }
            catch (Exception ex)
            {
                log.Error("getStringBase64Data" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// kiem tra viec ki client thanh cong thi cap nhat lai db
        /// </summary>
        /// <param name="signValue">gia tri chu ki</param>
        /// <param name="xmlData">empty</param>
        /// <param name="cert">serial cert</param>
        /// <param name="invToken">chuoi token da xac dinh hoa don</param>
        /// <param name="userName"></param>
        /// <param name="userPass"></param>
        /// <returns></returns>
        [Route("ConfirmSiganture")]
        [HttpPost]
        public string ConfirmSiganture(PortalAPI portal)
        {
            try
            {

                string pattern;
                string serial;
                decimal invNo;
                string rv = parseInvToken(portal.invToken, out pattern, out serial, out invNo);
                if (!rv.Equals("OK"))
                {
                    return rv;
                }
                Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                int comID = _currentCompany.id;
                if (_currentCompany == null) return "ERR:7";//username khong phu hop - ko tim thay company phu hop voi [username]

                _iInvoicSrv = InvServiceFactory.GetService(pattern, comID);
                //IInvoice oInvoiceBase = _iInvoicSrv.Getbykey<IInvoice>(idInvoice);
                IInvoice oInvoiceBase = _iInvoicSrv.GetByNo(comID, pattern, serial, invNo);
                if (null == oInvoiceBase)
                {
                    return "ERR:6";     //không tìm thấy hóa đơn
                }
                _iInvoicSrv = InvServiceFactory.GetService(pattern, comID);
                _genInv = InvServiceFactory.GetGenerator(pattern, comID);
                //IViewer _iViewerSrv = IoC.Resolve<IViewer>();
                IViewer _iViewerSrv = InvServiceFactory.GetViewer(pattern, comID);
                IRepositoryINV _repository = IoC.Resolve<IRepositoryINV>();
                //lay tu cache ra
                string key = portal.invToken + "_" + comID;
                byte[] invdata = (byte[])HttpRuntime.Cache[key];
                string certSerial = (string)HttpRuntime.Cache[key + "_cert"];
                if (null == invdata || null == certSerial)
                {
                    log.Warn("verifySiganture" + " can not find cache for: " + key);
                    return "Time out cache";        //khong tim thay du lieu goc bi cache
                }
                HttpRuntime.Cache.Remove(key);
                byte[] fullData = null;
                rv = "";
                try
                {
                    fullData = _genInv.WrapCustomerSign(invdata, Convert.FromBase64String(portal.signValue));
                }
                catch (Exception ex)
                {
                    rv = ex.Message;
                    log.Error("verifySiganture eror: " + ex.Message);
                }
                if (fullData != null)
                {
                    _repository.Store(fullData, oInvoiceBase);
                    Certificate cer = _CerSrv.GetCertFromSerial(certSerial);
                    cer.Used = true;
                    oInvoiceBase.CusSignStatus = cusSignStatus.SignStatus;
                    _iInvoicSrv.CommitChanges();

                    //tra ve html
                    rv = _iViewerSrv.GetHtml(fullData);
                }
                return rv;
            }
            catch (Exception ex)
            {
                log.Error("verifySiganture: " + ex.Message);
                throw ex;
            }
        }

        class ImageGenerator
        {
            public static void AddCompanyImage(XmlDocument xDoc, bool verifyStatus)
            {
                XmlElement xe = xDoc.CreateElement("image");
                if (verifyStatus)
                    xe.InnerText = "Signature Valid";
                else xe.InnerText = "Signature Invalid";
                xe.SetAttribute("URI", ImageEmbed(verifyStatus));
                xDoc.DocumentElement.AppendChild(xe);
            }
            public static void AddCustomerImage(XmlDocument xDoc, bool verifyStatus)
            {
                XmlElement xe = xDoc.CreateElement("imageClient");
                if (verifyStatus)
                    xe.InnerText = "Signature Valid";
                else xe.InnerText = "Signature Invalid";
                xe.SetAttribute("URI", ImageEmbed(verifyStatus));
                xDoc.DocumentElement.AppendChild(xe);
            }
            private static string ImageEmbed(bool verifyStatus)
            {
                WebClient wc = new WebClient();
                byte[] data;
                if (verifyStatus)
                    data = wc.DownloadData(ConfigurationManager.AppSettings["PublishDomain"] + "/Content/Images/t.png");
                else
                    data = wc.DownloadData(ConfigurationManager.AppSettings["PublishDomain"] + "/Content/Images/f.png");
                return "data:image/png;base64," + Convert.ToBase64String(data);
            }
        }
        #endregion

        #region dung external
        [Route("getCus")]
        [HttpGet]
        public string getCus(PortalAPI portal)
        {
            //check role
            log.Info("getCus: cusCode: " + portal.cusCode);

            Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int comID = _currentCompany.id;
            if (_currentCompany == null) return "ERR:7";//username khong phu hop - ko tim thay company phu hop voi [username]
            ICustomerService _cusSrv = IoC.Resolve<ICustomerService>();
            Customer cus = _cusSrv.Query.Where(c => c.ComID == comID && c.Code == portal.cusCode).FirstOrDefault();
            if (cus == null) return "ERR:3";
            StringBuilder rv = new StringBuilder("<Data>");
            rv.AppendFormat("<code>{0}</code>", cus.Code);
            rv.AppendFormat("<name><![CDATA[{0}]]></name>", cus.Name);
            rv.AppendFormat("<address><![CDATA[{0}]]></address>", cus.Address);
            rv.AppendFormat("<phone>{0}</phone>", cus.Phone);
            rv.AppendFormat("<taxcode>{0}</taxcode>", cus.TaxCode);
            rv.AppendFormat("<email>{0}</email></Data>", cus.Email);
            return rv.ToString();
        }

        [Route("getStaff")]
        [HttpGet]
        public string getStaff(PortalAPI portal)
        {
            //check role
            log.Info("getStaff: accountName: " + portal.accountName);

            Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int comID = _currentCompany.id;
            if (_currentCompany == null) return "ERR:7";//username khong phu hop - ko tim thay company phu hop voi [username]
            IStaffService _staffSrv = IoC.Resolve<IStaffService>();
            Staff staff = _staffSrv.Query.Where(s => s.ComID == comID && s.AccountName == portal.accountName).FirstOrDefault();
            if (staff == null) return "ERR:3";
            StringBuilder rv = new StringBuilder("<Data>");
            rv.AppendFormat("<code>{0}</code>", staff.AccountName);
            rv.AppendFormat("<name><![CDATA[{0}]]></name>", staff.FullName);
            rv.AppendFormat("<address><![CDATA[{0}]]></address>", staff.Address);
            rv.AppendFormat("<phone>{0}</phone></Data>", staff.Mobile);
            return rv.ToString();
        }
        #endregion

    }
}
