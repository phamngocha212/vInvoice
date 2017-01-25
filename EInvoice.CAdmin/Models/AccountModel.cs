using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FX.Utils.MvcPaging;
using IdentityManagement.Domain;
namespace EInvoice.CAdmin.Models
{
    #region Models

    public class ChangePasswordModel
    {
        public string username { get; set; }
        [DataType(DataType.Password)]
        [DisplayName("Current password")]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("New password")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Confirm new password")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        [DisplayName("User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }
        [DisplayName("Remember me?")]
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
        public string lblErrorMessage { get; set; }
        public bool IsThread { get; set; }
    }

    public class ResetModel
    {
        public string Username { get; set; }
        public string lblErrorMessage { get; set; }  
    }

    public class ActiveModels
    {
        public string code { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string comfirmpassword { get; set; }
        public string ErrMessages { get; set; }        
    }

    public class RegisterModel
    {
        [Required]
        [DisplayName("User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [DisplayName("Email address")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Confirm password")]
        public string ConfirmPassword { get; set; }
    }
    public class IndexAccountModel
    {
        public string username { get; set; }
        public IPagedList<user> PageListUser;
    }
    public class AccountModel
    {        
        public string[] AllRoles { get; set; }
        public string[] UserRoles { get; set; }
        public string RetypePassword { get; set; }
        public user UserTmp { get; set; }
    }
    #endregion
}