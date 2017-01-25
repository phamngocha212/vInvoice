using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EInvoice.CAdmin.Models
{
    public class WriteNoteModel
    {
        public int id { get; set; }
        public string pattern { get; set;}
        public string Note { get; set; }
        public string TypeView { get; set; }
    }
}