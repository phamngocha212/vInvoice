using EInvoice.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EInvoice.CAdmin.Models
{
    public class RegisterTempModels
    {
        public RegisterTemp RegisTemp { get; set; }
        public Company CurrentCom { get; set; }
        public int tempId { get; set; }        
        public string imgFile { get; set; }
        public string logoFile { get; set; }                      
    }

    public class RegisterTempChoiseModels
    {
        public IList<InvCategory> InvCategories { get; set; }                
        public Company CurrentCom { get; set; }     
    }
}