using FX.Utils.MVCMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EInvoice.CAdmin.Controllers
{    
    [MessagesFilter]
    public class BaseController : Controller
    {       
        protected MessageViewData Messages
        {
            get
            {
                if (!ViewData.ContainsKey("Messages"))
                {
                    throw new InvalidOperationException("Messages are not available. Did you add the MessageFilter attribute to the controller?");
                }
                return (MessageViewData)ViewData["Messages"];
            }
        }
    }
}
