using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IdentityManagement.Domain;

namespace EInvoice.CAdmin.Models
{
    public class RoleModel
    {
        public int Id { get; set; }
        public string name { get; set; }
        public List<permission> Permissions { get; set; }
    }
}