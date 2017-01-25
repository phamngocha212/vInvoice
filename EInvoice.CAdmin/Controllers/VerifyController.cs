using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Xml;
using EInvoice.Core;
using EInvoice.Core.Domain;
using EInvoice.Core.IService;
using EInvoice.Core.Launching;
using FX.Core;
using FX.Context;

namespace EInvoice.CAdmin.Controllers
{
    public class VerifyController : BaseController
    {
        //
        // GET: /Verify/
        private IInvoiceService IInvSrv;
        private readonly Company currentCom;
        private readonly ICertificateService iCer;
        //private readonly IGeneratorINV iGen;
        private IGeneratorINV iGen;
        private readonly IRepositoryINV iRepo;
        public VerifyController()
        {
            currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            iCer = IoC.Resolve<ICertificateService>();
            //iGen = IoC.Resolve<IGeneratorINV>();            
            iRepo = IoC.Resolve<IRepositoryINV>();
        }
        public ActionResult Index()
        {
            return View();
        }

        [WebMethod]
        public JsonpResult verify(int invNo, string pattern, string serial, int ck)
        {
            IInvSrv = InvServiceFactory.GetService(pattern, currentCom.id);
            iGen = InvServiceFactory.GetGenerator(pattern, currentCom.id);
            IInvoice inv = IInvSrv.GetByNo(currentCom.id, pattern, serial, invNo);

            byte[] data = iRepo.GetData(inv);

            XmlDocument xd = new XmlDocument();
            xd.PreserveWhitespace = true;
            xd.LoadXml(System.Text.Encoding.UTF8.GetString(data));

            int k = iGen.VerifyInvoice(Encoding.UTF8.GetBytes(xd.OuterXml));
            bool c = false;
            if (k == 0 || k == 1 || (k == 2 && ck == 0))
                c = true;
            return new JsonpResult
            {
                Data = c,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

    }
    public class JsonpResult : JsonResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            var request = context.HttpContext.Request;
            var response = context.HttpContext.Response;
            string jsoncallback = (context.RouteData.Values["jsoncallback"] as string) ?? request["jsoncallback"];
            if (!string.IsNullOrEmpty(jsoncallback))
            {
                if (string.IsNullOrEmpty(base.ContentType))
                {
                    base.ContentType = "application/x-javascript";
                }
                response.Write(string.Format("{0}(", jsoncallback));
            }
            base.ExecuteResult(context);
            if (!string.IsNullOrEmpty(jsoncallback))
            {
                response.Write(")");
            }
        }
    }
}
