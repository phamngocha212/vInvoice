using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EInvoice.CAdmin.Models;
using EInvoice.Core.IService;
using FX.Core;
using FX.Utils.MVCMessage;
using System.Text.RegularExpressions;
using FX.Utils.MvcPaging;
using System.Text;
using IdentityManagement.Authorization;

namespace EInvoice.CAdmin.Controllers
{
    public class LogManagerController : BaseController
    {
        private readonly string logFolder;
        private ILogSystemService logService;
        public LogManagerController()
        {
            logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["FolderLog"]);
            logService = IoC.Resolve<ILogSystemService>();
        }
        [RBACAuthorize(Permissions = "Search_Log")]
        public ActionResult Index(LogSystemModels model)
        {
            IList<FileInfo> list = new List<FileInfo>();
            model.LogsInfo = new List<FileInfo>();
            if (!string.IsNullOrWhiteSpace(model.DateModify))
            {
                DateTime? DateMod = null;
                DateMod = DateTime.ParseExact(model.DateModify, "dd/MM/yyyy", null);
                list = logService.GetbyDateMod(DateMod.Value, logFolder);
            }
            else
            {
                list = logService.GetbyDateNow(logFolder);
            }
            if (!string.IsNullOrEmpty(model.Keysearch))
            {
                foreach (FileInfo file in list)
                {
                    var fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    try
                    {
                        using (var sr = new StreamReader(fs))
                        {
                            string allRead = sr.ReadToEnd();
                            sr.Close();
                            if (Regex.IsMatch(allRead, model.Keysearch))
                            {
                                model.LogsInfo.Add(file);
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            }
            else
            {
                model.LogsInfo = list;
            }
            //model.pageLogsInfo = new PagedList<FileInfo>(model.LogsInfo, currentPageIndex, defautPagesize, total);
            return View(model);
        }

        [RBACAuthorize(Permissions = "View_Log")]
        public void Download(string name)
        {
            try
            {
                if (!name.StartsWith("log"))
                    throw new HttpRequestValidationException();
                string path = Path.Combine(logFolder, name);
                if (System.IO.File.Exists(path) && !string.IsNullOrEmpty(name))
                {
                    try
                    {
                        string content = EInvoice.Core.Utils.GetContent(path);
                        if (!string.IsNullOrEmpty(content))
                        {
                            Response.Clear();
                            Response.ContentType = "text/plain";// "application/ms-word";
                            Response.AddHeader("Content-Disposition", "attachment;filename=log.txt");
                            Response.WriteFile(path);
                            Response.Flush();
                            Response.Close();
                        }
                    }
                    catch (IOException ex)
                    {
                        string newFile = Path.Combine(logFolder, ".CurrentLog");
                        if (System.IO.File.Exists(newFile))
                            System.IO.File.Delete(newFile);
                        System.IO.File.Copy(path, newFile);
                        Response.Clear();
                        Response.ContentType = "text/plain";// "application/ms-word";
                        Response.AddHeader("Content-Disposition", "attachment;filename=log.txt");
                        Response.WriteFile(newFile);
                        Response.Flush();
                        Response.Close();
                    }
                }
                Response.Clear();
                Response.Write("<script type='text/javascript'>alert('Có lỗi trong quá trình tải dữ liệu!'); document.location = '/Logmanager/Index';</script>");
                Response.End();
                Response.Flush();
            }
            catch (Exception ex)
            {
                Response.Clear();
                Response.Write("<script type='text/javascript'>alert('Có lỗi trong quá trình tải dữ liệu!'); document.location = '/Logmanager/Index';</script>");
                Response.End();
                Response.Flush();
            }            
        }        
    }
}
