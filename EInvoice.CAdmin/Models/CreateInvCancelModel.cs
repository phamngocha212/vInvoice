using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EInvoice.Core.Domain;
using System.Web.Mvc;
namespace EInvoice.CAdmin.Models
{
    public class CreateInvCancelModel
    {                
        public string LstCancelDetail { get; set; }
        public InvCancel CancelTemp { get; set; }
        public SelectList lstInvCategory { get; set; }               
    }
}
 



