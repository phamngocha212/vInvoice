using EInvoice.CAdmin.IService;
using EInvoice.Core;
using EInvoice.Core.Domain;
using EInvoice.Core.IService;
using FX.Context;
using FX.Core;
using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EInvoice.CAdmin
{
    public class LaunchInvoices
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LaunchInvoices));
        private static Hashtable LockTable = new Hashtable();
        private static LaunchInvoices _instance;
        public static LaunchInvoices Instance
        {
            get
            {
                if (_instance == null) _instance = new LaunchInvoices();
                return _instance;
            }
        }

        public void Launch(int[] invIds, string pattern, string Serial, out string Messages)
        {
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IInvoiceService IInvSrv = InvServiceFactory.GetService(pattern, currentCom.id);
            Messages = "";
            if (!LockTable.Contains(String.Format("{0}${1}", pattern, currentCom.id)))
            {
                object lockobj = new object();
                LockTable.Add(String.Format("{0}${1}", pattern, currentCom.id), lockobj);
            }
            lock (LockTable[String.Format("{0}${1}", pattern, currentCom.id)])
            {
                IList<IInvoice> lst = IInvSrv.GetByID(currentCom.id, invIds).OrderBy(p => p.ArisingDate).ToList();
                ILauncherService _launcher = IoC.Resolve(Type.GetType(currentCom.Config["LauncherType"])) as ILauncherService;
                _launcher.PublishInv(pattern, Serial, lst.ToArray(), HttpContext.Current.User.Identity.Name);
                Messages = _launcher.Message;                
            }
        }        

        public bool ExistNoInPubInv(int ComID, string pattern, string serial, out string newPattern, out string newSerial, out string ErrorMessage)
        {
            newPattern = pattern;
            newSerial = serial;
            ErrorMessage = "";
            IPublishInvoiceService _pubSrv = IoC.Resolve<IPublishInvoiceService>();
            var pubInv = _pubSrv.Query.Where(pub => pub.ComId == ComID && pub.InvPattern == pattern && pub.InvSerial == serial && (pub.Status == 2 || pub.Status == 1)).ToList();

            if (pubInv != null && pubInv.Count > 0)
            {
                decimal MaxNo = pubInv.Max(p => p.ToNo);
                decimal CurrentNo = pubInv.Max(p => p.CurrentNo);
                if (MaxNo > CurrentNo)
                    return true;
            }

            IList<PublishInvoice> publishInvoices = _pubSrv.Query.Where(p => p.ComId == ComID && (p.Status == 2 || p.Status == 1)).OrderBy(p => p.StartDate).ToList();
            if (publishInvoices.Count == 0)
            {
                ErrorMessage = "Giải đã hết hóa đơn để cấp số xin vui lòng đăng ký giải mới.";
                return false;
            }
            var pInv = publishInvoices.Where(p => p.InvPattern == pattern).FirstOrDefault();
            newPattern = pInv != null ? pInv.InvPattern : publishInvoices.First().InvPattern;
            newSerial = pInv != null ? pInv.InvSerial : publishInvoices.First().InvSerial;
            return true;
        }
    }
}