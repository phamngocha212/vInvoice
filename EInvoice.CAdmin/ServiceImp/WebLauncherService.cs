using EInvoice.Core.Domain;
using EInvoice.Core.IService;
using log4net;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace EInvoice.CAdmin.ServiceImp
{
    public class WebLauncherService : ILauncherService
    {
        ILog log = LogManager.GetLogger(typeof(WebLauncherService));

        public string Message { get; set; }

        private string callApi(string action, string data)
        {
            string API_URI = FX.Utils.UrlUtil.GetSiteUrl();
            var client = new RestClient(API_URI);
            var request = new RestRequest(action);
            request.Method = Method.POST;
            request.AddHeader("Admin-Agent", "VSI-HDDT");
            request.AddHeader("Content-Type", "application/json");

            //Calculate UNIX time     
            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = DateTime.UtcNow - epochStart;
            string Timestamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();
            request.AddParameter("application/json", data, ParameterType.RequestBody);

            //Mã duy nhất
            string nonce = Guid.NewGuid().ToString("N").ToLower();

            //Tạo dữ liệu mã hóa
            string signatureRawData = String.Format("{0}{1}{2}", request.Method.ToString().ToUpper(), Timestamp, nonce);

            MD5 md5 = MD5.Create();
            var hash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(signatureRawData));
            var signature = Convert.ToBase64String(hash);

            //Tạo dữ liệu Authentication
            string value = string.Format("{0}:{1}:{2}:{3}", signature, nonce, Timestamp, HttpContext.Current.User.Identity.Name);
            request.AddHeader("Authentication", value);
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return "ERR:1 - Tài khoản không có quyền thực hiện";
            return response.Content;
        }


        public void PublishAdjust(IInvoice OriINV, IList<ProductInv> lst, InvoiceBase INV, string AttacheFile = "")
        {
            try
            {
                INV.Products = lst.Select(p => p).ToList<IProductInv>();
                string xmldata = string.Empty;
                string message = string.Empty;
                xmldata = INV.SerializeToXML();
                string data = "{'xmlData':'" + xmldata + "','pattern':'" + OriINV.Pattern + "', 'serial': '" + OriINV.Serial + "','invNo':'" + OriINV.No + "','fkey':'" + OriINV.Fkey + "','convert':0}";
                Message = callApi("api/business/adjustInv", data);
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
        }

        public void PublishReplace(IInvoice OriINV, IList<ProductInv> lst, InvoiceBase INV, string AttacheFile = "")
        {
            try
            {
                INV.Products = lst.Select(p => p).ToList<IProductInv>();
                string xmldata = string.Empty;
                xmldata = INV.SerializeToXML();
                string data = "{'xmlData':'" + xmldata + "','pattern':'" + OriINV.Pattern + "', 'serial': '" + OriINV.Serial + "','invNo':'" + OriINV.No + "','fkey':'" + OriINV.Fkey + "','convert':0}";
                Message = callApi("api/business/replaceInv", data);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                Message = ex.Message;
            }
        }

        public void PublishInv(string pattern, string serial, IInvoice[] mInvoiceList, string username = null)
        {
            try
            {
                string data = "{'invIDs':[" + string.Join(",", mInvoiceList.Select(p => p.id).ToArray()) + "],'pattern':'" + pattern + "','serial':'" + serial + "'}";
                Message = callApi("api/publish/publishInv", data);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                Message = ex.Message;
            }
        }
    }
}