using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace EInvoice.CAdmin.Api
{
    public class SecurityManager
    {
        private static ILog log = LogManager.GetLogger(typeof(SecurityManager));
        static int requestMaxAgeInSeconds = 3000;               

        public static bool IsTokenValid(string data, string dataSignature, string nonce, string epoch)
        {
            if (!isValidRequest(data, dataSignature))
                return false;
            if (isReplayRequest(nonce, epoch))                            
                return false;
            return true;
        }

        static bool isReplayRequest(string nonce, string requestTimeStamp)
        {
            if (System.Runtime.Caching.MemoryCache.Default.Contains(nonce))
            {
                log.Error("isReplayRequest: " + nonce);
                return true;
            }

            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan currentTs = DateTime.UtcNow - epochStart;

            var serverTotalSeconds = Convert.ToInt32(currentTs.TotalSeconds);
            var requestTotalSeconds = Convert.ToInt32(requestTimeStamp);

            if ((serverTotalSeconds - requestTotalSeconds) > requestMaxAgeInSeconds)
            {
                log.Error("isReplayRequest, TotalSeconds:" + (serverTotalSeconds - requestTotalSeconds));
                return true;
            }

            System.Runtime.Caching.MemoryCache.Default.Add(nonce, requestTimeStamp, DateTimeOffset.UtcNow.AddSeconds(requestMaxAgeInSeconds));

            return false;
        }

        static bool isValidRequest(string data, string base64Mess)
        {
            string hash = getHashedData(data);
            return base64Mess.Equals(hash);
        }

        static string getHashedData(string data)
        {
            MD5 md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToBase64String(hash);
        }
    }
}