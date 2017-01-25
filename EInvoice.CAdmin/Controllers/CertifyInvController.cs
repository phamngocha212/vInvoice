using EInvoice.CAdmin.Models;
using EInvoice.Core;
using EInvoice.Core.Domain;
using EInvoice.Core.IService;
using FX.Context;
using FX.Core;
using FX.Utils.MVCMessage;
using FX.Utils.MvcPaging;
using IdentityManagement.Authorization;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EInvoice.CAdmin.Controllers
{    
    public class CertifyInvController : ShareController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CertifyInvController));

        [RBACAuthorize(Permissions = "Release_invInList")]
        public ActionResult Index(CertInvModel model, int? page)
        {
            int pageIndex = page.HasValue ? page.Value - 1 : 0;
            int pageSize = 20;
            int total = 0;
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IInvCertifyService invcertSrv = IoC.Resolve<IInvCertifyService>();
            IPublishInvoiceService _PubIn = IoC.Resolve<IPublishInvoiceService>();
            IList<string> lstpattern = _PubIn.LstByPattern(currentCom.id, 1);            
            if (lstpattern.Count == 0)
            {
                Messages.AddErrorFlashMessage(Resources.Message.MInv_SMesNoPubAccess);
                return RedirectToAction("/");
            }
            model.pattern = string.IsNullOrEmpty(model.pattern) ? lstpattern[0] : model.pattern;
            model.Patterns = lstpattern;
            var lst = invcertSrv.GetCertify(currentCom.id, model.pattern, model.status, model.type, pageIndex, pageSize, out total);
            model.PagedListInvCert = new PagedList<InvCertify>(lst, pageIndex, pageSize, total);
            return View(model);
        }

        [RBACAuthorize(Permissions = "Release_invInList")]
        [HttpPost]
        public ActionResult Certifies(string[] cbid, string cpattern, int ctype)
        {            
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            try
            {
                EInvoice.Core.Launching.ICertifyProvider certifySrv = new EInvoice.Core.Launching.TaxCertifyProvider();
                IInvoiceService IInvSrv = InvServiceFactory.GetService(cpattern, currentCom.id);
                int[] ids = (from s in cbid select Convert.ToInt32(s)).ToArray();
                if (ids.Length < 0)
                {
                    Messages.AddErrorFlashMessage("Bạn chưa chọn hóa đơn ký.");
                    return RedirectToAction("Index", new { pattern = cpattern });
                }
                IList<IInvoice> lst = IInvSrv.GetByID(currentCom.id, ids);
                var rl = certifySrv.Certify(cpattern,lst, currentCom);

                if (!string.IsNullOrWhiteSpace(rl))
                {                    
                    Messages.AddErrorFlashMessage("Có lỗi trong quá trình xác thực, vui lòng thực hiện lại.");
                }                
                else
                {
                    log.Info("Certifies by:" + HttpContext.User.Identity.Name + ", Date: " + DateTime.Now);
                    Messages.AddFlashMessage("Xác thực thành công.");
                }

                return RedirectToAction("Index", new { pattern = cpattern });
            }
            catch (Exception ex)
            {
                Messages.AddErrorFlashMessage("Có lỗi trong quá trình xử lý, vui lòng thực hiện lại.");
                log.Error("Certifies", ex);
                return RedirectToAction("Index", new { pattern = cpattern });
            }
        }        
    }
}
