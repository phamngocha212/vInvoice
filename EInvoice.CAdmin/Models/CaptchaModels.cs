using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EInvoice.CAdmin.Models
{
    public class Captcha
    {
        [Display(Name = "Captcha", Order = 20)]
        [Remote("ValidateCaptcha", "Captcha", "", ErrorMessage = "ErrorMessage")]
        [Required(ErrorMessage = "Required")]
        public virtual string captch { get; set; }        
    }
}