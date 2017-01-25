using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EInvoice.CAdmin.Models
{
    public class adjustInvoice
    {
        private int _RecordsId = 0;

        public virtual int RecordsId
        {
            get { return _RecordsId; }
            set { _RecordsId = value; }
        }
        //hiển thị thông tin hóa đơn bị thay thế
        //+ Pattern, serial, no, status
        public int id { get; set; }
        public string pattern { get; set; }
        public string serial { get; set; }
        public string no { get; set; }
        public string status { get; set; }
        //hiển thị hóa đơn thay thế
        //+ Pattern, serial, no, status
        public int ID { get; set; }
        public string patternNew { get; set; }
        public string serialNew { get; set; }
        public string noNew { get; set; }
        public string description { get; set; }
        
    }
}