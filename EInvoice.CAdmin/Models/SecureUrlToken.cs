using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace EInvoice.CAdmin.Models
{
    public class SecureUrlToken
    {
        public static string generateUrlToken(string controller, string action, ArrayList argumentParams)
        {
            string password = ConfigurationManager.AppSettings["SecurityPass"];
            string token = "";
            //generating the partial url
            string stringToToken = controller.ToUpper() + "/" + action.ToUpper() + "/";
            foreach (string param in argumentParams)
            {
                stringToToken += "/" + param;
            }
            //Converting the salt in to a byte array
            byte[] saltValueBytes = System.Text.Encoding.ASCII.GetBytes(stringToToken);
            //Encrypt the salt bytes with the password
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(stringToToken, saltValueBytes);
            //get the key bytes from the above process
            byte[] secretKey = key.GetBytes(16);
            //generate the hash
            HMACSHA1 tokenHash = new HMACSHA1(secretKey);
            tokenHash.ComputeHash(System.Text.Encoding.ASCII.GetBytes(stringToToken));
            //convert the hash to a base64string
            token = Convert.ToBase64String(tokenHash.Hash);
            return token;
        }

        public static string generateUrlToken(ControllerContext controller, ArrayList argumentParams)
        {
            string password = ConfigurationManager.AppSettings["SecurityPass"];
            string token = "";
            //generating the partial url
            string stringToToken = controller.RouteData.Values["controller"].ToString().ToUpper() + "/" + controller.RouteData.Values["action"].ToString().ToUpper() + "/";
            foreach (string param in argumentParams)
            {
                stringToToken += "/" + param;
            }
            //Converting the salt in to a byte array
            byte[] saltValueBytes = System.Text.Encoding.ASCII.GetBytes(stringToToken);
            //Encrypt the salt bytes with the password
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(stringToToken, saltValueBytes);
            //get the key bytes from the above process
            byte[] secretKey = key.GetBytes(16);
            //generate the hash
            HMACSHA1 tokenHash = new HMACSHA1(secretKey);
            tokenHash.ComputeHash(System.Text.Encoding.ASCII.GetBytes(stringToToken));
            //convert the hash to a base64string
            token = Convert.ToBase64String(tokenHash.Hash);
            return token;
        }
    }
}