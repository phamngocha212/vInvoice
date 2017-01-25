using EInvoice.Core;
using EInvoice.Core.Domain;
using EInvoice.Core.IService;
using EInvoice.Core.Launching;
using FX.Context;
using FX.Core;
using IdentityManagement;
using IdentityManagement.Authorization;
using IdentityManagement.Domain;
using IdentityManagement.Service;
using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace EInvoice.CAdmin.ServiceImp
{
    public class PoolingService : IPoolingService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PoolingService));
        private static Hashtable LockTable = new Hashtable();
        private readonly string FIX_LENGTH_INV_NUMBER = "0000000";

        private EInvoiceContext CurrentContext { get; set; }

        public PoolingService(EInvoiceContext Context)
        {
            this.CurrentContext = Context;
        }        

        public string importAndPublishInv(Guid key)
        {
            Thread myThread = new Thread(publishInvoice);
            myThread.Start(key);
            return "start to import and publish Inv";
        }

        public string importCus(Guid key)
        {
            Thread myThread = new Thread(updateCus);
            myThread.Start(key);
            return "Start to import Cus Thread";
        }

        public string updateInv(Guid key)
        {
            Thread myThread = new Thread(updateAndPublishInv);
            myThread.Start(key);
            return "Start to update Inv Thread";
        }

        public string cancelInv(Guid key)
        {
            Thread myThread = new Thread(cancelInvoice);
            myThread.Start(key);
            return "Start to cancel Inv Thread";
        }

        private void publishInvoice(object keyObject)
        {
            ILog log = LogManager.GetLogger(typeof(PoolingService));
            Guid key = (Guid)keyObject;
            if (key == null || key == Guid.Empty)            
                return;
            BootstrapperForThread.InitializeContainer();
            ITransactionService tranSrv = IoC.Resolve<ITransactionService>();
            Transaction tran = tranSrv.Getbykey(key);
            try
            {
                log.Error("Xu ly XML:" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                
                int failedInt = 0;
                int failed = 0;
                string mess = "";
                
                if (tran.Status != TranSactionStatus.NewUpload) return;
                try
                {
                    tran.Status = TranSactionStatus.Processing;
                    tranSrv.updateResult(tran);
                    //it.TranLock = 1;                                        
                    //end
                    string[] roles = { "Printer" };
                    IList<FanxiPermission> fxPer = new List<FanxiPermission>();
                    UserIdentity tempId = new UserIdentity(tran.AccountName, fxPer, roles);
                    FanxiPrincipal _principal = new FanxiPrincipal(tempId);
                    CurrentContext.SetUserContext(_principal);
                    Company currentCompany = CurrentContext.CurrentCompany;
                    IDeliver _DeliverService = currentCompany.Config.ContainsKey("IDeliver") ? InvServiceFactory.GetDeliver(currentCompany.Config["IDeliver"]) : null;
                    string tempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["TempFolder"], tran.id.ToString());
                    CompressHelper.unZip(tran.Data, tempPath);
                    //read data and validate
                    IList<IInvoice> listINV = new List<IInvoice>();
                    IList<string> lstKey = new List<string>();
                    Type temp = InvServiceFactory.GetInvoiceType(tran.InvPattern, tran.ComID);
                    StringBuilder Failed = new StringBuilder("<Invoices>");
                    int count = 0;
                    string rv = "";
                    int totalRecord = 0;

                    string nameInvoice = getNameInvoice(tran.InvPattern, tran.InvSerial, currentCompany.id);
                    XmlSchemaValidator validator = new XmlSchemaValidator();
                    using (StreamReader reader = new StreamReader(tempPath))
                    {
                        string tempString = reader.ReadLine();  //đọc qua BillTime
                        if (!tempString.Contains(tran.BillTime))
                        {
                            log.Error("Error ValidXml" + validator.ValidationError);
                            //customer xml string not valid, don't do anything
                            tran.Status = TranSactionStatus.Failed;
                            tran.Messages = "Thời gian phát hành không đúng!";
                            tran.FailResult = tran.Data;
                            tran.TranLock = 0;
                            tranSrv.Save(tran);
                            tranSrv.CommitChanges();
                            return;
                        }
                        while (!string.IsNullOrWhiteSpace(tempString) && (reader.Peek() >= 0))
                        {
                            tempString = reader.ReadLine();
                            //log.Error(totalRecord + " - " + tempString);
                            if (reader.EndOfStream)
                                break;
                            totalRecord++;
                            if (!validator.ValidXmlDoc(tempString, "", AppDomain.CurrentDomain.BaseDirectory + @"XMLValidate\VATInVoice_ws.xsd"))
                            {
                                log.Error("Error ValidXml" + validator.ValidationError);
                                //customer xml string not valid, don't do anything
                                tran.Status = TranSactionStatus.Failed;
                                tran.Messages = validator.ValidationError;
                                tran.FailResult = tran.Data;
                                tran.TranLock = 0;
                                tranSrv.Save(tran);
                                tranSrv.CommitChanges();
                                return;
                            }
                            XElement e = XElement.Parse(tempString);
                            XElement ele = e.Element("Invoice");
                            string fKey = e.Element("key").Value;
                            fKey = fKey.Trim();
                            IInvoice inv = (IInvoice)Activator.CreateInstance(temp);//InvServiceFactory.NewInstance(it.InvPattern, it.ComID);
                            string read = string.Concat(ele);
                            bool d = DataHelper.DeserializeEinvFromXML(read, inv);
                            if (!d)
                            {
                                Failed.Append("<ERR>DeserializeFromXML</ERR>:" + read);
                                failed++;
                                continue;
                            }
                            if (string.IsNullOrWhiteSpace(inv.CusName) && string.IsNullOrWhiteSpace(inv.Buyer))
                            {
                                Failed.Append("<ERR>Name invalid</ERR>:" + read);
                                tran.Status = TranSactionStatus.Failed;
                                tran.Messages = "Không có tên khách hàng.";
                                tran.FailResult = tran.Data;
                                tran.TranLock = 0;
                                tranSrv.Save(tran);
                                tranSrv.CommitChanges();
                                return;
                            }
                            inv.Name = nameInvoice;
                            inv.Pattern = tran.InvPattern;
                            inv.Serial = tran.InvSerial;
                            inv.CreateBy = tran.AccountName;
                            inv.Fkey = fKey;
                            listINV.Add(inv);
                            count++;
                        }
                    }
                    System.IO.File.Delete(tempPath);                    
                    proceedData(listINV.ToArray(), currentCompany, tran.InvPattern, tran.InvSerial, out mess, out failedInt);
                    //end launching
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < listINV.Count; i++)
                    {
                        if (listINV[i].No > 0)
                        {
                            sb.AppendFormat("{0}_{1}_{2}_{3},", listINV[i].Fkey, listINV[i].No.ToString(FIX_LENGTH_INV_NUMBER), listINV[i].Serial, listINV[i].PublishDate.ToString("dd/MM/yyyy"));
                        }
                        else
                        {
                            sb.AppendFormat("{0}_{1}_{2}_{3},", listINV[i].Fkey, listINV[i].Pattern, listINV[i].Serial, listINV[i].ArisingDate.ToString("dd/MM/yyyy"));
                        }
                    }
                    decimal MaxNo = 0;
                    decimal MinNo = 0;
                    if (listINV.Count > 0 && listINV[0].No > 0)
                    {
                        MaxNo = listINV.Max(i => i.No);
                        MinNo = listINV.Where(i => i.No > 0).Min(ii => ii.No);
                    }
                    rv = sb.ToString();
                    if (failed > 0 || failedInt > 0)
                    {
                        Failed.Append("</Invoices>");
                        //remove the last "," character
                        rv = "OK:" + tran.InvPattern + ";" + tran.InvSerial + "-" + rv.Remove(rv.Length - 1, 1);
                        log.Error("start compress false " + key + ": " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                        tran.CompleteResult = CompressHelper.CompressFile(System.Text.Encoding.ASCII.GetBytes(rv), "hoadon.xml"); ;
                        tran.FailResult = CompressHelper.CompressFile(System.Text.Encoding.ASCII.GetBytes(Failed.ToString()), "hoadon.xml");
                        tran.Messages = "Phát hành thành công: " + (count + failedInt) + ",lỗi: " + (totalRecord - count - failedInt - 1) + " hóa đơn";
                        log.Error("error: " + key + " count:" + count + " falseInt: " + failedInt + ",lst.Count(): " + totalRecord + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                        log.Error("end compress false " + key + ": " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                        tran.Status = TranSactionStatus.NotComplete;
                        tran.TranLock = 0;
                        tran.NoRange = MinNo.ToString() + "_" + MaxNo.ToString();
                    }
                    else
                    {
                        //ghi nhan so hoa don, key tra ve
                        rv = "OK:" + tran.InvPattern + ";" + tran.InvSerial + "-" + rv.Remove(rv.Length - 1, 1);
                        tran.CompleteResult = CompressHelper.CompressFile(System.Text.Encoding.ASCII.GetBytes(rv), "hoadon.xml");
                        tran.Messages = "Phát hành thành công: " + count + " hóa đơn";
                        tran.Status = TranSactionStatus.Completed;
                        tran.TranLock = 0;
                        tran.NoRange = MinNo.ToString() + "_" + MaxNo.ToString();
                    }
                    tranSrv.updateResult(tran);
                    if (tran.Status == TranSactionStatus.Completed && _DeliverService != null)
                        _DeliverService.PrepareDeliver(listINV.ToArray(), currentCompany);
                }

                catch (EInvoice.Core.Launching.NoFactory.OpenTranException ex)
                {
                    log.Error(ex);
                    tran.Messages = ex.Message;
                    tran.Status = TranSactionStatus.Failed;
                    tran.TranLock = 0;
                    tranSrv.updateResult(tran);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    tran.Messages = ex.Message;
                    tran.Status = TranSactionStatus.Failed;
                    tran.TranLock = 0;
                    tranSrv.updateResult(tran);
                }
                finally
                {
                    tranSrv.UnbindSession(tran);
                }
            }
            catch (Exception ex)
            {                
                log.Error(ex);
                tran.Messages = ex.Message;
                tran.Status = TranSactionStatus.Failed;
                tran.TranLock = 0;
                tranSrv.updateResult(tran);
            }
            return;
        }

        private void proceedData(IInvoice[] listINV, Company currentCompany, string invPattern, string invSerial, out string errorMessage, out int failedInt)
        {
            failedInt = 0;
            errorMessage = "";
            if (!currentCompany.Config.Keys.Contains("SignPlugin") || int.Parse(currentCompany.Config["SignPlugin"].Trim()) == 0)
            {
                Launcher.Instance.Launch(invPattern, invSerial, listINV, out failedInt, out errorMessage, CurrentContext);
            }
            else
            {
                IInvoiceService t = EInvoice.Core.InvServiceFactory.GetService(invPattern, currentCompany.id);
                t.BeginTran();
                try
                {
                    if (listINV.Length > 50)
                        t.isStateLess = true;
                    t.CreateInvoice(listINV.ToArray(), out errorMessage);
                    t.CommitTran();
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    t.RolbackTran();
                    failedInt = 1;
                }
                finally
                {
                    t.isStateLess = false;
                }
            }
        }

        public void updateAndPublishInv(object keyObject)
        {
            ILog log = LogManager.GetLogger(typeof(PoolingService));
            Guid key = (Guid)keyObject;
            try
            {
                log.Error("Xu ly XML:" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                BootstrapperForThread.InitializeContainer();
                int failedInt = 0;
                int failed = 0;
                string mess = "";
                ITransactionService tranSrv = IoC.Resolve<ITransactionService>();
                Transaction it = tranSrv.Getbykey(key);
                if (it.Status != TranSactionStatus.NewUpload) return;
                try
                {
                    it.Status = TranSactionStatus.Processing;
                    tranSrv.updateResult(it);
                    //tranSrv.CommitChanges(); 
                    it.TranLock = 1;
                    string[] roles = { "Printer" };
                    IList<FanxiPermission> fxPer = new List<FanxiPermission>();
                    UserIdentity tempId = new UserIdentity(it.AccountName, fxPer, roles);
                    FanxiPrincipal _principal = new FanxiPrincipal(tempId);
                    FXContext.Current.SetUserContext(_principal);

                    var _currentCompany = IoC.Resolve<ICompanyService>().Getbykey(it.ComID);
                    ((EInvoiceContext)FXContext.Current).SetCurrentCompany(_currentCompany);
                    IDeliver _DeliverService = _currentCompany.Config.ContainsKey("IDeliver") ? InvServiceFactory.GetDeliver(_currentCompany.Config["IDeliver"]) : IoC.Resolve<IDeliver>();
                    string tempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["TempFolder"], it.id.ToString());
                    CompressHelper.unZip(it.Data, tempPath);
                    //read data and validate                    
                    //loc xem hoa don nao update, hoa don nao phat hanh lai
                    IList<string> lstUpdateKey = new List<string>();
                    List<IInvoice> lstINV = new List<IInvoice>();
                    List<string> lstKey = new List<string>();
                    StringBuilder Failed = new StringBuilder("<Invoices>");
                    int count = 0;
                    string rv = "";
                    Type temp = InvServiceFactory.GetInvoiceType(it.InvPattern, it.ComID);
                    XmlSchemaValidator validator = new XmlSchemaValidator();
                    using (StreamReader reader = new StreamReader(tempPath))
                    {
                        string tempString = reader.ReadLine();
                        if (!tempString.Contains(it.BillTime))
                        {
                            //customer xml string not valid, don't do anything
                            it.Status = TranSactionStatus.Failed;
                            it.Messages = "Thời gian phát hành không đúng!";
                            it.FailResult = it.Data;
                            it.TranLock = 0;
                            tranSrv.Save(it);
                            tranSrv.CommitChanges();
                            return;
                        }
                        while (!string.IsNullOrWhiteSpace(tempString) && (reader.Peek() >= 0))
                        {
                            tempString = reader.ReadLine();
                            if (reader.EndOfStream)
                                break;
                            if (!validator.ValidXmlDoc(tempString, "", AppDomain.CurrentDomain.BaseDirectory + @"XMLValidate\UpdateVATInVoice.xsd"))
                            {
                                log.Error("Error ValidXml" + validator.ValidationError);
                                //customer xml string not valid, don't do anything
                                it.Status = TranSactionStatus.Failed;
                                it.Messages = validator.ValidationError;
                                it.FailResult = it.Data;
                                it.TranLock = 0;
                                tranSrv.Save(it);
                                tranSrv.CommitChanges();
                                return;
                            }
                            XElement elem = XElement.Parse(tempString);
                            XElement ele = elem.Element("Invoice");
                            string fKey = elem.Element("key").Value;
                            if (string.IsNullOrWhiteSpace(fKey))
                            {
                                log.Error("Không có FKey");
                                it.Status = TranSactionStatus.Failed;
                                it.Messages = "Không có FKey.";
                                it.FailResult = it.Data;
                                it.TranLock = 0;
                                tranSrv.Save(it);
                                tranSrv.CommitChanges();
                                return;
                            }
                            lstKey.Add(fKey);
                            IInvoice inv = (IInvoice)Activator.CreateInstance(temp);
                            string read = string.Concat(ele);
                            bool d = DataHelper.DeserializeEinvFromXML(read, inv);
                            if (!d)
                            {
                                Failed.Append(read);
                                failed++;
                                continue;
                            }

                            inv.Pattern = it.InvPattern;
                            inv.Serial = it.InvSerial;
                            inv.CreateBy = it.AccountName;
                            inv.Fkey = fKey;
                            lstINV.Add(inv);
                            count++;

                        }
                    }

                    System.IO.File.Delete(tempPath);
                    //check valiadate xml
                    List<IInvoice> foundInv = GetByFkey(it.ComID, it.InvPattern, lstKey.ToArray());
                    List<IInvoice> updateInv = lstINV.GetRange(0, lstINV.Count);
                    foreach (IInvoice inv in foundInv)
                    {
                        if (inv.PaymentStatus != Payment.Paid)
                        {
                            lstKey.Remove(inv.Fkey);//hóa đơn chưa thanh toán, bỏ khỏi danh sách tạo mới để có thể update
                        }
                        else
                        {
                            count--;
                            failed++;
                            Failed.AppendFormat("<inv><key>{0}</key><ERR>{1}</ERR></inv>", inv.Fkey, "Đã thanh toán");   //hóa đơn đã thanh toán, để trong lstKey để ko update
                        }
                        lstUpdateKey.Add(inv.Fkey);//luôn để trong lstUpdate để ko tạo mới hóa đơn đã có fkey
                    }

                    //end read data and validate
                    lstINV.RemoveAll(inv => lstUpdateKey.Contains(inv.Fkey));
                    updateInv.RemoveAll(inv => lstKey.Contains(inv.Fkey));
                    foundInv.RemoveAll(inv => inv.PaymentStatus == Payment.Paid);
                    updateInv = updateInv.OrderByDescending(inv => inv.Fkey).ToList();
                    foundInv = foundInv.OrderByDescending(inv => inv.Fkey).ToList();
                    // launching
                    log.Error("launch " + key + ": " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                    Launcher t = Launcher.Instance;
                    t.Launch(it.InvPattern, it.InvSerial, lstINV.ToArray(), out failedInt, out mess);

                    //end launching
                    //phat hanh lai giu nguyen so
                    //log.Error("relaunch " + key + ": " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                    //t.ReLaunch(it.InvPattern, it.InvSerial, foundInv.ToArray(), updateInv.ToArray(), out failedInt, out mess);
                    //end relaunching

                    //phat hanh lai ko giu so
                    log.Error("relaunch-cancel inv " + key + ": " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                    t.Cancel(foundInv.ToArray());
                    log.Error("end relaunch-cancel inv " + key + ": " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                    t.Launch(it.InvPattern, it.InvSerial, updateInv.ToArray(), out failedInt, out mess);


                    log.Error("end relaunch-launch inv " + key + ": " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < lstINV.Count; i++)
                    {
                        if (lstINV[i].No > 0)
                        {
                            sb.AppendFormat("{0}_{1}_{2}_{3},", lstKey[i], lstINV[i].No.ToString(FIX_LENGTH_INV_NUMBER), lstINV[i].Serial, lstINV[i].PublishDate.ToString("dd/MM/yyyy"));
                            //rv += lstKey[i] + "_" + lstINV[i].No + ",";
                        }
                        else break;
                    }
                    for (int i = 0; i < updateInv.Count; i++)
                    {
                        if (updateInv[i].No > 0)
                        {
                            sb.AppendFormat("{0}_{1}_{2}_{3},", lstUpdateKey[i], updateInv[i].No.ToString(FIX_LENGTH_INV_NUMBER), updateInv[i].Serial, updateInv[i].PublishDate.ToString("dd/MM/yyyy"));
                            //rv += lstKey[i] + "_" + lstINV[i].No + ",";
                        }
                        else break;
                    }
                    decimal MaxNo = 0;
                    decimal MinNo = 0;
                    if (lstINV.Count > 0)
                    {
                        MaxNo = lstINV.Max(i => i.No);
                        MinNo = lstINV.Where(i => i.No > 0).Min(ii => ii.No);
                    }
                    if (updateInv.Count > 0)
                    {
                        decimal TempNo = updateInv.Max(i => i.No);
                        if (MaxNo < TempNo) MaxNo = TempNo;
                        TempNo = updateInv.Where(i => i.No > 0).Min(ii => ii.No);
                        if (MinNo > TempNo || MinNo == 0) MinNo = TempNo;
                    }
                    rv = sb.ToString();
                    if (failed > 0 || failedInt > 0)
                    {
                        Failed.Append("</Payment>");
                        //remove the last "," character
                        if (rv.Length > 1)
                        {
                            rv = "OK:" + it.InvPattern + ";" + it.InvSerial + "-" + rv.Remove(rv.Length - 1, 1);
                        }
                        else
                        {
                            rv = "OK:" + it.InvPattern + ";" + it.InvSerial + "-";
                        }
                        log.Error("start compress false " + key + ": " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                        it.CompleteResult = CompressHelper.CompressFile(System.Text.Encoding.ASCII.GetBytes(rv), "hoadon.xml"); ;
                        it.FailResult = CompressHelper.CompressFile(System.Text.Encoding.ASCII.GetBytes(Failed.ToString()), "hoadon.xml");
                        it.Messages = "Phát hành mới: " + lstINV.Count + ", chỉnh sửa: " + updateInv.Count + " hóa đơn,lỗi: " + (lstINV.Count() - count - failedInt - 1) + " hóa đơn";
                        log.Error("error: " + key + " count:" + count + " falseInt: " + failedInt + ",lst.Count(): " + lstINV.Count() + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                        log.Error("end compress false " + key + ": " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                        it.Status = TranSactionStatus.NotComplete;
                        it.TranLock = 0;
                        it.NoRange = MinNo.ToString() + "_" + MaxNo.ToString();
                    }
                    else
                    {
                        //ghi nhan so hoa don, key tra ve
                        if (rv.Length > 1)
                        {
                            rv = "OK:" + it.InvPattern + ";" + it.InvSerial + "-" + rv.Remove(rv.Length - 1, 1);
                        }
                        else
                        {
                            rv = "OK:" + it.InvPattern + ";" + it.InvSerial + "-";
                        }
                        it.CompleteResult = CompressHelper.CompressFile(System.Text.Encoding.ASCII.GetBytes(rv), "hoadon.xml");
                        it.Messages = "Phát hành thành công: " + lstINV.Count + " hóa đơn mới, chỉnh sửa: " + updateInv.Count;
                        it.Status = TranSactionStatus.Completed;
                        it.TranLock = 0;
                        it.NoRange = MinNo.ToString() + "_" + MaxNo.ToString();
                    }
                    tranSrv.updateResult(it);
                    if (it.Status == TranSactionStatus.Completed)
                    {
                        _DeliverService.PrepareDeliver(lstINV.ToArray(), _currentCompany);
                        _DeliverService.PrepareDeliver(updateInv.ToArray(), _currentCompany);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(FXContext.Current.CurrentUser.username);
                    log.Error(ex);
                    it.Messages = ex.Message;
                    it.Status = TranSactionStatus.Failed;
                    it.TranLock = 0;
                    tranSrv.updateResult(it);
                }
            }
            catch (Exception ex)
            {
                log.Error("err service " + ex);
            }
            return;
        }

        public void cancelInvoice(object keyObject)
        {
            ILog log = LogManager.GetLogger(typeof(PoolingService));
            Guid key = (Guid)keyObject;
            try
            {
                log.Info("cancelInvoice service " + key + ": " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                BootstrapperForThread.InitializeContainer();
                ITransactionService tranSrv = IoC.Resolve<ITransactionService>();
                Transaction it = tranSrv.Getbykey(key);
                if (it.Status != TranSactionStatus.NewUpload) return;
                try
                {
                    it.Status = TranSactionStatus.Processing;
                    tranSrv.updateResult(it);
                    //tranSrv.CommitChanges();
                    // khoi tao context                     
                    string[] roles = { "Printer" };
                    IList<FanxiPermission> fxPer = new List<FanxiPermission>();
                    UserIdentity tempId = new UserIdentity(it.AccountName, fxPer, roles);
                    FanxiPrincipal _principal = new FanxiPrincipal(tempId);
                    FXContext.Current.SetUserContext(_principal);
                    ((EInvoiceContext)FXContext.Current).SetCurrentCompany(IoC.Resolve<ICompanyService>().Getbykey(it.ComID));
                    //end
                    string tempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["TempFolder"], it.id.ToString());
                    CompressHelper.unZip(it.Data, tempPath);
                    log.Info("unzip " + key + ": " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                    //read data and validate
                    string xmlData;
                    using (StreamReader reader = new StreamReader(tempPath))
                    {
                        xmlData = reader.ReadToEnd();
                    }
                    System.IO.File.Delete(tempPath);
                    //check valiadate xml
                    XmlSchemaValidator validator = new XmlSchemaValidator();
                    if (!validator.ValidXmlDoc(xmlData, "", AppDomain.CurrentDomain.BaseDirectory + @"XMLValidate\DeleteInVoice.xsd"))
                    {
                        //xml string not valid, don't do any thing
                        it.Status = TranSactionStatus.Failed;
                        it.Messages = validator.ValidationError;
                        it.FailResult = it.Data;
                        tranSrv.Save(it);
                        tranSrv.CommitChanges();
                        return;
                    }
                    log.Info("validate " + key + ": " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                    //end read data and validate
                    XElement elem = XElement.Parse(xmlData);
                    IEnumerable<XElement> lst = elem.Elements("key");
                    Type temp = InvServiceFactory.GetInvoiceType(it.InvPattern, it.ComID);
                    StringBuilder Failed = new StringBuilder("<Invoices>");
                    int count = 0;
                    string rv = "";
                    List<string> lstKey = new List<string>();
                    //parse xml to invoice
                    log.Info("parse data " + key + ": " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                    foreach (XElement e in lst)
                    {
                        string fKey = e.Value.Trim();
                        lstKey.Add(fKey);
                        count++;
                    }
                    List<IInvoice> foundInv = GetByFkey(it.ComID, it.InvPattern, lstKey.ToArray());
                    List<string> lstPaid = new List<string>();
                    foreach (IInvoice inv in foundInv)
                    {
                        lstKey.Remove(inv.Fkey);
                        if (inv.PaymentStatus == Payment.Paid)
                        {
                            lstPaid.Add(inv.Fkey);
                        }
                    }
                    foundInv.RemoveAll(inv => inv.PaymentStatus == Payment.Paid);
                    //ghi nhận key không tìm thấy
                    foreach (string s in lstKey)
                    {
                        Failed.Append("<inv>" + s + "</inv>");
                    }
                    //ghi nhận key đã thanh toán
                    Failed.Append("</Invoices><Payment>");
                    foreach (string s in lstPaid)
                    {
                        Failed.Append("<inv>" + s + "</inv>");
                    }

                    // cancel
                    log.Info("cancel " + key + ": " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                    Launcher t = Launcher.Instance;
                    t.Cancel(foundInv.ToArray());
                    //end cancel
                    StringBuilder sb = new StringBuilder();
                    foreach (IInvoice inv in foundInv)
                    {
                        sb.AppendFormat("{0},", inv.Fkey);
                    }
                    rv = sb.ToString();
                    if (lstKey.Count > 0 || lstPaid.Count > 0)
                    {
                        Failed.Append("</Payment>");
                        if (rv.Length > 1)
                        {
                            rv = "OK:" + rv.Remove(rv.Length - 1, 1);
                        }
                        else
                        {
                            rv = "OK:";
                        }
                        log.Info("start compress false " + key + ": " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                        it.CompleteResult = CompressHelper.CompressFile(System.Text.Encoding.ASCII.GetBytes(rv), "hoadon.xml"); ;
                        it.FailResult = CompressHelper.CompressFile(System.Text.Encoding.ASCII.GetBytes(Failed.ToString()), "hoadon.xml");
                        it.Messages = "Tổng: " + count + " hóa đơn.\r\nXóa thành công: " + foundInv.Count + " hóa đơn.\r\nĐã thanh toán: " + lstPaid.Count + " hóa đơn.\r\nKhông tìm thấy: " + lstKey.Count + " hóa đơn.";
                        log.Info("end compress false " + key + ": " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                        it.Status = TranSactionStatus.NotComplete;
                    }
                    else
                    {
                        //ghi nhan so hoa don, key tra ve
                        if (rv.Length > 1)
                        {
                            rv = "OK:" + rv.Remove(rv.Length - 1, 1);
                        }
                        else
                        {
                            rv = "OK:";
                        }
                        log.Info("start compress " + key + ": " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                        it.CompleteResult = CompressHelper.CompressFile(System.Text.Encoding.ASCII.GetBytes(rv), "hoadon.xml");
                        log.Info("end compress " + key + ": " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                        it.Messages = "Tổng: " + count + " hóa đơn.\r\n Xóa thành công: " + foundInv.Count + " hóa đơn.";
                        it.Status = TranSactionStatus.Completed;
                    }
                    tranSrv.updateResult(it);
                    //ghi lại vào bảng db log
                    ILogSystemService businessLog = IoC.Resolve<ILogSystemService>();
                    businessLog.WriteLogCancel(it.ComID, it.AccountName, it.InvPattern, it.InvSerial, foundInv.ToArray());
                }
                catch (Exception ex)
                {
                    log.Error(FXContext.Current.CurrentUser.username);
                    log.Error(ex);
                    it.Messages = ex.Message;
                    it.Status = TranSactionStatus.Failed;
                    tranSrv.updateResult(it);
                }
            }
            catch (Exception ex)
            {
                log.Error("err service " + ex);
            }
            log.Info("end service " + key + ": " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
            return;
        }

        public void updateCus(object keyObject)
        {
            ILog log = LogManager.GetLogger(typeof(PoolingService));
            log.Error("start updateCus " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
            Guid key = (Guid)keyObject;
            try
            {
                log.Error("updateCus " + key + ": " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                BootstrapperForThread.InitializeContainer();
                int failedInt = 0;
                int failed = 0;
                ITransactionService tranSrv = IoC.Resolve<ITransactionService>();
                Transaction it = tranSrv.Getbykey(key);
                if (it.Status != TranSactionStatus.NewUpload) return;
                it.Status = TranSactionStatus.Processing;
                tranSrv.updateResult(it);

                string[] roles = { "" };
                IList<FanxiPermission> fxPer = new List<FanxiPermission>();
                UserIdentity tempId = new UserIdentity(it.AccountName, fxPer, roles);
                FanxiPrincipal _principal = new FanxiPrincipal(tempId);
                FXContext.Current.SetUserContext(_principal);
                ((EInvoiceContext)FXContext.Current).SetCurrentCompany(IoC.Resolve<ICompanyService>().Getbykey(it.ComID));

                EInvoiceContext _Einvoicecontext = (EInvoiceContext)FXContext.Current;
                EInvoice.Core.Domain.Company _currentCom = _Einvoicecontext.CurrentCompany;
                string config = (_currentCom.Config.ContainsKey("SetDefaultCusPass")) ? _currentCom.Config["SetDefaultCusPass"] : "";

                string tempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["TempFolder"], it.id.ToString());
                CompressHelper.unZip(it.Data, tempPath);
                XmlSchemaValidator validator = new XmlSchemaValidator();
                //check xsd   
                string xmlData = null;
                using (StreamReader reader = new StreamReader(tempPath))
                {
                    xmlData = reader.ReadToEnd();
                }
                System.IO.File.Delete(tempPath);
                //check valiadate xml

                if (!validator.ValidXmlDoc(xmlData, "", AppDomain.CurrentDomain.BaseDirectory + @"XMLValidate\CustomerValidate.xsd"))
                {
                    //customer xml string not valid, don't do any thing
                    it.Status = TranSactionStatus.Failed;
                    it.Messages = validator.ValidationError;
                    it.FailResult = it.Data;
                    tranSrv.Save(it);
                    tranSrv.CommitChanges();
                    return;
                }
                log.Error("end validateCus " + key + ": " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                XElement xelement = XElement.Parse(xmlData);

                //bat dau thuc hien import/update cus
                IuserService _userSVC = IoC.Resolve<IuserService>();
                ICustomerService _cusSVC = IoC.Resolve<ICustomerService>();
                IApplicationsService _AppSVC = IoC.Resolve<IApplicationsService>();
                Applications _app = _AppSVC.GetByName("EInvoice");  //Chu y fix cung phu hop voi services.config/IRBACMembershipProvider

                List<XElement> lst = (from item in xelement.Elements("Customer") select item).ToList();
                //string Failed = "<Customers>";
                StringBuilder Failed = new StringBuilder("<Customers>");
                int count = 0;
                log.Error("load createCus " + key + ": " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));

                List<string> uniqueCode = (from item in lst select item.Element("Code").Value).ToList();
                List<string> codes = uniqueCode.Distinct().ToList();
                if (uniqueCode.Count != codes.Count)
                {
                    //duplicate cuscode, reject.
                    it.Status = TranSactionStatus.Failed;
                    it.Messages = "<ERR>Dupplicate Cuscode</ERR>";
                    it.FailResult = it.Data;
                    tranSrv.Save(it);
                    tranSrv.CommitChanges();
                    return;
                }
                List<string> parames = new List<string>();
                StringBuilder param = new StringBuilder();

                List<List<string>> listOfLists = new List<List<string>>();
                for (int i = 0; i < codes.Count(); i += 1000)
                {
                    listOfLists.Add(codes.Skip(i).Take(1000).ToList());
                }

                log.Error("end create query createCus " + key + ": " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                List<Customer> CusLst = new List<Customer>();
                List<user> ULst = new List<user>();
                string temp = it.ComID.ToString();
                foreach (List<string> list in listOfLists)
                {
                    Customer[] cusList = _cusSVC.Query.Where(c => list.Contains(c.Code) && c.ComID == it.ComID).ToArray();
                    CusLst.AddRange(cusList);
                    user[] userList = _userSVC.Query.Where(u => list.Contains(u.username) && u.GroupName == temp).ToArray();
                    ULst.AddRange(userList);
                }
                log.Error("Code count: " + codes.Count + " Cus count: " + CusLst.Count + " user count: " + ULst.Count);
                //Dictionary<string, int> cusCount = new Dictionary<string, int>();
                count = 0;
                log.Error("start createCus " + key + ": " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                _cusSVC.BeginTran();
                StringBuilder sb = new StringBuilder();
                int createNewCount = 0;
                for (int i = 0; i < lst.Count; i++)
                {
                    XElement item = lst[i];
                    string cuscode = (string)item.Element("Code");
                    cuscode = cuscode.Trim();
                    Customer cus = CusLst.Where(c => c.Code == cuscode).FirstOrDefault();
                    user u = ULst.Where(obj => obj.username == cuscode).FirstOrDefault();
                    if (cus == null && u == null)
                    {
                        log.Info("Add cus: " + cuscode);
                        cus = new Customer();
                        if (DataHelper.DeserializeCus(item, cus) != 0)
                        {
                            Failed.AppendFormat("{0}{1}", string.Concat(item), "<ERR>Custype invalid</ERR>");
                            failed++;
                            continue;
                        }
                        if (string.IsNullOrWhiteSpace(cus.Name))
                        {
                            Failed.AppendFormat("{0}{1}", string.Concat(item), "<ERR>Name invalid</ERR>");
                            failed++;
                            continue;
                        }
                        count++;
                        cus.ComID = it.ComID;
                        cus.AccountName = cus.Code;
                        //tạo pass ngẫu nhiên
                        //string randompass = IdentityManagement.WebProviders.RBACMembershipProvider.CreateRandomPassword(8);

                        string randompass = Utils.CreatePassword(cus.AccountName, config);
                        createNewCount++;
                        sb.AppendFormat("{0};{1},", cuscode, randompass);
                        user newuser = new user();
                        newuser.username = cus.AccountName;
                        newuser.PasswordSalt = GeneratorPassword.GenerateSalt();
                        newuser.password = GeneratorPassword.EncodePassword(randompass, 1, newuser.PasswordSalt);
                        newuser.email = cus.Email;
                        newuser.IsApproved = true;
                        newuser.GroupName = it.ComID.ToString();
                        newuser.PasswordSalt = "MD5";
                        newuser.PasswordFormat = 1;
                        newuser.ApplicationList = new List<Applications>();
                        newuser.ApplicationList.Add(_app);
                        _userSVC.CreateNew(newuser);
                        _cusSVC.CreateNew(cus);
                    }
                    else if (cus != null && u != null)
                    {
                        //update cus da ton tai
                        if (DataHelper.DeserializeCus(item, cus) != 0)
                        {
                            Failed.AppendFormat("{0}{1}", string.Concat(item), "<ERR>Custype invalid</ERR>");
                            failed++;
                            continue;
                        }
                        count++;
                        if (u.email != cus.Email)
                        {
                            u.email = cus.Email;
                        }
                    }
                    else
                    {
                        Failed.AppendFormat("{0}{1}", string.Concat(item), "<ERR>Just user or cus exist</ERR>");
                        failed++;
                    }
                }
                log.Error("commit tran customer");
                try
                {
                    _userSVC.CommitTran();
                }
                catch (Exception ex)
                {
                    _userSVC.RolbackTran();
                    log.Error(ex);
                    it.Messages = ex.Message;
                    it.Status = TranSactionStatus.Failed;
                    tranSrv.updateResult(it);
                    return;
                }
                log.Error("end createCus " + key + ": " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                string rv = sb.ToString();
                if (!string.IsNullOrEmpty(rv))
                {
                    rv = createNewCount + ":" + rv.Remove(rv.Length - 1, 1);
                }
                else rv = createNewCount + ":";
                if (failed > 0 || failedInt > 0)
                {
                    Failed.Append("</Customers>");
                    it.CompleteResult = CompressHelper.CompressFile(System.Text.Encoding.UTF8.GetBytes(rv), "khachhang.xml");
                    it.FailResult = CompressHelper.CompressFile(System.Text.Encoding.ASCII.GetBytes(Failed.ToString()), "khachhang.xml");
                    it.Messages = "Import " + count + " khách hàng, tổng: " + lst.Count() + " lỗi: " + (lst.Count - count) + " khách hàng";
                    log.Error("end compressCus " + key + ": " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                    it.Status = TranSactionStatus.NotComplete;
                    log.Error("failed: " + failed + " failedInt: " + failedInt);
                }
                else
                {
                    it.CompleteResult = CompressHelper.CompressFile(System.Text.Encoding.UTF8.GetBytes(rv), "khachhang.xml");
                    it.Messages = "Import thành công: " + count + " khách hàng";
                    it.Status = TranSactionStatus.Completed;
                }
                tranSrv.updateResult(it);
                //tranSrv.CommitChanges();
                log.Error("end updateCus " + key + ": " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
            }
            catch (Exception ex)
            {
                log.Error("loi updateCus :" + key + ": " + ex);
            }
        }

        private List<IInvoice> GetByFkey(int ComID, string pattern, string[] Fkey)
        {
            List<string[]> splitted = new List<string[]>();//This list will contain all the splitted arrays.
            int lengthToSplit = 1000;
            int arrayLength = Fkey.Length;
            for (int i = 0; i < arrayLength; i = i + lengthToSplit)
            {
                if (arrayLength < i + lengthToSplit)
                {
                    lengthToSplit = arrayLength - i;
                }
                string[] val = new string[lengthToSplit];
                Array.Copy(Fkey, i, val, 0, lengthToSplit);
                splitted.Add(val);
            }
            List<IInvoice> rv = new List<IInvoice>();
            IInvoiceService invSrv = InvServiceFactory.GetService(pattern, ComID);
            foreach (string[] j in splitted)
            {
                IQueryable<IInvoice> qr = from inv in invSrv.IQuery<IInvoice>() where j.Contains(inv.Fkey) && inv.ComID == ComID && inv.Status == InvoiceStatus.SignedInv select (IInvoice)inv;
                rv.AddRange(qr.ToList().ToArray());
            }
            return rv;
        }

        private static Dictionary<string, string> invNameDic = new Dictionary<string, string>();
        private string getNameInvoice(string pattern, string serial, int ComID)
        {
            string _key = pattern + "$" + ComID + "$" + serial;
            if (!invNameDic.ContainsKey(_key))
            {
                IPublishInvoiceService _PubInvSrv = IoC.Resolve<IPublishInvoiceService>();
                string invname = _PubInvSrv.GetInvName(ComID, pattern, serial);
                invNameDic.Add(_key, invname);
                _PubInvSrv.UnbindSession();
            }
            return invNameDic[_key];
        }
    }
}