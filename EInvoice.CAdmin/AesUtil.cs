using System.Text;
using System;
namespace EInvoice.CAdmin
{
    public static class AesUtil
    {
        private static readonly string Password = "abc@123jkl*";
        private static readonly string Salt = "quangltAES";
        private static readonly string HashAlgorithm = "SHA1";
        private static readonly int PasswordIterations = 2;
        private static readonly string InitialVector = "OFRna73m*aze01xY";
        private static readonly int KeySize = 256;
        public static string Encrypt(string[] keyArray)
        {
            string key="";
            foreach (string s in keyArray)
            {
                key = key + s + ";";
            }
            byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(InitialVector);
            byte[] SaltValueBytes = Encoding.ASCII.GetBytes(Salt);
            byte[] PlainTextBytes = Encoding.UTF8.GetBytes(key);
            System.Security.Cryptography.PasswordDeriveBytes DerivedPassword = new System.Security.Cryptography.PasswordDeriveBytes(Password, SaltValueBytes, HashAlgorithm, PasswordIterations);
            byte[] KeyBytes = DerivedPassword.GetBytes(KeySize / 8);
            System.Security.Cryptography.RijndaelManaged SymmetricKey = new System.Security.Cryptography.RijndaelManaged();
            SymmetricKey.Mode = System.Security.Cryptography.CipherMode.CBC;
            byte[] CipherTextBytes = null;
            using (System.Security.Cryptography.ICryptoTransform Encryptor = SymmetricKey.CreateEncryptor(KeyBytes, InitialVectorBytes))
            {
                using (System.IO.MemoryStream MemStream = new System.IO.MemoryStream())
                {
                    using (System.Security.Cryptography.CryptoStream CryptoStream = new System.Security.Cryptography.CryptoStream(MemStream, Encryptor, System.Security.Cryptography.CryptoStreamMode.Write))
                    {
                        CryptoStream.Write(PlainTextBytes, 0, PlainTextBytes.Length);
                        CryptoStream.FlushFinalBlock();
                        CipherTextBytes = MemStream.ToArray();
                        MemStream.Close();
                        CryptoStream.Close();
                    }
                }
            }
            SymmetricKey.Clear();
            return Convert.ToBase64String(CipherTextBytes);
        }
    }
}