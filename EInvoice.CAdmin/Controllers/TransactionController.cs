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
    public class TransactionController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PublishController));
        //
        // GET: /Transaction/
        [RBACAuthorize(Permissions = "Release_invInTime")]
        public ActionResult Index(TransactionIndexModels model, int? TypeTran, int? page)
        {
            IList<Transaction> lst = new List<Transaction>();
            int defautPagesize = 10;
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            int totalRecords = 0;
            try
            {
                int typetran = TypeTran.HasValue ? TypeTran.Value : 0;
                if (typetran == 0) ViewData["Title"] = "Quản lý upload và phát hành lô hóa đơn";
                else if (typetran == 1) ViewData["Title"] = "Quản lý upload khách hàng";
                else if (typetran == 2) ViewData["Title"] = "Quản lý hủy lô hóa đơn";
                else if (typetran == 3) ViewData["Title"] = "Quản lý Phát hành lại hóa đơn";

                Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                ITransactionService tranSrv = IoC.Resolve<ITransactionService>();
                Guid gID = Guid.Empty;
                try
                {
                    gID = !string.IsNullOrEmpty(model.code) ? Guid.Parse(model.code.Trim()) : Guid.Empty;
                }
                catch (Exception ex)
                {
                    gID = Guid.NewGuid();
                    log.Error("Error:" + ex.Message);
                    Messages.AddErrorMessage("Vui lòng nhập đúng mã giao dịch!");
                }
                lst = tranSrv.GetByFilter(gID, currentCom.id, model.status, typetran, currentPageIndex, defautPagesize, out totalRecords);


            }
            catch (Exception ex)
            {
                log.Error(" Index -" + ex.Message);
                Messages.AddErrorMessage("Không có giao dịch trong hệ thống");
            }
            model.PagedListTransaction = new PagedList<Transaction>(lst, currentPageIndex, defautPagesize, totalRecords);
            ViewData["TypeTran"] = TypeTran;
            return View(model);
        }

        //Upload file phat hanh theo lo
        [RBACAuthorize(Permissions = "Release_invInTime")]
        public ActionResult Upload(int? TypeTran)
        {
            int typetran = TypeTran.HasValue ? TypeTran.Value : 0;
            IPublishInvoiceService _PubIn = IoC.Resolve<IPublishInvoiceService>();
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            UploadModel model = new UploadModel();
            model.Month = DateTime.Now.Month;
            model.Year = DateTime.Now.Year;
            if (DateTime.Now.Month < 12)
                model.Years = new List<int>() { DateTime.Now.Year - 1, DateTime.Now.Year };
            else
                model.Years = new List<int>() { DateTime.Now.Year };
            int m = DateTime.Now.Month;
            model.Months = new List<int>();
            while (m > 0)
            {
                model.Months.Add(m);
                m--;
            }
            model.TypeTrans = typetran;
            model.TypeLabel = TransactionName.NameByType(typetran);
            try
            {
                //lstpattern
                List<string> lstpattern = _PubIn.LstByPattern(currentCom.id, 1);
                if (lstpattern.Count == 0)
                {
                    Messages.AddErrorFlashMessage(Resources.Message.MInv_SMesNoPubAccess);
                    return RedirectToAction("Index", new { TypeTran = model.TypeTrans });
                }
                Dictionary<string, string> dicPattern = new Dictionary<string, string>();
                foreach (string str in lstpattern)
                {
                    dicPattern.Add(str, GetResxNameByValue(str));
                }

                model.Listpattern = new SelectList(dicPattern, "key", "value");
                //lstserial
                List<string> oserial = (from s in _PubIn.Query where ((s.ComId == currentCom.id) && (s.Status == 1 || s.Status == 2)) select s.InvSerial).ToList<string>();
                model.Listserial = new SelectList(oserial);
            }
            catch
            {
                Messages.AddErrorFlashMessage(Resources.Message.Key_MesReqConfig);
                return RedirectToAction("Index", new { TypeTran = model.TypeTrans });
            }
            return View(model);
        }

        [HttpPost]
        [RBACAuthorize(Permissions = "Release_invInTime")]
        public ActionResult UploadInvoiceCancelData(UploadModel model)
        {
            try
            {
                ITransactionService tranSrv = IoC.Resolve<ITransactionService>();
                EInvoiceContext Context = (EInvoiceContext)FXContext.Current;
                Company currentCom = Context.CurrentCompany;
                HttpPostedFileBase fileUpload = Request.Files["FilePath"];
                HttpPostedFileBase fileQDUpload = Request.Files["FileQDPath"];
                if (fileUpload == null || fileUpload.ContentLength == 0)
                {
                    Messages.AddErrorFlashMessage("Chưa chọn file dữ liệu hủy.");
                    return RedirectToAction("Upload", new { TypeTran = model.TypeTrans });
                }

                if (fileQDUpload == null || fileQDUpload.ContentLength == 0)
                {
                    Messages.AddErrorFlashMessage("Chưa chọn file quyết định hủy.");
                    return RedirectToAction("Upload", model);
                }
                byte[] qdinhHuybuffer = new byte[fileQDUpload.ContentLength];
                fileQDUpload.InputStream.Read(qdinhHuybuffer, 0, fileQDUpload.ContentLength);
                if (!fileQDUpload.FileName.ToLower().Contains(".jpg") && !fileQDUpload.FileName.ToLower().Contains(".png"))
                {
                    Messages.AddErrorFlashMessage("File QĐ upload phải là file .jpg hoặc .png!");
                    return RedirectToAction("Upload", new { TypeTran = model.TypeTrans });
                }

                byte[] dulieuhuyBuffer = new byte[fileUpload.ContentLength];
                fileUpload.InputStream.Read(dulieuhuyBuffer, 0, fileUpload.ContentLength);
                if (!fileUpload.FileName.ToLower().Contains(".zip") && !fileUpload.FileName.ToLower().Contains(".xls"))
                {
                    Messages.AddErrorFlashMessage("File upload phải là file .zip hoặc .xls.");
                    return RedirectToAction("Upload", new { TypeTran = model.TypeTrans });
                }

                Transaction mTran = new Transaction();
                mTran.Data = dulieuhuyBuffer;
                mTran.ComID = currentCom.id;
                mTran.id = Guid.NewGuid();
                mTran.InvPattern = model.Pattern;
                mTran.InvSerial = model.Serial;
                mTran.AccountName = HttpContext.User.Identity.Name;
                mTran.Status = TranSactionStatus.NewUpload;
                mTran.TypeTrans = model.TypeTrans;
                string fullPath = "";
                //string filePath = WebConfigurationManager.AppSettings["InvUnReleaseDecisionDir"]
                string filePath = AppDomain.CurrentDomain.BaseDirectory + WebConfigurationManager.AppSettings["InvUnReleaseDecisionDir"];
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                if (!filePath.EndsWith(@"\"))
                    filePath += @"\";
                fullPath = filePath + fileUpload.FileName;
                System.IO.File.Delete(fullPath);
                System.IO.File.WriteAllBytes(fullPath, qdinhHuybuffer);
                mTran.AttachedFile = fullPath;
                mTran.Reason = model.ReasonDel;

                if (fileUpload.FileName.ToLower().Contains(".xls"))
                {
                    //xu ly doc excell và tạo xml.
                    IDataTranService dataTranSrv = currentCom.Config.Keys.Contains("IDataTranService") ? IoC.Resolve(Type.GetType(currentCom.Config["IDataTranService"])) as IDataTranService : new ExcelDefaultDataTranService();
                    string parseError = "";
                    string xmlData = dataTranSrv.ParseIInvoiceCancel(mTran.Data, ref parseError);
                    //update lai du lieu data xml
                    if (null != xmlData)
                    {
                        mTran.Data = CompressFile(System.Text.Encoding.UTF8.GetBytes(xmlData));
                    }
                    else
                    {
                        mTran.Status = TranSactionStatus.Failed;
                        mTran.Messages = parseError;
                        tranSrv.CreateNew(mTran);
                        tranSrv.CommitChanges();
                        log.Info("Upload Transaction by: " + HttpContext.User.Identity.Name);
                        Messages.AddErrorFlashMessage("Upload file khách hàng lên hệ thống không thành công, theo dõi kết quả qua mã giao dịch: " + mTran.id);
                        return RedirectToAction("Index", new { TypeTran = model.TypeTrans });
                    }
                }

                tranSrv.CreateNew(mTran);
                tranSrv.CommitChanges();
                log.Info("Upload Transaction by: " + HttpContext.User.Identity.Name + "--Begin call webservice");
                IPoolingService service = new PoolingService(Context);
                service.cancelInv(mTran.id);
                Messages.AddFlashMessage("Upload file lên hệ thống thành công, theo dõi kết quả qua mã giao dịch: " + mTran.id);
                return RedirectToAction("Index", new { TypeTran = model.TypeTrans });
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return RedirectToAction("Upload", new { TypeTran = model.TypeTrans });
            }
        }

        [HttpPost]
        [RBACAuthorize(Permissions = "Release_invInTime")]
        public ActionResult UploadInvoiceData(UploadModel model, string DateNow)
        {
            try
            {
                ITransactionService tranSrv = IoC.Resolve<ITransactionService>();
                Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                HttpPostedFileBase fileUpload = Request.Files["FilePath"];
                if (fileUpload == null || fileUpload.ContentLength <= 0)
                {
                    Messages.AddErrorFlashMessage("Chưa chọn file dữ liệu hóa đơn.");
                    return RedirectToAction("Upload", new { TypeTran = model.TypeTrans });
                }

                Transaction mTran = new Transaction();
                byte[] bf = new byte[fileUpload.ContentLength];
                fileUpload.InputStream.Read(bf, 0, fileUpload.ContentLength);
                if (!fileUpload.FileName.ToLower().Contains(".zip") && !fileUpload.FileName.ToLower().Contains(".xls"))
                {
                    Messages.AddErrorFlashMessage("File upload phải là file .zip hoặc .xls.");
                    return RedirectToAction("Upload", new { TypeTran = model.TypeTrans });
                }

                mTran.Data = bf;
                mTran.ComID = currentCom.id;
                mTran.id = Guid.NewGuid();
                mTran.InvPattern = model.Pattern;
                mTran.InvSerial = model.Serial;
                mTran.AccountName = HttpContext.User.Identity.Name;
                mTran.Status = TranSactionStatus.NewUpload;
                mTran.TypeTrans = model.TypeTrans;
                mTran.BillTime = DateNow;
                IPoolingService service = new PoolingService((EInvoiceContext)FXContext.Current);

                if (fileUpload.FileName.ToLower().Contains(".xls"))
                {
                    //xu ly doc excell và tạo xml.
                    IDataTranService dataTranSrv = currentCom.Config.Keys.Contains("IDataTranService") ? IoC.Resolve(Type.GetType(currentCom.Config["IDataTranService"])) as IDataTranService : new ExcelDefaultDataTranService();
                    string parseError = "";
                    int failed = 0;
                    string xmlData = dataTranSrv.ParseIInvoice(mTran.Data, mTran.InvPattern, mTran.InvSerial, mTran.ComID, mTran.AccountName, ref parseError, ref failed);
                    xmlData = xmlData.Replace("<BillTime></BillTime>", "<BillTime>" + mTran.BillTime + "</BillTime>");
                    //update lai du lieu data xml
                    if (null != xmlData)
                    {
                        mTran.Data = CompressFile(System.Text.Encoding.UTF8.GetBytes(xmlData));
                    }
                    else
                    {
                        mTran.Status = TranSactionStatus.Failed;
                        mTran.Messages = parseError;
                        tranSrv.CreateNew(mTran);
                        tranSrv.CommitChanges();
                        log.Info("Upload Transaction by: " + HttpContext.User.Identity.Name);
                        Messages.AddErrorFlashMessage("Upload file khách hàng lên hệ thống không thành công, vui lòng kiểm tra theo mã giao dịch: " + mTran.id);
                        return RedirectToAction("Index", new { TypeTran = model.TypeTrans });
                    }
                }
                tranSrv.CreateNew(mTran);
                tranSrv.CommitChanges();                
                service.importAndPublishInv(mTran.id);
                Messages.AddFlashMessage("Upload file lên hệ thống thành công, theo dõi kết quả qua mã giao dịch: " + mTran.id);
                return RedirectToAction("Index", new { TypeTran = model.TypeTrans });
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return RedirectToAction("Upload", new { TypeTran = model.TypeTrans });
            }
        }

        [HttpPost]
        [RBACAuthorize(Permissions = "Release_invInTime")]
        public ActionResult UploadCustomerData(UploadModel model)
        {
            try
            {
                ITransactionService tranSrv = IoC.Resolve<ITransactionService>();
                EInvoiceContext Context = (EInvoiceContext)FXContext.Current;
                Company currentCom = Context.CurrentCompany;
                HttpPostedFileBase fileUpload = Request.Files["FilePath"];
                if (fileUpload == null || fileUpload.ContentLength <= 0)
                {
                    Messages.AddErrorFlashMessage("Chưa chọn file dữ liệu khách hàng.");
                    return RedirectToAction("Upload", new { TypeTran = model.TypeTrans });
                }

                Transaction mTran = new Transaction();
                byte[] bf = new byte[fileUpload.ContentLength];
                fileUpload.InputStream.Read(bf, 0, fileUpload.ContentLength);
                if (!fileUpload.FileName.ToLower().Contains(".zip") && !fileUpload.FileName.ToLower().Contains(".xls"))
                {
                    Messages.AddErrorFlashMessage("File upload phải là file .zip hoặc .xls !");
                    return RedirectToAction("Upload", new { TypeTran = model.TypeTrans });
                }

                mTran.Data = bf;
                mTran.ComID = currentCom.id;
                mTran.id = Guid.NewGuid();
                mTran.InvPattern = model.Pattern;
                mTran.InvSerial = model.Serial;
                mTran.AccountName = HttpContext.User.Identity.Name;
                mTran.Status = TranSactionStatus.NewUpload;
                mTran.TypeTrans = model.TypeTrans;
                IPoolingService service = new PoolingService(Context);
                if (fileUpload.FileName.ToLower().Contains(".xls"))
                {
                    //xu ly doc excell và tạo xml.
                    IDataTranService dataTranSrv = currentCom.Config.Keys.Contains("IDataTranService") ? IoC.Resolve(Type.GetType(currentCom.Config["IDataTranService"])) as IDataTranService : new ExcelDefaultDataTranService();
                    string parseError = "";
                    string xmlData = dataTranSrv.ParseCustomer(mTran.Data, ref parseError);
                    //update lai du lieu data xml
                    if (null != xmlData)
                    {
                        mTran.Data = CompressFile(System.Text.Encoding.UTF8.GetBytes(xmlData));
                    }
                    else
                    {
                        mTran.Status = TranSactionStatus.Failed;
                        mTran.Messages = parseError;
                        tranSrv.CreateNew(mTran);
                        tranSrv.CommitChanges();
                        log.Info("Upload Transaction by: " + HttpContext.User.Identity.Name);
                        Messages.AddErrorFlashMessage("Upload file khách hàng lên hệ thống không thành công, theo dõi kết quả qua mã giao dịch: " + mTran.id);
                        return RedirectToAction("Index", new { TypeTran = model.TypeTrans });
                    }
                }
                tranSrv.CreateNew(mTran);
                tranSrv.CommitChanges();
                log.Info("Upload Customer by: " + HttpContext.User.Identity.Name + "--Begin call webservice");
                service.importCus(mTran.id);
                Messages.AddFlashMessage("Upload file lên hệ thống thành công, theo dõi kết quả qua mã giao dịch: " + mTran.id);
                return RedirectToAction("Index", new { TypeTran = model.TypeTrans });
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return RedirectToAction("Upload", new { TypeTran = model.TypeTrans });
            }
        }

        public ActionResult Compare(Guid id)
        {
            try
            {
                ITransactionService tranSrv = IoC.Resolve<ITransactionService>();
                Transaction model = tranSrv.Getbykey(id);
                model.TranLock = 3;
                tranSrv.Save(model);
                tranSrv.CommitChanges();
                log.Info("Compare Transaction by: " + HttpContext.User.Identity.Name);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return RedirectToAction("Index");
        }

        [RBACAuthorize(Permissions = "Release_invInTime")]
        public ActionResult Details(Guid id, int? TypeTran)
        {
            int typetran = TypeTran.HasValue ? TypeTran.Value : 0;
            ITransactionService tranSrv = IoC.Resolve<ITransactionService>();
            Transaction model = tranSrv.Getbykey(id);
            ViewData["TypeTran"] = typetran;
            return View(model);
        }

        [RBACAuthorize(Permissions = "Release_invInTime")]
        public ActionResult Delete(Guid id, int? TypeTran)
        {
            try
            {
                int typetran = TypeTran.HasValue ? TypeTran.Value : 0;
                ITransactionService tranSrv = IoC.Resolve<ITransactionService>();
                Transaction model = tranSrv.Getbykey(id);
                tranSrv.Delete(model);
                tranSrv.CommitChanges();
                Messages.AddFlashMessage("Xóa thành công!");
                log.Info("Delete Transaction by: " + HttpContext.User.Identity.Name + " Info-- Type Tran: " + model.TypeTrans + "  GuiID: " + model.id + "------");
            }
            catch (Exception ex)
            {
                log.Error(" Delete -" + ex.Message);
                Messages.AddErrorFlashMessage("Có lỗi xảy ra, vui lòng kiểm tra log để biết thêm chi tiết.");
            }
            return RedirectToAction("Index", new { TypeTran = TypeTran });
        }

        // Download ket qua upload loi
        public void Download(Guid id, string downloadtype)
        {
            ITransactionService tranSrv = IoC.Resolve<ITransactionService>();
            Transaction model = tranSrv.Getbykey(id);
            if (model == null)
            {
                Response.Clear();
                Response.Write("<script type='text/javascript'>alert('Có lỗi trong quá trình tải dữ liệu! <br /> Lỗi: Không tồn tại giao dịch');</script>");
                Response.Redirect("/Home/Index");
                Response.End();
                Response.Flush();
            }
            if (downloadtype != "dbf" && downloadtype != "xml")
            {
                Response.Clear();
                Response.Redirect("/Home/PotentiallyError");
                Response.End();
                Response.Flush();
            }
            byte[] buffer = model.FailResult;
            if (buffer != null)
                try
                {
                    if (downloadtype == "dbf")
                    {
                        // Hoa don phat hanh moi
                        if (model.TypeTrans == 0)
                        {
                            var bytestring = Utils.Decompress(buffer);
                            string _b2str = Encoding.UTF8.GetString(bytestring);
                            if (_b2str.EndsWith("</Payment>"))
                            {
                                string strarr = _b2str.Split(new string[] { "</Invoices>" }, StringSplitOptions.RemoveEmptyEntries)[1];

                                DbfFile _file = new DbfFile(Encoding.GetEncoding(1252));
                                MemoryStream m = new MemoryStream();

                                _file.Open(m);

                                _file.Header.AddColumn(new DbfColumn("Key", DbfColumn.DbfColumnType.Character, 20, 0));
                                _file.Header.AddColumn(new DbfColumn("Err", DbfColumn.DbfColumnType.Character, 20, 0));


                                DbfRecord _record = new DbfRecord(_file.Header);


                                XElement docx = XElement.Parse(strarr, LoadOptions.PreserveWhitespace);
                                foreach (XElement element in docx.Elements("inv"))
                                {
                                    _record[0] = element.Element("key").Value;
                                    _record[1] = element.Element("ERR").Value;
                                    _file.Write(_record, true);
                                }

                                _file.WriteHeader();
                                _file.Close();

                                var fbuff = Utils.Compress(m.ToArray(), "hoadonfailes.dbf");


                                if (fbuff != null)
                                {
                                    Response.ContentType = "text/plain";
                                    Response.OutputStream.Write(fbuff, 0, fbuff.Length);
                                    Response.AddHeader("Content-Disposition", "attachment;filename=hoadondbffailes.zip");
                                }
                                else
                                    Response.Write(String.Format("<script type='text/javascript'>alert('Có lỗi trong quá trình tải dữ liệu!'); document.location = '/Transaction/Index?TypeTran={0}';</script>", model.TypeTrans));
                            }
                            else
                            {
                                if (buffer != null)
                                {
                                    Response.ContentType = "text/plain";
                                    Response.OutputStream.Write(buffer, 0, buffer.Length);
                                    Response.AddHeader("Content-Disposition", "attachment;filename=hoadonfailes.zip");
                                }
                                else
                                    Response.Write(String.Format("<script type='text/javascript'>alert('Có lỗi trong quá trình tải dữ liệu!'); document.location = '/Transaction/Index?TypeTran={0}';</script>", model.TypeTrans));
                            }
                        }
                    }
                    // downloadtype == "xml"
                    if (downloadtype == "xml")
                    {
                        // Khach hang
                        if (model.TypeTrans == 1)
                        {
                            if (buffer != null)
                            {
                                var khBuff = Utils.Decompress(buffer);
                                var fbuff = Utils.Compress(khBuff, "khachhang.xml");
                                Response.ContentType = "text/plain";
                                Response.OutputStream.Write(fbuff, 0, fbuff.Length);
                                Response.AddHeader("Content-Disposition", "attachment;filename=khachhangfailes.zip");
                            }
                            else
                                Response.Write(String.Format("<script type='text/javascript'>alert('Có lỗi trong quá trình tải dữ liệu!'); document.location = '/Transaction/Index?TypeTran={0}';</script>", model.TypeTrans));
                        }
                        // Hoa don moi
                        else if (model.TypeTrans == 0)
                        {
                            if (buffer != null)
                            {
                                Response.ContentType = "text/plain";
                                Response.OutputStream.Write(buffer, 0, buffer.Length);
                                Response.AddHeader("Content-Disposition", "attachment;filename=hoadonfailes.zip");
                            }
                            else
                                Response.Write(String.Format("<script type='text/javascript'>alert('Có lỗi trong quá trình tải dữ liệu!'); document.location = '/Transaction/Index?TypeTran={0}';</script>", model.TypeTrans));
                        }
                        // Hoa don huy
                        else if (model.TypeTrans == 2)
                        {
                            if (buffer != null)
                            {
                                Response.ContentType = "text/plain";
                                Response.OutputStream.Write(buffer, 0, buffer.Length);
                                Response.AddHeader("Content-Disposition", "attachment;filename=hoadonhuyfailes.zip");
                            }
                            else
                                Response.Write(String.Format("<script type='text/javascript'>alert('Có lỗi trong quá trình tải dữ liệu!'); document.location = '/Transaction/Index?TypeTran={0}';</script>", model.TypeTrans));
                        }
                        // Hoa don lai
                        else if (model.TypeTrans == 3)
                        {
                            if (buffer != null)
                            {
                                Response.ContentType = "text/plain";
                                Response.OutputStream.Write(buffer, 0, buffer.Length);
                                Response.AddHeader("Content-Disposition", "attachment;filename=hoadonlaifailes.zip");
                            }
                            else
                                Response.Write(String.Format("<script type='text/javascript'>alert('Có lỗi trong quá trình tải dữ liệu!'); document.location = '/Transaction/Index?TypeTran={0}';</script>", model.TypeTrans));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Response.Clear();
                    Response.Write(String.Format("<script type='text/javascript'>alert('Có lỗi trong quá trình tải dữ liệu!'); document.location = '/Transaction/Index?TypeTran={0}';</script>", model.TypeTrans));
                    Response.End();
                    Response.Flush();
                }
            else
            {
                Response.Clear();
                Response.Write(String.Format("<script type='text/javascript'>alert('Có lỗi trong quá trình tải dữ liệu!'); document.location = '/Transaction/Index?TypeTran={0}';</script>", model.TypeTrans));
                Response.End();
                Response.Flush();
            }
        }

        // Download ket qua upload thanh cong
        public void Completed(Guid id, string downloadtype = "xml")
        {
            ITransactionService tranSrv = IoC.Resolve<ITransactionService>();
            Transaction model = tranSrv.Getbykey(id);
            if (model == null)
            {
                Response.Clear();
                Response.Write("<script type='text/javascript'>alert('Có lỗi trong quá trình tải dữ liệu! <br /> Lỗi: Không tồn tại file');</script>");
                Response.Redirect("/Home/Index");
                Response.End();
                Response.Flush();
            }
            if (downloadtype != "dbf" && downloadtype != "xml")
            {
                Response.Clear();
                Response.Redirect("/Home/PotentiallyError");
                Response.End();
                Response.Flush();
            }
            byte[] buffer = model.CompleteResult;

            if (buffer != null)
                try
                {
                    if (downloadtype == "dbf")
                    {
                        // Khach hang
                        if (model.TypeTrans == 1)
                        {
                            var bytestring = Utils.Decompress(buffer);
                            string _b2str = Encoding.UTF8.GetString(bytestring);

                            //string[] strarr = _b2str.Replace("OK:", "").Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                            //var xxx = strarr[1].Replace("-", "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                            string[] strarr = _b2str.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                            var xxx = strarr[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                            DbfFile _file = new DbfFile(Encoding.GetEncoding(1252));
                            MemoryStream m = new MemoryStream();

                            _file.Open(m);

                            _file.Header.AddColumn(new DbfColumn("Account", DbfColumn.DbfColumnType.Character, 50, 0));
                            _file.Header.AddColumn(new DbfColumn("Password", DbfColumn.DbfColumnType.Character, 50, 0));

                            DbfRecord _record = new DbfRecord(_file.Header);

                            foreach (string item in xxx)
                            {
                                if (item.Contains(";"))
                                {
                                    var ix = item.Split(';');
                                    _record[0] = ix[0];
                                    _record[1] = ix[1];
                                    _file.Write(_record, true);
                                }
                            }

                            _file.WriteHeader();
                            _file.Close();

                            var fbuff = Utils.Compress(m.ToArray(), "khachhang.dbf");

                            if (fbuff != null)
                            {
                                Response.ContentType = "text/plain";
                                Response.OutputStream.Write(fbuff, 0, fbuff.Length);
                                Response.AddHeader("Content-Disposition", "attachment;filename=khachhangdbf.zip");
                            }
                            else
                                Response.Write(String.Format("<script type='text/javascript'>alert('Có lỗi trong quá trình tải dữ liệu!'); document.location = '/Transaction/Index?TypeTran={0}';</script>", model.TypeTrans));
                        }
                        // Hoa don phat hanh moi
                        else if (model.TypeTrans == 0 || model.TypeTrans == 3)
                        {
                            var bytestring = Utils.Decompress(buffer);
                            string _b2str = Encoding.UTF8.GetString(bytestring);
                            string[] strarr = _b2str.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                            var xxx = strarr[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            DbfFile _file = new DbfFile(Encoding.GetEncoding(1252));
                            MemoryStream m = new MemoryStream();

                            _file.Open(m);

                            _file.Header.AddColumn(new DbfColumn("FKey", DbfColumn.DbfColumnType.Character, 20, 0));
                            _file.Header.AddColumn(new DbfColumn("No", DbfColumn.DbfColumnType.Character, 20, 0));
                            _file.Header.AddColumn(new DbfColumn("Pattern", DbfColumn.DbfColumnType.Character, 20, 0));
                            _file.Header.AddColumn(new DbfColumn("Serial", DbfColumn.DbfColumnType.Character, 20, 0));
                            _file.Header.AddColumn(new DbfColumn("PublishDate", DbfColumn.DbfColumnType.Character, 20, 0));

                            DbfRecord _record = new DbfRecord(_file.Header);

                            foreach (string item in xxx)
                            {
                                if (item.Contains("_"))
                                {
                                    var ix = item.Split('_');
                                    _record[0] = ix[0];
                                    _record[1] = ix[1];
                                    //_record[2] = strarr[0];
                                    //_record[3] = ix.Length == 4 ? ix[2] : model.InvSerial;
                                    //_record[4] = ix.Length == 4 ? ix[3] : "";
                                    _record[2] = model.InvPattern;
                                    _record[3] = model.InvSerial;
                                    _record[4] = ix[3];
                                    _file.Write(_record, true);
                                }
                            }

                            _file.WriteHeader();
                            _file.Close();

                            string strName = "";
                            if (model.TypeTrans == 0)
                            {
                                strName = "hoadon";
                            }
                            else
                            {
                                strName = "hoadonlai";
                            }
                            var fbuff = Utils.Compress(m.ToArray(), strName + ".dbf");

                            if (fbuff != null)
                            {
                                Response.ContentType = "text/plain";
                                Response.OutputStream.Write(fbuff, 0, fbuff.Length);
                                Response.AddHeader("Content-Disposition", "attachment;filename=" + strName + "dbf.zip");
                            }
                            else
                                Response.Write(String.Format("<script type='text/javascript'>alert('Có lỗi trong quá trình tải dữ liệu!'); document.location = '/Transaction/Index?TypeTran={0}';</script>", model.TypeTrans));
                        }
                        // Hoa don huy
                        else if (model.TypeTrans == 2)
                        {
                            var bytestring = Utils.Decompress(buffer);
                            string _b2str = Encoding.UTF8.GetString(bytestring);
                            string[] strarr = _b2str.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                            var xxx = strarr[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            DbfFile _file = new DbfFile(Encoding.GetEncoding(1252));
                            MemoryStream m = new MemoryStream();

                            _file.Open(m);

                            _file.Header.AddColumn(new DbfColumn("FKey", DbfColumn.DbfColumnType.Character, 20, 0));

                            DbfRecord _record = new DbfRecord(_file.Header);

                            foreach (string item in xxx)
                            {
                                _record[0] = item;
                                _file.Write(_record, true);
                            }

                            _file.WriteHeader();
                            _file.Close();

                            var fbuff = Utils.Compress(m.ToArray(), "hoadonhuy.dbf");

                            if (fbuff != null)
                            {
                                Response.ContentType = "text/plain";
                                Response.OutputStream.Write(fbuff, 0, fbuff.Length);
                                Response.AddHeader("Content-Disposition", "attachment;filename=hoadonhuydbf.zip");
                            }
                            else
                                Response.Write(String.Format("<script type='text/javascript'>alert('Có lỗi trong quá trình tải dữ liệu!'); document.location = '/Transaction/Index?TypeTran={0}';</script>", model.TypeTrans));
                        }
                    }
                    // downloadtype == "xml"
                    if (downloadtype == "xml")
                    {
                        // Khach hang
                        if (model.TypeTrans == 1)
                        {
                            var fff = Utils.Decompress(buffer);
                            var fbuff = Utils.Compress(fff, "khachhang.xml");
                            Response.ContentType = "text/plain";
                            Response.OutputStream.Write(fbuff, 0, fbuff.Length);
                            Response.AddHeader("Content-Disposition", "attachment;filename=khachhang.zip");
                        }
                        // Hoa don moi
                        else if (model.TypeTrans == 0)
                        {
                            Response.ContentType = "text/plain";
                            Response.OutputStream.Write(buffer, 0, buffer.Length);
                            Response.AddHeader("Content-Disposition", "attachment;filename=hoadon.zip");
                        }
                        // Hoa don huy
                        else if (model.TypeTrans == 2)
                        {
                            Response.ContentType = "text/plain";
                            Response.OutputStream.Write(buffer, 0, buffer.Length);
                            Response.AddHeader("Content-Disposition", "attachment;filename=hoadonhuy.zip");
                        }
                        // Hoa don lai
                        else if (model.TypeTrans == 3)
                        {
                            Response.ContentType = "text/plain";
                            Response.OutputStream.Write(buffer, 0, buffer.Length);
                            Response.AddHeader("Content-Disposition", "attachment;filename=hoadonlai.zip");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Response.Write(String.Format("<script type='text/javascript'>alert('Có lỗi trong quá trình tải dữ liệu!'); document.location = '/Transaction/Index?TypeTran={0}';</script>", model.TypeTrans));
                }
            else
                Response.Write(String.Format("<script type='text/javascript'>alert('Có lỗi trong quá trình tải dữ liệu!'); document.location = '/Transaction/Index?TypeTran={0}';</script>", model.TypeTrans));
        }

        public void DownloadCompare(Guid id)
        {
            ITransactionService tranSrv = IoC.Resolve<ITransactionService>();
            Transaction model = tranSrv.Getbykey(id);
            try
            {
                if (model == null)
                {
                    Response.Write(String.Format("<script type='text/javascript'>alert('Không tồn tại, Key: {0}');</script>", id));
                    return;
                }
                var guidString = id.ToString("n");
                var byteString = ToHexString(id.ToByteArray());
                string fullpath = ConfigurationSettings.AppSettings["FileCompare"] + @"\" + byteString.ToUpper() + ".TXT";
                string fileName = Path.GetFileName(fullpath);
                byte[] buffer = System.IO.File.ReadAllBytes(fullpath);
                Response.ContentType = "text/plain";
                Response.OutputStream.Write(buffer, 0, buffer.Length);
                Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);
                return;
            }
            catch (FileNotFoundException fe)
            {
                Response.Write(String.Format("<script type='text/javascript'>alert('Không tồn tại, Key: {0}'); document.location = '/Transaction/Index?TypeTran={1}';</script>", id, model.TypeTrans));
                return;
            }
            catch (Exception ex)
            {
                Response.Write(String.Format("<script type='text/javascript'>alert('Không tồn tại, Key: {0}'); document.location = '/Transaction/Index?TypeTran={1}';</script>", id, model.TypeTrans));
                return;
            }
        }

        private String ToHexString(Byte[] bytes)
        {
            var hex = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        private byte[] CompressFile(byte[] data)
        {
            Stream stream = new MemoryStream(data);
            // Compress
            using (MemoryStream fsOut = new MemoryStream())
            {
                using (ICSharpCode.SharpZipLib.Zip.ZipOutputStream zipStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(fsOut))
                {
                    zipStream.SetLevel(3);
                    ICSharpCode.SharpZipLib.Zip.ZipEntry newEntry = new ICSharpCode.SharpZipLib.Zip.ZipEntry("data.xml");
                    newEntry.DateTime = DateTime.UtcNow;
                    zipStream.PutNextEntry(newEntry);
                    //zipStream.Write(data, 0, data.Length);
                    StreamUtils.Copy(stream, zipStream, new byte[2048]);
                    zipStream.Finish();
                    zipStream.Close();
                }
                return fsOut.ToArray();
            }
        }

        private string GetResxNameByValue(string value)
        {
            var key = "";
            try
            {
                System.Resources.ResourceManager rm = new System.Resources.ResourceManager("Resources.Einvoice", Assembly.Load("App_GlobalResources"));

                var entry = rm.GetResourceSet(System.Threading.Thread.CurrentThread.CurrentCulture, true, true)
                  .OfType<DictionaryEntry>()
                  .FirstOrDefault(e => e.Key.ToString() == value);

                key = entry.Value.ToString();
            }
            catch
            {
                key = value;
            }
            return key;
        }
    }
}
