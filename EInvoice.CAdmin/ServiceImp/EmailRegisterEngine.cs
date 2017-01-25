using EInvoice.CAdmin.IService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace EInvoice.CAdmin.ServiceImp
{
    public class EmailRegisterEngine : IEmailRegisterEngine
    {
        public string[] ProcessTemplate(string templatePath, Dictionary<string, string> subjectParams, Dictionary<string, string> bodyParams)
        {
            string emailTemplateContent;
            using (StreamReader sr = new StreamReader(templatePath))
            {
                emailTemplateContent = sr.ReadToEnd();
            }

            Regex subjectRegex = new Regex(@"\[subject\]\r\n(.*)\r\n\[\/subject\]"
                , RegexOptions.Compiled | RegexOptions.Singleline);
            Regex bodyRegex = new Regex(@"\[body\]\r\n(.*)\r\n\[/body\]"
                , RegexOptions.Compiled | RegexOptions.Singleline);

            string subject = subjectRegex.Match(emailTemplateContent).Groups[1].Value;
            string body = bodyRegex.Match(emailTemplateContent).Groups[1].Value;

            subject = ReplacePlaceholdersWithValues(subjectParams, subject);
            body = ReplacePlaceholdersWithValues(bodyParams, body);

            return new string[2] { subject, body };
        }

        private string ReplacePlaceholdersWithValues(Dictionary<string, string> parameters, string textWithPlaceholders)
        {
            string processedText = textWithPlaceholders;
            foreach (KeyValuePair<string, string> param in parameters)
            {
                processedText = processedText.Replace(param.Key, param.Value);
            }
            return processedText;
        }
    }
}