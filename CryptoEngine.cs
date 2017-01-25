using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace EInvoice.Core
{
    public static class CryptoEngine
    {
        public static byte[] Encryption(X509Certificate2 cert, byte[] data)
        {
            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert.PrivateKey;
            return rsa.SignData(data, CryptoConfig.MapNameToOID("SHA1"));
        }

        public static bool Decryption(X509Certificate2 cert, byte[] signed, byte[] signature)
        {
            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert.PublicKey.Key;
            // Verify the signature with the hash
            return rsa.VerifyData(signed, CryptoConfig.MapNameToOID("SHA1"), signature);
        }

        public static byte[] PrivateEncryption(this X509Certificate2 cert, byte[] data)
        {

            if (data == null)
                throw new ArgumentNullException("data");
            if (!cert.HasPrivateKey)
                throw new InvalidOperationException("Private key is not loaded");

            int maxDataLength = (cert.PrivateKey.KeySize / 8) - 6;
            if (data.Length > maxDataLength)
                throw new ArgumentOutOfRangeException("data", string.Format(
                    "Maximum data length for the current key size ({0} bits) is {1} bytes (current length: {2} bytes)",
                    cert.PrivateKey.KeySize, maxDataLength, data.Length));


            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert.PrivateKey;            
            // Add 4 byte padding to the data, and convert to BigInteger struct
            BigInteger numData = GetBig(AddPadding(data));

            RSAParameters rsaParams = rsa.ExportParameters(true);
            BigInteger D = GetBig(rsaParams.D);
            BigInteger Modulus = GetBig(rsaParams.Modulus);
            BigInteger encData = BigInteger.ModPow(numData, D, Modulus);

            return encData.ToByteArray();
        }

        public static byte[] PublicDecryption(this RSACryptoServiceProvider rsa, byte[] cipherData)
        {
            if (cipherData == null)
                throw new ArgumentNullException("cipherData");

            BigInteger numEncData = new BigInteger(cipherData);

            RSAParameters rsaParams = rsa.ExportParameters(false);
            BigInteger Exponent = GetBig(rsaParams.Exponent);
            BigInteger Modulus = GetBig(rsaParams.Modulus);

            BigInteger decData = BigInteger.ModPow(numEncData, Exponent, Modulus);

            byte[] data = decData.ToByteArray();
            byte[] result = new byte[data.Length - 1];
            Array.Copy(data, result, result.Length);
            result = RemovePadding(result);

            Array.Reverse(result);
            return result;
        }

        private static BigInteger GetBig(byte[] data)
        {
            byte[] inArr = (byte[])data.Clone();
            Array.Reverse(inArr);  // Reverse the byte order
            byte[] final = new byte[inArr.Length + 1];  // Add an empty byte at the end, to simulate unsigned BigInteger (no negatives!)
            Array.Copy(inArr, final, inArr.Length);

            return new BigInteger(final);
        }

        // Add 4 byte random padding, first bit *Always On*
        private static byte[] AddPadding(byte[] data)
        {
            Random rnd = new Random();
            byte[] paddings = new byte[4];
            rnd.NextBytes(paddings);
            paddings[0] = (byte)(paddings[0] | 128);

            byte[] results = new byte[data.Length + 4];

            Array.Copy(paddings, results, 4);
            Array.Copy(data, 0, results, 4, data.Length);
            return results;
        }

        private static byte[] RemovePadding(byte[] data)
        {
            byte[] results = new byte[data.Length - 4];
            Array.Copy(data, results, results.Length);
            return results;
        }
    }
}
