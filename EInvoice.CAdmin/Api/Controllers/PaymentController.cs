using EInvoice.Core;
using EInvoice.Core.Domain;
using EInvoice.Core.IService;
using EInvoice.Core.Launching;
using FX.Context;
using FX.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;

namespace EInvoice.CAdmin.Api.Controllers
{
    [APIAuthenticate]
    [RoutePrefix("api/payment")]
    public class PaymentController : ApiController
    {
        ILog log = LogManager.GetLogger(typeof(PaymentController));

        [Route("confirmPayment")]
        [HttpPost]
        public string confirmPayment(ListInvoice lsinv)
        {
            Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int comID = _currentCompany.id;
            if (_currentCompany == null) return "ERR:7";//username khong phu hop - ko tim thay company phu hop voi [username]

            List<IInvoice> invLst = new List<IInvoice>();
            string[] invTokens = lsinv.lstInvToken.Split('_');
            string pattern;
            string serial;
            decimal invNo;
            if (invTokens.Length < 1 || (!DataHelper.parseInvToken(invTokens[0], out pattern, out serial, out invNo).Equals("OK")))
            {
                return DataHelper.parseInvToken(invTokens[0], out pattern, out serial, out invNo);
            }
            IInvoiceService _iInvoicSrv = InvServiceFactory.GetService(pattern, comID);
            Dictionary<string, List<decimal>> serialAndNo = new Dictionary<string, List<decimal>>();
            foreach (string invToken in invTokens)
            {
                pattern = "";
                serial = "";
                invNo = 0;
                string rv = DataHelper.parseInvToken(invToken, out pattern, out serial, out invNo);
                if (!rv.Equals("OK"))
                {
                    return rv;
                }
                if (!serialAndNo.ContainsKey(serial))
                {
                    serialAndNo[serial] = new List<decimal>();
                }
                serialAndNo[serial].Add(invNo);
            }
            //lấy list hóa đơn theo serial
            foreach (KeyValuePair<string, List<decimal>> i in serialAndNo)
            {
                invLst.AddRange(_iInvoicSrv.GetByNo(comID, pattern, i.Key, i.Value.ToArray()));
            }
            invLst.RemoveAll(inv => inv.Status != InvoiceStatus.SignedInv && inv.Status != InvoiceStatus.AdjustedInv);
            //ktra mọi token đều tìm được hóa đơn
            if (invTokens.Length != invLst.Count)
            {
                return "ERR:6";
            }

            invLst.RemoveAll(inv => inv.PaymentStatus == Payment.Paid || (inv.Status != InvoiceStatus.SignedInv && inv.Status != InvoiceStatus.AdjustedInv));
            if (invLst.Count == 0)
            {
                return "ERR:13";            // hoa đơn đã gạch nợ/bỏ gạch nợ rồi
            }
            ICompanyService _comSrv = IoC.Resolve<ICompanyService>();
            Company com = _comSrv.Getbykey(comID);
            string strNote = "Thực hiện gạch nợ:   Người gạch nợ: " + HttpContext.Current.User.Identity.Name + "  Ngày gạch nợ: " + DateTime.Now.ToString();
            if (_iInvoicSrv.ConfirmPayment(invLst, strNote))
            {
                //thuc hien deliveriy
                IDeliver _deliver = _currentCompany.Config.Keys.Contains("IDeliver") ? IoC.Resolve(Type.GetType(_currentCompany.Config["IDeliver"])) as IDeliver : null;
                if (_deliver != null)
                    _deliver.Deliver(invLst.ToArray(), com);
                return "OK:";
            }
            return "ERR:7"; //sao lai khong thanh toán được ?
        }

        [Route("confirmPaymentDetail")]
        [HttpPost]
        public string confirmPaymentDetail(ListInvoice inv)
        {
            Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int comID = _currentCompany.id;
            if (_currentCompany == null) return "ERR:7";//username khong phu hop - ko tim thay company phu hop voi [username]

            List<IInvoice> invLst = new List<IInvoice>();
            string[] invTokens = inv.lstInvToken.Split('_');
            List<string> unPaid = new List<string>();
            List<string> notFound = new List<string>();
            List<string> paid = new List<string>();
            string pattern;
            string serial;
            decimal invNo;
            string rv = "";
            if (invTokens.Length < 1 || (!DataHelper.parseInvToken(invTokens[0], out pattern, out serial, out invNo).Equals("OK")))
            {
                return DataHelper.parseInvToken(invTokens[0], out pattern, out serial, out invNo);
            }
            IInvoiceService _iInvoicSrv = InvServiceFactory.GetService(pattern, comID);

            foreach (string invToken in invTokens)
            {
                pattern = "";
                serial = "";
                invNo = 0;
                rv = DataHelper.parseInvToken(invToken, out pattern, out serial, out invNo);
                if (!rv.Equals("OK"))
                {
                    return rv;
                }
                IInvoice oInvoiceBase = _iInvoicSrv.GetByNo(comID, pattern, serial, invNo);
                if (oInvoiceBase == null)
                {
                    //return "ERR:6"; //khong tim thay hoa don
                    notFound.Add(invToken);
                }
                if (oInvoiceBase.PaymentStatus == Payment.Unpaid && (oInvoiceBase.Status == InvoiceStatus.AdjustedInv || oInvoiceBase.Status == InvoiceStatus.SignedInv))
                {
                    invLst.Add(oInvoiceBase);
                    unPaid.Add(invToken);
                }
                else
                {
                    paid.Add(invToken);
                }
            }
            rv = "";
            StringBuilder sb = new StringBuilder("ERR:6#");
            foreach (string s in notFound)
            {
                sb.AppendFormat("{0}_", s);
            }
            rv = sb.ToString();
            rv = rv.Remove(rv.Length - 1, 1);
            sb = new StringBuilder("ERR:13#");
            foreach (string s in paid)
            {
                sb.AppendFormat("{0}_", s);
            }
            rv = rv + "||" + sb.ToString();
            rv = rv.Remove(rv.Length - 1, 1);
            if (invLst.Count == 0)
            {
                //return "ERR:13";            // hoa đơn đã gạch nợ/bỏ gạch nợ rồi
                return rv;
            }
            ICompanyService _comSrv = IoC.Resolve<ICompanyService>();
            Company com = _comSrv.Getbykey(comID);
            string strNote = "Thực hiện gạch nợ:   Người gạch nợ: " + HttpContext.Current.User.Identity.Name + "  Ngày gạch nợ: " + DateTime.Now.ToString();
            if (_iInvoicSrv.ConfirmPayment(invLst, strNote))
            {
                //thuc hien deliveriy
                IDeliver _deliver = _currentCompany.Config.Keys.Contains("IDeliver") ? IoC.Resolve(Type.GetType(_currentCompany.Config["IDeliver"])) as IDeliver : null;
                if (_deliver != null)
                    _deliver.Deliver(invLst.ToArray(), com);
                //return "OK:";
                sb = new StringBuilder("OK:#");
                foreach (string s in unPaid)
                {
                    sb.AppendFormat("{0}_", s);
                }
                rv = rv + "||" + sb.ToString();
                rv = rv.Remove(rv.Length - 1, 1);
                return rv;      //ok
            }
            return "ERR:7"; //sao lai khong thanh toán được ?
        }

        [Route("unConfirmPayment")]
        [HttpPost]
        public string unConfirmPayment(ListInvoice lsInv)
        {
            Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int comID = _currentCompany.id;
            if (_currentCompany == null) return "ERR:7";//username khong phu hop - ko tim thay company phu hop voi [username]
            string[] invTokens = lsInv.lstInvToken.Split('_');

            List<IInvoice> invLst = new List<IInvoice>();
            string pattern;
            string serial;
            decimal invNo;
            if (invTokens.Length < 1 || (!DataHelper.parseInvToken(invTokens[0], out pattern, out serial, out invNo).Equals("OK")))
            {
                return DataHelper.parseInvToken(invTokens[0], out pattern, out serial, out invNo);
            }
            IInvoiceService _iInvoicSrv = InvServiceFactory.GetService(pattern, comID);
            Dictionary<string, List<decimal>> serialAndNo = new Dictionary<string, List<decimal>>();
            foreach (string invToken in invTokens)
            {
                pattern = "";
                serial = "";
                invNo = 0;
                string rv = DataHelper.parseInvToken(invToken, out pattern, out serial, out invNo);
                if (!rv.Equals("OK"))
                {
                    return rv;
                }
                if (!serialAndNo.ContainsKey(serial))
                {
                    serialAndNo[serial] = new List<decimal>();
                }
                serialAndNo[serial].Add(invNo);
            }
            //lấy list hóa đơn theo serial
            foreach (KeyValuePair<string, List<decimal>> i in serialAndNo)
            {
                invLst.AddRange(_iInvoicSrv.GetByNo(comID, pattern, i.Key, i.Value.ToArray()));
            }
            if (invTokens.Length != invLst.Count)
            {
                return "ERR:6";
            }
            invLst.RemoveAll(inv => inv.PaymentStatus == Payment.Unpaid || (inv.Status != InvoiceStatus.SignedInv && inv.Status != InvoiceStatus.AdjustedInv));
            if (invLst.Count == 0)
            {
                return "ERR:13";            // hoa đơn đã gạch nợ/bỏ gạch nợ rồi
            }
            ICompanyService _comSrv = IoC.Resolve<ICompanyService>();
            Company com = _comSrv.Getbykey(comID);
            if (_iInvoicSrv.UnConfirmPayment(invLst))
            {
                return "OK:";
            }
            return "ERR:7"; //sao lai khong bo thanh toán được ?
        }

        [Route("confirmPaymentFkey")]
        [HttpPost]
        public string confirmPaymentFkey(ListInvoice lsInv)
        {
            Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int comID = _currentCompany.id;
            if (_currentCompany == null) return "ERlR:7";//username khong phu hop - ko tim thay company phu hop voi [username]

            List<IInvoice> invLst = new List<IInvoice>();
            string[] invTokens = lsInv.lsFkey.Split('_');
            IPublishInvoiceService _PubInvSrv = IoC.Resolve<IPublishInvoiceService>();
            PublishInvoice pubinv = _PubInvSrv.GetFirst(comID, new int[] { 1, 2 });
            string pattern = "";
            if (pubinv != null)
            {
                pattern = pubinv.InvPattern;
                _PubInvSrv.UnbindSession(pubinv);
            }
            IInvoiceService invSrv = InvServiceFactory.GetService(pattern, comID);
            try
            {
                invLst = (List<IInvoice>)invSrv.GetByFkey(comID, invTokens);
                if (invTokens.Length != invLst.Count)
                {
                    return "ERR:6";
                }
                invLst.RemoveAll(inv => inv.PaymentStatus == Payment.Paid || (inv.Status != InvoiceStatus.SignedInv && inv.Status != InvoiceStatus.AdjustedInv));
            }
            catch (Exception ex)
            {
                log.Error("confirmPaymentFkey " + ex);
                return "ERR:6 " + ex.Message;
            }
            if (invLst.Count == 0)
            {
                return "ERR:13";            // hoa đơn đã gạch nợ/bỏ gạch nợ rồi
            }
            string strNote = "    ||    Thực hiện gạch nợ:   Người gạch nợ: " + HttpContext.Current.User.Identity.Name + "  Ngày gạch nợ: " + DateTime.Now.ToString();
            if (invSrv.ConfirmPayment(invLst, strNote))
            {
                //thuc hien deliveriy
                IDeliver _deliver = _currentCompany.Config.Keys.Contains("IDeliver") ? IoC.Resolve(Type.GetType(_currentCompany.Config["IDeliver"])) as IDeliver : null;
                if (_deliver != null)
                    _deliver.Deliver(invLst.ToArray(), _currentCompany);
                return "OK:";
            }
            return "ERR:7"; //sao lai khong thanh toán được ?
        }

        [Route("confirmPaymentDetailFkey")]
        [HttpPost]
        public string confirmPaymentDetailFkey(ListInvoice lsInv)
        {
            Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int comID = _currentCompany.id;
            if (_currentCompany == null) return "ERR:7";//username khong phu hop - ko tim thay company phu hop voi [username]

            List<IInvoice> invLst = new List<IInvoice>();
            string[] invTokens = lsInv.lsFkey.Split('_');
            if (invTokens.Count() > 1000)
            {
                return "ERR:14";
            }
            IPublishInvoiceService _PubInvSrv = IoC.Resolve<IPublishInvoiceService>();
            PublishInvoice pubinv = _PubInvSrv.GetFirst(comID, new int[] { 1, 2 });
            string pattern = "";
            if (pubinv != null)
            {
                pattern = pubinv.InvPattern;
                _PubInvSrv.UnbindSession(pubinv);
            }
            IInvoiceService invSrv = InvServiceFactory.GetService(pattern, comID);
            StringBuilder sb = new StringBuilder();
            List<string> unPaidFkey = new List<string>();
            string rv;
            try
            {
                invLst = (List<IInvoice>)invSrv.GetByFkey(comID, invTokens);
                List<string> foundFkey = invLst.Select(inv => inv.Fkey).ToList();
                List<string> notFoundFkey = invTokens.ToList().GetRange(0, invTokens.Count());
                notFoundFkey.RemoveAll(inv => foundFkey.Contains(inv));             // list fkey khong ton tai
                invLst.RemoveAll(inv => inv.PaymentStatus == Payment.Paid || (inv.Status != InvoiceStatus.SignedInv && inv.Status != InvoiceStatus.AdjustedInv));
                unPaidFkey = invLst.Select(inv => inv.Fkey).ToList();  // list fkey can gach no
                foundFkey.RemoveAll(inv => unPaidFkey.Contains(inv));               // list fkey da gach no roi
                sb = new StringBuilder("ERR:6#");
                foreach (string s in notFoundFkey)
                {
                    sb.AppendFormat("{0}_", s);
                }
                rv = sb.ToString();
                rv = rv.Remove(rv.Length - 1, 1);
                sb = new StringBuilder("ERR:13#");
                foreach (string s in foundFkey)
                {
                    sb.AppendFormat("{0}_", s);
                }
                rv = rv + "||" + sb.ToString();
                rv = rv.Remove(rv.Length - 1, 1);
            }
            catch (Exception ex)
            {
                log.Error("confirmPaymentFkey " + ex);
                return "ERR:6 " + ex.Message;
            }
            if (invLst.Count == 0)
            {
                //toan bo lo hoa don hoac ko tim thay, hoac da gach no roi
                return rv;
            }
            string strNote = "    ||    Thực hiện gạch nợ:   Người gạch nợ: " + HttpContext.Current.User.Identity.Name + "  Ngày gạch nợ: " + DateTime.Now.ToString();
            if (invSrv.ConfirmPayment(invLst, strNote))
            {
                //thuc hien deliveriy
                IDeliver _deliver = _currentCompany.Config.Keys.Contains("IDeliver") ? IoC.Resolve(Type.GetType(_currentCompany.Config["IDeliver"])) as IDeliver : null;
                if (_deliver != null)
                    _deliver.Deliver(invLst.ToArray(), _currentCompany);
                //return "OK:";
                sb = new StringBuilder("OK:#");
                foreach (string s in unPaidFkey)
                {
                    sb.AppendFormat("{0}_", s);
                }
                rv = rv + "||" + sb.ToString();
                rv = rv.Remove(rv.Length - 1, 1);
                return rv;      //ok
            }
            return "ERR:7"; //sao lai khong thanh toán được ?
        }

        [Route("confirmPaymentFkeyVNP")]
        [HttpPost]
        public string confirmPaymentFkeyVNP(ListInvoice lsInv)
        {
            Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int comID = _currentCompany.id;
            if (_currentCompany == null) return "ERR:7";//username khong phu hop - ko tim thay company phu hop voi [username]

            List<IInvoice> invLst = new List<IInvoice>();
            string[] invTokens = lsInv.lsFkey.Split(';');
            IPublishInvoiceService _PubInvSrv = IoC.Resolve<IPublishInvoiceService>();
            PublishInvoice pubinv = _PubInvSrv.GetFirst(comID, new int[] { 1, 2 });
            string pattern = "";
            if (pubinv != null)
            {
                pattern = pubinv.InvPattern;
                _PubInvSrv.UnbindSession(pubinv);
            }
            IInvoiceService invSrv = InvServiceFactory.GetService(pattern, comID);
            try
            {
                invLst = (List<IInvoice>)invSrv.GetByFkey(comID, invTokens);
                if (invTokens.Length != invLst.Count)
                {
                    return "ERR:6";
                }
                invLst.RemoveAll(inv => inv.PaymentStatus == Payment.Paid || (inv.Status != InvoiceStatus.SignedInv && inv.Status != InvoiceStatus.AdjustedInv));
                for (int i = 0; i < invTokens.Length; i++)
                {
                    invTokens[i] = invTokens[i] + "K";
                }
                List<IInvoice> invKLst = new List<IInvoice>();
                invKLst = (List<IInvoice>)invSrv.GetByFkey(comID, invTokens);
                invKLst.RemoveAll(inv => inv.PaymentStatus == Payment.Paid || (inv.Status != InvoiceStatus.SignedInv && inv.Status != InvoiceStatus.AdjustedInv));
                if (invKLst.Count > 0)
                {
                    invLst.AddRange(invKLst);
                }
            }
            catch (Exception ex)
            {
                log.Error("confirmPaymentFkey " + ex);
                return "ERR:6 " + ex.Message;
            }
            if (invLst.Count == 0)
            {
                return "ERR:13";            // hoa đơn đã gạch nợ/bỏ gạch nợ rồi
            }
            string strNote = "    ||    Thực hiện gạch nợ:   Người gạch nợ: " + HttpContext.Current.User.Identity.Name + "  Ngày gạch nợ: " + DateTime.Now.ToString();
            if (invSrv.ConfirmPayment(invLst, strNote))
            {
                //thuc hien deliveriy
                IDeliver _deliver = _currentCompany.Config.Keys.Contains("IDeliver") ? IoC.Resolve(Type.GetType(_currentCompany.Config["IDeliver"])) as IDeliver : null;
                if (_deliver != null)
                    _deliver.Deliver(invLst.ToArray(), _currentCompany);

                //lay link email
                String weblink = System.Configuration.ConfigurationManager.AppSettings["Portal"] + "/Email/EmailInvoiceView?token=";
                StringBuilder rv = new StringBuilder("OK:<Data>");
                foreach (IInvoice inv in invLst)
                {
                    rv.Append("<Item><index>").Append(inv.PublishDate.Month).Append("</index>");
                    rv.Append("<cusCode>").Append(inv.CusCode).Append("</cusCode>");
                    if (inv.Fkey.EndsWith("K"))
                    {
                        rv.Append("<month>").Append(inv.Fkey.Substring(inv.Fkey.Length - 7, 6)).Append("</month>");
                    }
                    else
                    {
                        rv.Append("<month>").Append(inv.Fkey.Substring(inv.Fkey.Length - 6, 6)).Append("</month>");
                    }
                    rv.Append("<pattern>").Append(inv.Pattern).Append("</pattern>");
                    rv.Append("<serial>").Append(inv.Serial).Append("</serial>");
                    rv.Append("<status>").Append((int)inv.Status).Append("</status>");
                    string stoken = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(inv.id.ToString() + "_" + inv.Pattern + "_" + _currentCompany.id.ToString()));
                    rv.AppendFormat("<link>{0}{1}</link></Item>", weblink, stoken);
                }
                rv.Append("</Data>");
                return rv.ToString();
                //}
                //else return "OK:";
            }
            return "ERR:7"; //sao lai khong thanh toán được ?
        }

        [Route("UnConfirmPaymentFkey")]
        [HttpPost]
        public string UnConfirmPaymentFkey(ListInvoice lsInv)
        {
            Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int comID = _currentCompany.id;
            if (_currentCompany == null) return "ERR:7";//username khong phu hop - ko tim thay company phu hop voi [username]

            List<IInvoice> invLst = new List<IInvoice>();
            string[] invTokens = lsInv.lsFkey.Split('_');
            IPublishInvoiceService _PubInvSrv = IoC.Resolve<IPublishInvoiceService>();
            PublishInvoice pubinv = _PubInvSrv.GetFirst(comID, new int[] { 1, 2 });
            string pattern = "";
            if (pubinv != null)
            {
                pattern = pubinv.InvPattern;
                _PubInvSrv.UnbindSession(pubinv);
            }
            IInvoiceService invSrv = InvServiceFactory.GetService(pattern, comID);
            try
            {
                invLst = (List<IInvoice>)invSrv.GetByFkey(comID, invTokens);
                if (invTokens.Length != invLst.Count)
                {
                    return "ERR:6";
                }
                invLst.RemoveAll(inv => inv.PaymentStatus == Payment.Unpaid || (inv.Status != InvoiceStatus.SignedInv && inv.Status != InvoiceStatus.AdjustedInv));
            }
            catch (Exception ex)
            {
                log.Error("UnConfirmPaymentFkey " + ex);
                return "ERR:6 " + ex.Message;
            }
            if (invLst.Count == 0)
            {
                return "ERR:13";            // hoa đơn đã gạch nợ/bỏ gạch nợ rồi
            }
            if (invSrv.UnConfirmPayment(invLst))
            {
                return "OK:";
            }
            return "ERR:7"; //sao lai khong bo thanh toán được ?
        }

        [Route("UnConfirmPaymentFkey")]
        [HttpPost]
        public string UnConfirmPaymentFkeyVNP(ListInvoice lsInv)
        {
            Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int comID = _currentCompany.id;
            if (_currentCompany == null) return "ERR:7";//username khong phu hop - ko tim thay company phu hop voi [username]

            List<IInvoice> invLst = new List<IInvoice>();
            string[] invTokens = lsInv.lsFkey.Split('_');
            IPublishInvoiceService _PubInvSrv = IoC.Resolve<IPublishInvoiceService>();
            PublishInvoice pubinv = _PubInvSrv.GetFirst(comID, new int[] { 1, 2 });
            string pattern = "";
            if (pubinv != null)
            {
                pattern = pubinv.InvPattern;
                _PubInvSrv.UnbindSession(pubinv);
            }
            IInvoiceService invSrv = InvServiceFactory.GetService(pattern, comID);
            try
            {
                invLst = (List<IInvoice>)invSrv.GetByFkey(comID, invTokens);
                if (invTokens.Length != invLst.Count)
                {
                    return "ERR:6";
                }
                invLst.RemoveAll(inv => inv.PaymentStatus == Payment.Unpaid || (inv.Status != InvoiceStatus.SignedInv && inv.Status != InvoiceStatus.AdjustedInv));
                for (int i = 0; i < invTokens.Length; i++)
                {
                    invTokens[i] = invTokens[i] + "K";
                }
                List<IInvoice> invKLst = new List<IInvoice>();
                invKLst = (List<IInvoice>)invSrv.GetByFkey(comID, invTokens);
                invKLst.RemoveAll(inv => inv.PaymentStatus == Payment.Unpaid || (inv.Status != InvoiceStatus.SignedInv && inv.Status != InvoiceStatus.AdjustedInv));
                if (invKLst.Count > 0)
                {
                    invLst.AddRange(invKLst);
                }
            }
            catch (Exception ex)
            {
                log.Error("UnConfirmPaymentFkey " + ex);
                return "ERR:6 " + ex.Message;
            }
            if (invLst.Count == 0)
            {
                return "ERR:13";            // hoa đơn đã gạch nợ/bỏ gạch nợ rồi
            }
            if (invSrv.UnConfirmPayment(invLst))
            {
                StringBuilder rv = new StringBuilder("OK:<Data>");
                foreach (IInvoice inv in invLst)
                {
                    rv.Append("<Item><index>").Append(inv.PublishDate.Month).Append("</index>");
                    rv.Append("<cusCode>").Append(inv.CusCode).Append("</cusCode>");
                    if (inv.Fkey.EndsWith("K"))
                    {
                        rv.Append("<month>").Append(inv.Fkey.Substring(inv.Fkey.Length - 7, 6)).Append("</month>");
                    }
                    else
                    {
                        rv.Append("<month>").Append(inv.Fkey.Substring(inv.Fkey.Length - 6, 6)).Append("</month>");
                    }
                    rv.Append("<pattern>").Append(inv.Pattern).Append("</pattern>");
                    rv.Append("<serial>").Append(inv.Serial).Append("</serial>");
                    rv.Append("<status>").Append((int)inv.Status).Append("</status></Item>");
                }
                rv.Append("</Data>");
                return rv.ToString();
            }
            return "ERR:7"; //sao lai khong bo thanh toán được ?
        }

    }
}