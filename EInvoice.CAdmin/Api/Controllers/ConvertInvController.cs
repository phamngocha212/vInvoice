using EInvoice.Core;
using EInvoice.Core.Domain;
using EInvoice.Core.IService;
using EInvoice.Core.Launching;
using FX.Context;
using FX.Core;
using log4net;
using System.Collections.Generic;
using System.Web.Http;
using System.Linq;
using System.Net.Http;

namespace EInvoice.CAdmin.Api.Controllers
{
    [RoutePrefix("api/convertinv")]
    public class ConvertInvController : ApiController
    {
        ILog log = LogManager.GetLogger(typeof(ConvertInvController));

        [APIAuthenticate]
        [Route("convertForStore")]
        [HttpPost]
        public IHttpActionResult convertForStore(DataAPI portal)
        {
            Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int comID = _currentCompany.id;
            if (_currentCompany == null) return Unauthorized();//username khong phu hop - ko tim thay company phu hop voi [username]
            string invPattern = null, invSerial = null;
            getPublishInvoice(portal.pattern, _currentCompany.id, out invPattern, out invSerial);
            if (string.IsNullOrWhiteSpace(invPattern))
                return Ok("ERR:20");
            IInvoiceService _iInvoicSrv = InvServiceFactory.GetService(invPattern, comID);
            IInvoice currentInv = InvServiceFactory.NewInstance(invPattern, comID);
            if (portal.invNo > 0)
                currentInv = _iInvoicSrv.GetByNo(comID, invPattern, invSerial, portal.invNo);
            else
                currentInv = _iInvoicSrv.GetByFkey(comID, portal.fkey);
            if (null == currentInv)
            {
                return NotFound();
            }
            //lay html
            IRepositoryINV _repository = IoC.Resolve<IRepositoryINV>();
            byte[] invdata = _repository.GetData(currentInv);
            string err = "";
            string rv = _iInvoicSrv.ConvertForStore(currentInv, "", out err);
            return Ok<string>(rv);
        }
       
        [APIAuthenticate]
        [Route("convertToHTML")]
        [HttpPost]
        public HttpResponseMessage convertToHTML(DataAPI portal)
        {
            Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int comID = _currentCompany.id;
            if (_currentCompany == null) return new HttpResponseMessage()
            {
                Content = new StringContent("ERR:1-Unauthorized")
            };
            string invPattern = null, invSerial = null;
            getPublishInvoice(portal.pattern, _currentCompany.id, out invPattern, out invSerial);
            if (string.IsNullOrWhiteSpace(invPattern))
                return new HttpResponseMessage()
                {
                    Content = new StringContent("ERR:20")
                };

            IInvoiceService _iInvoicSrv = InvServiceFactory.GetService(invPattern, comID);
            IInvoice currentInv = InvServiceFactory.NewInstance(invPattern, comID);
            if (portal.invNo > 0)
                currentInv = _iInvoicSrv.GetByNo(comID, invPattern, invSerial, portal.invNo);
            else
                currentInv = _iInvoicSrv.GetByFkey(comID, portal.fkey);
            if (null == currentInv)
            {
                return new HttpResponseMessage()
                {
                    Content = new StringContent("ERR:2")
                };  //khong ton tai hoa don
            }

            //lay html
            IRepositoryINV _repository = IoC.Resolve<IRepositoryINV>();
            byte[] invdata = _repository.GetData(currentInv);
            string err = "";
            string rv = _iInvoicSrv.ConvertForStore(currentInv, "", out err);
            if (!string.IsNullOrWhiteSpace(err))
            {
                log.Error(err);
                return new HttpResponseMessage()
                {
                    Content = new StringContent("ERR:" + err)
                };
            }
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(rv);
            return new HttpResponseMessage()
            {
                Content = new StringContent(System.Convert.ToBase64String(plainTextBytes))
            };         
        }

        [APIAuthenticate]
        [Route("convertToPDF")]
        [HttpPost]
        public HttpResponseMessage convertToPDF(DataAPI portal)
        {
            Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int comID = _currentCompany.id;
            if (_currentCompany == null)
                return new HttpResponseMessage()
                {
                    Content = new StringContent("ERR:1-Unauthorized")
                };
            string invPattern = null, invSerial = null;
            getPublishInvoice(portal.pattern, _currentCompany.id, out invPattern, out invSerial);
            if (string.IsNullOrWhiteSpace(invPattern))
                return new HttpResponseMessage()
                {
                    Content = new StringContent("ERR:20")
                };

            IInvoiceService _iInvoicSrv = InvServiceFactory.GetService(invPattern, comID);
            IInvoice currentInv = InvServiceFactory.NewInstance(invPattern, comID);
            if (portal.invNo > 0)
                currentInv = _iInvoicSrv.GetByNo(comID, invPattern, invSerial, portal.invNo);
            else
                currentInv = _iInvoicSrv.GetByFkey(comID, portal.fkey);
            if (null == currentInv)
            {
                return new HttpResponseMessage()
                {
                    Content = new StringContent("ERR:2")
                };
            }

            //lay html
            IRepositoryINV _repository = IoC.Resolve<IRepositoryINV>();
            byte[] invdata = _repository.GetData(currentInv);
            string err = "";
            string rv = _iInvoicSrv.ConvertForStore(currentInv, "", out err);
            if (!string.IsNullOrWhiteSpace(err))
            {
                log.Error(err);
                return new HttpResponseMessage()
                {
                    Content = new StringContent("ERR:" + err)
                };
            }

            // var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(rv);
            var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();

            var pdf = htmlToPdf.GeneratePdf(rv);

            return new HttpResponseMessage()
            {
                Content = new StringContent(System.Convert.ToBase64String(pdf))
            };
        }

        [APIAuthenticate]
        [Route("convertForVerify")]
        [HttpPost]
        public IHttpActionResult convertForVerify(DataAPI portal)
        {
            Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int comID = _currentCompany.id;
            if (_currentCompany == null) return Unauthorized();//username khong phu hop - ko tim thay company phu hop voi [username]

            string invPattern = null, invSerial = null;
            getPublishInvoice(portal.pattern, _currentCompany.id, out invPattern, out invSerial);
            if (string.IsNullOrWhiteSpace(invPattern))
                return Ok("ERR:20");
            IInvoiceService _iInvoicSrv = InvServiceFactory.GetService(invPattern, comID);
            IInvoice oInvoiceBase = InvServiceFactory.NewInstance(invPattern, comID);

            if (portal.invNo > 0)
                oInvoiceBase = _iInvoicSrv.GetByNo(comID, invPattern, invSerial, portal.invNo);
            else
                oInvoiceBase = _iInvoicSrv.GetByFkey(comID, portal.fkey);
            if (null == oInvoiceBase)
            {
                return Ok<string>("ERR:6");     //không tìm thấy hóa đơn
            }
            //lay html
            IRepositoryINV _repository = IoC.Resolve<IRepositoryINV>();
            byte[] invdata = _repository.GetData(oInvoiceBase);
            string err = "";
            string rv = _iInvoicSrv.ConvertForVerify(oInvoiceBase, "", out err);
            if (string.IsNullOrEmpty(rv) && !string.IsNullOrEmpty(err))
            {
                return Ok<string>("ERR:8"); //hóa đơn đã convert rồi
            }
            return Ok<string>(rv);
        }

        private void getPublishInvoice(string pattern, int comID, out string invPattern, out string invSerial)
        {
            invPattern = invSerial = null;
            int isPattern = string.IsNullOrWhiteSpace(pattern) ? 0 : 1;
            IPublishInvoiceService _PubInvSrv = IoC.Resolve<IPublishInvoiceService>();
            switch ((isPattern))
            {
                case 0:
                    PublishInvoice pubinv = _PubInvSrv.GetFirst(comID, new int[] { 1, 2 });
                    if (pubinv != null)
                    {
                        invPattern = pubinv.InvPattern;
                        invSerial = pubinv.InvSerial;
                        _PubInvSrv.UnbindSession(pubinv);
                    }
                    else return; //tham so pattern va serial khong hop le
                    break;
                case 1:
                    PublishInvoice pubFirst = _PubInvSrv.GetbyPattern(comID, pattern, new int[] { 1, 2 }).FirstOrDefault();
                    if (pubFirst == null)
                        return;
                    else
                    {
                        invPattern = pattern;
                        invSerial = pubFirst.InvSerial;
                        _PubInvSrv.UnbindSession(pubFirst);
                    }
                    break;
            }
        }
    }
}