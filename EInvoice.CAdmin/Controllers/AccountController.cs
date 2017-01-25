using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using EInvoice.CAdmin.Models;
using log4net;
using FX.Core;
using IdentityManagement.Authorization;
using IdentityManagement.Service;
using FX.Context;
using EInvoice.Core;
using EInvoice.Core.IService;
using IdentityManagement.Domain;
using EInvoice.Core.Domain;
using FX.Utils.MvcPaging;
using IdentityManagement.WebProviders;
using FX.Utils.MVCMessage;
using IdentityManagement;

namespace EInvoice.CAdmin.Controllers
{
    public class AccountController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AccountController));

        public ActionResult Active()
        {
            Company currentComp = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            ActiveModels model = new ActiveModels();
            string code = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(currentComp.AccountName));
            string username = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(code));
            model.code = code;
            model.username = username;
            model.ErrMessages = "";
            return View(model);
        }

        [HttpPost]
        public ActionResult Active(ActiveModels model, string captch)
        {
            if (string.IsNullOrWhiteSpace(captch))
            {
                model.ErrMessages = "Nhập đúng mã xác thực.";
                return View(model);
            }
            bool cv = CaptchaController.IsValidCaptchaValue(captch);
            if (!cv)
            {
                model.ErrMessages = "Nhập đúng mã xác thực.";
                return View(model);
            }
            try
            {
                if (string.IsNullOrWhiteSpace(model.username))
                {
                    model.ErrMessages = "Không tồn tại tài khoản trong hệ thống.";
                    return View(model);
                }
                if (model.username != System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(model.code)))
                {
                    model.ErrMessages = "Không tồn tại tài khoản trong hệ thống.";
                    return View(model);
                }
                Company currentComp = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                IRBACMembershipProvider _MemberShipProvider = IoC.Resolve<IRBACMembershipProvider>();
                if (_MemberShipProvider.GetUser(model.username, true) != null)
                {
                    model.ErrMessages = "Tài khoản đã được kích hoạt trước đó, liên hệ để được hỗ trợ.";
                    return View("Active", model);
                }
                if (!model.password.Equals(model.comfirmpassword))
                {
                    model.ErrMessages = "Nhập đúng mật khẩu xác thực.";
                    return View("Active", model);
                }
                string status = "";
                user tmp = _MemberShipProvider.CreateUser(model.username, model.password, currentComp.Email, null, null, true, null, currentComp.id.ToString(), out status);
                if (status != "Success" || tmp == null)
                {
                    model.ErrMessages = "Chưa kích hoạt được tài khoản, liên hệ để được hỗ trợ.";
                    return View("Active", model);
                }
                IRBACRoleProvider _RoleProvider = IoC.Resolve<IRBACRoleProvider>();
                if (_RoleProvider.RoleExists("Admin"))
                    _RoleProvider.UpdateUsersToRoles(tmp.userid, new string[] { "Admin" });
                return Redirect("/Account/Logon");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                model.ErrMessages = "Chưa kích hoạt được tài khoản, liên hệ để được hỗ trợ.";
                return View();
            }
        }

        public ActionResult Logon(LogOnModel model)
        {
            model.lblErrorMessage = "";
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult LogOn(LogOnModel _model, string captch)
        {
            if (string.IsNullOrWhiteSpace(captch))
            {
                _model.lblErrorMessage = "Nhập đúng mã xác thực.";
                _model.Password = "";
                return View(_model);
            }
            bool cv = CaptchaController.IsValidCaptchaValue(captch);
            if (!cv)
            {
                _model.lblErrorMessage = "Nhập đúng mã xác thực.";
                _model.Password = "";
                return View(_model);
            }
            log.Info("Login: " + _model.UserName);
            FanxiAuthenticationBase _authenticationService = IoC.Resolve<FanxiAuthenticationBase>();
            try
            {
                if (_model.UserName != null && _model.Password != null)
                {
                    if (_authenticationService.Logon(_model.UserName, _model.Password) == true)
                    {
                        log.Info("LogOn:" + HttpContext.User.Identity.Name + ", Date:" + DateTime.Now);
                        if (!string.IsNullOrWhiteSpace(_model.ReturnUrl) && Url.IsLocalUrl(_model.ReturnUrl))
                            return Redirect(_model.ReturnUrl);
                        return Redirect("/");
                    }
                    else
                    {
                        IuserService userSrv = IoC.Resolve<IuserService>();
                        var currComp = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                        user TempUser = userSrv.Query.Where(u => u.username == _model.UserName && u.GroupName.Equals(currComp.id.ToString())).FirstOrDefault();
                        if (TempUser != null)
                        {
                            if (TempUser.IsLockedOut)
                                _model.lblErrorMessage = "Tài khoản đã bị khóa.";
                            else
                            {
                                if (!_model.IsThread)
                                {
                                    if (TempUser.FailedPasswordAttemptCount > 0)
                                    {
                                        TempUser.FailedPasswordAttemptCount = 0;
                                        userSrv.Save(TempUser);
                                        userSrv.CommitChanges();
                                    }
                                    _model.lblErrorMessage = Resources.Message.User_MesWrongAccOrPass;
                                    _model.Password = "";
                                    _model.IsThread = true;
                                    return View(_model);
                                }
                                if (TempUser.FailedPasswordAttemptCount == 4)
                                    TempUser.IsLockedOut = true;
                                TempUser.FailedPasswordAttemptCount++;
                                _model.lblErrorMessage = Resources.Message.User_MesWrongAccOrPass;
                                userSrv.Save(TempUser);
                                userSrv.CommitChanges();
                            }
                            _model.Password = "";
                            return View(_model);
                        }
                        _model.lblErrorMessage = Resources.Message.User_MesWrongAccOrPass;
                        _model.Password = "";
                        return View(_model);
                    }
                }
                else
                {
                    _model.Password = "";
                    return View("LogOn", _model);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                _model.lblErrorMessage = Resources.Message.User_MesWrongAccOrPass;
                _model.Password = "";
                return View("LogOn", _model);
            }
        }

        public ActionResult Logout()
        {
            log.Info("LogOut:" + HttpContext.User.Identity.Name + ", Date:" + DateTime.Now);
            FormsAuthentication.SignOut();
            return RedirectToAction("LogOn");
        }

        public ActionResult ResetPassword()
        {
            ResetModel model = new ResetModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Reset(string username)
        {
            ResetModel mm = new ResetModel();
            try
            {
                IuserService _userService = IoC.Resolve<IuserService>();
                Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                user model = _userService.Query.Where(u => u.GroupName.Equals(_currentCompany.id.ToString()) && u.username == username).FirstOrDefault();
                if (model != null)
                {
                    string randompass = IdentityManagement.WebProviders.RBACMembershipProvider.CreateRandomPassword(8);
                    IService.IRegisterEmailService emailSrv = FX.Core.IoC.Resolve<IService.IRegisterEmailService>();
                    Dictionary<string, string> subjectParams = new Dictionary<string, string>(1);
                    subjectParams.Add("$subject", "");
                    Dictionary<string, string> bodyParams = new Dictionary<string, string>(3);
                    bodyParams.Add("$password", randompass);
                    bodyParams.Add("$site", FX.Utils.UrlUtil.GetSiteUrl());
                    emailSrv.ProcessEmail("hoadondientu@v-invoice.vn", model.email, "ResetPassword", subjectParams, bodyParams);
                    model.password = GeneratorPassword.EncodePassword(randompass, model.PasswordFormat, model.PasswordSalt);//FormsAuthentication.HashPasswordForStoringInConfigFile(randompass, "MD5");
                    model.LastPasswordChangedDate = DateTime.Now;
                    _userService.Save(model);
                    _userService.CommitChanges();
                    mm.lblErrorMessage = "Kiểm tra email để lấy mật khẩu của bạn.";
                    return View("ResetPassword", mm);
                }
                mm.lblErrorMessage = "Tài khoản không tồn tại trên hệ thống";
                return View("ResetPassword", mm);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                ResetModel model = new ResetModel();
                model.lblErrorMessage = "Tài khoản không tồn tại trên hệ thống";
                return View("ResetPassword", mm);
            }
        }

        [RBACAuthorize(Permissions = "Search_user")]
        public ActionResult Index(IndexAccountModel model, int? page)
        {
            IuserService _userService = IoC.Resolve<IuserService>();
            Company _currentCompany = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            int defautPageSize = 15;
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            IQueryable<user> query = _userService.Query.Where(u => u.GroupName.Equals(_currentCompany.id.ToString()));
            IList<user> lst;
            int total = 0;
            if (!string.IsNullOrWhiteSpace(model.username))
                query = query.Where(u => u.username.Contains(model.username.ToLower().Trim()));
            total = query.Count();
            query = query.OrderByDescending(i => i.userid);
            lst = query.Skip(currentPageIndex * defautPageSize).Take(defautPageSize).ToList();
            model.PageListUser = new PagedList<user>(lst, currentPageIndex, defautPageSize, total);
            return View(model);
        }

        /// <summary>
        /// chang pass cho khach hang
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [RBACAuthorize]
        public ActionResult ChangePasswordCustomer(string username)
        {
            ChangePasswordModel model = new ChangePasswordModel();
            model.username = username;
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [RBACAuthorize]
        public ActionResult UpdatePasswordCustomer(string username, string newPassword, string confirmPassword)
        {
            IRBACMembershipProvider _MemberShipProvider = IoC.Resolve<IRBACMembershipProvider>();
            user userCustomer = _MemberShipProvider.GetUser(username, true);
            if (userCustomer == null)
            {
                Messages.AddErrorFlashMessage("Tài khoản không có trên hệ thống.");
                return RedirectToAction("Index", "Customer");
            }
            try
            {
                if (newPassword == confirmPassword)
                {
                    userCustomer.PasswordSalt = GeneratorPassword.GenerateSalt();
                    userCustomer.password = GeneratorPassword.EncodePassword(newPassword, userCustomer.PasswordFormat, userCustomer.PasswordSalt);
                    _MemberShipProvider.UpdateUser(userCustomer);
                    Messages.AddFlashMessage(Resources.Message.User_MesChangePasswordSuccess);
                }
                else
                {
                    Messages.AddErrorMessage(Resources.Message.User_MesErrConfirmPass);
                    ChangePasswordModel model = new ChangePasswordModel();
                    model.username = username;
                    return View(model);
                }
                return RedirectToAction("Index", "Customer");
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                Messages.AddErrorMessage("Có lỗi xảy ra, vui lòng thực hiện lại!");
                ChangePasswordModel model = new ChangePasswordModel();
                model.username = username;
                return View("ChangePasswordCustomer", model);
            }
        }

        /// <summary>
        /// thay doi pass cho quan tri
        /// </summary>
        /// <returns></returns>
        [RBACAuthorize]
        public ActionResult ChangePassword()
        {
            string username = HttpContext.User.Identity.Name;
            ChangePasswordModel model = new ChangePasswordModel();
            model.username = username;
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [RBACAuthorize]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            IRBACMembershipProvider _MemberShipProvider = IoC.Resolve<IRBACMembershipProvider>();
            user _oUser = _MemberShipProvider.GetUser(model.username, true);
            try
            {
                if (_oUser.password == GeneratorPassword.EncodePassword(model.OldPassword, _oUser.PasswordFormat, _oUser.PasswordSalt))
                {
                    if (model.NewPassword != model.OldPassword && model.NewPassword == model.ConfirmPassword)
                    {
                        _oUser.PasswordSalt = GeneratorPassword.GenerateSalt();
                        _oUser.password = GeneratorPassword.EncodePassword(model.NewPassword, _oUser.PasswordFormat, _oUser.PasswordSalt);
                        _MemberShipProvider.UpdateUser(_oUser);
                        log.Info("Change Password By: " + HttpContext.User.Identity.Name + " Info-- UserName: " + _oUser.username + "  ID: " + _oUser.userid + "------");
                        Messages.AddFlashMessage(Resources.Message.User_MesChangePasswordSuccess);
                    }
                    else if (model.NewPassword == model.OldPassword)
                    {
                        Messages.AddErrorMessage(Resources.Message.User_MesNewPassLikeOldPass);
                        return View(model);
                    }
                    else if (model.NewPassword != model.ConfirmPassword)
                    {
                        Messages.AddErrorMessage(Resources.Message.User_MesErrConfirmPass);
                        return View(model);
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    Messages.AddErrorMessage(Resources.Message.User_MesWrongPass);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error ChangePassword:", ex);
                Messages.AddErrorMessage("Có lỗi xảy ra, vui lòng thực hiện lại!");
                return View(model);
            }

        }

        [RBACAuthorize(Permissions = "Edit_user")]
        public ActionResult Edit(int id)
        {
            IRBACMembershipProvider _MemberShipProvider = IoC.Resolve<IRBACMembershipProvider>();
            IRBACRoleProvider _RoleProvider = IoC.Resolve<IRBACRoleProvider>();
            Company currentComp = ((EInvoiceContext)FXContext.Current).CurrentCompany;


            user oUser = _MemberShipProvider.GetUser(id, false);
            if (HttpContext.User.Identity.Name == oUser.username)
            {
                Messages.AddErrorFlashMessage(Resources.Message.User_UMesCantEdit);
                return RedirectToAction("index");
            }

            IStaffService _staSrv = IoC.Resolve<IStaffService>();
            ViewData["fullname"] = _staSrv.SearchByAccountName(oUser.username, currentComp.id).FullName;

            AccountModel model = new AccountModel();
            model.UserTmp = oUser;
            try
            {
                model.UserRoles = _RoleProvider.GetRolesForUser(oUser.userid);
                if (model.UserRoles.Contains("ServiceRole"))
                {
                    Messages.AddErrorFlashMessage(Resources.Message.User_UMesCantEdit);
                    return RedirectToAction("index");
                }
                List<String> lst = new List<string>(_RoleProvider.GetAllRoles());
                if (lst.IndexOf("ServiceRole") >= 0)
                    lst.RemoveAt(lst.IndexOf("ServiceRole"));
                if (lst.IndexOf("Root") >= 0)
                    lst.RemoveAt(lst.IndexOf("Root"));
                model.RetypePassword = oUser.password;
                model.AllRoles = lst.ToArray();
            }
            catch (Exception ex)
            {
                log.Error("edit: " + id, ex);
                throw ex;
            }
            return View(model);
        }

        [RBACAuthorize(Permissions = "Edit_user")]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update(int id, string RetypePassword, string[] UserRoles, string fullname)
        {
            if (id <= 0)
                throw new HttpRequestValidationException();
            IRBACMembershipProvider _MemberShipProvider = IoC.Resolve<IRBACMembershipProvider>();
            IRBACRoleProvider _RoleProvider = IoC.Resolve<IRBACRoleProvider>();
            user Ouser = _MemberShipProvider.GetUser(id, false);
            if (HttpContext.User.Identity.Name == Ouser.username)
            {
                Messages.AddErrorFlashMessage(Resources.Message.User_UMesCantEdit);
                return RedirectToAction("index");
            }
            //lay doi tuong tai khoan cu            
            string OldPassword = Ouser.password;
            string Oldusername = Ouser.username;
            AccountModel model = new AccountModel();
            try
            {
                TryUpdateModel<user>(Ouser);
                if (Ouser.password != RetypePassword)
                {
                    Messages.AddErrorMessage(Resources.Message.User_MesConfirmPass);
                    List<String> lst = new List<string>(_RoleProvider.GetAllRoles());
                    if (lst.IndexOf("ServiceRole") >= 0)
                        lst.RemoveAt(lst.IndexOf("ServiceRole"));
                    if (lst.IndexOf("Root") >= 0)
                        lst.RemoveAt(lst.IndexOf("Root"));
                    model.RetypePassword = Ouser.password = OldPassword;
                    model.AllRoles = lst.ToArray();
                    model.UserRoles = _RoleProvider.GetRolesForUser(Ouser.userid);
                    model.UserTmp = Ouser;
                    return View("Edit", model);
                }
                if (Ouser.password != OldPassword)
                {
                    Ouser.PasswordSalt = GeneratorPassword.GenerateSalt();
                    Ouser.password = GeneratorPassword.EncodePassword(Ouser.password, Ouser.PasswordFormat, Ouser.PasswordSalt);//FormsAuthentication.HashPasswordForStoringInConfigFile(RetypePassword, "MD5");
                }
                Ouser.FailedPasswordAttemptCount = 0;
                //update lai tai khoan
                _MemberShipProvider.UpdateUser(Ouser);
                model.UserRoles = UserRoles ?? new string[] { };
                _RoleProvider.UpdateUsersToRoles(Ouser.userid, model.UserRoles);
                Messages.AddFlashMessage(Resources.Message.User_UMesSuccess);
                log.Info("Update Account:" + HttpContext.User.Identity.Name + ", Date: " + DateTime.Now);

                Company currentComp = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                IStaffService _staSrv = IoC.Resolve<IStaffService>();
                Staff sta = _staSrv.SearchByAccountName(Ouser.username, currentComp.id);
                sta.FullName = fullname;
                _staSrv.UpdateStaff(sta);

                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                log.Error("Error Update:", ex);
                Messages.AddErrorMessage("Có lỗi xảy ra, vui lòng thực hiện lại!");
                List<String> lst = new List<string>(_RoleProvider.GetAllRoles());
                if (lst.IndexOf("ServiceRole") >= 0)
                    lst.RemoveAt(lst.IndexOf("ServiceRole"));
                if (lst.IndexOf("Root") >= 0)
                    lst.RemoveAt(lst.IndexOf("Root"));
                model.RetypePassword = Ouser.password = OldPassword;
                model.AllRoles = lst.ToArray();
                model.UserRoles = _RoleProvider.GetRolesForUser(Ouser.userid);
                model.UserTmp = Ouser;
                return View("Edit", model);
            }
        }

        [RBACAuthorize(Permissions = "Add_user")]
        public ActionResult Create()
        {
            IRBACRoleProvider _RoleProvider = IoC.Resolve<IRBACRoleProvider>();
            AccountModel model = new AccountModel();
            model.UserTmp = new user();
            List<String> lst = new List<string>(_RoleProvider.GetAllRoles());
            if (lst.IndexOf("ServiceRole") >= 0)
                lst.RemoveAt(lst.IndexOf("ServiceRole"));
            if (lst.IndexOf("Root") >= 0)
                lst.RemoveAt(lst.IndexOf("Root"));
            model.AllRoles = lst.ToArray();
            model.UserRoles = new string[] { };
            return View(model);
        }

        [RBACAuthorize(Permissions = "Add_user")]
        public ActionResult New(user temp, string RetypePassword, string[] UserRoles, string fullname)
        {
            IRBACMembershipProvider _MemberShipProvider = IoC.Resolve<IRBACMembershipProvider>();
            IRBACRoleProvider _RoleProvider = IoC.Resolve<IRBACRoleProvider>();
            if (string.IsNullOrWhiteSpace(temp.username))
            {
                AccountModel model = new AccountModel();
                Messages.AddErrorMessage("Cần nhập những thông tin bắt buộc.");
                List<String> lst = new List<string>(_RoleProvider.GetAllRoles());
                if (lst.IndexOf("ServiceRole") >= 0)
                    lst.RemoveAt(lst.IndexOf("ServiceRole"));
                if (lst.IndexOf("Root") >= 0)
                    lst.RemoveAt(lst.IndexOf("Root"));
                model.RetypePassword = temp.password = "";
                model.AllRoles = lst.ToArray();
                model.UserRoles = UserRoles ?? new string[] { };
                model.UserTmp = temp;
                return View("Create", model);
            }
            try
            {
                if (temp.password != RetypePassword)
                {
                    AccountModel model = new AccountModel();
                    Messages.AddErrorMessage(Resources.Message.User_MesConfirmPass);
                    List<String> lst = new List<string>(_RoleProvider.GetAllRoles());
                    if (lst.IndexOf("ServiceRole") >= 0)
                        lst.RemoveAt(lst.IndexOf("ServiceRole"));
                    if (lst.IndexOf("Root") >= 0)
                        lst.RemoveAt(lst.IndexOf("Root"));
                    model.RetypePassword = temp.password = "";
                    model.AllRoles = lst.ToArray();
                    model.UserRoles = UserRoles ?? new string[] { };
                    model.UserTmp = temp;
                    return View("Create", model);
                }
                //Tao tai khoan
                string status = "";
                Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                user u = _MemberShipProvider.CreateUser(temp.username, temp.password, temp.email, null, null, temp.IsApproved, null, currentCom.id.ToString(), out status);
                if (status != "Success")
                {
                    AccountModel model = new AccountModel();
                    Messages.AddErrorMessage("Tài khoản đã có trên hệ thống hoặc dữ liệu không hợp lệ.");
                    List<String> lst = new List<string>(_RoleProvider.GetAllRoles());
                    if (lst.IndexOf("ServiceRole") >= 0)
                        lst.RemoveAt(lst.IndexOf("ServiceRole"));
                    if (lst.IndexOf("Root") >= 0)
                        lst.RemoveAt(lst.IndexOf("Root"));
                    model.RetypePassword = temp.password = "";
                    model.AllRoles = lst.ToArray();
                    model.UserRoles = UserRoles ?? new string[] { };
                    model.UserTmp = temp;
                    return View("Create", model);
                }
                _RoleProvider.UpdateUsersToRoles(u.userid, UserRoles);
                Messages.AddFlashMessage(Resources.Message.User_UMesSuccess);
                log.Info("Create Account:" + HttpContext.User.Identity.Name + ", Date: " + DateTime.Now);

                Company currentComp = ((EInvoiceContext)FXContext.Current).CurrentCompany;
                IStaffService _staSrv = IoC.Resolve<IStaffService>();
                Staff newStaff = new Staff
                {
                    FullName = fullname,
                    AccountName = u.username,
                    ComID = currentComp.id,
                    Email = u.email
                };

                _staSrv.CreateNew(newStaff);
                _staSrv.CommitChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error("Create Error:", ex);
                AccountModel model = new AccountModel();
                Messages.AddErrorMessage("Tài khoản đã có trên hệ thống hoặc dữ liệu không hợp lệ.");
                List<String> lst = new List<string>(_RoleProvider.GetAllRoles());
                if (lst.IndexOf("ServiceRole") >= 0)
                    lst.RemoveAt(lst.IndexOf("ServiceRole"));
                if (lst.IndexOf("Root") >= 0)
                    lst.RemoveAt(lst.IndexOf("Root"));
                model.RetypePassword = temp.password = "";
                model.AllRoles = lst.ToArray();
                model.UserRoles = new string[] { };
                model.UserTmp = temp;
                return View("Create", model);
            }
        }

        [RBACAuthorize(Permissions = "Delete_user")]
        public ActionResult Delete(int id)
        {
            IRBACMembershipProvider _MemberShipProvider = IoC.Resolve<IRBACMembershipProvider>();
            IRBACRoleProvider _RoleProvider = IoC.Resolve<IRBACRoleProvider>();
            try
            {
                user model = _MemberShipProvider.GetUser(id, false);
                string[] roles = _RoleProvider.GetRolesForUser(model.userid);
                if (model.username.ToUpper() == HttpContext.User.Identity.Name.ToUpper() || roles.Contains("ServiceRole"))
                {
                    Messages.AddErrorFlashMessage("Không được xóa tài khoản đang sử dụng.");
                    return RedirectToAction("index");
                }
                if (!_MemberShipProvider.DeleteUser(model.userid, true))
                    Messages.AddErrorFlashMessage("Chưa xóa được tài khoản.");
                else
                    Messages.AddFlashMessage("Xóa tài khoản thành công!");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                Messages.AddErrorFlashMessage("Có lỗi xảy ra, vui lòng thực hiện lại.");
            }
            return RedirectToAction("index");
        }

        #region RoleService
        [RBACAuthorize(Roles = "Admin")]
        public ActionResult ServiceRoleIndex(string username, int? page)
        {
            IRBACRoleProvider _RoleProvider = IoC.Resolve<IRBACRoleProvider>();
            IuserService _userService = IoC.Resolve<IuserService>();
            int defautPageSize = 10;
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IQueryable<user> query = _userService.Query.Where(p => p.GroupName.Equals(currentCom.id.ToString()));
            IList<user> lst;
            int total = 0;
            List<String> temp = new List<String>(_RoleProvider.GetUsersInRole("ServiceRole"));
            if (!string.IsNullOrWhiteSpace(username))
            {
                query = query.Where(u => u.username.ToUpper().Contains(username.ToUpper().Trim()) && temp.Contains(u.username)).OrderByDescending(i => i.userid);
                total = query.Count();
                lst = query.Skip(currentPageIndex * defautPageSize).Take(defautPageSize).ToList();
            }
            else
            {
                query = query.Where(u => temp.Contains(u.username)).OrderByDescending(i => i.userid);
                total = query.Count();
                lst = query.Skip(currentPageIndex * defautPageSize).Take(defautPageSize).ToList();
            }
            IPagedList<user> model = new PagedList<user>(lst, currentPageIndex, defautPageSize, total);
            ViewData["username"] = username;
            return View(model);
        }

        [RBACAuthorize(Roles = "Admin")]
        public ActionResult NewServiceRole()
        {
            IRBACRoleProvider _RoleProvider = IoC.Resolve<IRBACRoleProvider>();
            IuserService _userService = IoC.Resolve<IuserService>();
            user _model = new user();
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            _model.GroupName = currentCom.id.ToString();
            ViewData["RetypePassword"] = _model.password;
            return View(_model);
        }

        [RBACAuthorize(Roles = "Admin")]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateNewServiceRole(user _model, string RetypePassword)
        {
            if (string.IsNullOrWhiteSpace(_model.username))
            {
                Messages.AddErrorMessage("Cần nhập tên tài khoản người dùng.");
                ViewData["RetypePassword"] = _model.password;
                return View("NewServiceRole", _model);
            }
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            IRBACRoleProvider _RoleProvider = IoC.Resolve<IRBACRoleProvider>();
            IRBACMembershipProvider _MemberShipProvider = IoC.Resolve<IRBACMembershipProvider>();
            try
            {
                string status = "";
                if (!_model.password.Equals(RetypePassword))
                {
                    Messages.AddErrorMessage(Resources.Message.User_MesConfirmPass);
                    ViewData["RetypePassword"] = _model.password;
                    return View("NewServiceRole", _model);
                }
                else
                {
                    _model.GroupName = currentCom.id.ToString();
                    _MemberShipProvider.CreateUser(_model.username, _model.password, _model.email, _model.PasswordQuestion, _model.PasswordAnswer, _model.IsApproved, _model.userid, _model.GroupName, out status);
                    if (status != "Success")
                    {
                        Messages.AddErrorMessage("Dữ liệu không hợp lệ hoặc tài khoản đã có trên hệ thống.");
                        ViewData["RetypePassword"] = _model.password;
                        return View("NewServiceRole", _model);
                    }
                    string[] roleservice = new string[] { "ServiceRole" };
                    _RoleProvider.UpdateUsersToRoles(_model.username, roleservice);
                    Messages.AddFlashMessage(Resources.Message.User_IMesSuccess);
                    log.Info("CreateNewServiceRole by:" + HttpContext.User.Identity.Name + " Info-- tai khoan " + _model.username);
                    return RedirectToAction("ServiceRoleIndex");
                }
            }
            catch (Exception ex)
            {
                Messages.AddErrorMessage("Có lỗi xảy ra, vui lòng thực hiện lại.");
                log.Error("CreateNewServiceRole-" + ex.Message);
                ViewData["RetypePassword"] = _model.password;
                return View("NewServiceRole", _model);
            }
        }

        [RBACAuthorize(Roles = "Admin")]
        public ActionResult ServiceRoleEdit(int id)
        {
            IRBACMembershipProvider _MemberShipProvider = IoC.Resolve<IRBACMembershipProvider>();
            user model = _MemberShipProvider.GetUser(id, false);
            if (HttpContext.User.Identity.Name == model.username)
            {
                Messages.AddErrorFlashMessage(Resources.Message.User_UMesCantEdit);
                return RedirectToAction("ServiceRoleIndex");
            }
            ViewData["RetypePassword"] = model.password;
            return View(model);
        }

        [RBACAuthorize(Roles = "Admin")]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveServiceRole(int userid, string RetypePassword)
        {
            if (userid <= 0)
                throw new HttpRequestValidationException();
            IRBACMembershipProvider _MemberShipProvider = IoC.Resolve<IRBACMembershipProvider>();
            IRBACRoleProvider _RoleProvider = IoC.Resolve<IRBACRoleProvider>();
            Company currentCom = ((EInvoiceContext)FXContext.Current).CurrentCompany;
            user model = _MemberShipProvider.GetUser(userid, false);
            if (HttpContext.User.Identity.Name == model.username)
            {
                Messages.AddErrorFlashMessage(Resources.Message.User_UMesCantEdit);
                return RedirectToAction("ServiceRoleIndex");
            }
            string oldpassHash = model.password;
            string username = model.username;
            string email = model.email;
            try
            {
                TryUpdateModel(model);
                model.username = username;
                if (model.password != RetypePassword)
                {
                    ViewData["RetypePassword"] = model.password;
                    Messages.AddErrorFlashMessage(Resources.Message.User_MesConfirmPass);
                    return View("ServiceRoleEdit", model);
                }
                if (RetypePassword != oldpassHash)
                    model.password = GeneratorPassword.EncodePassword(RetypePassword, model.PasswordFormat, model.PasswordSalt);//FormsAuthentication.HashPasswordForStoringInConfigFile(RetypePassword, "MD5");
                model.FailedPasswordAttemptCount = 0;
                _MemberShipProvider.UpdateUser(model);
                _RoleProvider.UpdateUsersToRoles(model.username, new string[] { "ServiceRole" });
                Messages.AddFlashMessage(Resources.Message.User_UMesSuccess);
                log.Info("ServiceRoleUpdate by: " + HttpContext.User.Identity.Name + "Info-- tai khoan " + model.username);
                return RedirectToAction("ServiceRoleIndex");
            }
            catch (Exception ex)
            {
                ViewData["RetypePassword"] = model.password;
                log.Error("ServiceRoleUpdate-" + ex.Message);
                Messages.AddFlashException(ex);
                return View("ServiceRoleEdit", model);
            }
        }
        #endregion
    }
}
