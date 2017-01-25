using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using FX.Core;
using IdentityManagement.Authorization;
using FX.Context;
using IdentityManagement.WebProviders;
using EInvoice.Core;
using IdentityManagement.Domain;
using IdentityManagement.Service;
using log4net;
namespace EInvoice.CAdmin.Api
{
    public class APIAuthenticateAttribute : ActionFilterAttribute
    {
        private static ILog log = LogManager.GetLogger(typeof(APIAuthenticateAttribute));        
        private const string AuthenticationHeaderName = "Authentication";
        private const string AgentHeaderName = "Admin-Agent";
        private const string AgentHeaderValue = "VSI-HDDT";
        private string[] _Roles;
        private string[] _Permissions;
        public string Roles
        {
            get { return string.Join(",", _Roles); }
            set { _Roles = value.Split(','); }
        }

        public string Permissions
        {
            get { return string.Join(",", _Permissions); }
            set { _Permissions = value.Split(','); }
        }
        
        private static string GetHttpRequestHeader(HttpHeaders headers, string headerName)
        {
            if (!headers.Contains(headerName))
                return null;

            return headers.GetValues(headerName).SingleOrDefault();
        }        

        private bool isAccountCorrect(string username, string password)
        {                        
            FanxiAuthenticationBase _authenticationService = IoC.Resolve<FanxiAuthenticationBase>();
            UserIdentity tempId = _authenticationService.Authenticate(username, password);
            if (tempId == null) return false;
            FanxiPrincipal _principal = new FanxiPrincipal(tempId);
            HttpContext.Current.User = _principal;
            if (_Roles != null && _Roles.Length > 0)
            {
                IEnumerable<string> TempRoles = (from r in tempId.Roles where _Roles.Contains(r) select r);
                if (TempRoles == null || TempRoles.Count() == 0) { return false; }
            }
            if (_Permissions != null && _Permissions.Length > 0)
            {
                List<string> HasPermission = new List<string>();
                IList<IdentityManagement.Domain.role> roles = FX.Core.IoC.Resolve<IroleService>().Query.Where(p => tempId.Roles.Contains(p.name)).ToList();
                foreach (var r in roles)
                {
                    foreach (var per in r.Permissions)
                    {
                        if (HasPermission.Contains(per.name)) continue;
                        HasPermission.Add(per.name);
                    }
                }
                string[] TempPer = (from per in _Permissions where (!HasPermission.Contains(per)) select per).ToArray();
                if (TempPer != null && TempPer.Length > 0) { return false; }
            }
            return true;
        }

        private bool IsAuthenticated(HttpActionContext actionContext)
        {            
            var headers = actionContext.Request.Headers;

            var authenticationString = GetHttpRequestHeader(headers, AuthenticationHeaderName);
            if (string.IsNullOrEmpty(authenticationString))
                return false;

            var authenticationParts = authenticationString.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            if (authenticationParts == null || authenticationParts.Count() == 0) 
                return false;
            var nonce = authenticationParts[1];
            var epoch = authenticationParts[2];
            string data = String.Format("{0}{1}{2}", actionContext.Request.Method.ToString().ToUpper(), epoch, nonce);

            var signature = authenticationParts[0];
            if (!SecurityManager.IsTokenValid(data, signature, nonce, epoch))
                return false;
            
            var agentString = GetHttpRequestHeader(headers, AgentHeaderName);
            if (agentString != null && agentString.Equals(AgentHeaderValue))
            {
                if (authenticationParts.Length < 4)
                    return false;
                var accname = authenticationParts[3];
                EInvoice.Core.Domain.Company _currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                if (_currentCom == null) return false;
                string GroupName = _currentCom.id.ToString();
                IuserService userSrv = IoC.Resolve<IuserService>();
                user tempUser = userSrv.Query.Where(u => u.username == accname && u.IsApproved && !u.IsLockedOut && u.GroupName.Equals(GroupName)).FirstOrDefault();
                if (tempUser == null)
                    return false;
                IList<FanxiPermission> fxPer = new List<FanxiPermission>();
                UserIdentity tempId = new UserIdentity(accname, fxPer, new string[] { "Printer" });
                FanxiPrincipal _principal = new FanxiPrincipal(tempId);                
                HttpContext.Current.User = _principal;
                tempId.Roles = tempUser.Roles.Select(p => p.name).ToArray();
                if (_Roles != null && _Roles.Length > 0)
                {
                    IEnumerable<string> TempRoles = (from r in tempId.Roles where _Roles.Contains(r) select r);
                    if (TempRoles == null || TempRoles.Count() == 0) { return false; }
                }
                if (_Permissions != null && _Permissions.Length > 0)
                {
                    List<string> HasPermission = new List<string>();
                    IList<IdentityManagement.Domain.role> roles = FX.Core.IoC.Resolve<IroleService>().Query.Where(p => tempId.Roles.Contains(p.name)).ToList();
                    foreach (var r in roles)
                    {
                        foreach (var per in r.Permissions)
                        {
                            if (HasPermission.Contains(per.name)) continue;
                            HasPermission.Add(per.name);
                        }
                    }
                    string[] TempPer = (from per in _Permissions where (!HasPermission.Contains(per)) select per).ToArray();
                    if (TempPer != null && TempPer.Length > 0) { return false; }
                }
                return true;
            }
            if (authenticationParts.Length != 5)
                return false;
            var username = authenticationParts[3];
            var password = authenticationParts[4];

            //Kiểm tra username và pass có tồn tại trong db 
            if (!isAccountCorrect(username, password))
                return false;
            return true;
        }        

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            bool isAuthenticated = IsAuthenticated(actionContext);
            if (!isAuthenticated)
            {
                var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                actionContext.Response = response;
            }
        }
    }
}
