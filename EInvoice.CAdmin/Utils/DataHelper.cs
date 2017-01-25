using EInvoice.Core;
using EInvoice.Core.Domain;
using EInvoice.Core.IService;
using FX.Context;
using FX.Core;
using IdentityManagement.Authorization;
using log4net;
using System;
using System.Web;
using System.Xml.Linq;

namespace EInvoice.CAdmin
{
    public class DataHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DataHelper));
        public static bool IsAcitve(Menu temp)
        {
            if (string.IsNullOrWhiteSpace(temp.PermissionCode) || ((IFanxiPrincipal)HttpContext.Current.User).IsInPermission(temp.PermissionCode) || (!string.IsNullOrWhiteSpace(temp.RoleCode) && HttpContext.Current.User.IsInRole(temp.RoleCode)))
                return true;
            return false;
        }

        public static bool IsValidated(out string errorMessage)
        {
            Company currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            errorMessage = null;
            try
            {
                ICertificateService _certSrv = IoC.Resolve<ICertificateService>();
                KeyStores _keyStore = KeyStoresManagement.GetKeyStore(currentCompany.id);
                if (_keyStore == null)
                {
                    errorMessage = "Chưa đăng ký chữ ký số, liên hệ nhà cung cấp để được hỗ trợ.";
                    return false;
                }
                if (_keyStore.KeyStoresOf == 1)
                    return true;
                System.Security.Cryptography.X509Certificates.X509Certificate2 cert = _keyStore.OpenSession();
                if (DateTime.Parse(cert.GetExpirationDateString()) <= DateTime.Today.AddDays(7))
                {
                    errorMessage = "Chữ ký số sắp hết hạn, liên hệ nhà cung cấp để được hỗ trợ.";
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return true;
            }
        }

        public static bool isAccountCorrect(string username, string password)
        {
            FanxiAuthenticationBase _authenticationService = IoC.Resolve<FanxiAuthenticationBase>();
            UserIdentity tempId = _authenticationService.Authenticate(username, password);
            if (tempId == null) return false;
            FanxiPrincipal _principal = new FanxiPrincipal(tempId);
            HttpContext.Current.User = _principal;
            return true;
        }

        public static string parseInvToken(string invToken, out string pattern, out string serial, out decimal invNo)
        {
            pattern = "";
            serial = "";
            invNo = -1;
            string[] temp = invToken.Split(';');
            if (temp.Length != 3)
            {
                return "ERR:2"; //chuoi token khong chinh xac
            }
            pattern = temp[0];
            serial = temp[1];
            if (!decimal.TryParse(temp[2], out invNo))
            {
                return "ERR:2"; // idInvoice không phải số
            }
            return "OK";
        }

        private static char[] convertTable = null;

        private static char[] tcvnchars = {
            'µ', '¸', '¶', '·', '¹', 
            '¨', '»', '¾', '¼', '½', 'Æ', 
            '©', 'Ç', 'Ê', 'È', 'É', 'Ë', 
            '®', 'Ì', 'Ð', 'Î', 'Ï', 'Ñ', 
            'ª', 'Ò', 'Õ', 'Ó', 'Ô', 'Ö', 
            '×', 'Ý', 'Ø', 'Ü', 'Þ', 
            'ß', 'ã', 'á', 'â', 'ä', 
            '«', 'å', 'è', 'æ', 'ç', 'é', 
            '¬', 'ê', 'í', 'ë', 'ì', 'î', 
            'ï', 'ó', 'ñ', 'ò', 'ô', 
            '­', 'õ', 'ø', 'ö', '÷', 'ù', 
            'ú', 'ý', 'û', 'ü', 'þ', 
            '¡', '¢', '§', '£', '¤', '¥', '¦'
        };
        private static char[] unichars = {
            'à', 'á', 'ả', 'ã', 'ạ', 
            'ă', 'ằ', 'ắ', 'ẳ', 'ẵ', 'ặ', 
            'â', 'ầ', 'ấ', 'ẩ', 'ẫ', 'ậ', 
            'đ', 'è', 'é', 'ẻ', 'ẽ', 'ẹ', 
            'ê', 'ề', 'ế', 'ể', 'ễ', 'ệ', 
            'ì', 'í', 'ỉ', 'ĩ', 'ị', 
            'ò', 'ó', 'ỏ', 'õ', 'ọ', 
            'ô', 'ồ', 'ố', 'ổ', 'ỗ', 'ộ', 
            'ơ', 'ờ', 'ớ', 'ở', 'ỡ', 'ợ', 
            'ù', 'ú', 'ủ', 'ũ', 'ụ', 
            'ư', 'ừ', 'ứ', 'ử', 'ữ', 'ự', 
            'ỳ', 'ý', 'ỷ', 'ỹ', 'ỵ', 
            'Ă', 'Â', 'Đ', 'Ê', 'Ô', 'Ơ', 'Ư'
        };

        public static string convertTCVN3ToUnicode(string value)
        {
            char[] chars = value.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
                if (chars[i] < (char)256)
                {
                    chars[i] = convertTable[chars[i]];
                }

            return new string(chars);
        }

        public static bool DeserializeEinvFromXML(string xml, IInvoice inv)
        {
            try
            {
                inv.DeserializeFromXML(xml);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static int DeserializeCus(XElement xmldata, Customer customer)
        {
            if (xmldata.Element("Name") != null)
                customer.Name = xmldata.Element("Name").Value.Trim();

            if (xmldata.Element("Code") != null)
                customer.Code = (xmldata.Element("Code").Value).Trim();

            if (xmldata.Element("TaxCode") != null)
                customer.TaxCode = (xmldata.Element("TaxCode").Value).Trim();

            if (xmldata.Element("Address") != null)
                customer.Address = xmldata.Element("Address").Value.Trim();

            if (xmldata.Element("BankAccountName") != null)
                customer.BankAccountName = xmldata.Element("BankAccountName").Value.Trim();

            if (xmldata.Element("BankName") != null)
                customer.BankName = xmldata.Element("BankName").Value.Trim();

            if (xmldata.Element("BankNumber") != null)
                customer.BankNumber = xmldata.Element("BankNumber").Value.Trim();

            if (xmldata.Element("Email") != null)
                customer.Email = (xmldata.Element("Email").Value).Trim();

            if (xmldata.Element("Fax") != null)
                customer.Fax = xmldata.Element("Fax").Value.Trim();

            if (xmldata.Element("Phone") != null)
                customer.Phone = xmldata.Element("Phone").Value.Trim();

            if (xmldata.Element("ContactPerson") != null)
                customer.ContactPerson = (xmldata.Element("ContactPerson").Value).Trim();

            if (xmldata.Element("RepresentPerson") != null)
                customer.RepresentPerson = (xmldata.Element("RepresentPerson").Value).Trim();

            if (xmldata.Element("AccountName") != null)
                customer.AccountName = (xmldata.Element("AccountName").Value).Trim();

            if (xmldata.Element("CusType") != null)
            {
                int rv;
                bool kt = Int32.TryParse(xmldata.Element("CusType").Value, out rv);
                if (kt && (rv == 1 || rv == 0))
                {
                    customer.CusType = rv;
                }
                else return -1;
            }

            if (xmldata.Element("Descriptions") != null)
                customer.Descriptions = (xmldata.Element("Descriptions").Value).Trim();
            return 0;
        }
    }
}