using EInvoice.Core;
using EInvoice.Core.Domain;
using EInvoice.Core.IService;
using EInvoice.Core.ServiceImp;
using FX.Context;
using FX.Core;
using System;
using System.IO;
using System.Text;
using System.Web.Http;
using System.Xml;
using System.Xml.Xsl;

namespace EInvoice.CAdmin.Api.Controllers
{
    [APIAuthenticate(Permissions = "Report")]
    [RoutePrefix("api/invreport")]
    public class InvReportController : ApiController
    {
        /// <summary>
        /// Bao cao tinh hinh su dung hoa don
        /// </summary>
        /// <param name="year"></param>
        /// <param name="quarter"></param>
        /// <param name="username"></param>
        /// <param name="pass"></param>
        /// <returns>chuoi xml tra ve bao cao</returns>
        [Route("reportInvUsed")]
        [HttpPost]
        public IHttpActionResult reportInvUsed(Invoice invoice)
        {
            Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int comID = _currentCompany.id;
            if (_currentCompany == null) return Unauthorized();//username khong phu hop - ko tim thay company phu hop voi [username]            
            IReportService repSrv = _currentCompany.Config.Keys.Contains("IReportService") ? IoC.Resolve(Type.GetType(_currentCompany.Config["IReportService"])) as IReportService : new ReportService();             
            string rv = repSrv.Report(comID, invoice.quarter, invoice.year, invoice.currentQuarter);
            return Ok<string>(rv);
        }
        [Route("reportMonth")]
        [HttpPost]
        public IHttpActionResult reportMonth(Invoice invoice)
        {            
            Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int comID = _currentCompany.id;
            if (_currentCompany == null) return Unauthorized();//username khong phu hop - ko tim thay company phu hop voi [username]            
            IReportService repSrv = _currentCompany.Config.Keys.Contains("IReportService") ? IoC.Resolve(Type.GetType(_currentCompany.Config["IReportService"])) as IReportService : new ReportService(); 
            //Hàm trả về path của file xml đã lưu
            string path = repSrv.ReportMonth(comID, invoice.month, invoice.year, DateTime.Now.Month);
            
            StringBuilder sb = new StringBuilder();  
            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    sb.Append(line + "\r\n");
                }
            }
            return Ok<string>(sb.ToString());
        }

        [Route("reportUserMonth")]
        [HttpPost]
        public IHttpActionResult reportUserMonth(Invoice invoice)
        {
            Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int comID = _currentCompany.id;
            if (_currentCompany == null) return Unauthorized();//username khong phu hop - ko tim thay company phu hop voi [username]            
            IReportService repSrv = _currentCompany.Config.Keys.Contains("IReportService") ? IoC.Resolve(Type.GetType(_currentCompany.Config["IReportService"])) as IReportService : new ReportService(); 
            string rv = repSrv.ReportUserMonth(comID, invoice.month, invoice.year, DateTime.Now.Month);
            return Ok<string>(rv);
        }
    }
}