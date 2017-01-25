using EInvoice.CAdmin.Api.Entity;
using EInvoice.Core;
using EInvoice.Core.Domain;
using EInvoice.Core.IService;
using FX.Context;
using FX.Core;
using IdentityManagement;
using IdentityManagement.Domain;
using IdentityManagement.WebProviders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;

namespace EInvoice.CAdmin.Api.Controllers
{
    [RoutePrefix("api/company")]
    public class CompanyInfoController : ApiController
    {
        [Route("getInfo")]
        [HttpPost]
        public IHttpActionResult GetInfo()
        {
            try
            {
                Company currentComp = ((EInvoiceContext)FX.Context.FXContext.Current).CurrentCompany;
                return Ok(currentComp);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [APIAuthenticate(Roles = "Admin")]
        [Route("setKeystore")]
        [HttpPost]
        public IHttpActionResult SetKeystores(KeystoresInfo data)
        {
            try
            {
                string Error = "";
                Company currentComp = ((EInvoiceContext)FX.Context.FXContext.Current).CurrentCompany;
                IKeyStoresService keystoreSrv = IoC.Resolve<IKeyStoresService>();
                KeyStores keyStore = keystoreSrv.GetKeyStoreByComID(currentComp.id).FirstOrDefault();
                if (keyStore == null)
                    keyStore = new KeyStores();
                keyStore.ComID = currentComp.id;
                keyStore.SerialCert = data.CertSerial;
                keyStore.Password = data.PassWord;
                keyStore.Type = 4;
                X509Certificate2 x509Cert = new X509Certificate2(Convert.FromBase64String(data.CertData));
                Certificate cert = new Certificate();
                cert.ComID = currentComp.id;
                cert.Cert = data.CertData;
                cert.SerialCert = data.CertSerial;
                cert.Used = true;
                cert.ValidFrom = x509Cert.NotBefore;
                cert.ValidTo = x509Cert.NotAfter;
                if (keyStore.Id > 0)
                {
                    if (keystoreSrv.UpdateKeyStore(keyStore, cert, out Error))
                        return Ok("OK");
                }
                else
                    if (keystoreSrv.CreateKeyStore(keyStore, cert, out Error))
                    return Ok("OK");
                return Ok("ERROR:1");
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [APIAuthenticate(Roles = "Admin")]
        [Route("getRegister")]
        [HttpPost]
        public IHttpActionResult GetRegister(List<RegisterTempApi> data)
        {
            try
            {
                Company currentComp = ((EInvoiceContext)FX.Context.FXContext.Current).CurrentCompany;
                IRegisterTempService registempSrv = IoC.Resolve<IRegisterTempService>();
                string[] patterns = data.Select(p => p.InvPattern).ToArray();
                var temps = registempSrv.Query.Where(p => patterns.Contains(p.InvPattern) && p.ComId == currentComp.id).Select(p =>
                        new RegisterData()
                        {
                            Name = p.Name,
                            InvPattern = p.InvPattern,
                            InvoiceType = p.InvoiceTemp.InvoiceType,
                            InvoiceView = p.InvoiceTemp.InvoiceView,
                            NameInvoice = p.NameInvoice,
                            TemplateName = p.InvoiceTemp.TemplateName,
                            ServiceType = p.InvoiceTemp.ServiceType
                        }
                    );
                return Ok(temps);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [Route("userRegister")]
        [HttpPost]
        [APIAuthenticate(Permissions = "Add_user")]
        public IHttpActionResult UserdataInfo(UserdataInfo data)
        {
            IRBACMembershipProvider _MemberShipProvider = IoC.Resolve<IRBACMembershipProvider>();
            IRBACRoleProvider _RoleProvider = IoC.Resolve<IRBACRoleProvider>();
            if (string.IsNullOrWhiteSpace(data.username) || string.IsNullOrWhiteSpace(data.password))
            {
                return Ok<string>("ERROR:1");//Cần nhập đủ thông tin
            }
            try
            {
                //Tao tai khoan
                string status = "";
                user u = _MemberShipProvider.GetUser(data.username, false);
                if (u != null)
                {
                    u.email = data.email;
                    if (data.ChangePass)
                        u.password = GeneratorPassword.EncodePassword(data.password, u.PasswordFormat, u.PasswordSalt);
                    u.FailedPasswordAttemptCount = 0;
                    //update lai tai khoan
                    _MemberShipProvider.UpdateUser(u);
                    return Ok<string>("OK");
                }
                Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                _MemberShipProvider.CreateUser(data.username, data.password, data.email, null, null, data.IsApproved, null, currentCom.id.ToString(), out status);
                if (status != "Success")
                {
                    return Ok<string>("ERROR:2");//Status != Success
                }
                return Ok<string>("OK");
            }
            catch (Exception ex)
            {
                return Ok<string>("ERROR:3");
            }
        }
    }
}