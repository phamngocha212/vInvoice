using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EInvoice.CAdmin.IService
{
    public interface IEmailRegisterEngine
    {
        /// <summary>
        /// Loads an email template and merges the optional parameters into the template.
        /// </summary>
        /// <param name="templatePath">Physical path of the template</param>
        /// <param name="subjectParams"></param>
        /// <param name="bodyParams"></param>
        /// <returns>The merged email subject and body (position 0 is the subject, position 1 is the body)</returns>
        /// TODO: refactor to return a decent MailMessage class or something.
        string[] ProcessTemplate(string templateName, Dictionary<string, string> subjectParams, Dictionary<string, string> bodyParams);
    }
}
