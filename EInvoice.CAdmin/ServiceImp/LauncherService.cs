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
using System.Web;

namespace EInvoice.CAdmin.ServiceImp
{
    public class LauncherService : ILauncherService
    {
        public string Message { get; set; }
        ILog log = LogManager.GetLogger(typeof(LauncherService));

        public void PublishAdjust(IInvoice OriINV, IList<ProductInv> lst, InvoiceBase INV, string AttacheFile = "")
        {
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            try
            {
                Launcher.Instance.PublishAdjust(OriINV, lst, INV, AttacheFile);
                Message = "OK:";
                try
                {
                    ILogSystemService businessLog = IoC.Resolve<ILogSystemService>();
                    businessLog.WriteLogReplaceAdjust(currentCom.id, OriINV.Pattern, OriINV.Serial, OriINV.No, OriINV.PublishDate, OriINV.Amount, INV.Pattern, INV.Serial, INV.No, INV.PublishDate, INV.Amount, INV.CusName, INV.CusAddress, INV.CusCode, INV.CusTaxCode, HttpContext.Current.User.Identity.Name, BusinessLogType.Adjust);
                    businessLog.CommitChanges();
                }
                catch { }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
        }

        public void PublishReplace(IInvoice OriINV, IList<ProductInv> lst, InvoiceBase INV, string AttacheFile = "")
        {
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            try
            {
                Launcher.Instance.PublishReplace(OriINV, lst, INV, AttacheFile);
                Message = "OK:";
                try
                {
                    ILogSystemService businessLog = IoC.Resolve<ILogSystemService>();
                    businessLog.WriteLogReplaceAdjust(currentCom.id, OriINV.Pattern, OriINV.Serial, OriINV.No, OriINV.PublishDate, OriINV.Amount, INV.Pattern, INV.Serial, INV.No, INV.PublishDate, INV.Amount, INV.CusName, INV.CusAddress, INV.CusCode, INV.CusTaxCode, HttpContext.Current.User.Identity.Name, BusinessLogType.Replace);
                    businessLog.CommitChanges();
                }
                catch { }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                Message = ex.Message;
            }
        }

        public void PublishInv(string pattern, string serial, IInvoice[] mInvoiceList, string username = null)
        {
            try
            {
                Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                IDeliver _DeliverService = currentCom.Config.ContainsKey("IDeliver") ? InvServiceFactory.GetDeliver(currentCom.Config["IDeliver"]) : null;
                if (mInvoiceList.Length <= 50)
                {
                    Launcher.Instance.Launch(pattern, serial, mInvoiceList);
                    Message = "OK";
                }
                else
                {
                    for (int i = 0; i < mInvoiceList.Length / 50; i++)
                    {
                        Launcher.Instance.Launch(pattern, serial, mInvoiceList.Skip(i * 50).Take(50).ToArray());
                    }
                    Message = "OK";
                }
                if (_DeliverService != null)
                    _DeliverService.PrepareDeliver(mInvoiceList, currentCom);
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
        }
    }
}