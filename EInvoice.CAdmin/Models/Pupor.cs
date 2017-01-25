using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EInvoice.CAdmin.Models
{
    public class Pupor
    {
        private string _InvCateName;

        public string InvCateName
        {
            get { return _InvCateName; }
            set { _InvCateName = value; }
        }
        private string _InvPattern;

        public string InvPattern
        {
            get { return _InvPattern; }
            set { _InvPattern = value; }
        }
        private string _Mucdich;

        public string Mucdich
        {
            get { return _Mucdich; }
            set { _Mucdich = value; }
        }
    }
}