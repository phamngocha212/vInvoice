using EInvoice.Core;
using EInvoice.Core.Domain;
using EInvoice.Core.IService;
using EInvoice.Core.Launching;
using FX.Context;
using FX.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace EInvoice.CAdmin.Api.Controllers
{
    [APIAuthenticate]
    [RoutePrefix("api/deliverinv")]
    public class DeliverInvController : ApiController
    {
        ILog log = LogManager.GetLogger(typeof(DeliverInvController));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lstInvToken">list cac InvToken, phan cach boi dau _</param>
        /// <param name="userName"></param>
        /// <param name="userPass"></param>
        /// <returns></returns>
        [Route("deliverInvFkey")]
        [HttpPost]
        public string deliverInvFkey(ListInvoice lsInv)
        {
            Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int comID = _currentCompany.id;
            if (_currentCompany == null) return "ERR:7";//username khong phu hop - ko tim thay company phu hop voi [username]
            try
            {
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
                IInvoiceService _iInvoicSrv = InvServiceFactory.GetService(pattern, comID);
                invLst = (List<IInvoice>)_iInvoicSrv.GetByFkey(comID, invTokens);

                if (invLst.Count != invTokens.Length) return "ERR:6"; //khong tim thay hoa don
                ICompanyService _comSrv = IoC.Resolve<ICompanyService>();
                Company com = _comSrv.Getbykey(comID);
                IDeliver _deliver = _currentCompany.Config.Keys.Contains("IDeliver") ? IoC.Resolve(Type.GetType(_currentCompany.Config["IDeliver"])) as IDeliver : null;
                if (_deliver != null)
                    _deliver.Deliver(invLst.ToArray(), com);
            }
            catch (Exception ex)
            {
                log.Error("deliver: " + ex);
            }
            return "OK:";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lstInvToken">list cac InvToken, phan cach boi dau _</param>
        /// <param name="userName"></param>
        /// <param name="userPass"></param>
        /// <returns></returns>
        [Route("deliverInv")]
        [HttpPost]
        public string deliverInv(ListInvoice lsInv)
        {
            Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int comID = _currentCompany.id;
            if (_currentCompany == null) return "ERR:7";//username khong phu hop - ko tim thay company phu hop voi [username]
            try
            {
                List<IInvoice> invLst = new List<IInvoice>();
                //List<decimal> invNos = new List<decimal>();
                Dictionary<string, List<decimal>> patternAndNo = new Dictionary<string, List<decimal>>();
                string[] invTokens = lsInv.lstInvToken.Split('_');
                foreach (string invToken in invTokens)
                {
                    string pattern;
                    string serial;
                    decimal invNo;
                    string rv = DataHelper.parseInvToken(invToken, out pattern, out serial, out invNo);
                    if (!rv.Equals("OK"))
                    {
                        return rv;
                    }
                    if (!patternAndNo.ContainsKey(pattern + ";" + serial))
                    {
                        patternAndNo[pattern + ";" + serial] = new List<decimal>();
                    }
                    patternAndNo[pattern + ";" + serial].Add(invNo);
                }
                foreach (KeyValuePair<string, List<decimal>> i in patternAndNo)
                {
                    string[] key = i.Key.Split(';');
                    IInvoiceService _iInvoicSrv = InvServiceFactory.GetService(key[0], comID);
                    invLst.AddRange(_iInvoicSrv.GetByNo(comID, key[0], key[1], i.Value.ToArray()));
                }
                if (invLst.Count != invTokens.Length) return "ERR:6"; //khong tim thay hoa don
                ICompanyService _comSrv = IoC.Resolve<ICompanyService>();
                Company com = _comSrv.Getbykey(comID);
                IDeliver _deliver = _currentCompany.Config.Keys.Contains("IDeliver") ? IoC.Resolve(Type.GetType(_currentCompany.Config["IDeliver"])) as IDeliver : null;
                if(_deliver != null)
                _deliver.Deliver(invLst.ToArray(), com);
            }
            catch (Exception ex)
            {
                log.Error("deliver: " + ex);
            }
            return "OK:";
        }
    }
}