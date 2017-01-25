using System.Web.Http;
using EInvoice.Core.Domain;
using EInvoice.Core.IService;
using FX.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Xml;
using System.Xml.Serialization;
using FX.Utils;
using System.Text;
using EInvoice.Core;
using log4net;
using EInvoice.CAdmin.Api;

namespace EInvoice.Api.Controllers
{
    [RoutePrefix("api/manager")]
    public class ManagerController : ApiController
    {
        ILog log = LogManager.GetLogger(typeof(ManagerController));

        [Route("getPublishInv")]
        [HttpPost]
        public IHttpActionResult GetPublishInvoice()
        {
            Company comp = ((EInvoiceContext)FX.Context.FXContext.Current).CurrentCompany;
            IPublishInvoiceService pubInvSrv = IoC.Resolve<IPublishInvoiceService>();
            IList<PublishInvoice> list = pubInvSrv.Query.Where(p => p.ComId == comp.id && p.Status >= 1).ToList();
            return Ok(list);
        }        
    }
}
