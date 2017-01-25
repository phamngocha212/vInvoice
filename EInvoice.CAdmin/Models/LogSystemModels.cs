using FX.Utils.MvcPaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace EInvoice.CAdmin.Models
{
    public class LogSystemModels
    {
        public string Keysearch { get; set; }
        public IList<FileInfo> LogsInfo { get; set; }
        public string DateModify { get; set; }        
    }

    public class LogInfo
    {
        public string Name { get; set; }
        public DateTime DateModify { get; set; }
        public string Size { get; set; }
        public string FilePath { get; set; }
    }
}