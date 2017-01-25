using EInvoice.Core;
using EInvoice.Core.Domain;
using EInvoice.Core.IService;
using FX.Context;
using FX.Core;
using IdentityManagement.Authorization;
using log4net;
using System;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace EInvoice.CAdmin.Controllers
{
    public class HomeController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HomeController));
        //
        // GET: /Index/
        [RBACAuthorize(Permissions = "View_home")]
        public ActionResult Index()
        {
            string errorMessage = null;
            if (!DataHelper.IsValidated(out errorMessage))
                Messages.AddErrorFlashMessage(errorMessage);
            return View();
        }

        public ActionResult PotentiallyError()
        {
            ViewBag.Message = "Dữ liệu không hợp lệ hoặc có chứa mã gây nguy hiểm tiềm tàng cho hệ thống.";
            return View();
        }

        public ActionResult Unauthorize()
        {
            //get the inner most exception
            Exception ex = Server.GetLastError();
            if (ex != null)
            {
                Exception exception = ex.GetBaseException();
                log.Warn("A 404 occurred", exception);
                ViewData["Data"] = exception.Message;
            }
            else ViewData["Message"] = "You don't have permission.";
            return View();

        }

        public ActionResult ErrorPage()
        {
            Exception exception = Server.GetLastError();
            if (exception != null)
            {
                Exception ex = exception.GetBaseException();
                log.Error("ErrorModule caught an unhandled exception", ex);
                if (exception is HttpRequestValidationException || exception is ArgumentException)
                    return Redirect("/Home/PotentiallyError");
                ViewData["Message"] = ex.Message + "\n\r" + ex.StackTrace;
            }

            else ViewData["Message"] = "Have Error or you don't have permission.";
            return View();
        }

        public class CompanyData
        {
            public string TaxCode { get; set; }
            public string CertSerial { get; set; }
            public string ExpireDate { get; set; }
            public int ComId { get; set; }
        }
    }
}
