using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EInvoice.Core.Domain;
using EInvoice.Core.IService;
using FX.Core;
using EInvoice.CAdmin.Models;
using EInvoice.Core;
using FX.Utils.MvcPaging;
using IdentityManagement.Authorization;
using System.IO;
using System.Web.Configuration;
using log4net;
using System.Configuration;
using System.Text;
using System.Collections;
using System.Reflection;
using EInvoice.CAdmin.IService;
using ICSharpCode.SharpZipLib.Core;
using VNPT.Invoice.DBFLib;
using System.Xml.Linq;
using EInvoice.CAdmin.ServiceImp;
using FX.Context;

namespace EInvoice.CAdmin.Controllers
{
    public class TaxDeclarationController : BaseController
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(ProductController));
        private ITaxDeclarationService _TaxSvc;
        private readonly Company _currentCom;
        public TaxDeclarationController()
        {
            _currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            _TaxSvc = IoC.Resolve<ITaxDeclarationService>();
        }

        [RBACAuthorize(Permissions = "Release_invInTime")]
        public ActionResult Index()
        {  
                     
            TaxDeclaration model = _TaxSvc.GetLastTaxDeclaration(_currentCom.id);
            string date = null;
            int month = 0;
            int startYear = 0;
            List<string> listquarter = new List<string>();

            int declarationOffsetDate = int.Parse(_currentCom.Config["DeclarationOffsetDate"]);
            int today = int.Parse(DateTime.UtcNow.Date.ToString("dd"));
            int tomonth = int.Parse(DateTime.UtcNow.Month.ToString());
            int toYear = int.Parse(DateTime.UtcNow.Year.ToString());
            ViewData["declarationOffsetDate"] = declarationOffsetDate > today ? "true" : "false";

            if (model == null)
            {
                ViewData["FirstDeclaration"] = "true";
                ViewData["Processedby"] = "";
                ViewData["userName"] = "";
                ViewData["Date"] = "";

                for (int i = 1; i <= 4; i++)
                {
                    string item = "Quý " + i + "/" + toYear;
                    listquarter.Add(item);
                }

                ViewData["listQuarter"] = listquarter;
                return View();
            }
            else
            {
                ViewData["FirstDeclaration"] = "false";
                ViewData["Processedby"] = model.Processedby;
                ViewData["userName"] = _TaxSvc.GetUserName(_currentCom.id, model.Processedby);
                ViewData["Date"] = model.ProcessedDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                date = model.LatestTaxDeclarationDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                month = int.Parse(date.Split('/')[1]);
                ViewData["Quarter"] = "Quý " + (month/3) + "/" + date.Split('/')[2];
                startYear = int.Parse(date.Split('/')[2]);

                if (startYear < toYear && (month / 3) == 3) 
                {
                    string item = "Quý " + ((month / 3) + 1) + "/" + startYear;
                    listquarter.Add(item);
                    //for (int i = 1; i <= 4; i++)
                    //{
                    //    item = "Quý " + i + "/" + toYear;
                    //    listquarter.Add(item);
                    //}
                }
                else if (startYear < toYear && (month / 3) == 4)
                {
                    for (int i = 1; i <= 4; i++)
                    {
                        string item = "Quý " + i + "/" + toYear;
                        listquarter.Add(item);
                    }
                }
                else if (startYear == toYear)
                {
                    for (int i = (month / 3) + 1; i <= 4; i++)
                    {
                        string item = "Quý " + i + "/" + toYear;
                        listquarter.Add(item);
                    }
                }

                ViewData["listQuarter"] = listquarter;
                return View();
            }                                             
        }

        [HttpPost]
        [RBACAuthorize(Permissions = "Release_invInTime")]
        public ActionResult Declare(string insertDeclarationDate = null, string Processedby = null)
        {
            if (!string.IsNullOrEmpty(insertDeclarationDate))
            {              
                int insertYear = int.Parse(insertDeclarationDate.Split(' ')[1].Split('/')[1]);
                int insertMonth = int.Parse(insertDeclarationDate.Split(' ')[1].Split('/')[0]) * 3;
                int insertDate = 0;
                if (insertMonth == 3 || insertMonth == 12)
                {
                    insertDate = 31;
                }
                else
                {
                    insertDate = 30;
                }

                string convertMonth = insertMonth < 10 ? ("0" + insertMonth) : insertMonth.ToString();
                string declarationDate = insertDate + "/" + convertMonth + "/" + insertYear;
                DateTime dt = DateTime.ParseExact(declarationDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                TaxDeclaration modelInsert = new TaxDeclaration
                {
                    ComId = _currentCom.id,
                    LatestTaxDeclarationDate = dt,
                    Processedby = int.Parse(Processedby),
                    ProcessedDate = DateTime.Now
                };

                _TaxSvc.Save(modelInsert);
                _TaxSvc.CommitChanges();
            }

            TaxDeclaration model = _TaxSvc.GetLastTaxDeclaration(_currentCom.id);
            string date = null;
            int month = 0;
            int startYear = 0;
            List<string> listquarter = new List<string>();

            int declarationOffsetDate = int.Parse(_currentCom.Config["DeclarationOffsetDate"]);
            int today = int.Parse(DateTime.UtcNow.Date.ToString("dd"));
            int tomonth = int.Parse(DateTime.UtcNow.Month.ToString());
            int toYear = int.Parse(DateTime.UtcNow.Year.ToString());
            ViewData["declarationOffsetDate"] = declarationOffsetDate > today ? "true" : "false";

            if (model == null)
            {
                ViewData["FirstDeclaration"] = "true";
                ViewData["Processedby"] = _TaxSvc.GetUserId(_currentCom.id, _currentCom.AccountName);
                ViewData["userName"] = "";
                ViewData["Date"] = "";

                for (int i = 1; i <= 4; i++)
                {
                    string item = "Quý " + i + "/" + toYear;
                    listquarter.Add(item);
                }

                ViewData["listQuarter"] = listquarter;
                return RedirectToAction("Index");
            }
            else
            {
                ViewData["FirstDeclaration"] = "false";
                ViewData["Processedby"] = model.Processedby;
                ViewData["userName"] = _TaxSvc.GetUserName(_currentCom.id, model.Processedby);
                ViewData["Date"] = model.ProcessedDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                date = model.LatestTaxDeclarationDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                month = int.Parse(date.Split('/')[1]);
                ViewData["Quarter"] = "Quý " + (month / 3) + "/" + date.Split('/')[2];
                startYear = int.Parse(date.Split('/')[2]);

                if (startYear < toYear && (month / 3) == 3)
                {
                    string item = "Quý " + ((month / 3) + 1) + "/" + startYear;
                    listquarter.Add(item);
                    for (int i = 1; i <= 4; i++)
                    {
                        item = "Quý " + i + "/" + toYear;
                        listquarter.Add(item);
                    }
                }
                else if (startYear == toYear)
                {
                    for (int i = (month / 3) + 1; i <= 4; i++)
                    {
                        string item = "Quý " + i + "/" + toYear;
                        listquarter.Add(item);
                    }
                }

                ViewData["listQuarter"] = listquarter;
                return RedirectToAction("Index");
            }
        }
    }   
}
